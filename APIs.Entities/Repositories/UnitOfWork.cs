using APIs.Entities.Data;
using APIs.Entities.Interfaces;
using APIs.Entities.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace APIs.Entities.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Products = new ProductRepository(_context);
            Categories = new CategoryRepository(_context);
            Orders = new OrderRepository(_context);
            Roles = new GenericRepository<Role>(_context);
            UserRoles = new GenericRepository<UserRole>(_context);
            UserProfiles = new GenericRepository<UserProfile>(_context);
            OrderItems = new GenericRepository<OrderItem>(_context);
        }

        public IUserRepository Users { get; private set; }
        public IProductRepository Products { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IGenericRepository<Role> Roles { get; private set; }
        public IGenericRepository<UserRole> UserRoles { get; private set; }
        public IGenericRepository<UserProfile> UserProfiles { get; private set; }
        public IGenericRepository<OrderItem> OrderItems { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}