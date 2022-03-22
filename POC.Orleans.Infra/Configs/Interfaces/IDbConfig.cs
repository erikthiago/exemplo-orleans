namespace POC.Orleans.Infra.Configs.Interfaces
{
    /// <summary>
    /// Interface que define os contratos a serem implementados referentes a pegar o conteudo da chave connection string no appsettings.json
    /// </summary>
    public interface IDbConfig
    {
        string ConnectionString();
    }
}
