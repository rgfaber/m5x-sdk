using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using M5x.Chassis.Compiler.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;

namespace M5x.Chassis.Compiler;

/// <summary>
///     Class DefaultAssemblyBuilder.
/// </summary>
/// <seealso cref="IAssemblyBuilder" />
public class DefaultAssemblyBuilder : IAssemblyBuilder
{
    /// <summary>
    ///     The context
    /// </summary>
    private readonly AssemblyLoadContext _context;

    /// <summary>
    ///     The resolvers
    /// </summary>
    private readonly IEnumerable<IMetaDataReferenceResolver> _resolvers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="DefaultAssemblyBuilder" /> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="resolvers">The resolvers.</param>
    public DefaultAssemblyBuilder(IAssemblyLoadContextProvider provider,
        IEnumerable<IMetaDataReferenceResolver> resolvers)
    {
        _resolvers = resolvers;
        _context = provider.Get();
    }

    /// <summary>
    ///     Creates the specified code.
    /// </summary>
    /// <param name="code">The code.</param>
    /// <param name="dependencies">The dependencies.</param>
    /// <returns>Assembly.</returns>
    public Assembly Create(string code, params Assembly[] dependencies)
    {
        var references = ResolveReferences(dependencies);
        var syntaxTree = CSharpSyntaxTree.ParseText(code);

        var compilation = CSharpCompilation.Create(
            $"{Guid.NewGuid()}.dll",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using (var peStream = new MemoryStream())
        {
            using (var pdbStream = new MemoryStream())
            {
                var result = compilation.Emit(peStream, pdbStream);

                var failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                peStream.Seek(0, SeekOrigin.Begin);
                var assembly = _context.LoadFromStream(peStream, pdbStream);
                return assembly;
            }
        }
    }

    /// <summary>
    ///     Resolves the references.
    /// </summary>
    /// <param name="dependencies">The dependencies.</param>
    /// <returns>IEnumerable&lt;MetadataReference&gt;.</returns>
    private IEnumerable<MetadataReference> ResolveReferences(IEnumerable<Assembly> dependencies)
    {
        var compiled = GetCompileTimeReferences();
        var references = new HashSet<MetadataReference>(compiled);
        foreach (var dependency in dependencies)
        foreach (var resolver in _resolvers)
        {
            var reference = resolver.Resolve(dependency);
            if (reference != null)
                references.Add(reference);
        }

        return references;
    }

    /// <summary>
    ///     Gets the compile time references.
    /// </summary>
    /// <returns>IEnumerable&lt;MetadataReference&gt;.</returns>
    private static IEnumerable<MetadataReference> GetCompileTimeReferences()
    {
        return
            from library in DependencyContext.Default.CompileLibraries
            from path in library.ResolveReferencePaths()
            select AssemblyMetadata.CreateFromFile(path)
            into assembly
            select assembly.GetReference();
    }
}