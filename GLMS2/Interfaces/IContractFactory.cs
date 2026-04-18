using GLMS2.Enums;
using GLMS2.Models;

namespace GLMS2.Interfaces
{
    public interface IContractFactory
    {
        IContract CreateContract(ContractType contractType, Contract contract);
    }
}