using M5x.DEC.Schema;
using M5x.DEC.TestKit.Integration.Client;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Integration.Cmd
{

    public abstract class ResponderTests<TResponder, TRequester, TID, THope, TFeedback> :
        RequesterTests<TResponder, TRequester, TID, THope, TFeedback>
        where TResponder : IResponder
        where TID : IIdentity
        where THope : IHope
        where TFeedback : IFeedback
        where TRequester : IRequester<THope, TFeedback>
    {
        protected ResponderTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }
        
        
        // [Fact]
        // public void 




    }
}