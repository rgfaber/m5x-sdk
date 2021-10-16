using M5x.Chassis.Container;
using Xunit;

namespace M5x.Chassis.Tests.Container
{
    public class ResolveTests : IClassFixture<NoContainerFixture>
    {
        private readonly NoContainerFixture _fixture;

        public ResolveTests(NoContainerFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Can_resolve_arbitrary_types()
        {
            _fixture.C.Register<IFoo>(() => new Foo(), Lifetime.Permanent);

            var first = _fixture.C.Resolve<Bar>();
            var second = _fixture.C.Resolve<Bar>();

            Assert.NotSame(first, second);
            Assert.Same(first.Baz, second.Baz);
        }

        [Fact]
        public void Can_resolve_instance_twice_with_same_reference()
        {
            var instance = new Foo();
            _fixture.C.Register<IFoo>(instance);

            var first = _fixture.C.Resolve<IFoo>();
            var second = _fixture.C.Resolve<IFoo>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Can_resolve_singleton_twice_with_same_reference()
        {
            _fixture.C.Register<IFoo>(() => new Foo(), Lifetime.Permanent);

            var first = _fixture.C.Resolve<IFoo>();
            var second = _fixture.C.Resolve<IFoo>();

            Assert.Same(first, second);
        }

        [Fact]
        public void Can_resolve_transient_twice_with_different_references()
        {
            _fixture.C.Register<IFoo>(() => new Foo());

            var first = _fixture.C.Resolve<IFoo>();
            var second = _fixture.C.Resolve<IFoo>();

            Assert.NotSame(first, second);
        }

        public interface IFoo
        {
        }

        public class Foo : IFoo
        {
        }

        public class Bar
        {
            public Bar(IFoo baz)
            {
                Baz = baz;
            }

            public IFoo Baz { get; set; }
        }
    }
}