using Microsoft.AspNetCore.Mvc;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            return View();
        }
    }
}
