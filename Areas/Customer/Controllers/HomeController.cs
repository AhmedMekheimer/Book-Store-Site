using Online_Book_Store.ViewModels.Customer;

namespace Online_Book_Store.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<Book> _bookRepo;
        private readonly IRepository<Category> _catRepo;

        public HomeController(IRepository<Book> bookRepo, IRepository<Category> catRepo)
        {
            _bookRepo = bookRepo;
            _catRepo = catRepo;
        }

        public async Task<IActionResult> Index(BooksSearchVM vm)
        {
            const int pageSize = 9;

            // 1. Fetch *unsorted* list from repo
            var allBooks = await _bookRepo.GetAsync(
                condition: b =>
                    (string.IsNullOrEmpty(vm.Search) || b.Name.Contains(vm.Search)) &&
                    (vm.CategoryId == 0 || b.CategoryId == vm.CategoryId),
                new List<Func<IQueryable<Book>, IQueryable<Book>>>{
                    a => a.Include(b => b.Category),
                    a => a.Include(b => b.Files)
                },
                tracked: false
            );
            if(allBooks.Count==0)
            {
                return View(vm);
            }
            // 2. Shuffle Books
            var shuffled = allBooks.OrderBy(b => b.Name).ToList();

            // 3. Paginate
            var totalPages = (int)Math.Ceiling(shuffled.Count / (double)pageSize);
            if (vm.PageId < 1 || vm.PageId > totalPages)
                return NotFound();

            vm.Categories = await _catRepo.GetAsync();
            vm.NoPages = totalPages;
            vm.Books = shuffled
                          .Skip((vm.PageId - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();

            return View(vm);
        }

        public async Task<IActionResult> BookDetails(int id)
        {
            if (await _bookRepo.GetOneAsync(b => b.Id == id,
                new List<Func<IQueryable<Book>, IQueryable<Book>>>{
                          a => a.Include(b => b.Category),
                          a => a.Include(b => b.Files),
                          a => a.Include(b => b.Authors),
                          a => a.Include(b => b.PublishingHouses) }, false) is Book book)
            {
                var RelatedBooks = await _bookRepo.GetAsync(b => (b.CategoryId == book.CategoryId) && (b.Id != id),
                    new List<Func<IQueryable<Book>, IQueryable<Book>>>{
                          a => a.Include(b => b.Category),
                          a => a.Include(b => b.Files) },
                tracked: false);
                return View(new BookDetailsVM()
                {
                    Book = book,
                    RelatedBooks = RelatedBooks
                });
            }

            return NotFound();
        }
    }
}
