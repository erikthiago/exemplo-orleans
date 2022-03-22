using Microsoft.Extensions.Configuration;
using System.IO;

namespace POC.Orleans.Infra.ReadJson
{
    /// <summary>
    /// Classe responsável por ler o arquivo appsettings.json a procura da chave de connection string para o banco de dados
    /// </summary>
    public class ReadJsonSettings
    {
        // Nome da connection string
        private readonly string _connectionString;

        // Passa o nome da connection string por construtor
        public ReadJsonSettings(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Método que vai ser chamado para encontrar o arquivo que contém a connection string
        /// </summary>
        /// <returns>Configura a conexão com o banco a partir do </returns>
        public string ConnectionString()
        {
            var config = GetConnectionString();
            return config;
        }

        // Procura o arquivo appsettings.json na raiz do projeto
        private string GetConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json")
                .Build();

            return (config.GetConnectionString(_connectionString));
        }
    }
}
