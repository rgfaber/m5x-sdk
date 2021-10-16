using System;
using System.Collections.Generic;
using M5x.DEC.Schema.Extensions;

namespace M5x.DEC.Specifications.Provided
{
    public class NotSpecification<T> : Specification<T>
    {
        private readonly ISpecification<T> _specification;

        public NotSpecification(
            ISpecification<T> specification)
        {
            _specification = specification ?? throw new ArgumentNullException(nameof(specification));
        }

        protected override IEnumerable<string> IsNotSatisfiedBecause(T aggregate)
        {
            if (_specification.IsSatisfiedBy(aggregate))
                yield return $"Specification '{_specification.GetType().PrettyPrint()}' should not be satisfied";
        }
    }
}