using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.TestingHost;
using POC.Orleans.Infra.Configs;
using POC.Orleans.Infra.Contexts;
using System;

namespace POC.Orleans.Tests.Configs
{
    public class OrleansTestSilo : ISiloConfigurator
    {
        private static readonly Func<IServiceProvider, DapperContext> repoFactoryDapper = (_) =>
        {
            return new DapperContext(new DBConfig());
        };

        public void Configure(ISiloBuilder siloBuilder)
        {
            var configuration = siloBuilder.GetConfiguration();

            siloBuilder
                .UseLocalhostClustering()
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = configuration.GetSection("ConnectionStrings:connectionString").Value;
                })
                .AddMemoryGrainStorage("Person")
                .ConfigureServices(option =>
                {
                    option.AddScoped(repoFactoryDapper);
                });
        }
    }
}
