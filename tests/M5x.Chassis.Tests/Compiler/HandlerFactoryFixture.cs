using System;
using System.Collections.Generic;
using System.Reflection;
using M5x.Chassis.Compiler;
using M5x.Chassis.Compiler.Interfaces;

namespace M5x.Chassis.Tests.Compiler;

/// <summary>
///     We need to explicitly provide the mscorlib assembly here, whereas doing so in a platform scenario
///     like ASP.NET would cause compilation failure.
/// </summary>
public class HandlerFactoryFixture : IDisposable
{
    public HandlerFactoryFixture()
    {
        var resolvers = new List<IMetaDataReferenceResolver> { new DefaultMetaDataReferenceResolver() };
        var builder = new DefaultAssemblyBuilder(new AssemblyLoadContextProvider(), resolvers);
        Factory = new HandlerFactory(builder, typeof(string).GetTypeInfo().Assembly); // mscorlib
    }

    public HandlerFactory Factory { get; }

    public void Dispose()
    {
    }
}