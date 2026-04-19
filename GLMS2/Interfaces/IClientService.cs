using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IClientService
    {
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}