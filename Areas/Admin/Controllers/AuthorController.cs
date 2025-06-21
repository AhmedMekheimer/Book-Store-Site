using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var Authors = _context.Authors.Include(p => p.Books).ToList();
            return View(Authors);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Author author)
        {
            var authors = _context.Authors;
            authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            var author = _context.Authors.Find(id);
            if (author is not null)
            {
                return View(author);
            }
            return NotFound();
        }
        [HttpPost]
        public IActionResult Edit(Author author)
        {
            var authors = _context.Authors;
            authors.Update(author);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var authors = _context.Authors;
            var author = authors.Find(id);
            if (author is not null)
            {
                authors.Remove(author);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }

}
