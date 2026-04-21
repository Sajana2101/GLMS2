using GLMS2.Data;
using GLMS2.Interfaces;
using GLMS2.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Services
{
    // Service responsible for retrieving client data from the database
    // Keeps database logic separate from controllers
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Returns all clients stored in the database
        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }
    }
}