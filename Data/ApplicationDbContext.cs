using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.ConstrainedExecution;

namespace Online_Book_Store.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PublishingHouse> PublishingHouses { get; set; }

        // File tables
        public DbSet<BookFile> BookFiles { get; set; }
        public DbSet<AuthorFile> AuthorFiles { get; set; }
        public DbSet<PublishingHouseFile> PublishingHouseFiles { get; set; }
        public DbSet<CategoryFile> CategoryFiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Onilne Book Store; Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships with cascade delete
            modelBuilder.Entity<BookFile>()
                .HasOne(bf => bf.Book)
                .WithMany(b => b.Files)
                .HasForeignKey(bf => bf.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuthorFile>()
                .HasOne(af => af.Author)
                .WithMany(a => a.Files)
                .HasForeignKey(af => af.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CategoryFile>()
                .HasOne(af => af.Category)
                .WithMany(a => a.Files)
                .HasForeignKey(af => af.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PublishingHouseFile>()
                .HasOne(af => af.PublishingHouse)
                .WithMany(a => a.Files)
                .HasForeignKey(af => af.PublishingHouseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
