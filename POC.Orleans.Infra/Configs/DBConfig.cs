using POC.Orleans.Infra.Configs.Interfaces;

namespace POC.Orleans.Infra.Configs
{
    /// <summary>
    /// Classe responsável por informar o nome da connection string com o banco
    /// </summary>
    public class DBConfig : IDbConfig
    {
        public DBConfig()
        {

        }

        // Método responsável por armazenar o nome da connection string presente no appsettings.json
        public string ConnectionString()
        {
            return "connectionString";
        }
    }
}
