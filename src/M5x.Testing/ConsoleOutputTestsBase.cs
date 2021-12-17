using System;
using System.IO;
using Xunit.Abstractions;

namespace M5x.Testing;

public abstract class ConsoleOutputTestsBase : IDisposable
{
    private readonly TextWriter _originalOut;
    private readonly TextWriter _textWriter;
    protected readonly ITestOutputHelper Output;

    protected ConsoleOutputTestsBase(ITestOutputHelper output)
    {
        Output = output;
        _originalOut = Console.Out;
        _textWriter = new StringWriter();
        Console.SetOut(_textWriter);
    }

    public virtual void Dispose()
    {
        Cleanup();
        Output.WriteLine(_textWriter.ToString());
        Console.SetOut(_originalOut);
    }

    protected virtual void Cleanup()
    {
    }
}