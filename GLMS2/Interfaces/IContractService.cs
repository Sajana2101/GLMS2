using GLMS2.Models;
using GLMS2.ViewModels;

namespace GLMS2.Interfaces
{
    public interface IContractService
    {
        Task<Contract> CreateContractAsync(ContractCreateViewModel model, string webRootPath);
        Task<IEnumerable<Contract>> GetAllContractsAsync();
        Task<Contract?> GetContractByIdAsync(int id);
        Task<IEnumerable<Contract>> FilterContractsAsync(DateTime? startDateFrom, DateTime? startDateTo, string? status);
    }
}
