using GLMS2.Enums;


namespace GLMS2.ViewModels
{
    public class ContractFilterViewModel
    {
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public ContractStatus? Status { get; set; }
    }
}