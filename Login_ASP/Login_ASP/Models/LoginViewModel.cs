using System.ComponentModel.DataAnnotations;

namespace Login_ASP.Models
{
    public class LoginViewModel
    {
        // Public properties for model binding
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
