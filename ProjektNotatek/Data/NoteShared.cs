namespace ProjektNotatek.Data {
    public class NoteShared {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public Note Note { get; set; }
        public string Username { get; set; }
    }
}
