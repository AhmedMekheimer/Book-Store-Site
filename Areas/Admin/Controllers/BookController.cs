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
                Files=new List<UploadedFile>()
            };

            var filesInDb=_context.UploadedFiles;
            // Common image extensions
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            // Common video extensions
            var videoExtensions = new[] { ".mp4", ".mov", ".avi", ".mkv", ".webm", ".wmv" };

            foreach (var file in files)
            {
                if(file is not null && file.Length>0)
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
            BookCatAuthPubsVM BookCatAuthPubsVM = new()
            {
                Categories = categories,
                Authors = authors,
                PublishingHouses = pubs,
                Book = new Book()
            };

            Book? book = books.Include(b=>b.Authors).Include(b=>b.PublishingHouses).SingleOrDefault(e => e.Id == id);
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
        public IActionResult Edit(BookDataVM bookDataVM)
        {
            var authors = _context.Authors;
            var pubs = _context.PublishingHouses;
            Book book = new()
            {
                Name = bookDataVM.Name,
                Price = bookDataVM.Price,
                AvailableCopies = bookDataVM.AvailableCopies,
                CategoryId = bookDataVM.CategoryId,
            };
            foreach (var authId in bookDataVM.AuthorsIds)
            {
                book.Authors.Add(authors.Find(authId));
            }
            foreach (var pubId in bookDataVM.PublishersIds)
            {
                book.PublishingHouses.Add(pubs.Find(pubId));
            }
            var books = _context.Books;
            Book oldBook = books.Find(bookDataVM.Id);
            books.Remove(oldBook);
            _context.SaveChanges();
            books.Add(book);
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
