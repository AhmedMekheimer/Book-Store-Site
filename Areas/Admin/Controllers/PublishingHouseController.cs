namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PublishingHouseController : Controller
    {
        private readonly IRepository<PublishingHouse> _pubRepo;
        private readonly IRepository<PublishingHouseFile> _pfRepo;

        public PublishingHouseController(IRepository<PublishingHouse> pubRepo, IRepository<PublishingHouseFile> pfRepo)
        {
            _pubRepo = pubRepo;
            _pfRepo = pfRepo;
        }
        public async Task<IActionResult> Index()
        {
            var PublishingHouses = await _pubRepo.GetAsync(null, new Expression<Func<PublishingHouse, object>>[] { p => p.Books });
            return View(PublishingHouses);
        }

        public IActionResult Create()
        {
            return View(new PublishingHouse());
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public async Task<IActionResult> Create(PublishingHouse publishingHouse, List<IFormFile> files)
        {
            if (publishingHouse is null)
                return NotFound();

            ModelState.Remove("files");
            if (!ModelState.IsValid)
                return View(publishingHouse);

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

                    await _pfRepo.CreateAsync(publishingHouseFile);

                    //Save File to Book Table
                    publishingHouse.Files.Add(publishingHouseFile);
                }
            }

            await _pubRepo.CreateAsync(publishingHouse);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var publishingHouse = await _pubRepo.GetOneAsync(p => p.Id == id, new Expression<Func<PublishingHouse, object>>[] { p => p.Files });

            if (publishingHouse is not null)
                return View(publishingHouse);

            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public async Task<IActionResult> Edit(PublishingHouse publishingHouse, List<IFormFile> files, List<int> ExistingFilesIds)
        {
            if (publishingHouse is null)
                return NotFound();

            ModelState.Remove("files");
            ModelState.Remove("ExistingFilesIds");
            //Invalid Server Side
            if (!ModelState.IsValid)
            {
                PublishingHouse? publishingHouse1 = await _pubRepo.GetOneAsync(c => c.Id == publishingHouse.Id,
                new Expression<Func<PublishingHouse, object>>[] { c => c.Files }, false);

                if (publishingHouse1 is null)
                    return NotFound();

                publishingHouse1.Name = publishingHouse.Name;
                publishingHouse1.Description = publishingHouse.Description;

                return View(publishingHouse1);
            }

            PublishingHouse NewPub = new()
            {
                Id = publishingHouse.Id,
                Description = publishingHouse.Description,
                Name = publishingHouse.Name
            };

            var existingPub = await _pubRepo.GetOneAsync(c => c.Id == publishingHouse.Id, new Expression<Func<PublishingHouse, object>>[] { c => c.Files });

            if (existingPub == null) return NotFound();

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

                // Remove file from database
                if (!await _pfRepo.DeleteAsync(file))
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
                    PublishingHouseFile publishingHouseFile = new()
                    {
                        Name = fileName,
                        FileType = fileType
                    };
                    await _pfRepo.CreateAsync(publishingHouseFile);

                    //Save File to Book Table
                    NewPub.Files.Add(publishingHouseFile);
                }
            }

            //Cut Connection from Existing
            _pubRepo.DetachEntity(existingPub);

            await _pubRepo.UpdateAsync(NewPub);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (await(_pubRepo.GetOneAsync(c => c.Id == id, new Expression<Func<PublishingHouse, object>>[] { c => c.Files })) is PublishingHouse publishingHouse)
            {
                foreach (var file in publishingHouse.Files)
                {
                    // Delete all files Physically
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", file.Name);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Delete Book from Db along with its Files
                await _pubRepo.DeleteAsync(publishingHouse);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }
}
