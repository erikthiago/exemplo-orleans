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

        public async Task AddNewPersonAsync(Person person)
        {
            State.Address = person.Address;
            State.BirthDate = person.BirthDate;
            State.Name = person.Name;
            State.CPF = person.CPF;

            await WriteStateAsync();
        }

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
