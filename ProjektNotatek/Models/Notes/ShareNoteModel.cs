using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class ShareNoteModel {
        [Required]
        public string Username { get; set; }
        public List<string>? Usernames { get; set; }
    }
}
