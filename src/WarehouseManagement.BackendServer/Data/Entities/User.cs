using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WarehouseManagement.BackendServer.Data.Entities
{
    [Table("Users")]
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

        public bool IsActive { get; set; } = true;

        public bool IsBanned { get; set; } = false;
    }
}
