using ProjektNotatek.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class ResetPasswordModel {
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$", ErrorMessage = "Password must have at least one upper case, lower case, number with minimum length of 6")]
        [DataType(DataType.Password, ErrorMessage = "Incorrect or missing password")]
        [ValidPassword]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
