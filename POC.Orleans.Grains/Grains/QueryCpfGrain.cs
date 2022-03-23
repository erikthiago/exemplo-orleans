using Dapper;
using Orleans;
using POC.Orleans.GrainsInterfaces.Interfaces;
using POC.Orleans.Infra.Contexts;
using POC.Orleans.Models.Models;
using System.Threading.Tasks;

namespace POC.Orleans.Grains.Grains
{
    /// <summary>
    /// Implementação dos contratos contidos na interface e herdando as configurações dos Grãos
    /// </summary>
    public class QueryCpfGrain : Grain, IQueryCpfGrain
    {
        public DapperContext _contextDapper;

        public QueryCpfGrain(DapperContext contextDapper)
        {
            _contextDapper = contextDapper;
        }

        /// <summary>
        /// Retorna a pessoa vinculada ao número de CPF informado
        /// </summary>
        /// <param name="cpf">Numero do CPF a ser pesquisado no banco</param>
        /// <returns>Retorna a pessoa vinculada ao CPF informado</returns>
        public Task<Person> QueryByCpfAsync(string cpf)
        {
            var sql = @"DECLARE @json AS NVARCHAR(MAX);
                       SELECT @json = ISNULL(@json + ', ', '') + PayloadJson
                           FROM OrleansStorage
                       SET @json = '[' + @json + ']'
                       --SELECT @json
                       SELECT*
                       FROM OPENJSON(@json)
                           WITH(
                           name NVARCHAR(50) '$.Name',
                           birthDate DATE '$.BirthDate',
                           address NVARCHAR(MAX) '$.Address',
                           cpf NVARCHAR(11) '$.CPF'
                           )
                        WHERE cpf = @cpf";

            return Task.FromResult(_contextDapper.Connection.QuerySingle<Person>(sql, new { cpf = cpf }));
        }
    }
}
