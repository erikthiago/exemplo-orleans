using Microsoft.Data.SqlClient;
using POC.Orleans.Infra.ReadJson;
using System;
using System.Data;

namespace POC.Orleans.Infra.Contexts.Base
{
    /// <summary>
    /// Classe base de configurações do contexto do dapper para criar conexão com o banco e fechar ela quando não estiver mais sendo usada
    /// </summary>
    public class DapperContextBase : IDisposable
    {
        // Configuração da conexão com o banco
        public SqlConnection Connection { get; set; }

        // Conexão com o banco de dados
        private DapperContextBase(ReadJsonSettings jsonSettings)
        {
            Connection = new SqlConnection(jsonSettings.ConnectionString());
            Connection.Open();
        }

        // Recebe o nome da connection string pelo construtor
        public DapperContextBase(string connectionString) : this(new ReadJsonSettings(connectionString))
        {
        }

        // Fecha a conexão após o uso
        public void Dispose()
        {
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }
    }
}
