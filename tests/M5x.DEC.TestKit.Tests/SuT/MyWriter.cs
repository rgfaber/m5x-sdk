using System.Threading.Tasks;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Persistence;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT
{
    
    public interface IMyWriter: IModelWriter<MyID, MyFact, MyReadModel> {}
    public class MyWriter : CouchWriter<MyID, MyFact, MyReadModel>, IMyWriter
    {
        public MyWriter(IMyDb store, ILogger logger) : base(store, logger)
        {
        }

        public override async Task<MyReadModel> UpdateAsync(MyFact fact)
        {
            var root = await Store.GetByIdAsync(fact.Meta.Id) ?? MyReadModel.New(fact.Meta.Id);
            root.Meta = fact.Meta;
            root.Content = fact.Payload;
            return await Store.AddOrUpdateAsync(root);
        }
    }
}