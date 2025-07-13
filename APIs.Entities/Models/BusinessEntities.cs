using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIs.Entities.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(50)]
        public string? SKU { get; set; }

        public int CategoryId { get; set; }

        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    }

    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Description { get; set; }

        public int? ParentCategoryId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("ParentCategoryId")]
        public virtual Category? ParentCategory { get; set; }

        public virtual ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        public int UserId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingAmount { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public DateTime? ShippedDate { get; set; }

        public DateTime? DeliveredDate { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new HashSet<OrderItem>();
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}