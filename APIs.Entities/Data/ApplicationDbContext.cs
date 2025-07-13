using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using APIs.Entities.Models;

namespace APIs.Entities.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Entity DbSets
        public DbSet<User> AppUsers { get; set; }
        public DbSet<Role> AppRoles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure entity relationships and constraints
            ConfigureUserEntities(builder);
            ConfigureBusinessEntities(builder);
            SeedData(builder);
        }

        private void ConfigureUserEntities(ModelBuilder builder)
        {
            // User configuration
            builder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            });

            // Role configuration
            builder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            // UserRole configuration
            builder.Entity<UserRole>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
                
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // UserProfile configuration
            builder.Entity<UserProfile>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
                
                entity.HasOne(up => up.User)
                      .WithMany(u => u.UserProfiles)
                      .HasForeignKey(up => up.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        private void ConfigureBusinessEntities(ModelBuilder builder)
        {
            // Product configuration
            builder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.SKU).IsUnique();
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);

                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Category configuration
            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);

                entity.HasOne(c => c.ParentCategory)
                      .WithMany(c => c.SubCategories)
                      .HasForeignKey(c => c.ParentCategoryId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Order configuration
            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TaxAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.ShippingAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(o => o.User)
                      .WithMany()
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // OrderItem configuration
            builder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.OrderItems)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void SeedData(ModelBuilder builder)
        {
            // Seed Categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets", CreatedAt = DateTime.UtcNow },
                new Category { Id = 2, Name = "Books", Description = "Books and literature", CreatedAt = DateTime.UtcNow },
                new Category { Id = 3, Name = "Clothing", Description = "Clothing and accessories", CreatedAt = DateTime.UtcNow },
                new Category { Id = 4, Name = "Home & Garden", Description = "Home and garden items", CreatedAt = DateTime.UtcNow }
            );

            // Seed Roles
            builder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin", Description = "System administrator", CreatedAt = DateTime.UtcNow },
                new Role { Id = 2, Name = "User", Description = "Regular user", CreatedAt = DateTime.UtcNow },
                new Role { Id = 3, Name = "Manager", Description = "Manager role", CreatedAt = DateTime.UtcNow }
            );

            // Seed Users
            builder.Entity<User>().HasData(
                new User 
                { 
                    Id = 1, 
                    FirstName = "John", 
                    LastName = "Doe", 
                    Email = "john.doe@example.com",
                    PhoneNumber = "123-456-7890",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new User 
                { 
                    Id = 2, 
                    FirstName = "Jane", 
                    LastName = "Smith", 
                    Email = "jane.smith@example.com",
                    PhoneNumber = "098-765-4321",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );

            // Seed Products
            builder.Entity<Product>().HasData(
                new Product 
                { 
                    Id = 1, 
                    Name = "Laptop", 
                    Description = "High-performance laptop", 
                    Price = 999.99m, 
                    SKU = "LAP001", 
                    CategoryId = 1, 
                    StockQuantity = 50,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                },
                new Product 
                { 
                    Id = 2, 
                    Name = "Programming Book", 
                    Description = "Learn programming fundamentals", 
                    Price = 29.99m, 
                    SKU = "BOOK001", 
                    CategoryId = 2, 
                    StockQuantity = 100,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                }
            );
        }
    }
}