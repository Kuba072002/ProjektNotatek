using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class SigninViewModel {

        [Required(ErrorMessage ="Username must be provided")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password must be provided")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //public bool RememberMe { get; set; }

    }
}
