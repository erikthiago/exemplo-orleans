using Microsoft.AspNetCore.Mvc;
using Orleans;
using POC.Orleans.GrainsInterfaces.Interfaces;
using POC.Orleans.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POC.Orleans.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private IClusterClient client;

        public PersonController(IClusterClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Faz a busca no banco de dados para trazer a lista de todas as pessoas existentes no banco
        /// </summary>
        /// <returns>Retorna uma lista de todas as pessoas existentes no banco de dados</returns>
        [HttpGet]
        public async Task<IEnumerable<Person>> GetAll()
        {
            var persons = client.GetGrain<IPersonGrain>(Guid.NewGuid());

            return await persons.GetAllPersonAsync();
        }

        /// <summary>
        /// Busca a pessoa pela identificação única informada (Guid)
        /// </summary>
        /// <returns>Retorna a pessoa vinculada a esssa identificação unica</returns>
        /// <param name="id">Identificação unica da pessoa</param>
        [HttpGet("{id}")]
        public async Task<Person> Get(Guid id)
        {
            var person = client.GetGrain<IPersonGrain>(id);

            return await person.GetPersonAsync();
        }

        /// <summary>
        /// Adiciona uma nova pessoa ao estado do Orleans para salvar no banco e na memória
        /// </summary>
        /// <param name="person">O objeto da pessoa com os dados</param>
        /// <returns>Retorna a identificação vinculada a pessoa/returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Person person)
        {
            var newPerson = client.GetGrain<IPersonGrain>(Guid.NewGuid());
            await newPerson.AddNewPersonAsync(person);

            return Ok(newPerson.GetPrimaryKey());
        }

        /// <summary>
        /// Altera os dados da pessoa a partir da identificação
        /// </summary>
        /// <param name="person">Objeto com os dados para serem alterados</param>
        /// <param name="id">Identificação unica da pessoa</param>
        /// <returns>Retorna a identificação da pessoa</returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Person person, [FromQuery] Guid id)
        {
            var updatePerson = client.GetGrain<IPersonGrain>(id);
            await updatePerson.UpdatePersonAsync(person);

            return Ok(updatePerson.GetPrimaryKey());
        }

        /// <summary>
        /// Método que faz a pesquisa no banco para encontrar pessoas com a idade informada. Idade informada ou abaixo.
        /// </summary>
        /// <param name="minimumAge">Idade informada</param>
        /// <returns>Retorna a lista de pessoas com a idade informada</returns>
        [HttpGet("age/{age}")]
        public async Task<IEnumerable<Person>> GetByMinimalAge(int age)
        {
            var personsWithAgeSelected = client.GetGrain<IQueryAgeGrain>(age);

            return await personsWithAgeSelected.QueryByMinimumAsync(age);
        }

        /// <summary>
        /// Retorna a pessoa vinculada ao número de CPF informado
        /// </summary>
        /// <param name="cpf">Numero do CPF a ser pesquisado no banco</param>
        /// <returns>Retorna a pessoa vinculada ao CPF informado</returns>
        [HttpGet("cpf/{cpf}")]
        public async Task<Person> GetByCPF(string cpf)
        {
            var personWithCPFSelected = client.GetGrain<IQueryCpfGrain>(cpf);

            return await personWithCPFSelected.QueryByCpfAsync(cpf);
        }
    }
}
