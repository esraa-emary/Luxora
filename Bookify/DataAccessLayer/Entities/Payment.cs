using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        [Column("P_ID")]
        public int PaymentId { get; set; }

        // Foreign Keys
        [Required]
        [Column("R_ID")]
        public int ReservationId { get; set; }

        // Navigation properties
        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; } = null!;
    }
}

