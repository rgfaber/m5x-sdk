using M5x.DEC.Schema;

namespace M5x.DEC.TestKit.Tests.SuT
{
    [DbName(MyConfig.MyDb)]
    public record MyReadModel : IStateEntity<MyID, MyStatus>
    {
        public MyReadModel()
        {
        }

        private MyReadModel(string id)
        {
            Id = id;
        }

        private MyReadModel(string id, string prev) 
        {
            Id = id;
            Prev = prev;
        }

        public string Id { get; set; }
        public string Prev { get; set; }
        public AggregateInfo Meta { get; set; }
        public MyPayload Content { get; set; }
        

        public static MyReadModel New(string id, string prev)
        {
            return new MyReadModel(id,prev);
        }

        public MyStatus Status { get; set; }
    }
}