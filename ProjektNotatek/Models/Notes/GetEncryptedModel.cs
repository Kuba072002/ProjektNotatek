using ProjektNotatek.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class GetEncryptedModel {
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Incorrect or missing password")]
        [ValidPassword]
        public string Password { get; set; }
    }
}
