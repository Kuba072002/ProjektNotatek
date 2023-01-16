using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class ForgotPasswordModel {

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
