using System.ComponentModel.DataAnnotations;

namespace GLMS.ViewModels
{
    public class ServiceRequestCreateViewModel
    {
        [Required]
        [Display(Name = "Contract")]
        public int ContractId { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000000)]
        [Display(Name = "Cost (USD)")]
        public decimal CostUSD { get; set; }

        [Display(Name = "Exchange Rate (USD to ZAR)")]
        public decimal ExchangeRate { get; set; }

        [Display(Name = "Cost (ZAR)")]
        public decimal CostZAR { get; set; }
    }
}