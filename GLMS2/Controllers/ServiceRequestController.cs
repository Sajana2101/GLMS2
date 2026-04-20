using GLMS2.Data;
using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestController(
            IServiceRequestService serviceRequestService,
            ApplicationDbContext context,
            ICurrencyService currencyService)
        {
            _serviceRequestService = serviceRequestService;
            _context = context;
            _currencyService = currencyService;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _serviceRequestService.GetAllServiceRequestsAsync();
            return View(serviceRequests);
        }

        public async Task<IActionResult> Create()
        {
            await LoadContractsDropdown();

            var model = new ServiceRequestCreateViewModel();

            try
            {
                model.ExchangeRate = await _currencyService.GetUsdToZarRateAsync();
            }
            catch
            {
                model.ExchangeRate = 0;
                ViewBag.ApiError = "Exchange rate could not be loaded. Please try again later.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadContractsDropdown();

                try
                {
                    model.ExchangeRate = await _currencyService.GetUsdToZarRateAsync();
                    model.CostZAR = model.CostUSD > 0
                        ? _currencyService.ConvertUsdToZar(model.CostUSD, model.ExchangeRate)
                        : 0;
                }
                catch
                {
                    ViewBag.ApiError = "Exchange rate could not be loaded.";
                }

                return View(model);
            }

            try
            {
                var createdRequest = await _serviceRequestService.CreateServiceRequestAsync(model);
                return RedirectToAction(nameof(Details), new { id = createdRequest.ServiceRequestId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadContractsDropdown();

                try
                {
                    model.ExchangeRate = await _currencyService.GetUsdToZarRateAsync();
                    model.CostZAR = model.CostUSD > 0
                        ? _currencyService.ConvertUsdToZar(model.CostUSD, model.ExchangeRate)
                        : 0;
                }
                catch
                {
                    ViewBag.ApiError = "Exchange rate could not be loaded.";
                }

                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest = await _serviceRequestService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var serviceRequest = await _serviceRequestService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _serviceRequestService.DeleteServiceRequestAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadContractsDropdown()
        {
            var contracts = await _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == ContractStatus.Active)
                .ToListAsync();

            ViewBag.Contracts = new SelectList(
                contracts.Select(c => new
                {
                    c.ContractId,
                    DisplayText = $"Contract #{c.ContractId} - {c.Client!.Name} ({c.Status})"
                }),
                "ContractId",
                "DisplayText");
        }
    }
}