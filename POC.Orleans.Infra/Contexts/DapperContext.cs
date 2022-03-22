using POC.Orleans.Infra.Configs.Interfaces;
using POC.Orleans.Infra.Contexts.Base;

namespace POC.Orleans.Infra.Contexts
{
    /// <summary>
    /// Classe que herda do contexto base para passar para ela o valor da chave connectio string contida no appsettings.json
    /// </summary>
    public class DapperContext : DapperContextBase
    {
        // Usando o construtor para pegar a conexão com o banco através do método DBConfig().ConnectionString()
        private readonly IDbConfig _dbConfig;

        public DapperContext(IDbConfig dbConfig) : base(dbConfig.ConnectionString())
        {
            _dbConfig = dbConfig;
        }
    }
}
