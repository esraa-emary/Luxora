using System.ComponentModel.DataAnnotations;

namespace Bookify.Models
{
    public class Payment
    {
        [Required]
        [Display(Name = "Guest Name")]
        public string GuestName { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        [StringLength(19)]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiry Date")]
        [StringLength(5)]
        public string ExpiryDate { get; set; }

        [Required]
        [Display(Name = "CVV")]
        [StringLength(4)]
        public string CVV { get; set; }

        public string RoomType { get; set; } = "Luxury Suite";
        public decimal TotalAmount { get; set; } = 250.00m;
    }
}
