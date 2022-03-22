using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.TestingHost;
using System;

namespace POC.Orleans.Tests.Configs
{
    public class OrleansTestCluster : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        public IServiceCollection ServiceCollection { get; }
        public TestCluster Cluster { get; private set; }
        public IConfiguration Configuration { get; }

        public IServiceScopeFactory ScopeFactory => _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        public OrleansTestCluster()
        {
            ServiceCollection = new ServiceCollection()
           .AddSingleton<IConfiguration>(sp =>
           {
               var builder = new ConfigurationBuilder();

               return builder
                   .GetJsonFile()
                   .Build();
           });

            _serviceProvider = ServiceCollection.BuildServiceProvider();

            var builder = new TestClusterBuilder()
                .ConfigureHostConfiguration(x => { x.GetJsonFile(); })
                .AddSiloBuilderConfigurator<OrleansTestSilo>();

            var cluster = builder.Build();
            Cluster = cluster ?? throw new InvalidOperationException("Testing cluster can't be null");

            Cluster.Deploy();
        }

        public IServiceScope CreateServiceScope() => ScopeFactory.CreateScope();


        public void Dispose()
        {
            Cluster.StopAllSilos();
        }
    }
}
