using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities;


public class Payment
{
    [Key]
    [Column("P_ID")]
    public int PaymentId { get; set; }

    [Column("R_ID")]
    public int ReservationId { get; set; }

    [ForeignKey("OrderId")]
    public int OrderId { get; set; }

    [Column("amount")]
    public decimal TotalAmount { get; set; }

    // Navigation properties
    public Reservation Reservation { get; set; }
    public Order Order { get; set; }
}
