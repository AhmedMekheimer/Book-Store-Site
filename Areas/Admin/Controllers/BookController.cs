using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            CategoriesAuthorsPublishers categoriesAuthorsPublishers = new()
            {
                Categories = categories,
                Authors = authors,
                PublishingHouses = pubs
            };
            return View(categoriesAuthorsPublishers);
        }
        [HttpPost]
        public IActionResult Create(BookDataVM bookDataVM)
        {
            var authors = _context.Authors;
            var pubs = _context.PublishingHouses;
            Book book = new()
            {
                Name = bookDataVM.Name,
                Price = bookDataVM.Price,
                Picture = bookDataVM.Picture,
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
            CategoriesAuthorsPublishers categoriesAuthorsPublishers = new()
            {
                Categories = categories,
                Authors = authors,
                PublishingHouses = pubs
            };

            Book? book = books.Include(b=>b.Authors).Include(b=>b.PublishingHouses).SingleOrDefault(e => e.Id == id);
            if (book is null)
            {
                return NotFound();
            }
            else
            {
                EditBookVM editBookVM = new()
                {
                    book = book,
                    categoriesAuthorsPublishers = categoriesAuthorsPublishers
                };
                return View(editBookVM);
            }
        }
        [HttpPost]
        public IActionResult Edit(BookDataVM bookDataVM)
        {
            var authors = _context.Authors;
            var pubs = _context.PublishingHouses;
            Book book = new()
            {
                Id=bookDataVM.Id,
                Name = bookDataVM.Name,
                Price = bookDataVM.Price,
                Picture = bookDataVM.Picture,
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
            books.Update(book);
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
