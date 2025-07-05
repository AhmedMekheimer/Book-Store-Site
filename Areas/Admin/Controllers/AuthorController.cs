using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_Book_Store.Models.File_Entities;

namespace Online_Book_Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context = new();
        public IActionResult Index()
        {
            var Authors = _context.Authors.Include(p => p.Books).ToList();
            return View(Authors);
        }
        public IActionResult Create()
        {
            return View(new Author());
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Create(Author author, IFormFile file)
        {
            var authors = _context.Authors;
            if (author is null)
            {
                return NotFound();
            }

            ModelState.Remove("file");
            if (!ModelState.IsValid)
            {
                return View(author);
            }

            string fileName;
            FileType fileType;
            (fileName, fileType) = FileService.UploadNewFile(file);

            AuthorFile authorFile = new()
            {
                Name = fileName,
                FileType = fileType
            };
            _context.AuthorFiles.Add(authorFile);
            _context.SaveChanges();

            author.Files.Add(authorFile);
            //_context.SaveChanges();

            authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id)
        {
            var author = _context.Authors.Include(a => a.Files).SingleOrDefault(a => a.Id == id);
            if (author is not null)
            {
                return View(author);
            }
            return NotFound();
        }

        [HttpPost]
        [RequestSizeLimit(1_000_000_000)] // 1GB limit
        public IActionResult Edit(Author author, IFormFile file)
        {
            if (author is null)
            {
                return NotFound();
            }
            var authorFilesList = _context.AuthorFiles.ToList();
            var authorFiles = _context.AuthorFiles;

            ModelState.Remove("file");
            if (!ModelState.IsValid)
            {
                if (authorFiles.SingleOrDefault(a => a.AuthorId == author.Id) is AuthorFile authFile)
                {
                    author.Files.Add(authFile);
                }
                return View(author);
            }

            if (file is not null)
            {
                foreach (var delFile in authorFilesList)
                {
                    // Delete physical file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Files", delFile.Name);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    // Remove from database
                    // Attach and mark for deletion
                    authorFiles.Attach(delFile);
                    authorFiles.Remove(delFile);
                    _context.SaveChanges();
                }

                string fileName;
                FileType fileType;
                (fileName, fileType) = FileService.UploadNewFile(file);

                AuthorFile authorFile = new()
                {
                    Name = fileName,
                    FileType = fileType
                };
                authorFiles.Add(authorFile);
                _context.SaveChanges();

                author.Files.Add(authorFile);
            }

            _context.Authors.Update(author);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var authors = _context.Authors;
            var author = authors.Find(id);
            if (author is not null)
            {
                authors.Remove(author);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }
    }

}
