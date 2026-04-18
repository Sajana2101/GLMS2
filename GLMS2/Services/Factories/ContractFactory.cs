using GLMS2.Enums;
using GLMS2.Models;
using GLMS2.Interfaces;

namespace GLMS2.Services.Factories
{
    public class ContractFactory : IContractFactory
    {
        public IContract CreateContract(ContractType contractType, Contract contract)
        {
            return contractType switch
            {
                ContractType.Domestic => new DomesticContract(contract),
                ContractType.International => new InternationalContract(contract),
                ContractType.Premium => new PremiumContract(contract),
                _ => throw new ArgumentException("Invalid contract type.")
            };
        }
    }
}