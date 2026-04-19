using GLMS2.Data;
using GLMS2.Interfaces;
using GLMS2.Models;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Services
{
    public class ClientService : IClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetAllClientsAsync()
        {
            return await _context.Clients.ToListAsync();
        }
    }
}