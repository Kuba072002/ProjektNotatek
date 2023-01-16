using ProjektNotatek.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class CreateNoteModel {

        [Required]
        public string Title { get; set; }
        [Required]
        public string NoteContent { get; set; }
        public string? Option { get; set; }
        
        public string? Password { get; set; }

    }
}
