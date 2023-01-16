using ProjektNotatek.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class SignupViewModel {

        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Username must have at least one upper case, lower case, number with minimum length of 6")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage ="Email addres is missing or invalid")]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must have at least one upper case, lower case, number with minimum length of 6")]
        [DataType(DataType.Password ,ErrorMessage ="Incorrect or missing password")]
        [ValidPassword]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
