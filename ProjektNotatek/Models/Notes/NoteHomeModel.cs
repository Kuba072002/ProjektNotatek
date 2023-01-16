using ProjektNotatek.Data;
using ProjektNotatek.Utility;
using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models {
    public class NoteHomeModel {
        public List<Note> PublicNotes { get; set; }
        public List<Note> YourNotes { get; set; }
        public List<Note> SharedNotes { get; set; }
        
        public string? Password { get; set; }
    }
}
