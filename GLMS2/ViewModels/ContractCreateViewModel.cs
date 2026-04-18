using GLMS.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GLMS.ViewModels
{
    public class ContractCreateViewModel
    {
        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Service Level")]
        public string ServiceLevel { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Contract Type")]
        public ContractType ContractType { get; set; }

        [Required]
        [Display(Name = "Signed Agreement (PDF only)")]
        public IFormFile? SignedAgreementFile { get; set; }
    }
}