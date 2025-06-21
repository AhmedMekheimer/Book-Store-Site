using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context=new();

        public IActionResult Index()
        {
            var categories = _context.Categories.Include(c=>c.Books).ToList();
            return View(categories);
        }
        public IActionResult Create()
        {   
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            var categories = _context.Categories;
            categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);

            if (category is not null)
            {
                return View(category);
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            var categories = _context.Categories;
            categories.Update(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var categories = _context.Categories;
            var category = categories.Find(id);
            if(category is not null)
            {
                categories.Remove(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
