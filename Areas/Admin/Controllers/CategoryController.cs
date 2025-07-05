using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Book_Store.Models.File_Entities;
using Online_Book_Store.Models;
using Online_Book_Store.ViewModels;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var categories = _context.Categories.Include(c => c.Books).ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Create(Category category, List<IFormFile> files)
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
                    CategoryFile categoryFile = new()
                    {
                        Name = fileName,
                        FileType = fileType
                    };
                    _context.CategoryFiles.Add(categoryFile);
                    _context.SaveChanges();

                    //Save File to Book Table
                    category.Files.Add(categoryFile);
                }
            }

            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Include(c => c.Files).SingleOrDefault(e => e.Id == id);

            if (category is not null)
            {
                return View(category);
            }

            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Edit(Category category, List<IFormFile> files, List<int> ExistingFilesIds)
        {
            var categories = _context.Categories;
            var existingCat = categories
            .Include(b => b.Files)
                .FirstOrDefault(b => b.Id == category.Id);

            if (existingCat == null) return NotFound();

            existingCat.Description = category.Description;
            existingCat.Name = category.Name;

            var filesInDb = _context.CategoryFiles;

            // Handle existing files - remove files not in ExistingFilesIds
            var filesToDelete = existingCat.Files
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
                _context.CategoryFiles.Attach(file);
                _context.CategoryFiles.Remove(file);
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
                    CategoryFile categoryFile = new()
                    {
                        Name = fileName,
                        FileType = fileType
                    };
                    _context.CategoryFiles.Add(categoryFile);
                    _context.SaveChanges();

                    //Save File to Book Table
                    existingCat.Files.Add(categoryFile);
                }
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var categories = _context.Categories;
            var category = categories.Find(id);
            if (category is not null)
            {
                categories.Remove(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
