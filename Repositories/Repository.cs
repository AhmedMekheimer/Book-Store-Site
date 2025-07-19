namespace Online_Book_Store.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;// = new();
        private DbSet<T> _db { set; get; }
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        // CRUD
        public async Task<bool> CreateAsync(T entity)
        {
            try
            {
                await _db.AddAsync(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                var result = _db.Update(entity);

                var result2 = await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _db.Attach(entity);
                _db.Remove(entity);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex}");
                return false;
            }
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? condition = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true)
        {
            IQueryable<T> entities = _db;

            if (condition is not null)
            {
                entities = entities.Where(condition);
            }

            if (includes is not null)
            {
                foreach (var item in includes)
                {
                    entities = entities.Include(item);
                }
            }

            if (!tracked)
            {
                entities = entities.AsNoTracking();
            }

            return (await entities.ToListAsync());
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>? condition = null, Expression<Func<T, object>>[]? includes = null, bool tracked = true)
        {
            return (await GetAsync(condition, includes, tracked)).SingleOrDefault();
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ex: {ex}");
                return false;
            }
        }

        public void DetachEntity(T entity)
        {
            _db.Entry(entity).State = EntityState.Detached;
        }
    }
}
