using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjektNotatek.Data {
    public class DataContext : IdentityDbContext<ApplicationUser> {
        public DataContext() {

        }

        public DataContext(DbContextOptions options) : base(options) {

        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteShared> NotesShared { get; set; }
    }
}
