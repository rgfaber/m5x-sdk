using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using M5x.DEC.Core;
using M5x.DEC.Events;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Extensions;

public static class TypeExtensions
{
    private static readonly ConcurrentDictionary<Type, AggregateName> AggregateNames =
        new();


    private static readonly ConcurrentDictionary<Type, AggregateName> SagaNames =
        new();


    private static readonly ConcurrentDictionary<Type, AggregateName> AggregateNameCache =
        new();

    private static readonly ConcurrentDictionary<Type, Type> AggregateEventTypeCache =
        new();


    public static AggregateName GetAggregateName(
        this Type aggregateType)
    {
        return AggregateNames.GetOrAdd(
            aggregateType,
            t =>
            {
                if (!typeof(IAggregateRoot).GetTypeInfo().IsAssignableFrom(aggregateType))
                    throw new ArgumentException($"Type '{aggregateType.PrettyPrint()}' is not an aggregate root");

                return new AggregateName(
                    t.GetTypeInfo().GetCustomAttributes<AggregateNameAttribute>().SingleOrDefault()?.Name ??
                    t.Name);
            });
    }


    internal static IReadOnlyDictionary<Type, Action<T, IAggregateEvent>> GetAggregateEventApplyMethods<TAggregate,
        TIdentity, T>(this Type type)
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        var aggregateEventType = typeof(IAggregateEvent<TAggregate, TIdentity>);

        return type
            .GetTypeInfo()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(mi =>
            {
                if (mi.Name != "Apply") return false;
                var parameters = mi.GetParameters();
                return
                    parameters.Length == 1 &&
                    aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
            })
            .ToDictionary(
                mi => mi.GetParameters()[0].ParameterType,
                mi => ReflectionHelper.CompileMethodInvocation<Action<T, IAggregateEvent>>(type, "Apply",
                    mi.GetParameters()[0].ParameterType));
    }


    public static IReadOnlyDictionary<Type, Func<T, IAggregateEvent, IAggregateEvent>>
        GetAggregateEventUpcastMethods<TAggregate, TIdentity, T>(this Type type)
        where TAggregate : IAggregateRoot<TIdentity>
        where TIdentity : IIdentity
    {
        var aggregateEventType = typeof(IAggregateEvent<TAggregate, TIdentity>);

        return type
            .GetTypeInfo()
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(mi =>
            {
                if (mi.Name != "Upcast")
                    return false;
                var parameters = mi.GetParameters();
                return
                    parameters.Length == 1 &&
                    aggregateEventType.GetTypeInfo().IsAssignableFrom(parameters[0].ParameterType);
            })
            .ToDictionary(
                //problem might be here
                mi => mi.GetParameters()[0].ParameterType,
                mi => ReflectionHelper.CompileMethodInvocation<Func<T, IAggregateEvent, IAggregateEvent>>(type,
                    "Upcast", mi.GetParameters()[0].ParameterType));
    }

    public static IReadOnlyList<Type> GetAggregateEventUpcastTypes(this Type type)
    {
        var interfaces = type
            .GetTypeInfo()
            .GetInterfaces()
            .Select(i => i.GetTypeInfo())
            .ToList();

        var upcastableEventTypes = interfaces
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IUpcast<,>))
            .Select(i => i.GetGenericArguments()[0])
            .ToList();

        return upcastableEventTypes;
    }

    public static Type GetBaseType(this Type type, string name)
    {
        var currentType = type;

        while (currentType != null)
        {
            if (currentType.Name.Contains(name)) return currentType;
            currentType = currentType.BaseType;
        }

        return type;
    }
}