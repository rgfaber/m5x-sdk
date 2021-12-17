using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Core;

public static class ReflectionHelper
{
    public static TResult CompileMethodInvocation<TResult>(Type type, string methodName,
        params Type[] methodSignature)
    {
        var typeInfo = type.GetTypeInfo();
        var methods = typeInfo
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.Name == methodName);

        var methodInfo = methodSignature == null || !methodSignature.Any()
            ? methods.SingleOrDefault()
            : methods.SingleOrDefault(m =>
                m.GetParameters().Select(mp => mp.ParameterType).SequenceEqual(methodSignature));

        if (methodInfo == null)
            throw new ArgumentException($"Type '{type.PrettyPrint()}' doesn't have a method called '{methodName}'");

        return CompileMethodInvocation<TResult>(methodInfo);
    }

    public static TResult CompileMethodInvocation<TResult>(MethodInfo methodInfo)
    {
        var genericArguments = typeof(TResult).GetTypeInfo().GetGenericArguments();
        var methodArgumentList = methodInfo.GetParameters().Select(p => p.ParameterType).ToList();
        var funcArgumentList = genericArguments.Skip(1).Take(methodArgumentList.Count).ToList();

        if (funcArgumentList.Count != methodArgumentList.Count)
            throw new ArgumentException("Incorrect number of arguments");

        var instanceArgument = Expression.Parameter(genericArguments[0]);

        var argumentPairs = funcArgumentList.Zip(methodArgumentList, (s, d) => new { Source = s, Destination = d })
            .ToList();
        if (argumentPairs.All(a => a.Source == a.Destination))
        {
            // No need to do anything fancy, the types are the same
            var parameters = funcArgumentList.Select(Expression.Parameter).ToList();
            return Expression.Lambda<TResult>(Expression.Call(instanceArgument, methodInfo, parameters),
                new[] { instanceArgument }.Concat(parameters)).Compile();
        }

        var lambdaArgument = new List<ParameterExpression>
        {
            instanceArgument
        };

        var type = methodInfo.DeclaringType;
        var instanceVariable = Expression.Variable(type);
        var blockVariables = new List<ParameterExpression>
        {
            instanceVariable
        };
        var blockExpressions = new List<Expression>
        {
            Expression.Assign(instanceVariable, Expression.ConvertChecked(instanceArgument, type))
        };
        var callArguments = new List<ParameterExpression>();

        foreach (var a in argumentPairs)
            if (a.Source == a.Destination)
            {
                var sourceParameter = Expression.Parameter(a.Source);
                lambdaArgument.Add(sourceParameter);
                callArguments.Add(sourceParameter);
            }
            else
            {
                var sourceParameter = Expression.Parameter(a.Source);
                var destinationVariable = Expression.Variable(a.Destination);
                var assignToDestination = Expression.Assign(destinationVariable,
                    Expression.Convert(sourceParameter, a.Destination));

                lambdaArgument.Add(sourceParameter);
                callArguments.Add(destinationVariable);
                blockVariables.Add(destinationVariable);
                blockExpressions.Add(assignToDestination);
            }

        var callExpression = Expression.Call(instanceVariable, methodInfo, callArguments);
        blockExpressions.Add(callExpression);

        var block = Expression.Block(blockVariables, blockExpressions);

        var lambdaExpression = Expression.Lambda<TResult>(block, lambdaArgument);

        return lambdaExpression.Compile();
    }
}