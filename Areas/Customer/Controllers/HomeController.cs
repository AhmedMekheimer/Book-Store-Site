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
        public async Task<IActionResult> Index(BooksSearchVM booksSearchVM, int PageId = 1)
        {
            const double NumberOfBookstInAPage = 8.0;
            IEnumerable<Book> books;
            if (string.IsNullOrEmpty(booksSearchVM.Search))
            {
                books = await _bookRepo.GetAsync(null, new Expression<Func<Book, object>>[] { b => b.Authors, b => b.PublishingHouses, b => b.Category });

                // Pagination
                var NoPages = Math.Ceiling(books.Count() / NumberOfBookstInAPage);
                if (NoPages < PageId)
                    return NotFound();
                books = books.Skip((PageId - 1) * (int)NumberOfBookstInAPage).Take((int)NumberOfBookstInAPage);
                booksSearchVM.NoPages = NoPages;
                booksSearchVM.PageId = PageId;
                booksSearchVM.Books = books;
            }
            else
            {
                // Search Filter
                books = await _bookRepo.GetAsync(b => b.Name.Contains(booksSearchVM.Search), new Expression<Func<Book, object>>[] { b => b.Authors, b => b.PublishingHouses, b => b.Category });

                // Pagination
                var NoPages = Math.Ceiling(books.Count() / NumberOfBookstInAPage);
                if (NoPages < PageId)
                    return NotFound();
                books = books.Skip((PageId - 1) * (int)NumberOfBookstInAPage).Take((int)NumberOfBookstInAPage);
                booksSearchVM.NoPages = NoPages;
                booksSearchVM.PageId = PageId;
                booksSearchVM.Books = books;
            }

            return View(booksSearchVM);
        }

        public async Task<IActionResult> BookDetails(int id)
        {
            if (await _bookRepo.GetOneAsync(b => b.Id == id) is Book book)
                return View(book);

            return NotFound();
        }
    }
}
