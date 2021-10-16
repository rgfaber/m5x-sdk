using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using M5x.Config;
using M5x.DEC.Schema.Utils;
using Microsoft.Extensions.DependencyInjection;
using Robby.Client.Infra;
using Robby.Contract.Game.Features;
using Robby.Contract.Game.Queries;
using Robby.Schema;
using Serilog;

namespace Robby.CLI
{
    internal class Program
    {
        private static readonly IServiceCollection Services = new ServiceCollection();
        private static ServiceProvider _container;

        public static async Task Main(string[] args)
        {
            DotEnv.FromEmbedded();
            _container = Services
                .AddRobbyRequesters()
                .AddRobbyHopeClients()
                .AddRobbyQueryClients()
                .BuildServiceProvider();
            ConsoleKeyInfo mainKeyInfo;
            mainKeyInfo = await ShowMainMenu();
            Console.Clear();
            Console.WriteLine("Simulation Ended");
        }

        private static async Task<ConsoleKeyInfo> InitializeSimulation()
        {
            Console.WriteLine();
            Console.WriteLine("*** Initialize a Simulation ***");
            Console.Write("Simulation Name: ");
            var name = Console.ReadLine();

            Console.Write("Space Dimensions: \n");

            Console.Write("\tX [100]: ");
            var xstr = Console.ReadLine();
            if (string.IsNullOrEmpty(xstr)) xstr = "100";
            var x = Convert.ToInt32(xstr);

            Console.Write("\tY [100]: ");
            var ystr = Console.ReadLine();
            if (string.IsNullOrEmpty(ystr)) ystr = "100";
            var y = Convert.ToInt32(ystr);

            Console.Write("\tZ [100]: ");
            var zstr = Console.ReadLine();
            if (string.IsNullOrEmpty(zstr)) zstr = "100";
            var z = Convert.ToInt32(zstr);

            Console.Write("NumberOfRobots [20] : ");
            var rbstr = Console.ReadLine();
            if (string.IsNullOrEmpty(rbstr)) rbstr = "20";
            var nbrOfRobots = Convert.ToInt32(rbstr);


            var order = InitializationOrder.New(name, x, y, z, nbrOfRobots);

            if (!string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine($"Requesting Initialization for {name}");
                var hope = Initialize.Hope.New(GuidUtils.NewCleanGuid, order);
                // var requester = _container.GetService<Client.Infra.Features.InitializeContext.IRequester>();
                // var rsp = await requester.RequestAsync(req);
                var initContextClient = _container.GetService<Client.Infra.Features.Initialize.IClient>();
                var rsp = await initContextClient.Post(hope);
                if (rsp.IsSuccess)
                {
                    Console.WriteLine($"Simulation *{rsp.Meta.Id}* was initialized succesfully:");
                    Thread.Sleep(5000);
                }
                else
                {
                    Console.WriteLine("Request was not Succesful!");
                    Console.WriteLine(JsonSerializer.Serialize(rsp,
                        new JsonSerializerOptions(JsonSerializerDefaults.General)));
                    Thread.Sleep(10000);
                }
            }

            return await ShowMainMenu();
        }


        private static async Task<ConsoleKeyInfo> ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("********************");
            Console.WriteLine(" RoboSim Main Menu");
            Console.WriteLine("********************");
            Console.WriteLine();
            Console.WriteLine("\tI - Initialize Simulation");
            Console.WriteLine("\tL - List Simulations");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\tH - Crazy Ivan on HTTP (10.000.000 simulations)");
            Console.WriteLine("\tN - Wild Yuri on NATS  (10.000.000 simulations)");
            Console.WriteLine();
            Console.WriteLine("\tQ - Quit");
            var mainKeyInfo = Console.ReadKey();
            while (mainKeyInfo.Key != ConsoleKey.Q)
            {
                switch (mainKeyInfo.Key)
                {
                    case ConsoleKey.I:
                    {
                        mainKeyInfo = await InitializeSimulation();
                        break;
                    }
                    case ConsoleKey.L:
                    {
                        mainKeyInfo = await ShowSimulationList();
                        break;
                    }
                    case ConsoleKey.H:
                    {
                        await CrazyIvanOnHTTP();
                        break;
                    }
                    case ConsoleKey.N:
                    {
                        await WildYuriOnNATS();
                        break;
                    }
                }

                mainKeyInfo = Console.ReadKey();
            }

            return mainKeyInfo;
        }

        private static async Task CrazyIvanOnHTTP()
        {
            var j = 0;
            var err = 0;
            Stopwatch.StartNew();
            var startTime = Stopwatch.GetTimestamp();
            long avg = 1;
            Console.WriteLine();
            Console.WriteLine();
            var progressPos = Console.GetCursorPosition();
            do
            {
                var clt = _container.GetService<Client.Infra.Features.Initialize.IClient>();
                var ord = InitializationOrder.New(GuidUtils.NewCleanGuid, 51, 53, 58, 42);
                var hope = Initialize.Hope.New(GuidUtils.NewCleanGuid, ord);
                var feedback = await clt.Post(hope);
                var currTime = Stopwatch.GetTimestamp();
                var sec = (currTime - startTime) / Stopwatch.Frequency;
                if (sec > 1)
                    avg = j / sec;
                Console.Write($"Pushed {j} simulations in {sec}s using [{clt.GetType()}]. \n" +
                              $"Average: {avg} msg/sec\n" +
                              $"[{err}] Errrors");
                Console.SetCursorPosition(progressPos.Left, progressPos.Top);
                if (!feedback.IsSuccess) err++;
                j++;
            } while (j <= 9999999);
        }


