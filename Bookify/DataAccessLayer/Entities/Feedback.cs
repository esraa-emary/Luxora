using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities
{
    [Table("Feedbacks")]
    public class Feedback
    {
        [Key]
        [Column("F_ID")]
        public int FeedbackId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        // Foreign Keys
        [Required]
        [Column("C_ID")]
        public int CustomerId { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
    }
}

