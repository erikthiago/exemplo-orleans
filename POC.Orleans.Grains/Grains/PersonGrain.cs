using Dapper;
using Orleans;
using Orleans.Providers;
using POC.Orleans.GrainsInterfaces.Interfaces;
using POC.Orleans.Infra.Contexts;
using POC.Orleans.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POC.Orleans.Grains.Grains
{
    /// <summary>
    /// Implementação dos contratos contidos na interface e herdando as configurações dos Grãos e definindo o nome do provedor para acessar os dados no silo
    /// </summary>
    [StorageProvider(ProviderName = "Person")]
    public class PersonGrain : Grain<Person>, IPersonGrain
    {
        public DapperContext _contextDapper;

        public PersonGrain(DapperContext contextDapper)
        {
            _contextDapper = contextDapper;
        }

        /// <summary>
        /// Adiciona uma nova pessoa ao estado do Orleans para salvar no banco e na memória
        /// </summary>
        /// <param name="person">O objeto da pessoa com os dados</param>
        /// <returns>Retorna uma Task</returns>
        public async Task AddNewPersonAsync(Person person)
        {
            State.Address = person.Address;
            State.BirthDate = person.BirthDate;
            State.Name = person.Name;
            State.CPF = person.CPF;

            await WriteStateAsync();
        }

        /// <summary>
        /// Faz a busca no banco de dados para trazer a lista de todas as pessoas existentes no banco
        /// </summary>
        /// <returns>Retorna uma lista de todas as pessoas existentes no banco de dados</returns>
        public Task<IEnumerable<Person>> GetAllPersonAsync()
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
                           )";

            return Task.FromResult(_contextDapper.Connection.Query<Person>(sql));
        }

        /// <summary>
        /// Busca a pessoa pela identificação única informada (Guid)
        /// </summary>
        /// <returns>Retorna a pessoa vinculada a esssa identificação unica</returns>
        public Task<Person> GetPersonAsync()
        {
            return Task.FromResult(new Person()
            {
                Address = State.Address,
                BirthDate = State.BirthDate,
                Name = State.Name,
                CPF = State.CPF
            });
        }

        /// <summary>
        /// Altera os dados da pessoa a partir da identificação
        /// </summary>
        /// <param name="person">Objeto com os dados para serem alterados</param>
        /// <returns>Retorna uma Task</returns>
        public async Task UpdatePersonAsync(Person person)
        {
            State.Address = person.Address;
            State.BirthDate = person.BirthDate;
            State.Name = person.Name;
            State.CPF = person.CPF;

            await WriteStateAsync();
        }
    }
}
