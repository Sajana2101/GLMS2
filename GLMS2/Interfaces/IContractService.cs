using GLMS2.Models;
using GLMS2.ViewModels;
using GLMS2.Enums;

namespace GLMS2.Interfaces
{
    // Handles business logic related to contracts
    // Keeps controllers separate from database operations
    public interface IContractService
    {
        // Creates a new contract and handles file upload
        Task<Contract> CreateContractAsync(ContractCreateViewModel model, string webRootPath);

        // Retrieves all contracts
        Task<IEnumerable<Contract>> GetAllContractsAsync();

        // Gets a single contract by ID
        Task<Contract?> GetContractByIdAsync(int id);

        // Filters contracts by date range and status
        Task<IEnumerable<Contract>> FilterContractsAsync(DateTime? startDateFrom, DateTime? startDateTo, ContractStatus? status);

        // Deletes a contract
        Task<bool> DeleteContractAsync(int id);

        // Retrieves contract data for editing
        Task<ContractEditViewModel?> GetContractForEditAsync(int id);

        // Updates contract details and file if changed
        Task<bool> UpdateContractAsync(ContractEditViewModel model, string webRootPath);
    }
}