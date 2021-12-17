// ***********************************************************************
// <copyright file="InstanceFactory.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace M5x.Chassis.Compiler;

/// <summary>
///     Provides high-performance object activation.
/// </summary>
public class InstanceFactory
{
    /// <summary>
    ///     Delegate ObjectActivator
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>System.Object.</returns>
    public delegate object ObjectActivator(params object[] parameters);

    /// <summary>
    ///     Delegate ParameterlessObjectActivator
    /// </summary>
    /// <returns>System.Object.</returns>
    public delegate object ParameterlessObjectActivator();

    /// <summary>
    ///     The instance
    /// </summary>
    public static InstanceFactory Instance = new();

    /// <summary>
    ///     The activators
    /// </summary>
    private readonly IDictionary<Type, ObjectActivator> _activators =
        new ConcurrentDictionary<Type, ObjectActivator>();

    /// <summary>
    ///     The constructor parameters
    /// </summary>
    private readonly IDictionary<ConstructorInfo, ParameterInfo[]> _constructorParameters =
        new ConcurrentDictionary<ConstructorInfo, ParameterInfo[]>();

    /// <summary>
    ///     The constructors
    /// </summary>
    private readonly IDictionary<Type, ConstructorInfo> _constructors =
        new ConcurrentDictionary<Type, ConstructorInfo>();

    /// <summary>
    ///     The empty activators
    /// </summary>
    private readonly IDictionary<Type, ParameterlessObjectActivator> _emptyActivators =
        new ConcurrentDictionary<Type, ParameterlessObjectActivator>();

    /// <summary>
    ///     Create an instance of the same type as the provided instance.
    /// </summary>
    /// <param name="example">The example.</param>
    /// <returns>System.Object.</returns>
    public object CreateInstance(object example)
    {
        return CreateInstance(example.GetType());
    }

    /// <summary>
    ///     Create a typed instance assuming a parameterless constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T.</returns>
    public T CreateInstance<T>()
    {
        return (T)CreateInstance(typeof(T));
    }

    /// <summary>
    ///     Creates the instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="args">The arguments.</param>
    /// <returns>T.</returns>
    public T CreateInstance<T>(object[] args)
    {
        return (T)CreateInstance(typeof(T), args);
    }

    /// <summary>
    ///     Create an instance of a type assuming a parameterless constructor.
    /// </summary>
    /// <param name="implementationType">Type of the implementation.</param>
    /// <returns>System.Object.</returns>
    public object CreateInstance(Type implementationType)
    {
        // activator 
        if (_emptyActivators.TryGetValue(implementationType, out var activator))
            return activator();
        var ctor = implementationType.GetConstructor(Type.EmptyTypes);
        _emptyActivators[implementationType] = activator = DynamicMethodFactory.Build(implementationType, ctor);

        return activator();
    }

    /// <summary>
    ///     Create an instance of a type assuming a set of parameters.
    /// </summary>
    /// <param name="implementationType">Type of the implementation.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>System.Object.</returns>
    public object CreateInstance(Type implementationType, object[] args)
    {
        if (args == null || args.Length == 0)
            return CreateInstance(implementationType);

        // activator 
        if (!_activators.TryGetValue(implementationType, out var activator))
        {
            var ctor = GetOrCacheConstructorForType(implementationType);
            var parameters = GetOrCacheParametersForConstructor(ctor);
            _activators[implementationType] =
                activator = DynamicMethodFactory.Build(implementationType, ctor, parameters);
        }

        return activator(args);
    }

    /// <summary>
    ///     Gets the or cache parameters for constructor.
    /// </summary>
    /// <param name="ctor">The ctor.</param>
    /// <returns>ParameterInfo[].</returns>
    public ParameterInfo[] GetOrCacheParametersForConstructor(ConstructorInfo ctor)
    {
        // constructor->parameters
        if (!_constructorParameters.TryGetValue(ctor, out var parameters))
            _constructorParameters[ctor] = parameters = ctor.GetParameters();
        return parameters;
    }

