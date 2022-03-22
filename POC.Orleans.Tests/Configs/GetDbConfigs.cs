using Microsoft.Extensions.Configuration;

namespace POC.Orleans.Tests.Configs
{
    public static class GetDbConfigs
    {
        public static IConfigurationBuilder GetJsonFile(this IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
               .AddJsonFile("appsettings.json")
               .Build();

            return configurationBuilder;
        }
    }
}
