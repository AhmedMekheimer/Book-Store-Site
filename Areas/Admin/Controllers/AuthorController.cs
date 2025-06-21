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
    }
}
