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

        public string Id { get; set; }
        public string Prev { get; set; }
        public AggregateInfo Meta { get; set; }
        public MyPayload Content { get; set; }
        
        

        public static MyReadModel New(string id)
        {
            return new MyReadModel(id);
        }

        public MyStatus Status { get; set; }
    }
}