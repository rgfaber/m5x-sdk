using CouchDB.Driver;
using FakeItEasy;
using M5x.DEC.Infra;
using M5x.DEC.TestKit.Tests.SuT.Infra.CouchDb;
using M5x.Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace M5x.DEC.TestKit.Tests.SuT
{
    public static class MyTestEtlInfra
    {

        public static IServiceCollection AddMyFakeDb(this IServiceCollection services)
        {
            var delResult = MyTestSchema.Model;
            delResult.Prev = string.Empty;
            var fakeDb = A.Fake<IMyCouchDb>();
            var fakeCouchClient = A.Fake<ICouchClient>(); 
            A.CallTo(
                    () => fakeDb.GetByIdAsync(MyTestSchema.TestID.Value))
                .Returns(MyTestSchema.Model);
            A.CallTo(
                    () => fakeDb.AddOrUpdateAsync(MyTestSchema.Model, false,false,default))
                .Returns(MyTestSchema.Model);
            A.CallTo(
                    () => fakeDb.DeleteAsync(MyTestSchema.TestID.Value))
                .Returns(delResult);
            return services
                .AddSingleton<ICouchClient>(p => fakeCouchClient)
                .AddSingleton<IMyCouchDb>(p => fakeDb);
        }


        public static IServiceCollection AddMyDb(this IServiceCollection services)
        {
            return services?
                .AddConsoleLogger()
                .AddCouchClient()
                .AddTransient<IMyCouchDb, MyCouchDb>();
        }

        public static IServiceCollection AddMyFakeWriter(this IServiceCollection services)
        {
            return services
                .AddConsoleLogger()
                .AddMyFakeDb()
                .AddTransient<IMyCouchWriter, MyCouchWriter>();
        }
        
        
        
        
        
    }
}