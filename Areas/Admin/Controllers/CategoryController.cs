using Microsoft.AspNetCore.Authorization;
using Online_Book_Store.Utility;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _catRepo;
        private readonly IRepository<CategoryFile> _cfRepo;

        public CategoryController(IRepository<Category> catRepo, IRepository<CategoryFile> cfRepo)
        {
            _catRepo = catRepo;
            _cfRepo = cfRepo;
        }
        [Authorize(Policy = $"{SD.Workers}")]
        public async Task<IActionResult> Index()
        {
            var categories = await _catRepo.GetAsync(null, new List<Func<IQueryable<Category>, IQueryable<Category>>> { a => a.Include(c => c.Books) });
            return View(categories);
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Create(Category category, List<IFormFile> files)
        {
            if (category is null)
                return NotFound();

            ModelState.Remove("files");
            if (!ModelState.IsValid)
                return View(category);

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
                    await _cfRepo.CreateAsync(categoryFile);

                    //Save File to Book Table
                    category.Files.Add(categoryFile);
                }
            }

            var CreateResult=await _catRepo.CreateAsync(category);
            if (CreateResult)
                TempData["success-notification"] = "Category Created Successfully";
            else
                TempData["error-notification"] = "Category Creation Failure";

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _catRepo.GetOneAsync(c => c.Id == id, new List<Func<IQueryable<Category>, IQueryable<Category>>> { a => a.Include(c => c.Files) });

            if (category is not null)
                return View(category);

            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Edit(Category category, List<IFormFile> files, List<int> ExistingFilesIds)
        {
            if (category is null)
                return NotFound();

            ModelState.Remove("files");
            ModelState.Remove("ExistingFilesIds");
            //Invalid Server Side
            if (!ModelState.IsValid)
            {
                Category? category1 = await _catRepo.GetOneAsync(c => c.Id == category.Id,
                new List<Func<IQueryable<Category>, IQueryable<Category>>> { a => a.Include(c => c.Files) }, false);

                if (category1 is null)
                    return NotFound();

                category1.Name = category.Name;
                category1.Description = category.Description;

                return View(category1);
            }

            Category NewCategory = new()
            {
                Id=category.Id,
                Description= category.Description,
                Name=category.Name
            };

            var existingCat = await _catRepo.GetOneAsync(c => c.Id == category.Id, 
                new List<Func<IQueryable<Category>, IQueryable<Category>>> { a => a.Include(c => c.Files) });

            if (existingCat == null) return NotFound();

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

                // Remove file from database
                if (!await _cfRepo.DeleteAsync(file))
                    return NotFound();
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
                    categoryFile.CategoryId=category.Id;
                    await _cfRepo.CreateAsync(categoryFile);
                }
            }

            //Cut Connection from Existing
            _catRepo.DetachEntity(existingCat);

            var UpdateResult=await _catRepo.UpdateAsync(NewCategory);
            if (UpdateResult)
                TempData["success-notification"] = "Book Updated Successfully";
            else
                TempData["error-notification"] = "Book Updating Failure";

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await(_catRepo.GetOneAsync(c => c.Id == id, 
                new List<Func<IQueryable<Category>, IQueryable<Category>>> { a => a.Include(c => c.Files) })) is Category category)
            {
                foreach (var file in category.Files)
                {
                    // Delete all files Physically
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", file.Name);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Delete Book from Db along with its Files
                var DeleteResult=await _catRepo.DeleteAsync(category);
                if (DeleteResult)
                    TempData["success-notification"] = "Book Removed Successfully";
                else
                    TempData["error-notification"] = "Book Removal Failure";

                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
