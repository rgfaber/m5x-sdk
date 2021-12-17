using System;

namespace M5x.DEC.Schema;

public interface IStateEntity<TId> : IReadEntity
    where TId : IIdentity
{
    AggregateInfo Meta { get; set; }
}

public interface IStateEntity<TId, TStatus> : IStateEntity<TId>
    where TId : IIdentity
    where TStatus : Enum
{
    TStatus Status { get; set; }
}

public abstract record StateEntity<TID, TStatus> : StateEntity<TID>, IStateEntity<TID, TStatus>
    where TID : IIdentity where TStatus : Enum
{
    private TStatus _status;

    protected StateEntity(TStatus status)
    {
        Status = status;
    }

    protected StateEntity(string id, string prev, AggregateInfo meta, TStatus status) : base(id, prev, meta)
    {
        Status = status;
    }

    protected StateEntity()
    {
        Status = default;
    }

    public TStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            Meta.Status = Convert.ToInt32(_status);
        }
    }
}

public abstract record StateEntity<TID> : IStateEntity<TID> where TID : IIdentity
{
    private string _id;

    protected StateEntity()
    {
        Meta = AggregateInfo.Empty;
    }

    protected StateEntity(string id, string prev, AggregateInfo meta)
    {
        Id = id;
        Prev = prev;
        Meta = meta;
    }

    public string Id
    {
        get => _id;
        set
        {
            _id = value;
            Meta.Id = _id;
        }
    }

    public string Prev { get; set; }
    public AggregateInfo Meta { get; set; }
}