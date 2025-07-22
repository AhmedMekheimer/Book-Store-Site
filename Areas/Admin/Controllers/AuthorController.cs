using Microsoft.AspNetCore.Authorization;
using Online_Book_Store.Utility;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly IRepository<Author> _authRepo;
        private readonly IRepository<AuthorFile> _afRepo;

        public AuthorController(IRepository<Author> authRepo, IRepository<AuthorFile> afRepo)
        {
            _afRepo = afRepo;
            _authRepo = authRepo;
        }
        [Authorize(Policy = $"{SD.Workers}")]
        public async Task<IActionResult> Index()
        {
            var Authors = await _authRepo.GetAsync(null, new List<Func<IQueryable<Author>, IQueryable<Author>>> { a => a.Include(a => a.Books) });
            return View(Authors);
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public IActionResult Create()
        {

            return View(new Author());
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Create(Author author, IFormFile file)
        {
            if (author is null)
                return NotFound();

            ModelState.Remove("file");
            if (!ModelState.IsValid)
                return View(author);

            if (file is not null)
            {
                string fileName;
                FileType fileType;
                (fileName, fileType) = FileService.UploadNewFile(file);

                AuthorFile authorFile = new()
                {
                    Name = fileName,
                    FileType = fileType
                };
                var FileResult = await _afRepo.CreateAsync(authorFile);
                if (FileResult)
                    author.Files.Add(authorFile);
            }

            var AuthResult = await _authRepo.CreateAsync(author);
            if (AuthResult)
                TempData["success-notification"] = "Author Created Successfully";
            else
                TempData["error-notification"] = "Author Creation Failure";

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authRepo.GetOneAsync(a => a.Id == id, new List<Func<IQueryable<Author>, IQueryable<Author>>> { a => a.Include(a => a.Files) });
            if (author is not null)
                return View(author);

            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Edit(Author author, IFormFile file)
        {
            if (author is null)
                return NotFound();

            ModelState.Remove("file");
            if (!ModelState.IsValid)
            {
                if (await (_afRepo.GetOneAsync(a => a.AuthorId == author.Id, null, false)) is AuthorFile authFile)
                {
                    author.Files.Add(authFile);
                }
                return View(author);
            }

            if (file is not null)
            {
                foreach (var delFile in (await _afRepo.GetAsync(a => a.AuthorId == author.Id, null, false)))
                {
                    // Delete physical file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", delFile.Name);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // Remove Author file from database
                    await _afRepo.DeleteAsync(delFile);
                }


                string fileName;
                FileType fileType;
                (fileName, fileType) = FileService.UploadNewFile(file);

                AuthorFile authorFile = new()
                {
                    Name = fileName,
                    FileType = fileType
                };
                authorFile.AuthorId = author.Id;
                await _afRepo.CreateAsync(authorFile);
            }

            var UpdateResult=await _authRepo.UpdateAsync(author);
            if(UpdateResult)
                TempData["success-notification"] = "Author Updated Successfully";
            else
                TempData["error-notification"] = "Author Updating Failure";

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await (_authRepo.GetOneAsync(a => a.Id == id, new List<Func<IQueryable<Author>, IQueryable<Author>>> { a => a.Include(a => a.Files) })) is Author author)
            {
                foreach (var file in author.Files)
                {
                    // Delete all files Physically
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", file.Name);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Deleting Author along with his Files in DB
                var DeleteReuslt=await _authRepo.DeleteAsync(author);
                if(DeleteReuslt)
                    TempData["success-notification"] = "Author Removed Successfully";
                else
                    TempData["error-notification"] = "Author Removal Failure";

                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }

}
