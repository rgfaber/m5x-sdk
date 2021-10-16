// adapted from sample code at http://linqinaction.net/files/folders/linqinaction/entry1952.aspx

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using M5x.Couch.Interfaces;

namespace M5x.Couch.Linq
{
    /// <summary>
    ///     QueryProvider for CouchDB queries
    /// </summary>
    public class CouchQueryProvider : IQueryProvider
    {
        private readonly ICouchDatabase _db;

        private readonly ICouchViewDefinition _definition;
        private readonly string _design;
        private readonly string _view;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CouchQueryProvider" /> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="design">The design.</param>
        /// <param name="view">The view.</param>
        public CouchQueryProvider(ICouchDatabase db, string design, string view)
        {
            _db = db;
            _view = view;
            _design = design;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CouchQueryProvider" /> class.
        /// </summary>
        /// <param name="db">The db.</param>
        /// <param name="definition">The definition.</param>
        public CouchQueryProvider(ICouchDatabase db, ICouchViewDefinition definition)
        {
            _db = db;
            _definition = definition;
        }

        #region IQueryProvider Members

        /// <summary>
        ///     Creates the query.
        /// </summary>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new CouchLinqQuery<TElement>(expression, this);
        }

        /// <summary>
        ///     Constructs an <see cref="T:System.Linq.IQueryable" /> object that can evaluate the query represented by a specified
        ///     expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        ///     An <see cref="T:System.Linq.IQueryable" /> that can evaluate the query represented by the specified expression
        ///     tree.
        /// </returns>
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator
                    .CreateInstance(typeof(CouchLinqQuery<>)
                        .MakeGenericType(elementType), expression, this);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        ///     Process the expression into an executable query and a select expression (if applicable)
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public virtual ExpressionVisitor Prepare(Expression expression)
        {
            return
                _definition == null
                    ? new ExpressionVisitor().ProcessExpression(expression, _db, _design, _view)
                    : new ExpressionVisitor().ProcessExpression(expression, _db, _definition);
        }

        /// <summary>
        ///     Executes the specified expression.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public TResult Execute<TResult>(Expression expression)
        {
            if (!typeof(CouchViewResult).IsAssignableFrom(typeof(TResult)))
                throw new InvalidCastException("Only subclasses of CouchViewResult are supported. " + typeof(TResult) +
                                               " is not a valid result type.");

            return (TResult)Execute(expression);
        }

        /// <summary>
        ///     Executes the query represented by a specified expression tree.
        /// </summary>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <returns>
        ///     The value that results from executing the specified query.
        /// </returns>
        public object Execute(Expression expression)
        {
            var expVisitor = Prepare(expression);
            var result = expVisitor.Query.GetResult().RowDocuments().First();

            if (expVisitor.SelectExpression == null)
                return result;

            return expVisitor.SelectExpression.Method.Invoke(result, null);
        }

        /// <summary>
        ///     Gets the query text.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public string GetQueryText(Expression expression)
        {
            return Prepare(expression).ToString();
        }

        #endregion
    }
}