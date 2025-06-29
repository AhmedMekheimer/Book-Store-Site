using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Data
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PublishingHouse> PublishingHouses { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Onilne Book Store; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure all file relationships
            modelBuilder.Entity<UploadedFile>()
                .HasOne(f => f.Author)
                .WithMany(a => a.Files)
                .HasForeignKey(f => f.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UploadedFile>()
                .HasOne(f => f.Book)
                .WithMany(b => b.Files)
                .HasForeignKey(f => f.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UploadedFile>()
                .HasOne(f => f.PublishingHouse)
                .WithMany(p => p.Files)
                .HasForeignKey(f => f.PublishingHouseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UploadedFile>()
                .HasOne(f => f.Category)
                .WithMany(c => c.Files)
                .HasForeignKey(f => f.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
