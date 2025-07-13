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
            var Authors = await _authRepo.GetAsync(null, new Expression<Func<Author, object>>[] { a => a.Books });
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

            string fileName;
            FileType fileType;
            (fileName, fileType) = FileService.UploadNewFile(file);

            AuthorFile authorFile = new()
            {
                Name = fileName,
                FileType = fileType
            };
            await _afRepo.CreateAsync(authorFile);

            author.Files.Add(authorFile);

            await _authRepo.CreateAsync(author);

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authRepo.GetOneAsync(a => a.Id == id, new Expression<Func<Author, object>>[] { a => a.Files });
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
                foreach (var delFile in (await _afRepo.GetAsync(null, null, false)))
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
                await _afRepo.CreateAsync(authorFile);

                author.Files.Add(authorFile);
            }

            await _authRepo.UpdateAsync(author);

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Policy = $"{SD.Admins}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await (_authRepo.GetOneAsync(a => a.Id == id, new Expression<Func<Author, object>>[] { a => a.Files })) is Author author)
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
                await _authRepo.DeleteAsync(author);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }

}
