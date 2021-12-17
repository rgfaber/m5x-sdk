using M5x.Chassis.Compiler;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Chassis.Tests.Compiler;

/// <summary>
///     These tests all require a reference to System.Runtime because outside of the runtime, all references
///     are explicit (i.e. it isn't required in ASP.NET projects where the runtime is abstracted).
///     See: https://github.com/aspnet/dnx/issues/1653
/// </summary>
public class HandlerFactoryTests : IClassFixture<HandlerFactoryFixture>
{
    private readonly HandlerFactoryFixture _fixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public HandlerFactoryTests(HandlerFactoryFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = fixture;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Can_build_default_handler()
    {
        var info = new HandlerInfo
        {
            Namespace = "M5x.Compiler",
            Function = "Execute",
            EntryPoint = "M5x.Compiler.Main",
            Code = @"
namespace M5x.Compiler
{ 
    public class Main
    { 
        public static string Execute()
        { 
            return ""Hello, macula.io!"";
        }
    }
}"
        };

        var h = _fixture.Factory.BuildHandler(info);
        var r = (string)h.Invoke(null, new object[] { });

        _testOutputHelper.WriteLine(r); // Hello, macula.io!
    }
}