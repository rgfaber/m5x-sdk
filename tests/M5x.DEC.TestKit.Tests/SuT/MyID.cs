using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT;

[IDPrefix(MyConfig.MyIdPrefix)]
public record MyID : Identity<MyID>
{
    public MyID(string value) : base(value)
    {
    }
}