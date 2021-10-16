// ***********************************************************************
// <copyright file="IMetadataReferenceResolver.cs" company="macula.io">
//     (c)2017 by macula.io
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Reflection;
using Microsoft.CodeAnalysis;

namespace M5x.Chassis.Compiler.Interfaces
{
    /// <summary>
    ///     Interface IMetaDataReferenceResolver
    /// </summary>
    public interface IMetaDataReferenceResolver
    {
        /// <summary>
        ///     Resolves the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>MetadataReference.</returns>
        MetadataReference Resolve(Assembly assembly);
    }
}