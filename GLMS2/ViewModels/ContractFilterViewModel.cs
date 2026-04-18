using GLMS.Enums;


namespace GLMS.ViewModels
{
    public class ContractFilterViewModel
    {
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public ContractStatus? Status { get; set; }
    }
}