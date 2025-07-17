using Microsoft.AspNetCore.Authorization;
using Online_Book_Store.Utility;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        [Authorize(Policy = $"{SD.Workers}")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
