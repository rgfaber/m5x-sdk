using System.Threading.Tasks;
using M5x.DEC.Infra.CouchDb;
using M5x.DEC.Persistence;
using Serilog;

namespace M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb
{
    public interface IMyCouchWriter : IFactWriter<MyID, MyFact, MyReadModel>
    {
    }

    public class MyCouchWriter : CouchWriter<MyID, MyFact, MyReadModel>, IMyCouchWriter
    {
        public MyCouchWriter(IMyCouchDb couchDb, ILogger logger) : base(couchDb, logger)
        {
        }

        public override async Task<MyReadModel> UpdateAsync(MyFact fact)
        {
            var root = await CouchDb.GetByIdAsync(fact.Meta.Id) ?? MyReadModel.New(fact.Meta.Id, fact.Meta.Id);
            root.Meta = fact.Meta;
            root.Content = fact.Payload;
            return await CouchDb.AddOrUpdateAsync(root);
        }
    }
}