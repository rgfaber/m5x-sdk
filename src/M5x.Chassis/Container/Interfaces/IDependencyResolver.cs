using System;
using System.Collections;
using System.Collections.Generic;

namespace M5x.Chassis.Container.Interfaces
{
    public interface IDependencyResolver : IDisposable
    {
        T Resolve<T>() where T : class;
        object Resolve(Type serviceType);
        IEnumerable<T> ResolveAll<T>() where T : class;
        IEnumerable ResolveAll(Type serviceType);
        T Resolve<T>(string name) where T : class;
        object Resolve(string name, Type serviceType);
    }
}