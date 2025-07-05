using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Online_Book_Store.Models;
using Online_Book_Store.Models.File_Entities;
using Online_Book_Store.ViewModels;
using System;

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
            if (bookDataVM.Book is null)
                return NotFound();

            var authors = _context.Authors;
            var pubs = _context.PublishingHouses;

            ModelState.Remove("files");
            if (!ModelState.IsValid)
            {
                foreach (var authId in bookDataVM.AuthorsIds)
                {
                    if (authors.Find(authId) is Author auth)
                    {
                        bookDataVM.Book.Authors.Add(auth);
                    }
                }
                foreach (var pubId in bookDataVM.PublishersIds)
                {
                    if (pubs.Find(pubId) is PublishingHouse pub)
                    {
                        bookDataVM.Book.PublishingHouses.Add(pub);
                    }
                }
                var categoriesList = _context.Categories.ToList();
                var authorsList = _context.Authors.ToList();
                var pubsList = _context.PublishingHouses.ToList();

                BookCatAuthPubsVM BookCatAuthPubsVM = new()
                {
                    Categories = categoriesList,
                    Authors = authorsList,
                    PublishingHouses = pubsList,
                    Book = bookDataVM.Book
                };
                return View(BookCatAuthPubsVM);
            }


            Book book = new()
            {
                Name = bookDataVM.Book.Name,
                Price = bookDataVM.Book.Price,
                AvailableCopies = bookDataVM.Book.AvailableCopies,
                CategoryId = bookDataVM.Book.CategoryId,
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
                else
                    return NotFound();
            }
            foreach (var authId in bookDataVM.AuthorsIds)
            {
                if (authors.Find(authId) is Author auth)
                {
                    book.Authors.Add(auth);
                }
            }
            foreach (var pubId in bookDataVM.PublishersIds)
            {
                if (pubs.Find(pubId) is PublishingHouse pub)
                {
                    book.PublishingHouses.Add(pub);
                }
            }
            _context.Books.Add(book);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Book? book = _context.Books.Include(b => b.Authors).Include(b => b.PublishingHouses).Include(b => b.Files).SingleOrDefault(e => e.Id == id);
            if (book is null)
            {
                return NotFound();
            }
            else
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

                BookCatAuthPubsVM.Book = book;
                return View(BookCatAuthPubsVM);
            }
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Edit(BookDataVM bookDataVM, List<IFormFile> files, List<int> ExistingFilesIds)
        {
            //Db Instances
            var categories = _context.Categories;
            var authors = _context.Authors;
            var pubs = _context.PublishingHouses;
            var books = _context.Books;

            ModelState.Remove("files");
            ModelState.Remove("ExistingFilesIds");
            //Invalid Server Side
            if (!ModelState.IsValid)
            {
                if (bookDataVM.Book is null)
                    return NotFound();

                Book? book = books.Include(b => b.Files).AsNoTracking().SingleOrDefault(e => e.Id == bookDataVM.Book.Id);
                if (book is null)
                {
                    return NotFound();
                }

                foreach (var authId in bookDataVM.AuthorsIds)
                {
                    if (authors.Find(authId) is Author auth)
                    {
                        book.Authors.Add(auth);
                    }
                }
                foreach (var pubId in bookDataVM.PublishersIds)
                {
                    if (pubs.Find(pubId) is PublishingHouse pub)
                    {
                        book.PublishingHouses.Add(pub);
                    }
                }

                BookCatAuthPubsVM BookCatAuthPubsVM = new()
                {
                    Categories = categories.ToList(),
                    Authors = authors.ToList(),
                    PublishingHouses = pubs.ToList(),
                    Book = new Book()
                };
                book.Name = bookDataVM.Book.Name;
                book.Price = bookDataVM.Book.Price;
                book.AvailableCopies = bookDataVM.Book.AvailableCopies;
                book.CategoryId = bookDataVM.Book.CategoryId;

                BookCatAuthPubsVM.Book = book;
                return View(BookCatAuthPubsVM);
            }

            var existingBook = books
                .Include(b => b.Authors)
                .Include(b => b.PublishingHouses)
                .Include(b => b.Files)
                .FirstOrDefault(b => b.Id == bookDataVM.Book!.Id);

            if (existingBook == null) return NotFound();

            //Primitive data types
            existingBook.Name = bookDataVM.Book!.Name;
            existingBook.Price = bookDataVM.Book.Price;
            existingBook.AvailableCopies = bookDataVM.Book.AvailableCopies;
            existingBook.CategoryId = bookDataVM.Book.CategoryId;

            //Db Instance
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
            foreach (var authId in bookDataVM.AuthorsIds)
            {
                if (authors.Find(authId) is Author auth)
                {
                    existingBook.Authors.Add(auth);
                }
            }

            // Handle Publishing Houses 
            existingBook.PublishingHouses.Clear();
            foreach (var pubId in bookDataVM.PublishersIds)
            {
                if (pubs.Find(pubId) is PublishingHouse pub)
                {
                    existingBook.PublishingHouses.Add(pub);
                }
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
