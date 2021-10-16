using System;
using System.Linq;

namespace M5x.DEC.TestKit
{
    public static class Extensions
    {
        public static bool ValidateState(this IAggregateRoot root, params Func<IAggregateRoot, bool>[] validations)
        {
            return validations.Aggregate(true, (current, func) => current && func.Invoke(root));
        }
    }
}