using System.ComponentModel.DataAnnotations;

namespace GLMS2.ViewModels
{
    // ViewModel used to collect client information from the UI
    public class ClientCreateViewModel
    {
        // Stores the client name
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Stores the client region or location
        [Required]
        [StringLength(100)]
        public string Region { get; set; } = string.Empty;

        // Stores the client email with format validation
        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;
    }
}