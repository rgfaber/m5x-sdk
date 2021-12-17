using System.Collections.Generic;

namespace M5x.DEC.Specifications.Provided;

public class AggregateIsNewSpecification : Specification<IAggregateRoot>
{
    protected override IEnumerable<string> IsNotSatisfiedBecause(IAggregateRoot aggregate)
    {
        if (!aggregate.IsNew) yield return $"'{aggregate.Name}' with ID '{aggregate.GetIdentity()}' is not new";
    }
}