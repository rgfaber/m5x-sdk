using System;
using Ardalis.GuardClauses;

namespace M5x.Ardalis
{
    public static class GuardClauses
    {
        public static void NotNegative(this IGuardClause clause, int value, string paramName)
        {
            if (value > 0) throw new ArgumentException($"[{paramName}=={value}] - Must be negative (<=0)");
        }

        public static void NotPositive(this IGuardClause clause, int value, string paramName)
        {
            if (value < 0) throw new ArgumentException($"[{paramName}=={value}] - Must be positive (>=0)");
        }

        public static void NotStrictlyNegative(this IGuardClause clause, int value, string paramName)
        {
            if (value >= 0) throw new ArgumentException($"[{paramName}=={value}] - Must be strictly negative (<0)");
        }

        public static void NotStrictlyPositive(this IGuardClause clause, int value, string paramName)
        {
            if (value <= 0) throw new ArgumentException($"[{paramName}=={value}] - Must strictly be positive (>0)");
        }
    }
}