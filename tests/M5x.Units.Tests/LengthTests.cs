using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using UnitsNet;
using Xunit;
using Xunit.Abstractions;

namespace M5x.Units.Tests;

public class LengthTests : IoCTestsBase
{
    [Fact]
    public void Should_9KmBe5Miles()
    {
        var nineKm  = Length.FromKilometers(9);
        var fiveMiles = nineKm.Miles;
        Output.WriteLine($"{nineKm.Kilometers} {nineKm.Unit} is {fiveMiles} Miles");
    }

    public LengthTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }

    protected override void Initialize()
    {

    }

    protected override void SetTestEnvironment()
    {

    }

    protected override void InjectDependencies(IServiceCollection services)
    {
    }
}