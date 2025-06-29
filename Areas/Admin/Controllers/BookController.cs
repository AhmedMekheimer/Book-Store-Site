using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Book_Store.Models;
using Online_Book_Store.ViewModels;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var books = _context.Books.Include(b => b.Authors).ToList();
            return View(books);
        }
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var authors = _context.Authors.ToList();
            var pubs = _context.PublishingHouses.ToList();
            BookCatAuthPubsVM BookCatAuthPubsVM = new()
            {
                Categories = categories,
                Authors = authors,
                PublishingHouses = pubs,
                Book = new Book()
            };
            return View(BookCatAuthPubsVM);
        }
        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Create(BookDataVM bookDataVM, List<IFormFile> files)
        {
            var authors = _context.Authors;
            var pubs = _context.PublishingHouses;
            Book book = new()
            {
                Name = bookDataVM.Name,
                Price = bookDataVM.Price,
                AvailableCopies = bookDataVM.AvailableCopies,
                CategoryId = bookDataVM.CategoryId,
                Files = new List<UploadedFile>()
            };

            var filesInDb = _context.UploadedFiles;
            // Common image extensions
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            // Common video extensions
            var videoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm", ".wmv" };

            foreach (var file in files)
            {
                if (file is not null && file.Length > 0)
                {
                    // Save File in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", fileName);
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Save File in Files Table
                    UploadedFile uploadedFile = new()
                    {
                        Name = fileName,
                    };
                    if (imageExtensions.Contains(extension))
                    {
                        uploadedFile.FileType = FileType.Image;
                    }
                    else if (videoExtensions.Contains(extension))
                    {
                        uploadedFile.FileType = FileType.Video;
                    }
                    filesInDb.Add(uploadedFile);
                    _context.SaveChanges();

                    //Save File to Book Table
                    book.Files.Add(uploadedFile);
                }
            }

            foreach (var authId in bookDataVM.AuthorsIds)
            {
                book.Authors.Add(authors.Find(authId));
            }
            foreach (var pubId in bookDataVM.PublishersIds)
            {
                book.PublishingHouses.Add(pubs.Find(pubId));
            }
            var books = _context.Books;
            books.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var books = _context.Books;
            var categories = _context.Categories.ToList();
            var authors = _context.Authors.ToList();
            var pubs = _context.PublishingHouses.ToList();
            var files = _context.UploadedFiles.Where(u => u.BookId == id).ToList();

            //Testing Removing a File from Db
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var deleteFile = _context.UploadedFiles.SingleOrDefault(u => u.Id == 1020);
                    var book1 = _context.Books.Find(1034);
                    book1.Files.Remove(deleteFile);
                    _context.Entry(deleteFile).State = EntityState.Deleted;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.Message);
                    // Handle error
                }
            }

            BookCatAuthPubsVM BookCatAuthPubsVM = new()
            {
                Categories = categories,
                Authors = authors,
                PublishingHouses = pubs,
                Book = new Book()
            };

            Book? book = books.Include(b => b.Authors).Include(b => b.PublishingHouses).SingleOrDefault(e => e.Id == id);
            if (book is null)
            {
                return NotFound();
            }
            else
            {
                BookCatAuthPubsVM.Book = book;
                return View(BookCatAuthPubsVM);
            }
        }
        [HttpPost]
        public IActionResult Edit(BookDataVM bookDataVM, List<IFormFile> files, List<int> ExistingFilesIds)
        {
            // Get the existing book from database including its relationships
            var existingBook = _context.Books
                .Include(b => b.Files)
                .Include(b => b.Authors)
                .Include(b => b.PublishingHouses)
                .FirstOrDefault(b => b.Id == bookDataVM.Id);

            if (existingBook == null)
            {
                return NotFound();
            }

            // Update book properties
            existingBook.Name = bookDataVM.Name;
            existingBook.Price = bookDataVM.Price;
            existingBook.AvailableCopies = bookDataVM.AvailableCopies;
            existingBook.CategoryId = bookDataVM.CategoryId;

            // Handle existing files - remove files not in ExistingFilesIds
            var filesToDelete = existingBook.Files
                .Where(f => !ExistingFilesIds.Contains(f.Id))
                .ToList();

            foreach (var file in filesToDelete)
            {
                // Delete physical file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", file.Name);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Remove from database
                _context.UploadedFiles.Remove(file);
            }

            // Handle authors
            existingBook.Authors.Clear();
            foreach (var authId in bookDataVM.AuthorsIds)
            {
                var author = _context.Authors.Find(authId);
                if (author != null)
                {
                    existingBook.Authors.Add(author);
                }
            }

            // Handle publishers
            existingBook.PublishingHouses.Clear();
            foreach (var pubId in bookDataVM.PublishersIds)
            {
                var pub = _context.PublishingHouses.Find(pubId);
                if (pub != null)
                {
                    existingBook.PublishingHouses.Add(pub);
                }
            }

            // Handle new file uploads
            var uploadedFiles_db = _context.UploadedFiles;
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var videoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm", ".wmv" };

            foreach (var newfile in files)
            {
                if (newfile is not null && newfile.Length > 0)
                {
                    // Save File in wwwroot
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(newfile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", fileName);
                    var extension = Path.GetExtension(newfile.FileName).ToLowerInvariant();

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        newfile.CopyTo(stream);
                    }

                    // Create new UploadedFile entity
                    var uploadedFile = new UploadedFile
                    {
                        Name = fileName,
                        FileType = imageExtensions.Contains(extension)
                            ? FileType.Image
                            : videoExtensions.Contains(extension)
                                ? FileType.Video
                                : throw new Exception("Unsupported file type")
                    };
                    // Add file to Db
                    uploadedFiles_db.Add(uploadedFile);
                    _context.SaveChanges();
                    // Add to book
                    existingBook.Files.Add(uploadedFile);
                }
            }

            // Save all changes at once
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var books = _context.Books;
            var book = books.Find(id);
            if (book is not null)
            {
                books.Remove(book);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }

}
