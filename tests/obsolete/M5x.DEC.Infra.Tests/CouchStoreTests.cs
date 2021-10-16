using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CouchDB.Driver;
using CouchDB.Driver.Indexes;
using CouchDB.Driver.Options;
using Flurl.Http.Configuration;
using M5x.DEC.Infra.CouchDb;
using M5x.Schemas;
using M5x.Schemas.Extensions;
using M5x.Serilog;
using M5x.Testing;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace M5x.DEC.Infra.Tests
{
    

    [IDPrefix("state")]
    public record StateEntityId : AggregateId<StateEntityId>
    {
        public StateEntityId(string value) : base(value)
        {
        }
    }

    
    [DbName("myread")]
    class MyReadModel : IReadEntity
    {
        private MyReadModel(string id,  string name, string prev=null)
        {
            Id = id;
            Name = name;
            Prev = prev;
        }
        
        public MyReadModel() {}

        public MyReadModel(string id, string name, string phone, string address,string prev=null)
        {
            Id = id;
            Prev = prev;
            Name = name;
            Phone = phone;
            Address = address;
        }

        public string Id { get; set; }
        public string Prev { get; set; }
        public string Name { get; set; }
        
        public string Phone { get; set; }
        
        public string Address { get; set; }
    
        public static MyReadModel CreateNew(string id, string name, string phone, string address,  string prev=null)
        {
            return new(id, name, phone, address, prev );
        }
    }


    interface ITestCouchStore: ICouchStore<MyReadModel>
    {

        
    }

    internal record MyReadModelPagedResponse : PagedResponse<IEnumerable<MyReadModel>>
    {
        public MyReadModelPagedResponse(int pageNumber) : base(pageNumber)
        {
        }

        public MyReadModelPagedResponse(string correlationId, int pageNumber) : base(correlationId, pageNumber)
        {
        }
    }


    class TestCouchStore : CouchStore<MyReadModel>, ITestCouchStore
    {
        public TestCouchStore(ICouchClient client, ILogger logger) : base(client, logger)
        {
        }

        public TestCouchStore(string dbName,
            string connectionString,
            Action<CouchOptionsBuilder> couchSettingsFunc,
            Action<ClientFlurlHttpSettings> flurlSettingsFunc) : base(dbName,
            connectionString,
            couchSettingsFunc,
            flurlSettingsFunc)
        {
        }

        public async Task<PagedResponse<IEnumerable<MyReadModel>>> RetrievePage(int pageNumber, 
            int pageSize, 
            DbIndexInfo<MyReadModel> indexInfo)
        {
            var res = new MyReadModelPagedResponse(pageNumber);
            try
            {
                var name = await Db.CreateIndexAsync(indexInfo.Name, indexInfo.Builder, indexInfo.Options);
                res.Data = Db
                    .OrderBy(doc => doc.Data.Name)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .Select(x=>x.Data)
                    .ToArray();
                res.PageNumber = pageNumber;
            }
            catch (Exception e)
            {
                res.ErrorState.Errors.Add("DbError", e.AsApiError());
                _logger.Error($"{e.AsApiError()}");
            }
            return res;
        }
    }
    
    
    
    public class CouchStoreTests: IoCTestsBase
    {
        private MyReadModel _model;
        private ITestCouchStore _store;
        private ILogger _logger;
        private ICouchClient _client;


        [Fact]
        public void Should_MyReadModelMustHaveTableName()
        {
            
        }

        [Fact]
        public void Should_MustHaveMyReadModel()
        {
            Assert.NotNull(_model);
        }

        [Fact]
        public void Should_MustHaveLogger()
        {
            Assert.NotNull(_logger);
        }
        
        
        public CouchStoreTests(ITestOutputHelper output, IoCTestContainer container) : base(output, container)
        {
        }

        [Fact]
        public void Should_MustContainCouchStore()
        {
            Assert.NotNull(_store);
        }

        [Fact]
        public void Should_ContainTransientCouchClient()
        {
            Assert.NotNull(_client);
        }

        [Fact]
        public void Should_MustBeAbleToCreateReadModel()
        {
            var id = TestAggregateId.NewId().Value;
            _model = MyReadModel.CreateNew(id,"raf", "123456789", "Gniezno");
            var expectedId = $"{_model.TableName()}-{GuidFactories.NullGuid}";
            Assert.Equal(id,_model.Id);
        }

        [Fact]
        public void Should_ReadModelMustHaveTaleName()
        {
            Assert.False(string.IsNullOrWhiteSpace(_model.TableName()));
        }

        [Fact]
        public async Task Should_StoreMustBeAbleToUpdateWithoutConflicts()
        {
            _model = MyReadModel.CreateNew("1234", "Raf", "1234563785", address:"gniezno");
            _model.Address = "Koln";
            var res =await _store.AddOrUpdateAsync(_model);
            Assert.NotNull(res);
            Assert.Equal("Raf", res.Name);
            res = await _store.GetByIdAsync(res.Id);
            res.Name = "Viktor";
            res.Address = "London";
            res = await _store.AddOrUpdateAsync(res, false, true);
            Assert.NotNull(res);
            Assert.Equal("Viktor", res.Name);
        }

        [Fact]
        public async Task Must_BeAbleToStoreStateEntity()
        {
            _model = MyReadModel.CreateNew("1234", "Raf", "1234563785", address:"gniezno");
            var res = await _store.AddOrUpdateAsync(_model);
        }


        [Fact]
        public async Task Must_BeAbleToGetPagedResults()
        {
            var j = 0;
            do
            {
                var rm = MyReadModel.CreateNew(TestAggregateId.New.Value, 
                    $"name-{j}", $"phone-{j}", $"address-{j}");
                await _store.AddOrUpdateAsync(rm);
                j++;
            } while (j < 100);
            int pageNumber = 3;
            int pageSize = 10;
            string indexName = "by-name";
            var indexInfo = DbIndexInfo<MyReadModel>.CreateNew(indexName,
                ba =>
                {
                    ba.IndexBy(rr => rr.Data.Name);
                },
                new IndexOptions()
                {
                    DesignDocument = "design_name",
                    Partitioned = false
                });
            var res = await _store.RetrieveRecent(pageNumber, pageSize);
            Assert.NotNull(res);
            Assert.Equal(res.Count(), pageSize);

        }
        
        

        protected override void Initialize()
        {
            _model = Container.GetRequiredService<MyReadModel>();
            _store = Container.GetRequiredService<ITestCouchStore>();
            _logger = Container.GetRequiredService<ILogger>();
            _client = Container.GetRequiredService<ICouchClient>();
        }

        protected override void SetTestEnvironment()
        {
            
        }

        protected override void InjectDependencies(IServiceCollection services)
        {
            services
                .AddConsoleLogger()
                .AddTransient<ITestCouchStore, TestCouchStore>()
                .AddTransient<MyReadModel>()
                .AddTransientCouchClient();
        }
    }
}