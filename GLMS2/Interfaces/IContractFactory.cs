using GLMS.Enums;
using GLMS.Models;

namespace GLMS.Interfaces
{
    public interface IContractFactory
    {
        IContract CreateContract(ContractType contractType, Contract contract);
    }
}