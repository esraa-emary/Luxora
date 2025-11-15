using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        [Column("Room_No")]
        public int RoomNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column("Status")]
        public string Status { get; set; } = "Available";

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        // Foreign Keys
        [Required]
        [Column("RT_ID")]
        public int RoomTypeId { get; set; }

        public string RoomName { get; set; }

        public string ImageUrl { get; set; }

        // Navigation properties
        [ForeignKey("RoomTypeId")]
        public virtual RoomType RoomType { get; set; } = null!;
        public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

        // Extra details in Rooms
        public int Capacity { get; set; }
        public string Size { get; set; }
        public string BedType { get; set; } = string.Empty;
        public string View { get; set; } = string.Empty;
        public string Amenities { get; set; } = string.Empty;
        public ICollection<CartItem> CartItems { get; set; }

    }
}

