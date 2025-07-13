using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIs.Entities.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
        public virtual ICollection<UserProfile> UserProfiles { get; set; } = new HashSet<UserProfile>();
    }

    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }

    public class UserRole
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public DateTime? RevokedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; } = null!;
    }

    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [StringLength(200)]
        public string? JobTitle { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(100)]
        public string? Company { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        [StringLength(255)]
        public string? Website { get; set; }

        [StringLength(255)]
        public string? LinkedInUrl { get; set; }

        [StringLength(255)]
        public string? TwitterUrl { get; set; }

        [StringLength(255)]
        public string? ProfileImageUrl { get; set; }

        [StringLength(100)]
        public string? TimeZone { get; set; }

        [StringLength(10)]
        public string? PreferredLanguage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}