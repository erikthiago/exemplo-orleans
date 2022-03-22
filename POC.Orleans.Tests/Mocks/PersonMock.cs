using Bogus;
using Bogus.Extensions.Brazil;

namespace POC.Orleans.Tests.Mocks
{
    public static class PersonMock
    {
        public static Faker<Models.Models.Person> Gerar()
        {
            Faker<Models.Models.Person> pessoa = new Faker<Models.Models.Person>("pt_BR")
                .RuleFor(s => s.Name, f => f.Person.FullName)
                .RuleFor(s => s.CPF, f => f.Person.Cpf())
                .RuleFor(s => s.Address, f => $"{f.Person.Address.City} {f.Person.Address.Street} {f.Person.Address.State}")
                .RuleFor(s => s.BirthDate, f => f.Person.DateOfBirth.Date);

            return pessoa;
        }
    }
}
