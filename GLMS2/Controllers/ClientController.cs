using GLMS2.Data;
using GLMS2.Models;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Dependency Injection of ApplicationDbContext
        // use EF Core with SQL Server for data persistence
        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Client
        //implement database entities and allow viewing stored data
        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync();
            return View(clients);
        }

        // GET: Client/Details/5
        //handle complex relationships (Client --> Contracts)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Client/Create
        // Returns form for creating a new client
        // UI must allow capturing client data
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // Saves new client to database using EF Core
        // Uses ViewModel for separation of concerns (MVC layered architecture requirement)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientCreateViewModel model)
        {
            // Model validation ensures data integrity
            // Meets requirement: validation of user input
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var client = new Client
            {
                Name = model.Name,
                Region = model.Region,
                Email = model.Email
            };
            // Adds client entity to SQL Server database
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Client/Edit/5
        // Retrieves existing client for editing
        // support updating stored records
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }
            // ViewModel used to maintain separation between UI and database model
            var model = new ClientCreateViewModel
            {
                Name = client.Name,
                Region = client.Region,
                Email = client.Email
            };

            ViewBag.ClientId = client.ClientId;
            return View(model);
        }

        // POST: Client/Edit/5
        // Updates existing client record in SQL Server database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ClientId = id;
                return View(model);
            }

            var client = await _context.Clients.FindAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            client.Name = model.Name;
            client.Region = model.Region;
            client.Email = model.Email;

            _context.Update(client);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Client/Delete/5
        // Displays confirmation page before deletion
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.ClientId == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Client/Delete/5
        // Permanently removes client from database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}