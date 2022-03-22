using Dapper;
using Orleans;
using POC.Orleans.GrainsInterfaces.Interfaces;
using POC.Orleans.Infra.Contexts;
using POC.Orleans.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POC.Orleans.Grains.Grains
{
    /// <summary>
    /// Implementação dos contratos contidos na interface e herdando as configurações dos Grãos
    /// </summary>
    public class QueryAgeGrain : Grain, IQueryAgeGrain
    {
        public DapperContext _contextDapper;

        public QueryAgeGrain(DapperContext contextDapper)
        {
            _contextDapper = contextDapper;
        }

        // Link de referencia para usar o sql e pegar o json do banco do orleans: https://docs.microsoft.com/pt-br/sql/relational-databases/json/json-data-sql-server?view=sql-server-ver15
        // Link de referencia para concatenar os dados do banco em uma linha: https://pt.stackoverflow.com/questions/203992/como-concatenar-linhas
        public Task<IEnumerable<Person>> QueryByMinimumAsync(int minimumAge)
        {
            var age = DateTime.Now.AddYears(-minimumAge);

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
                        WHERE birthDate <= @minimumAge";

            return Task.FromResult(_contextDapper.Connection.Query<Person>(sql, new { minimumAge = age }));
        }
    }
}
