using POC.Orleans.GrainsInterfaces.Interfaces;
using POC.Orleans.Tests.Configs;
using POC.Orleans.Tests.Mocks;
using System;
using System.Threading.Tasks;
using Xunit;

namespace POC.Orleans.Tests
{
    [Collection("PersonCollection")]
    public class PersonTest
    {
        private OrleansTestCluster _testCluster;

        public PersonTest()
        {
            _testCluster = new OrleansTestCluster();
        }

        [Fact]
        public async Task AddAndRetrievePersonByIdTest()
        {
            var personToSave = PersonMock.Gerar().Generate();

            var personExecutions = _testCluster.Cluster.GrainFactory.GetGrain<IPersonGrain>(Guid.NewGuid());
            await personExecutions.AddNewPersonAsync(personToSave);

            var clusteredPerson = await personExecutions.GetPersonAsync();

            Assert.Equal(personToSave, clusteredPerson);
            Assert.NotNull(personToSave);
            Assert.NotNull(clusteredPerson);
        }
    }
}
