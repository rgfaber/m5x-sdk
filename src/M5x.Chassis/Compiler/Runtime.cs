// ***********************************************************************
// <copyright file="Runtime.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace M5x.Chassis.Compiler
{
    /// <summary>
    ///     Class Runtime.
    /// </summary>
    public static class Runtime
    {
        /// <summary>
        ///     Gets the runtime assemblies.
        /// </summary>
        /// <returns>IEnumerable&lt;Assembly&gt;.</returns>
        public static IEnumerable<Assembly> GetRuntimeAssemblies()
        {
            var dependencies = DependencyContext.Default.RuntimeLibraries
                .SelectMany(info => info.Dependencies);

            var assemblies = dependencies
                .Select(info => Assembly.Load(info.Name));

            return assemblies;
        }
    }
}