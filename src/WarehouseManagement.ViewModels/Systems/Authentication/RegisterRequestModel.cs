using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.ViewModels.Systems.Authentication
{
    public class RegisterRequestModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
    }
}