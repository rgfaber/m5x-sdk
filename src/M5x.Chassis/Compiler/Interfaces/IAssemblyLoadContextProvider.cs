// ***********************************************************************
// <copyright file="IAssemblyLoadContextProvider.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Runtime.Loader;

namespace M5x.Chassis.Compiler.Interfaces
{
    /// <summary>
    ///     Interface IAssemblyLoadContextProvider
    /// </summary>
    public interface IAssemblyLoadContextProvider
    {
        /// <summary>
        ///     Gets this instance.
        /// </summary>
        /// <returns>AssemblyLoadContext.</returns>
        AssemblyLoadContext Get();
    }
}