    /// <summary>
    ///     Gets the type of the or cache constructor for.
    /// </summary>
    /// <param name="implementationType">Type of the implementation.</param>
    /// <returns>ConstructorInfo.</returns>
    public ConstructorInfo GetOrCacheConstructorForType(Type implementationType)
    {
        // type->constructor
        if (!_constructors.TryGetValue(implementationType, out var ctor))
            _constructors[implementationType] = ctor = GetWidestConstructor(implementationType);
        return ctor;
    }

    /// <summary>
    ///     Gets the widest constructor.
    /// </summary>
    /// <param name="implementationType">Type of the implementation.</param>
    /// <returns>ConstructorInfo.</returns>
    private static ConstructorInfo GetWidestConstructor(Type implementationType)
    {
        var ctors = implementationType.GetConstructors();
        var ctor = ctors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();
        return ctor ?? implementationType.GetConstructor(Type.EmptyTypes);
    }

    /// <summary>
    ///     Source:
    ///     <see
    ///         cref="http://stackoverflow.com/questions/2353174/c-sharp-emitting-dynamic-method-delegate-to-load-parametrized-constructor-proble" />
    /// </summary>
    private static class DynamicMethodFactory
    {
        /// <summary>
        ///     Builds the specified implementation type.
        /// </summary>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="ctor">The ctor.</param>
        /// <returns>ParameterlessObjectActivator.</returns>
        public static ParameterlessObjectActivator Build(Type implementationType, ConstructorInfo ctor)
        {
            var dynamicMethod = new DynamicMethod($"{implementationType.FullName}.0ctor", implementationType,
                Type.EmptyTypes, true);
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);
            return (ParameterlessObjectActivator)dynamicMethod.CreateDelegate(
                typeof(ParameterlessObjectActivator));
        }

        /// <summary>
        ///     Builds the specified implementation type.
        /// </summary>
        /// <param name="implementationType">Type of the implementation.</param>
        /// <param name="ctor">The ctor.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>ObjectActivator.</returns>
        public static ObjectActivator Build(Type implementationType, ConstructorInfo ctor,
            IReadOnlyList<ParameterInfo> parameters)
        {
            var dynamicMethod = new DynamicMethod($"{implementationType.FullName}.ctor", implementationType,
                new[] { typeof(object[]) });
            var il = dynamicMethod.GetILGenerator();
            for (var i = 0; i < parameters.Count; i++)
            {
                il.Emit(OpCodes.Ldarg_0);
                switch (i)
                {
                    case 0:
                        il.Emit(OpCodes.Ldc_I4_0);
                        break;
                    case 1:
                        il.Emit(OpCodes.Ldc_I4_1);
                        break;
                    case 2:
                        il.Emit(OpCodes.Ldc_I4_2);
                        break;
                    case 3:
                        il.Emit(OpCodes.Ldc_I4_3);
                        break;
                    case 4:
                        il.Emit(OpCodes.Ldc_I4_4);
                        break;
                    case 5:
                        il.Emit(OpCodes.Ldc_I4_5);
                        break;
                    case 6:
                        il.Emit(OpCodes.Ldc_I4_6);
                        break;
                    case 7:
                        il.Emit(OpCodes.Ldc_I4_7);
                        break;
                    case 8:
                        il.Emit(OpCodes.Ldc_I4_8);
                        break;
                    default:
                        il.Emit(OpCodes.Ldc_I4, i);
                        break;
                }

                il.Emit(OpCodes.Ldelem_Ref);
                var paramType = parameters[i].ParameterType;
                il.Emit(paramType.GetTypeInfo().IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass, paramType);
            }

            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Ret);
            return (ObjectActivator)dynamicMethod.CreateDelegate(typeof(ObjectActivator));
        }
    }
}