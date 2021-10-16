using M5x.Schemas;

namespace M5x.DEC.Events
{
    public interface IUpcast<in TFrom, out TTo>
        where TFrom : IAggregateEvent
        where TTo : IAggregateEvent
    {
        TTo Upcast(TFrom aggregateEvent);
    }
}