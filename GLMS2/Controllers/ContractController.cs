using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS2.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IClientService _clientService;
        private readonly IWebHostEnvironment _environment;

        // Uses Dependency Injection with Interfaces
        //layered architecture (Controller --> Service --> Data)
        public ContractController(
            IContractService contractService,
            IClientService clientService,
            IWebHostEnvironment environment)
        {
            _contractService = contractService;
            _clientService = clientService;
            _environment = environment;
        }
        // Displays all contracts with optional filtering
        // LINQ filtering mechanism for searching contracts
        public async Task<IActionResult> Index(DateTime? startDateFrom, DateTime? startDateTo, ContractStatus? status)
        {
            var contracts = await _contractService.FilterContractsAsync(startDateFrom, startDateTo, status);
            // Populates dropdown for filtering by workflow status
            // Implement Contract Status workflow (Draft, Active, Expired, OnHold)
            ViewBag.StatusList = Enum.GetValues(typeof(ContractStatus))
                .Cast<ContractStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                })
                .ToList();

            ViewBag.SelectedStartDateFrom = startDateFrom?.ToString("yyyy-MM-dd");
            ViewBag.SelectedStartDateTo = startDateTo?.ToString("yyyy-MM-dd");
            ViewBag.SelectedStatus = status?.ToString();

            return View(contracts);
        }
        // Displays form for creating a contract
        // UI for capturing contract data
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }
        // Creates contract and uploads signed agreement PDF
        // file handling (Signed Agreement must be uploaded and stored on server)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                // Service layer handles business logic and file saving
                await _contractService.CreateContractAsync(model, _environment.WebRootPath);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Handles validation errors from business logic
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDropdowns();
                return View(model);
            }
        }
        // Displays contract details including related information
        // view stored contract data

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }
        // Displays confirmation page before deletion
        // full CRUD support for Contract entity
        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }
        // Deletes contract from database via service layer
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _contractService.DeleteContractAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        // Allows user to download the uploaded PDF agreement
        // stored file must be downloadable via UI
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null || string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath))
            {
                return NotFound();
            }
            // Combines wwwroot path with stored file path
            var fullPath = Path.Combine(_environment.WebRootPath, contract.SignedAgreementFilePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }
            // Returns file as PDF download
            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var fileName = Path.GetFileName(fullPath);

            return File(fileBytes, "application/pdf", fileName);
        }
        // Loads dropdown data for Client and ContractType
        private async Task LoadDropdowns()
        {
            var clients = await _clientService.GetAllClientsAsync();

            // Allows user to link contract to client
            ViewBag.Clients = new SelectList(clients, "ClientId", "Name");
            // Provides workflow status options
            ViewBag.StatusList = Enum.GetValues(typeof(ContractStatus))
                .Cast<ContractStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                })
                .ToList();
            // Uses Factory-related enum to determine contract type
            // use design patterns (Factory Method)
            ViewBag.ContractTypes = Enum.GetValues(typeof(ContractType))
                .Cast<ContractType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.ToString()
                })
                .ToList();
        }
        // Retrieves contract data for editing
        // updating existing contracts in system
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _contractService.GetContractForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            await LoadDropdowns();
            return View(model);
        }
        // Updates contract and allows replacement of PDF file
        // maintain editable contract workflow and file handling
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContractEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                // Service updates database and handles updated file if provided
                var updated = await _contractService.UpdateContractAsync(model, _environment.WebRootPath);

                if (!updated)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDropdowns();
                return View(model);
            }
        }
    }
}