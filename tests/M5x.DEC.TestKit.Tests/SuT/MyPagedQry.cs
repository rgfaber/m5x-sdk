using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT;

public record MyPagedQry : PagedQuery
{
    public MyPagedQry()
    {
    }

    private MyPagedQry(string correlationId, int pageNumber, int pageSize) : base(correlationId, pageNumber,
        pageSize)
    {
    }

    public MyPagedQry(int pageNumber, int pageSize) : base(pageNumber, pageSize)
    {
    }

    public static MyPagedQry New(string correlationId, int pageNumber, int pageSize)
    {
        return new MyPagedQry(correlationId, pageNumber, pageSize);
    }
}