using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Online_Book_Store.Models;
using Online_Book_Store.Models.File_Entities;
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
                Files = new List<BookFile>()
            };

            foreach (var file in files)
            {
                //Save File in Physical Storage
                string fileName;
                FileType fileType;
                (fileName, fileType) = FileService.UploadNewFile(file);

                if (fileName is not null && (fileType == FileType.Image || fileType == FileType.Video))
                {
                    // Save File in Db
                    BookFile bookFile = new()
                    {
                        Name = fileName,
                        FileType = fileType
                    };
                    _context.BookFiles.Add(bookFile);
                    _context.SaveChanges();

                    //Save File to Book Table
                    book.Files.Add(bookFile);
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
            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var books = _context.Books;
            Book? book = books.Include(b => b.Authors).Include(b => b.PublishingHouses).Include(b => b.Files).SingleOrDefault(e => e.Id == id);
            if (book is null)
            {
                return NotFound();
            }
            else
            {
                var categories = _context.Categories.ToList();
                var authors = _context.Authors.ToList();
                var pubs = _context.PublishingHouses.ToList();
                var Bookfiles = _context.BookFiles.Where(u => u.BookId == id).ToList();

                BookCatAuthPubsVM BookCatAuthPubsVM = new()
                {
                    Categories = categories,
                    Authors = authors,
                    PublishingHouses = pubs,
                    Book = new Book()
                };

                BookCatAuthPubsVM.Book = book;
                return View(BookCatAuthPubsVM);
            }
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Edit(BookDataVM bookDataVM, List<IFormFile> files, List<int> ExistingFilesIds)
        {
            var books = _context.Books;
            var existingBook = books
                .Include(b => b.Authors)
                .Include(b => b.PublishingHouses)
                .Include(b => b.Files)
                .FirstOrDefault(b => b.Id == bookDataVM.Id);

            if (existingBook == null) return NotFound();

            var authors = _context.Authors;
            var pubs = _context.PublishingHouses;
            var filesInDb = _context.BookFiles;

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
                // Attach and mark for deletion
                _context.BookFiles.Attach(file);
                _context.BookFiles.Remove(file);
                _context.SaveChanges();
            }

            // Handle Authors 
            existingBook.Authors.Clear();
            foreach (var authorId in bookDataVM.AuthorsIds)
            {
                var author = _context.Authors.Find(authorId);
                if (author != null) existingBook.Authors.Add(author);
            }

            // Handle Publishing Houses 
            existingBook.PublishingHouses.Clear();
            foreach (var pubId in bookDataVM.PublishersIds)
            {
                var pub = _context.PublishingHouses.Find(pubId);
                if (pub != null) existingBook.PublishingHouses.Add(pub);
            }

            //Upload New Files
            foreach (var file in files)
            {
                //Save File in Physical Storage
                string fileName;
                FileType fileType;
                (fileName, fileType) = FileService.UploadNewFile(file);

                if (fileName is not null && (fileType == FileType.Image || fileType == FileType.Video))
                {
                    // Save File in Db
                    BookFile bookFile = new()
                    {
                        Name = fileName,
                        FileType = fileType
                    };
                    _context.BookFiles.Add(bookFile);
                    _context.SaveChanges();

                    //Save File to Book Table
                    existingBook.Files.Add(bookFile);
                }
            }
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
