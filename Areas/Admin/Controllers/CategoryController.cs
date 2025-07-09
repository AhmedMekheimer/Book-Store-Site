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

        public async Task<IActionResult> Index()
        {
            var categories = await _catRepo.GetAsync(null,new Expression<Func<Category, object>>[] {c=>c.Books});
            return View(categories);
        }
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
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

            await _catRepo.CreateAsync(category);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _catRepo.GetOneAsync(c => c.Id == id,new Expression<Func<Category, object>>[] {c => c.Files});

            if (category is not null)
                return View(category);

            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
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
                new Expression<Func<Category, object>>[] { c => c.Files }, false);

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

            var existingCat = await _catRepo.GetOneAsync(c => c.Id == category.Id, new Expression<Func<Category, object>>[] { c => c.Files });

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
                    await _cfRepo.CreateAsync(categoryFile);

                    //Save File to Book Table
                    NewCategory.Files.Add(categoryFile);
                }
            }

            //Cut Connection from Existing
            _catRepo.DetachEntity(existingCat);

            await _catRepo.UpdateAsync(NewCategory);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (await(_catRepo.GetOneAsync(c => c.Id == id, new Expression<Func<Category, object>>[] { c => c.Files })) is Category category)
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
                await _catRepo.DeleteAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
