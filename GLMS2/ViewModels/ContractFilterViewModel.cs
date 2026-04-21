using GLMS2.Enums;

namespace GLMS2.ViewModels
{
    // ViewModel used to filter contracts in the list view
    public class ContractFilterViewModel
    {
        // Filters contracts starting from this date
        public DateTime? StartDateFrom { get; set; }

        // Filters contracts up to this date
        public DateTime? StartDateTo { get; set; }

        // Filters contracts by workflow status
        public ContractStatus? Status { get; set; }
    }
}