using System.ComponentModel.DataAnnotations;
using GLMS2.Enums;
using Microsoft.AspNetCore.Http;

namespace GLMS2.ViewModels
{
    public class ContractEditViewModel
    {
        [Required]
        public int ContractId { get; set; }

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

        [Display(Name = "Replace Signed Agreement (PDF only)")]
        public IFormFile? SignedAgreementFile { get; set; }

        public string? ExistingSignedAgreementFilePath { get; set; }
    }
}