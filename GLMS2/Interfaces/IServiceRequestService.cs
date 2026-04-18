using GLMS2.ViewModels;
using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IServiceRequestService
    {
        Task<ServiceRequest> CreateServiceRequestAsync(ServiceRequestCreateViewModel model);
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync();
        Task<ServiceRequest?> GetServiceRequestByIdAsync(int id);
        Task<bool> CanCreateServiceRequestAsync(int contractId);
    }
}
