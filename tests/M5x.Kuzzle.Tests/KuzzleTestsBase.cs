using M5x.Testing;
using Xunit.Abstractions;

namespace M5x.Kuzzle.Tests;

public abstract class KuzzleTestsBase : IoCTestsBase
{
    protected KuzzleTestsBase(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
    {
    }
}