        private static async Task WildYuriOnNATS()
        {
            long avg = 1;
            var j = 0;
            var err = 0;
            var clt = _container.GetService<Client.Infra.Features.Initialize.IRequester>();
            Stopwatch.StartNew();
            var startTime = Stopwatch.GetTimestamp();
            Console.WriteLine();
            Console.WriteLine();
            do
            {
                var ord = InitializationOrder.New(GuidUtils.NewCleanGuid, 51, 53, 58, 42);
                var hope = Initialize.Hope.New(GuidUtils.NewCleanGuid, ord);
                var feedback = await clt.RequestAsync(hope);
                var progressPos = Console.GetCursorPosition();
                var currTime = Stopwatch.GetTimestamp();
                var sec = (currTime - startTime) / Stopwatch.Frequency;
                if (sec > 1)
                    avg = j / sec;
                Console.Write($"Pushed {j} simulations in {sec}s using [{clt.GetType()}]. \n" +
                              $"Average: {avg} msg/sec\n" +
                              $"[{err}] Errrors");
                Console.SetCursorPosition(progressPos.Left, progressPos.Top);
                if (!feedback.IsSuccess) err++;
                j++;
            } while (j <= 9999999);
        }


        private static async Task<ConsoleKeyInfo> ShowSimulationList()
        {
            var qry = First20.Qry.New(GuidUtils.NewCleanGuid);
            var clt = _container.GetService<Client.Infra.Queries.First20.IClient>();
            var rsp = await clt.Post(qry);
            IEnumerable<Game> data = null;
            if (rsp.IsSuccess)
            {
                data = rsp.Data;
            }
            else
            {
                Log.Fatal($"An Error occured: \n{rsp.ErrorState}");
                Thread.Sleep(5000);
                return await ShowMainMenu();
            }

            Console.Clear();
            Console.WriteLine("************************************************************************");
            Console.WriteLine("**                LIST OF ROBBY SIMULATIONS                           **");
            Console.WriteLine("************************************************************************");
            Console.WriteLine();
            Console.WriteLine("   \t\tWorld\t\tDimensions\t\t#of Robots\t\tStatus");
            Console.WriteLine("------------------------------------------------------------------------");
            var ls = data.OrderBy(x => x.Description.Name);
            var counter = 1;
            var selList = new Dictionary<int, string>();
            foreach (var simulation in ls)
            { 
                selList.Add(counter,simulation.Id);
                Console.WriteLine(
                    $"{counter}." +
                    $"\t{simulation.Description.Name}" +
                    $"\t\t[{simulation.Dimensions.X}x{simulation.Dimensions.Y}x{simulation.Dimensions.Z}]" +
                    $"\t\t{simulation.Population.Robots.Count()}" +
                    $"\t\t\t{(Game.Flags) simulation.Meta.Status}");
                counter++;
            }
            
           
            Console.WriteLine();
            Console.WriteLine("  Q-Quit\t\tD-Details\t\tM-Main Menu");            
            
            
            var lstKey = Console.ReadKey();
            switch (lstKey.Key)
            {
                case ConsoleKey.M:
                {
                    lstKey = await ShowMainMenu();
                    break;
                }
                case ConsoleKey.D:
                {
                    Console.WriteLine();
                    Console.WriteLine("Type the number for World Details...:");
                    var det = Convert.ToInt32(Console.ReadLine());
                    var selId = selList[det];
                    lstKey = await ShowDetails(selId);
                    break;
                }
                default:
                    return lstKey;
            }

            return lstKey;
        }

        private static async Task<ConsoleKeyInfo> ShowDetails(string selId)
        {
            var cltDet = _container.GetService<Client.Infra.Queries.ById.IClient>();
            var qry = new ById.Qry(GuidUtils.NewCleanGuid)
            {
                Id = selId
            };
            ById.Rsp detRsp = await cltDet.Post(qry);
            Game game = detRsp.Data.ToArray()[0];
            Console.Clear();
            Console.WriteLine("***************************************************************");
            Console.WriteLine($"   World: ${game.Description.Name}                            ");
            Console.WriteLine("***************************************************************");
            Console.WriteLine("  Citizen \t\t\t Position \t\t Health \t\t  Status  \t\t Powers ");
            Console.WriteLine("------------------------------------------------------------------");
            foreach (var robot in game.Population.Robots)
            {
                Console.WriteLine($"  {robot.Description.Name.PadRight(30)}" +
                                  $"\t\t[{robot.Position.X}.{robot.Position.Y}.{robot.Position.Z}]" +
                                  $"\t\t{robot.Health.Value}" +
                                  $"\t\t{robot.Status}" +
                                  $"\t\t{robot.Kind}");
            }
            Console.WriteLine();
            Console.WriteLine("  Q-Quit\t\tM-Main Menu");            
            
            
            
            var lstKey = Console.ReadKey();
            switch (lstKey.Key)
            {
                case ConsoleKey.M:
                {
                    lstKey = await ShowMainMenu();
                    break;
                }
                default:
                    return lstKey;
            }

            return lstKey;
        }
    }
}