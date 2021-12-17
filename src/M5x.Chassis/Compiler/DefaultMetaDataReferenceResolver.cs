using System.Reflection;
using M5x.Chassis.Compiler.Interfaces;
using Microsoft.CodeAnalysis;

namespace M5x.Chassis.Compiler;

/// <inheritdoc />
/// <summary>
///     Class DefaultMetaDataReferenceResolver.
/// </summary>
/// <seealso cref="T:M5x.Chassis.Compiler.Interfaces.IMetaDataReferenceResolver" />
public class DefaultMetaDataReferenceResolver : IMetaDataReferenceResolver
{
    /// <summary>
    ///     Resolves the specified assembly.
    /// </summary>
    /// <param name="assembly">The assembly.</param>
    /// <returns>MetadataReference.</returns>
    public MetadataReference Resolve(Assembly assembly)
    {
        return MetadataReference.CreateFromFile(assembly.Location);
    }
}