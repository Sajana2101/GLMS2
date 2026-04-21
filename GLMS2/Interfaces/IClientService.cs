using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IClientService
    {
        // Retrieves all clients from the database asynchronously
        // Used to populate dropdowns and display client information in the UI
        Task<IEnumerable<Client>> GetAllClientsAsync();
    }
}