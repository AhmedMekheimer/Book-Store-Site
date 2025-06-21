using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PublishingHouseController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var PublishingHouses = _context.PublishingHouses.Include(p => p.Books).ToList();
            return View(PublishingHouses);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(PublishingHouse publishingHouse)
        {
            var publishingHouses = _context.PublishingHouses;
            publishingHouses.Add(publishingHouse);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            var publishingHouse = _context.PublishingHouses.Find(id);

            if (publishingHouse is not null)
            {
                return View(publishingHouse);
            }

            return NotFound();
        }
        [HttpPost]
        public IActionResult Edit(PublishingHouse publishingHouse)
        {
            var publishingHouses = _context.PublishingHouses;
            publishingHouses.Update(publishingHouse);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var publishingHouses = _context.PublishingHouses;
            var publishingHouse = publishingHouses.Find(id);
            if (publishingHouse is not null)
            {
                publishingHouses.Remove(publishingHouse);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
