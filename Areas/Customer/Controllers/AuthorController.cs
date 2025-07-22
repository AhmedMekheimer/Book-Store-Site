using Online_Book_Store.ViewModels.Customer;

namespace Online_Book_Store.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AuthorController : Controller
    {
        private readonly IRepository<Author> _authRepo;

        public AuthorController(IRepository<Author> authRepo)
        {
            _authRepo = authRepo;
        }

        public async Task<IActionResult> Index(AuthorSearchVM vm)
        {
            const int pageSize = 9;
            var allAuthors = await _authRepo.GetAsync(
                condition: a => string.IsNullOrEmpty(vm.Search) || a.Name.Contains(vm.Search),
                new List<Func<IQueryable<Author>, IQueryable<Author>>> {
                    a=>a.Include(a => a.Files)
                },
                tracked: false);

            // Shuffling
            allAuthors = allAuthors.OrderBy(a => a.Books.Count()).ToList();

            // Paginate
            var totalPages = (int)Math.Ceiling(allAuthors.Count / (double)pageSize);
            if (vm.PageId < 1 || vm.PageId > totalPages)
                return NotFound();

            vm.NoPages = totalPages;
            vm.Authors = allAuthors.Skip((vm.PageId - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();
            return View(vm);
        }

        public async Task<IActionResult> AuthorDetails(int id)
        {
            if (await _authRepo.GetOneAsync(a => a.Id == id, 
                                            new List<Func<IQueryable<Author>, IQueryable<Author>>> { 
                                                q => q.Include(a => a.Files),
                                                q => q.Include(a => a.Books).ThenInclude(b => b.Category),
                                                q => q.Include(a => a.Books).ThenInclude(b => b.Files)
                                            },
                                            tracked: false ) is Author author)
            {
                return View(author);
            }
            return NotFound();
        }
    }
}
