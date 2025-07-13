using APIs.Entities.Data;
using APIs.Entities.Interfaces;
using APIs.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs.Entities.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            return true;
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.FindAsync(id) != null;
        }

        public virtual IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }

    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.UserProfiles)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await _dbSet
                .Where(u => u.IsActive)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName)
        {
            return await _dbSet
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName && ur.IsActive))
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }

        public override async Task<User?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.UserProfiles)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }

    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync()
        {
            return await _dbSet
                .Where(p => p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();
        }

        public async Task<Product?> GetBySkuAsync(string sku)
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.SKU == sku);
        }

        public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(p => p.Name.Contains(searchTerm) || 
                           p.Description!.Contains(searchTerm) ||
                           p.SKU!.Contains(searchTerm))
                .Include(p => p.Category)
                .ToListAsync();
        }

        public override async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }

    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetMainCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.ParentCategoryId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            return await _dbSet
                .Where(c => c.ParentCategoryId == parentCategoryId)
                .ToListAsync();
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Name == name);
        }

        public override async Task<Category?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.SubCategories)
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }

    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _dbSet
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
        {
            return await _dbSet
                .Where(o => o.Status == status)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .ToListAsync();
        }

        public override async Task<Order?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}