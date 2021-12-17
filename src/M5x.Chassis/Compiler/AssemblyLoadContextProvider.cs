using System.Runtime.Loader;
using M5x.Chassis.Compiler.Interfaces;

namespace M5x.Chassis.Compiler;

/// <summary>
///     Class AssemblyLoadContextProvider.
/// </summary>
/// <seealso cref="IAssemblyLoadContextProvider" />
public class AssemblyLoadContextProvider : IAssemblyLoadContextProvider
{
    /// <summary>
    ///     Gets this instance.
    /// </summary>
    /// <returns>AssemblyLoadContext.</returns>
    public AssemblyLoadContext Get()
    {
        return AssemblyLoadContext.Default;
    }
}