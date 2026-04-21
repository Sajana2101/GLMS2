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
        // Constructor injection keeps the controller connected to the service layer,
        // database context, and external currency conversion functionality
        public ServiceRequestController(
            IServiceRequestService serviceRequestService,
            ApplicationDbContext context,
            ICurrencyService currencyService)
        {
            _serviceRequestService = serviceRequestService;
            _context = context;
            _currencyService = currencyService;
        }
        // Displays all service requests saved in the system
        // This gives users access to the stored workflow data from the database
        public async Task<IActionResult> Index()
        {
            var serviceRequests = await _serviceRequestService.GetAllServiceRequestsAsync();
            return View(serviceRequests);
        }
        // Loads the create form and retrieves the live USD to ZAR exchange rate
        // This supports the external API integration required in Part 2
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
                // Handles API failure gracefully so the page can still load
                model.ExchangeRate = 0;
                ViewBag.ApiError = "Exchange rate could not be loaded. Please try again later.";
            }

            return View(model);
        }
        // Creates a new service request using validated user input
        // Business logic is delegated to the service layer rather than handled directly in the controller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadContractsDropdown();

                try
                {
                    // Recalculates converted cost so the user does not lose useful values after validation fails
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
                // Service handles workflow validation, including whether the linked contract allows a service request
                var createdRequest = await _serviceRequestService.CreateServiceRequestAsync(model);
                return RedirectToAction(nameof(Details), new { id = createdRequest.ServiceRequestId });
            }
            catch (Exception ex)
            {
                // Displays business rule errors, such as blocked creation for invalid contract statuses
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
        // Shows full details for a single service request
        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest = await _serviceRequestService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }
        // Displays a confirmation page before deleting a service request
        public async Task<IActionResult> Delete(int id)
        {
            var serviceRequest = await _serviceRequestService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }
        // Removes the selected service request from the database through the service layer
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
        // Loads only active contracts into the dropdown
        // This supports the workflow rule by preventing users from selecting contracts
        // that should not accept new service requests
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
        // Loads the edit form with existing service request data
        // Also refreshes the current exchange rate for updated currency conversion
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _serviceRequestService.GetServiceRequestForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            await LoadContractsDropdown();

            try
            {
                model.ExchangeRate = await _currencyService.GetUsdToZarRateAsync();
            }
            catch
            {
                // Prevents the edit page from failing completely if the API is temporarily unavailable
                model.ExchangeRate = 0;
            }

            return View(model);
        }
        // Updates an existing service request after validation
        // The service layer keeps the controller clean and handles the update logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceRequestEditViewModel model)
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
                var updated = await _serviceRequestService.UpdateServiceRequestAsync(model);

                if (!updated)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Returns service-layer business errors back to the form
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
    }
}