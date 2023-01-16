namespace ProjektNotatek.Data {
    public class Note {
        public int Id { get; set; }
        public string Title { get; set; }
        //public string UserId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool IsEncrypted { get; set; } = false;
        public string Password { get; set; } = string.Empty;
    }
}
