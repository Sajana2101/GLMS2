using GLMS2.Models;
using GLMS2.ViewModels;
using GLMS2.Data;
using GLMS2.Enums;
using GLMS2.Interfaces;
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

            var savedPath = await _fileService.SavePdfAsync(model.SignedAgreementFile!, webRootPath);

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
    }
}