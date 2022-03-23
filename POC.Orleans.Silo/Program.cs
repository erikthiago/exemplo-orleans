using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using POC.Orleans.Infra.Configs;
using POC.Orleans.Infra.Contexts;
using System;
using System.Net;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace POC.Orleans.Silo
{
    /// <summary>
    /// Classe de configuração do Silo
    /// </summary>
    class Program
    {
        private static readonly Func<IServiceProvider, DapperContext> repoFactoryDapper = (_) =>
        {
            return new DapperContext(new DBConfig());
        };

        private static ISiloHost silo;
        private static readonly ManualResetEvent siloStopped = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            silo = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "POC-Orleans";
                    options.ServiceId = "POCOrleans";
                })
                .AddAdoNetGrainStorage("Person", options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = config.GetConnectionString("connectionString");
                    options.UseJsonFormat = true;
                })
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = config.GetConnectionString("connectionString");
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureEndpoints(siloPort: 11111, gatewayPort: 30000)
                .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory())
                .ConfigureLogging(logging => logging.AddConsole())
                .UseDashboard(options => {
                })
                .ConfigureServices(option =>
                {
                    option.AddScoped(repoFactoryDapper);
                })
                .Build();

            Task.Run(StartSilo);

            AssemblyLoadContext.Default.Unloading += context =>
            {
                Task.Run(StopSilo);
                siloStopped.WaitOne();
            };

            siloStopped.WaitOne();
        }

        private static async Task StartSilo()
        {
            await silo.StartAsync();
            Console.WriteLine("Silo iniciado");
        }

        private static async Task StopSilo()
        {
            await silo.StopAsync();
            Console.WriteLine("Silo parado");
            siloStopped.Set();
        }
    }
}
