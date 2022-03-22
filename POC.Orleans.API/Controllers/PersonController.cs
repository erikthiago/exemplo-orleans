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

        [HttpGet]
        public async Task<IEnumerable<Person>> GetAll()
        {
            var persons = client.GetGrain<IPersonGrain>(Guid.NewGuid());

            return await persons.GetAllPersonAsync();
        }

        [HttpGet("{id}")]
        public async Task<Person> Get(Guid id)
        {
            var person = client.GetGrain<IPersonGrain>(id);

            return await person.GetPersonAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Person person)
        {
            var newPerson = client.GetGrain<IPersonGrain>(Guid.NewGuid());
            await newPerson.AddNewPersonAsync(person);

            return Ok(newPerson.GetPrimaryKey());
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Person person, [FromQuery] Guid id)
        {
            var updatePerson = client.GetGrain<IPersonGrain>(id);
            await updatePerson.UpdatePersonAsync(person);

            return Ok(updatePerson.GetPrimaryKey());
        }

        [HttpGet("age/{age}")]
        public async Task<IEnumerable<Person>> GetByMinimalAge(int age)
        {
            var personsWithAgeSelected = client.GetGrain<IQueryAgeGrain>(age);

            return await personsWithAgeSelected.QueryByMinimumAsync(age);
        }

        [HttpGet("cpf/{cpf}")]
        public async Task<Person> GetByCPF(string cpf)
        {
            var personWithCPFSelected = client.GetGrain<IQueryCpfGrain>(cpf);

            return await personWithCPFSelected.QueryByCpfAsync(cpf);
        }
    }
}
