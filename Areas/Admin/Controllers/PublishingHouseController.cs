/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Book_Store.Models.File_Entities;
using Online_Book_Store.Models;

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
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Create(PublishingHouse publishingHouse, List<IFormFile> files)
        {
            foreach (var file in files)
            {
                //Save File in Physical Storage
                string fileName;
                FileType fileType;
                (fileName, fileType) = FileService.UploadNewFile(file);

                if (fileName is not null && (fileType == FileType.Image || fileType == FileType.Video))
                {
                    // Save File in Db
                    PublishingHouseFile publishingHouseFile = new()
                    {
                        Name = fileName,
                        FileType = fileType
                    };
                    _context.PublishingHouseFiles.Add(publishingHouseFile);
                    _context.SaveChanges();

                    //Save File to Book Table
                    publishingHouse.Files.Add(publishingHouseFile);
                }
            }

            _context.PublishingHouses.Add(publishingHouse);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var publishingHouse = _context.PublishingHouses.Include(c => c.Files).SingleOrDefault(e => e.Id == id);

            if (publishingHouse is not null)
            {
                return View(publishingHouse);
            }

            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Edit(PublishingHouse publishingHouse, List<IFormFile> files, List<int> ExistingFilesIds)
        {
            var publishingHouses = _context.PublishingHouses;
            var existingPub = publishingHouses
            .Include(b => b.Files)
                .FirstOrDefault(b => b.Id == publishingHouse.Id);

            if (existingPub == null) return NotFound();

            existingPub.Description = publishingHouse.Description;
            existingPub.Name = publishingHouse.Name;

            var filesInDb = _context.PublishingHouseFiles;

            // Handle existing files - remove files not in ExistingFilesIds
            var filesToDelete = existingPub.Files
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
                _context.PublishingHouseFiles.Attach(file);
                _context.PublishingHouseFiles.Remove(file);
                _context.SaveChanges();
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
                    PublishingHouseFile publishingHouseFile = new()
                    {
                        Name = fileName,
                        FileType = fileType
                    };
                    _context.PublishingHouseFiles.Add(publishingHouseFile);
                    _context.SaveChanges();

                    //Save File to Book Table
                    existingPub.Files.Add(publishingHouseFile);
                }
            }

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
*/