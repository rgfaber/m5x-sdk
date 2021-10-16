using System;

namespace M5x.Chassis.Container.Interfaces
{
    public interface IDependencyRegistrar : IDisposable
    {
        void Register(Type type, Func<object> builder, Lifetime lifetime = Lifetime.AlwaysNew);
        void Register<T>(Func<T> builder, Lifetime lifetime = Lifetime.AlwaysNew) where T : class;
        void Register<T>(string name, Func<T> builder, Lifetime lifetime = Lifetime.AlwaysNew) where T : class;
        void Register<T>(Func<IDependencyResolver, T> builder, Lifetime lifetime = Lifetime.AlwaysNew) where T : class;

        void Register<T>(string name, Func<IDependencyResolver, T> builder, Lifetime lifetime = Lifetime.AlwaysNew)
            where T : class;

        void Register<T>(T instance);
    }
}