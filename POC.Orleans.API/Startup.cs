using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;

namespace POC.Orleans.API
{
    public class Startup
    {  
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IClusterClient _client;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(CreateClusterClient);

            // Configuração do swagger para testar a API
            services.AddSwaggerGen(x => {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "POC Orlenas API",
                    Description = "POC Orlenas API",
                    //TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Érik Thiago",
                        Email = string.Empty,
                        Url = new Uri("https://github.com/erikthiago"),
                    }
                });
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "POC Orlenas V1");
            });
        }

        private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
        {
            // Ao dar erro de SiloUnavailableException: Could not find any gateway in global::Orleans.Runtime.Membership.AdoNetGatewayListProvider.
            // O erro era causado porque da configuração abaixo estava com valores diferentes do que tem no Silo.
            // Link de referencia para resolver o erro: https://gitter.im/dotnet/orleans/archives/2019/01/15
            _client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "POC-Orleans";
                    options.ServiceId = "POCOrleans";
                })
                .UseAdoNetClustering(options =>
                {
                    options.Invariant = "System.Data.SqlClient";
                    options.ConnectionString = Configuration.GetConnectionString("connectionString");
                })
                .ConfigureLogging(_ => _.AddConsole())                
                .Build();

            StartClientWithRetries(_client).Wait();
            return _client;
        }

        private async Task StartClientWithRetries(IClusterClient client)
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    await client.Connect();
                    return;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }
    }
}
