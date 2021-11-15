using System.Text.Json;
using System.Threading.Tasks;
using M5x.DEC.Schema;
using M5x.DEC.Schema.Utils;
using M5x.Testing;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.TestKit.Unit.Schema
{
    public abstract class RootTests<TReadModel> : IoCTestsBase
        where TReadModel : IReadEntity
    {
        protected TReadModel Model;

        protected RootTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
            Model = CreateModel();
        }

        protected abstract TReadModel CreateModel();

        [Fact]
        public Task Needs_DbName()
        {
            var name = AttributeUtils.GetDbName<TReadModel>();
            Assert.False(string.IsNullOrWhiteSpace(name));
            return Task.CompletedTask;
        }


        [Fact]
        public Task Must_ModelMustBeDeserializable()
        {
            var s = JsonSerializer.SerializeToUtf8Bytes(Model);
            var des = JsonSerializer.Deserialize<TReadModel>(s);
            Assert.NotNull(des);
            Assert.IsType<TReadModel>(des);
            return Task.CompletedTask;
        }
    }
}