using APIs.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace APIs.Entities.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        IQueryable<T> GetQueryable();
    }

    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
    }

    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetActiveProductsAsync();
        Task<Product?> GetBySkuAsync(string sku);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    }

    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetMainCategoriesAsync();
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId);
        Task<Category?> GetByNameAsync(string name);
    }

    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
        Task<Order?> GetByOrderNumberAsync(string orderNumber);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    }

    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        IOrderRepository Orders { get; }
        IGenericRepository<Role> Roles { get; }
        IGenericRepository<UserRole> UserRoles { get; }
        IGenericRepository<UserProfile> UserProfiles { get; }
        IGenericRepository<OrderItem> OrderItems { get; }
        
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}