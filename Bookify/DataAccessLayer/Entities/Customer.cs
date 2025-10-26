using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities
{
    [Table("Customers")]
    public class Customer
    {
        [Key]
        
        public int CustomerId { get; set; }

        [Required]
        [StringLength(50)]
        
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
       
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string SSN { get; set; } = string.Empty;

        [Required]
        public int Age { get; set; }

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}

