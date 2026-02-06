using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WarehouseManagement.Domain.Entities
{
    public class User : IdentityUser
    {
        public User() { }

        public User(string id, string userName, string firstName, string lastName, string email, string phoneNumber)
        {
            Id = id;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            Email = email;
        }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
    }
}
