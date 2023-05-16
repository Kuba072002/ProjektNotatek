using System.ComponentModel.DataAnnotations;

namespace ProjektNotatek.Models.Notes
{
    public class EditNoteModel
    {
        [Required]
        public string EditedNoteContent { get; set; }
    }
}
