using GLMS2.Models;
using GLMS2.Interfaces;

namespace GLMS2.Services.Factories
{
    public class DomesticContract : IContract
    {
        private readonly Contract _contract;

        public DomesticContract(Contract contract)
        {
            _contract = contract;
        }

        public string GetDetails()
        {
            return $"Domestic contract for Client ID {_contract.ClientId} from {_contract.StartDate:d} to {_contract.EndDate:d}.";
        }

        public bool Validate()
        {
            return _contract.StartDate < _contract.EndDate;
        }
    }
}