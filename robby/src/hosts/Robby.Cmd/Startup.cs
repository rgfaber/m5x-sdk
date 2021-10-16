using System.Reflection;
using M5x.DEC.Infra.Chassis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Robby.Client.Infra;
using Robby.Cmd.Infra;
using Robby.Cmd.Infra.Game;

namespace Robby.Cmd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBaseApi(Config.CmdDef)
                .AddRobbyRequesters()
                .AddRobbyCmdInfra()
                .AddControllers()
                .PartManager.ApplicationParts
                .Add(new AssemblyPart(Assembly.GetAssembly(typeof(InjectCmd))));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logFact)
        {
            app.UseBaseApi(env, Config.CmdDef.DisplayName, logFact);
            app.UseRouting();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}