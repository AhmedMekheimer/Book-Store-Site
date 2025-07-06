/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index(string search)
        {
            IQueryable<Book> books = _context.Books.Include(b=>b.Authors);
            if (search is not null)
            {
                books=books.Where(b => b.Name.Contains(search));
            }
            ViewData["Search"] = search;
            books.ToList();
            return View(books);
        }
    }
}
*/