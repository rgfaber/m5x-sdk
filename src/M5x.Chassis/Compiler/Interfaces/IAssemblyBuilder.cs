// ***********************************************************************
// <copyright file="IAssemblyBuilder.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Reflection;

namespace M5x.Chassis.Compiler.Interfaces;

/// <summary>
///     Interface IAssemblyBuilder
/// </summary>
public interface IAssemblyBuilder
{
    /// <summary>
    ///     Creates the specified code.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="assemblies">The assemblies.</param>
    /// <returns>Assembly.</returns>
    Assembly Create(string code, params Assembly[] assemblies);
}