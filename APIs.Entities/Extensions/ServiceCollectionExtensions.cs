using APIs.Entities.Data;
using APIs.Entities.Interfaces;
using APIs.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APIs.Entities.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEntitiesServices(this IServiceCollection services, string connectionString)
        {
            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddEntitiesServicesWithInMemoryDatabase(this IServiceCollection services, string databaseName = "TestDb")
        {
            // Add DbContext with In-Memory database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));

            // Add repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IServiceCollection AddEntitiesServicesWithSqlite(this IServiceCollection services, string connectionString)
        {
            // Add DbContext with SQLite
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            // Add repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}