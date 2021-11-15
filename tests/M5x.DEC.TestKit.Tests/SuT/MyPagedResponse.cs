using System.Collections.Generic;
using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public record MyPagedResponse : PagedResponse<MyReadModel>
    {
        public MyPagedResponse()
        {
        }

        public MyPagedResponse(int pageNumber) : base(pageNumber)
        {
        }

        public MyPagedResponse(int pageNumber, IEnumerable<MyReadModel> payload) : base(pageNumber, payload)
        {
        }

        private MyPagedResponse(string correlationId, int pageNumber, IEnumerable<MyReadModel> payload)
            : base(correlationId, pageNumber, payload)
        {
        }

        public static MyPagedResponse New(string correlationId, int pageNumber, IEnumerable<MyReadModel> payload)
        {
            return new MyPagedResponse(correlationId, pageNumber, payload);
        }
    }
}