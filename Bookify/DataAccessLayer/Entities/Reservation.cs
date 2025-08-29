using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities
{
    [Table("Reservations")]
    public class Reservation
    {
        [Key]
        [Column("R_ID")]
        public int ReservationId { get; set; }

        [Required]
        [Column("S_Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column("E_Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("Date")]
        public DateTime ReservationDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Foreign Keys
        [Required]
        [Column("C_ID")]
        public int CustomerId { get; set; }

        [Required]
        [Column("Room_No")]
        public int RoomNumber { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey("RoomNumber")]
        public virtual Room Room { get; set; } = null!;

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}

