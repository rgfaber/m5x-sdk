// adapted from sample code at http://linqinaction.net/files/folders/linqinaction/entry1952.aspx

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using M5x.Couch.Interfaces;

namespace M5x.Couch.Linq;

/// <summary>
///     Computes a <see cref="CouchQuery" /> from a LINQ expression tree
/// </summary>
public class ExpressionVisitor
{
    private readonly List<object> _keys = new();

    private bool _startKeySet, _endKeySet, _hasAnd, _hasOr;

    /// <summary>
    ///     Gets or sets the query.
    /// </summary>
    /// <value>The query.</value>
    public virtual CouchQuery Query { get; protected set; }

    /// <summary>
    ///     Gets or sets the select expression. This is the MethodInfo representing the Lambda inside of .Select()
    /// </summary>
    /// <value>The select expression.</value>
    public virtual MethodCallExpression SelectExpression { get; protected set; }

    /// <summary>
    ///     Processes the expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="db">The db.</param>
    /// <param name="design">The name of the design document.</param>
    /// <param name="view">The name of the view.</param>
    /// <returns></returns>
    public ExpressionVisitor ProcessExpression(Expression expression, ICouchDatabase db, string design, string view)
    {
        Query = db.Query(design, view);
        VisitExpression(expression);
        return this;
    }

    /// <summary>
    ///     Processes the expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="db">The db.</param>
    /// <param name="definition">The view definition.</param>
    /// <returns></returns>
    public ExpressionVisitor ProcessExpression(Expression expression, ICouchDatabase db,
        ICouchViewDefinition definition)
    {
        Query = db.Query(definition);
        VisitExpression(expression);

        switch (_keys.Count)
        {
            case 0: // 0 keys means it's a range query. do nothing.
                break;
            case 1: // 1 key means it's a single Equals or a Contains on a single. 
                Query.Key(_keys[0]);
                break;
            default: // neither 0 nor 1 means that we've got a set of keys to test.
                Query.Keys(_keys);
                break;
        }

        return this;
    }

    /// <summary>
    ///     Recursivley processes the expression tree
    /// </summary>
    /// <param name="expression">The expression.</param>
    protected virtual void VisitExpression(Expression expression)
    {
        switch (expression.NodeType)
        {
            case ExpressionType.AndAlso:
                _hasAnd = true;
                VisitBinary((BinaryExpression)expression);
                break;
            case ExpressionType.OrElse:
                _hasOr = true;
                VisitBinary((BinaryExpression)expression);
                break;
            case ExpressionType.GreaterThan:
            case ExpressionType.LessThan:
                throw new NotSupportedException(expression.NodeType + " is not a supported expression");
            case ExpressionType.Equal:
                if (_hasAnd)
                    throw new NotSupportedException(
                        "'and' operations cannot be performed on key sets. All key sets are 'or' operations.");
                if (_startKeySet || _endKeySet)
                    throw new NotSupportedException(
                        "Key and range operations cannot be combined in a single query");
                CallIfPresent((BinaryExpression)expression, val => _keys.Add(val));
                break;
            case ExpressionType.LessThanOrEqual:
                if (_endKeySet || _hasOr)
                    throw new NotSupportedException("Range queries over multiple ranges are not supported");
                if (_keys.Count > 0)
                    throw new NotSupportedException(
                        "Key and range operations cannot be combined in a single query");
                CallIfPresent((BinaryExpression)expression, val => Query.EndKey(val));
                _endKeySet = true;
                break;
            case ExpressionType.GreaterThanOrEqual:
                if (_startKeySet || _hasOr)
                    throw new NotSupportedException("Range queries over multiple ranges are not supported");
                if (_keys.Count > 0)
                    throw new NotSupportedException(
                        "Key and range operations cannot be combined in a single query");
                CallIfPresent((BinaryExpression)expression, val => Query.StartKey(val));
                _startKeySet = true;
                break;
            case ExpressionType.Lambda:
                VisitExpression(((LambdaExpression)expression).Body);
                break;
            default:
                if (expression is MethodCallExpression)
                    VisitMethodCall((MethodCallExpression)expression);
                break;
        }
    }

