using GLMS.ViewModels;
using GLMS.Models;

namespace GLMS.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequestCreateViewModel model);
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync();
        Task<ServiceRequest?> GetServiceRequestByIdAsync(int id);
        Task<bool> CanCreateServiceRequestAsync(int contractId);
    }
}
