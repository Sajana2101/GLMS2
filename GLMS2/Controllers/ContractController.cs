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

        public ContractController(
            IContractService contractService,
            IClientService clientService,
            IWebHostEnvironment environment)
        {
            _contractService = contractService;
            _clientService = clientService;
            _environment = environment;
        }

        public async Task<IActionResult> Index(DateTime? startDateFrom, DateTime? startDateTo, ContractStatus? status)
        {
            var contracts = await _contractService.FilterContractsAsync(startDateFrom, startDateTo, status);

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

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

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
                await _contractService.CreateContractAsync(model, _environment.WebRootPath);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDropdowns();
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

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

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);

            if (contract == null || string.IsNullOrWhiteSpace(contract.SignedAgreementFilePath))
            {
                return NotFound();
            }

            var fullPath = Path.Combine(_environment.WebRootPath, contract.SignedAgreementFilePath);

            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound();
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(fullPath);
            var fileName = Path.GetFileName(fullPath);

            return File(fileBytes, "application/pdf", fileName);
        }

        private async Task LoadDropdowns()
        {
            var clients = await _clientService.GetAllClientsAsync();

            ViewBag.Clients = new SelectList(clients, "ClientId", "Name");

            ViewBag.StatusList = Enum.GetValues(typeof(ContractStatus))
                .Cast<ContractStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                })
                .ToList();

            ViewBag.ContractTypes = Enum.GetValues(typeof(ContractType))
                .Cast<ContractType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.ToString()
                })
                .ToList();
        }
    }
}