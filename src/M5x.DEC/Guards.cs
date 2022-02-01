using System;
using Ardalis.GuardClauses;
using M5x.DEC.Events;
using M5x.DEC.Schema;

namespace M5x.DEC;

public static class Guards
{
    public static void BadEvent<TID>(this IGuardClause clause, IEvent<TID> evt) where TID : IIdentity
    {
        Guard.Against.Null(evt, nameof(evt));
        Guard.Against.Null(evt.Meta, nameof(evt.Meta));
        Guard.Against.NullOrWhiteSpace(evt.CorrelationId, nameof(evt.CorrelationId));
        Guard.Against.NullOrWhiteSpace(evt.Meta.Id, nameof(evt.Meta.Id));
    }

    public static void ArgumentNull(this IGuardClause clause, object arg, string paramName)
    {
        if (arg == null) throw new ArgumentNullException(paramName, $"Parameter {paramName} cannot be null.");
    }
    
    
}