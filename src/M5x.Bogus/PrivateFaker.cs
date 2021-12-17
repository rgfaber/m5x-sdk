using System;
using Bogus;

namespace M5x.Bogus;

public class PrivateFaker<T> : Faker<T> where T : class
{
    public PrivateFaker<T> UsePrivateConstructor()
    {
        return base.CustomInstantiator(f => Activator.CreateInstance(typeof(T), true) as T)
            as PrivateFaker<T>;
    }

    public PrivateFaker<T> RuleForPrivate<TProperty>(string propertyName, Func<Faker, TProperty> setter)
    {
        base.RuleFor(propertyName, setter);
        return this;
    }
}