using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Online_Book_Store.ViewModels.Identity;

namespace Online_Book_Store.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<PublishingHouse> PublishingHouses { get; set; }

        // File tables
        public DbSet<BookFile> BookFiles { get; set; }
        public DbSet<AuthorFile> AuthorFiles { get; set; }
        public DbSet<PublishingHouseFile> PublishingHouseFiles { get; set; }
        public DbSet<CategoryFile> CategoryFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email)
                .IsUnique();


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
        public DbSet<Online_Book_Store.ViewModels.Identity.SignInVM> SignInVM { get; set; } = default!;
    }
}
