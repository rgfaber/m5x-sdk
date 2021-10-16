using System;
using M5x.Chassis.Container;
using M5x.Chassis.Container.Interfaces;

namespace M5x.Chassis.Tests.Container
{
    public class NoContainerFixture : IDisposable
    {
        public NoContainerFixture()
        {
            C = new NoContainer();
        }

        public IContainer C { get; }

        public void Dispose()
        {
            C.Dispose();
        }
    }
}