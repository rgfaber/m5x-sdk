using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using M5x.Chassis.Compiler.Interfaces;

namespace M5x.Chassis.Compiler
{
    /// <summary>
    ///     Class HandlerFactory.
    /// </summary>
    public class HandlerFactory
    {
        /// <summary>
        ///     The no code handler
        /// </summary>
        private const string NoCodeHandler = @"
namespace macula.io.Fx
{ 
    public class Main
    { 
        public static string Execute()
        { 
            return ""Hello, macula.io!"";
        }
    }
}";

        /// <summary>
        ///     The builder
        /// </summary>
        private readonly IAssemblyBuilder _builder;

        /// <summary>
        ///     The default dependencies
        /// </summary>
        private readonly IEnumerable<Assembly> _defaultDependencies;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HandlerFactory" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="defaultDependencies">The default dependencies.</param>
        public HandlerFactory(IAssemblyBuilder builder, IEnumerable<Assembly> defaultDependencies)
        {
            _builder = builder;
            _defaultDependencies = defaultDependencies ?? Runtime.GetRuntimeAssemblies();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="HandlerFactory" /> class.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="defaultDependencies">The default dependencies.</param>
        public HandlerFactory(IAssemblyBuilder builder, params Assembly[] defaultDependencies)
        {
            _builder = builder;
            _defaultDependencies = defaultDependencies ?? Runtime.GetRuntimeAssemblies();
        }

        /// <summary>
        ///     Builds the handler.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="dependencies">The dependencies.</param>
        /// <returns>MethodInfo.</returns>
        public MethodInfo BuildHandler(HandlerInfo info, params Assembly[] dependencies)
        {
            var code = info.Code ?? NoCodeHandler;
            var entryPoint = info.EntryPoint ?? $"{info.Namespace ?? "macula.io.Fx"}.Main";
            var function = info.Function ?? "Execute";

            var mergedDependencies = _defaultDependencies.Union(dependencies).Distinct().ToArray();
            var a = _builder.Create(code, mergedDependencies);
            var t = a?.GetType(entryPoint);
            var h = t?.GetMethod(function, BindingFlags.Public | BindingFlags.Static);

            return h;
        }
    }
}