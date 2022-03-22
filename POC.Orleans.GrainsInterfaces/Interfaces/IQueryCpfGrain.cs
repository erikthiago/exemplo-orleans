using Orleans;
using POC.Orleans.Models.Models;
using System.Threading.Tasks;

namespace POC.Orleans.GrainsInterfaces.Interfaces
{
    /// <summary>
    /// Interface que define os contratos e qual será o tipo de PK no banco dados e também a identificação no silo
    /// </summary>
    public interface IQueryCpfGrain : IGrainWithStringKey
    {
        Task<Person> QueryByCpfAsync(string cpf);
    }
}
