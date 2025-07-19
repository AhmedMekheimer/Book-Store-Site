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
        /*        public async Task<IActionResult> Index(BooksSearchVM booksSearchVM, int PageId = 1)
                {
                    const double NumberOfBookstInAPage = 8.0;
                    if (string.IsNullOrEmpty(booksSearchVM.Search))
                    {
                        var books = await _bookRepo.GetAsync(null, new Expression<Func<Book, object>>[] { b => b.Category, b=>b.Files });
                        books = books.OrderBy(b => 100).ToList();
                        // Pagination
                        var NoPages = Math.Ceiling(books.Count() / NumberOfBookstInAPage);
                        if (NoPages < PageId)
                            return NotFound();
                        books = books.Skip((PageId - 1) * (int)NumberOfBookstInAPage).Take((int)NumberOfBookstInAPage);
                        booksSearchVM.NoPages = NoPages;
                        booksSearchVM.PageId = PageId;
                        booksSearchVM.Books = books.ToList();
                    }
                    else
                    {
                        // Search Filter
                        var books = await _bookRepo.GetAsync(b => b.Name.Contains(booksSearchVM.Search), new Expression<Func<Book, object>>[] { b => b.Category, b => b.Files });
                        books = books.OrderBy(b => 100).ToList();

                        // Pagination
                        var NoPages = Math.Ceiling(books.Count() / NumberOfBookstInAPage);
                        if (NoPages < PageId)
                            return NotFound();
                        books = books.Skip((PageId - 1) * (int)NumberOfBookstInAPage).Take((int)NumberOfBookstInAPage);
                        booksSearchVM.NoPages = NoPages;
                        booksSearchVM.PageId = PageId;
                        booksSearchVM.Books = books.ToList();
                    }

                    return View(booksSearchVM);
                }*/

        public async Task<IActionResult> Index( BooksSearchVM vm, int pageId = 1,int? seed = null)
        {
            const int pageSize = 8;

            // 1. Create or preserve seed
            if (!seed.HasValue)
                seed = new Random().Next();

            // 2. Fetch *unsorted* list from repo
            var allBooks = (await _bookRepo.GetAsync(
                condition: string.IsNullOrEmpty(vm.Search)
                             ? null
                             : (b => b.Name.Contains(vm.Search)),
                includes: new[] { (Expression<Func<Book, object>>)(b => b.Category),
                          b => b.Files },
                tracked: false
            )).ToList();

            // 3. Shuffle in memory with seeded Random
            var rnd = new Random(seed.Value);
            var shuffled = allBooks.OrderBy(_ => rnd.Next()).ToList();

            // 4. Paginate
            var totalPages = (int)Math.Ceiling(shuffled.Count / (double)pageSize);
            if (pageId < 1 || pageId > totalPages)
                return NotFound();

            vm.PageId = pageId;
            vm.NoPages = totalPages;
            vm.Seed = seed.Value;
            vm.Books = shuffled
                          .Skip((pageId - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();

            return View(vm);
        }


        public async Task<IActionResult> BookDetails(int id)
        {
            if (await _bookRepo.GetOneAsync(b => b.Id == id) is Book book)
                return View(book);

            return NotFound();
        }
    }
}
