using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Zuwarak.Models
{
    public class Employee : IdentityUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        //  الربط مع IdentityUser
        public string UserId { get; set; } = string.Empty;

    }
}
