using GLMS2.Data;
using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.Models;
using GLMS2.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GLMS2.Services
{
    public class ContractService : IContractService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;
        private readonly IContractFactory _contractFactory;

        public ContractService(
            ApplicationDbContext context,
            IFileService fileService,
            IContractFactory contractFactory)
        {
            _context = context;
            _fileService = fileService;
            _contractFactory = contractFactory;
        }

        public async Task<Contract> CreateContractAsync(ContractCreateViewModel model, string webRootPath)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.StartDate >= model.EndDate)
            {
                throw new InvalidOperationException("Start date must be before end date.");
            }

            if (model.SignedAgreementFile == null)
            {
                throw new InvalidOperationException("A signed agreement PDF is required.");
            }

            var savedPath = await _fileService.SavePdfAsync(model.SignedAgreementFile, webRootPath);

            var contract = new Contract
            {
                ClientId = model.ClientId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ServiceLevel = model.ServiceLevel,
                ContractType = model.ContractType,
                SignedAgreementFilePath = savedPath
            };

            // Factory Pattern
            var contractPatternObject = _contractFactory.CreateContract(model.ContractType, contract);

            if (!contractPatternObject.Validate())
            {
                throw new InvalidOperationException("Contract validation failed.");
            }

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return contract;
        }

        public async Task<IEnumerable<Contract>> GetAllContractsAsync()
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .ToListAsync();
        }

        public async Task<Contract?> GetContractByIdAsync(int id)
        {
            return await _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.ServiceRequests)
                .FirstOrDefaultAsync(c => c.ContractId == id);
        }

        public async Task<IEnumerable<Contract>> FilterContractsAsync(DateTime? startDateFrom, DateTime? startDateTo, ContractStatus? status)
        {
            var query = _context.Contracts
                .Include(c => c.Client)
                .AsQueryable();

            if (startDateFrom.HasValue)
            {
                query = query.Where(c => c.StartDate >= startDateFrom.Value);
            }

            if (startDateTo.HasValue)
            {
                query = query.Where(c => c.StartDate <= startDateTo.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return false;
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<ContractEditViewModel?> GetContractForEditAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);

            if (contract == null)
            {
                return null;
            }

            return new ContractEditViewModel
            {
                ContractId = contract.ContractId,
                ClientId = contract.ClientId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel,
                ContractType = contract.ContractType,
                ExistingSignedAgreementFilePath = contract.SignedAgreementFilePath
            };
        }

        public async Task<bool> UpdateContractAsync(ContractEditViewModel model, string webRootPath)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var contract = await _context.Contracts.FindAsync(model.ContractId);

            if (contract == null)
            {
                return false;
            }

            if (model.StartDate >= model.EndDate)
            {
                throw new InvalidOperationException("Start date must be before end date.");
            }

            contract.ClientId = model.ClientId;
            contract.StartDate = model.StartDate;
            contract.EndDate = model.EndDate;
            contract.Status = model.Status;
            contract.ServiceLevel = model.ServiceLevel;
            contract.ContractType = model.ContractType;

            if (model.SignedAgreementFile != null)
            {
                var savedPath = await _fileService.SavePdfAsync(model.SignedAgreementFile, webRootPath);
                contract.SignedAgreementFilePath = savedPath;
            }

            var contractPatternObject = _contractFactory.CreateContract(model.ContractType, contract);

            if (!contractPatternObject.Validate())
            {
                throw new InvalidOperationException("Contract validation failed.");
            }

            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}