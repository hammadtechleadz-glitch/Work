using System.ComponentModel.DataAnnotations;

namespace project02.Models
{
    public class RegisterViewModel
    {

        [Required]
        [EmailAddress]
        public string email { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Compare("password", ErrorMessage = "Passwords do not match.")]
        public string confirm_password { get; set; }
    }
}
