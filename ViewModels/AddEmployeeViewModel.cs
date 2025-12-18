using System.ComponentModel.DataAnnotations;

namespace Zuwarak.ViewModels
{
    public class AddEmployeeViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public string JobTitle { get; set; } = string.Empty;

        [Required]
        public string Department { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        //  كلمة المرور (لإنشاء المستخدم فقط

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
