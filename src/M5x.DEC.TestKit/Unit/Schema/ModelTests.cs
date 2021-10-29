using System;
using System.Text.Json;
using M5x.DEC.Schema;
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
        public void Needs_DbName()
        {
            var name = GetDbName();
            Assert.False(string.IsNullOrWhiteSpace(name));
        }


        [Fact]
        public void Must_ModelMustBeDeserializable()
        {
            var s = JsonSerializer.SerializeToUtf8Bytes(Model);
            var des = JsonSerializer.Deserialize<TReadModel>(s);
            Assert.NotNull(des);
            Assert.IsType<TReadModel>(des);
        }

        private string GetDbName()
        {
            var atts = (DbNameAttribute[])typeof(TReadModel).GetCustomAttributes(typeof(DbNameAttribute), true);
            if (atts.Length == 0) throw new Exception($"Attribute [DbName] is missing on {typeof(TReadModel)}");
            return atts[0].DbName;
        }
    }
}