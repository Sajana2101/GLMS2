
using GLMS2.Models;
using GLMS2.Services.Mediator;
using GLMS2.Services.Observers;
using GLMS2.ViewModels;
using GLMS2.Data;
using GLMS2.Enums;
using GLMS2.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;
        private readonly IMediator _mediator;

        public ServiceRequestService(
            ApplicationDbContext context,
            ICurrencyService currencyService,
            IMediator mediator)
        {
            _context = context;
            _currencyService = currencyService;
            _mediator = mediator;
        }

        public async Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequestCreateViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.ContractId == model.ContractId);

            if (contract == null)
            {
                throw new InvalidOperationException("Selected contract does not exist.");
            }

            if (contract.Status == ContractStatus.Expired || contract.Status == ContractStatus.OnHold)
            {
                throw new InvalidOperationException("A service request cannot be created for an expired or on-hold contract.");
            }

            if (contract.Status != ContractStatus.Active)
            {
                throw new InvalidOperationException("A service request can only be created for an active contract.");
            }

            var rate = await _currencyService.GetUsdToZarRateAsync();
            var localCost = _currencyService.ConvertUsdToZar(model.CostUSD, rate);

            var serviceRequest = new ServiceRequest
            {
                ContractId = model.ContractId,
                Description = model.Description,
                CostUSD = model.CostUSD,
                CostZAR = localCost,
                Status = ServiceRequestStatus.Pending,
                CreatedDate = DateTime.UtcNow
            };

            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            var subject = new ServiceRequestSubject
            {
                ServiceRequestId = serviceRequest.ServiceRequestId
            };

            subject.Attach(new NotificationObserver());
            subject.Attach(new AuditObserver());
            subject.SetStatus(serviceRequest.Status.ToString());

            _mediator.Notify(this, "ServiceRequestCreated");

            return serviceRequest;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync()
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .ToListAsync();
        }

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(int id)
        {
            return await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);
        }

        public async Task<bool> CanCreateServiceRequestAsync(int contractId)
        {
            var contract = await _context.Contracts.FirstOrDefaultAsync(c => c.ContractId == contractId);

            if (contract == null)
            {
                return false;
            }

            return contract.Status == ContractStatus.Active;
        }
    }
}