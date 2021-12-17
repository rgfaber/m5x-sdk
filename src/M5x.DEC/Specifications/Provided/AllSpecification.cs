using System;
using System.Collections.Generic;
using System.Linq;

namespace M5x.DEC.Specifications.Provided;

public class AllSpecifications<T> : Specification<T>
{
    private readonly IReadOnlyList<ISpecification<T>> _specifications;

    public AllSpecifications(
        IEnumerable<ISpecification<T>> specifications)
    {
        var specificationList = (specifications ?? Enumerable.Empty<ISpecification<T>>()).ToList();

        if (!specificationList.Any())
            throw new ArgumentException("Please provide some specifications", nameof(specifications));

        _specifications = specificationList;
    }

    protected override IEnumerable<string> IsNotSatisfiedBecause(T aggregate)
    {
        return _specifications.SelectMany(s => s.WhyIsNotSatisfiedBy(aggregate));
    }
}