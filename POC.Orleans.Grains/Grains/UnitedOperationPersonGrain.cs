using Dapper;
using Orleans;
using Orleans.Providers;
using POC.Orleans.GrainsInterfaces.Interfaces;
using POC.Orleans.Infra.Contexts;
using POC.Orleans.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POC.Orleans.Grains.Grains
{
    [StorageProvider(ProviderName = "Person")]
    public class UnitedOperationPersonGrain : Grain<Person>, IPersonGrain, IQueryAgeGrain, IQueryCpfGrain
    {
        public DapperContext _contextDapper;

        public UnitedOperationPersonGrain(DapperContext contextDapper)
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

        public Task UpdatePersonAsync(Person person)
        {
            throw new NotImplementedException();
        }
    }
}