    /// <summary>
    ///     Processes an "AndAlso" node
    /// </summary>
    /// <param name="andAlso">The and also.</param>
    private void VisitBinary(BinaryExpression expr)
    {
        VisitExpression(expr.Left);
        VisitExpression(expr.Right);
    }

    /// <summary>
    ///     Processes a "MethodCall" node
    /// </summary>
    /// <param name="expression">The expression.</param>
    private void VisitMethodCall(MethodCallExpression expression)
    {
        if (expression.Method.DeclaringType == typeof(Queryable) &&
            expression.Method.Name == "Where")
        {
            VisitExpression(((UnaryExpression)expression.Arguments[1]).Operand);
        }
        else if (expression.Method.DeclaringType == typeof(Queryable) &&
                 expression.Method.Name == "Select")
        {
            SelectExpression = expression;
            VisitExpression(expression.Arguments[0]);
        }
        else if (expression.Method.DeclaringType == typeof(Enumerable) &&
                 expression.Method.Name == "Contains")
        {
            VisitContains((MemberExpression)expression.Arguments[0]);
        }
        else
        {
            throw new NotSupportedException("Method not supported: " + expression.Method.Name);
        }
    }

    private void VisitContains(MemberExpression memberExpression)
    {
        foreach (var elem in (IEnumerable)GetMemberValue(memberExpression))
            _keys.Add(elem);
    }

    /// <summary>
    ///     Invokes the callback if the right-hand side of the BinaryExpression has a value
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="callback">The callback.</param>
    private void CallIfPresent(Expression expression, Action<object> callback)
    {
        object val;
        if ((val = GetRightExpressionValue((BinaryExpression)expression)) == null)
            return;

        callback(val);
    }

    /// <summary>
    ///     Gets the value of the right-hand side of the expression
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns></returns>
    private object GetRightExpressionValue(BinaryExpression expression)
    {
        // since all things will have to translate to "key", and the only
        // place where we know what "key" really is is within the view,
        // just check that it's a member access expression
        if (expression.Left.NodeType != ExpressionType.MemberAccess)
            return null;

        object key;
        if (expression.Right.NodeType == ExpressionType.Constant)
            key = ((ConstantExpression)expression.Right).Value;
        else if (expression.Right.NodeType == ExpressionType.MemberAccess)
            key = GetMemberValue((MemberExpression)expression.Right);
        else
            throw new NotSupportedException("Expression type not supported: " + expression.Right.NodeType);

        return key;
    }

    /// <summary>
    ///     Gets the member value.
    /// </summary>
    /// <param name="memberExpression">The member expression.</param>
    /// <returns></returns>
    private object GetMemberValue(MemberExpression memberExpression)
    {
        MemberInfo memberInfo;
        object obj;

        if (memberExpression == null)
            throw new ArgumentNullException("memberExpression");

        // Get object
        if (memberExpression.Expression is ConstantExpression)
            obj = ((ConstantExpression)memberExpression.Expression).Value;
        else if (memberExpression.Expression is MemberExpression)
            obj = GetMemberValue((MemberExpression)memberExpression.Expression);
        else
            throw new NotSupportedException("Expression type not supported: " +
                                            memberExpression.Expression.GetType().FullName);

        // Get value

        // do this through a collection of 'as' calls, as opposed to is/cast. this way
        // we're not coercing twice.
        memberInfo = memberExpression.Member;
        var property = memberInfo as PropertyInfo;
        if (property == null)
        {
            var field = memberInfo as FieldInfo;
            if (field == null)
                throw new NotSupportedException("MemberInfo type not supported: " + memberInfo.GetType().FullName);

            return field.GetValue(obj);
        }

        return property.GetValue(obj, null);
    }
}