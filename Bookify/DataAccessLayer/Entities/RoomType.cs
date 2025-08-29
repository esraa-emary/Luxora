using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities
{
    [Table("Room_Types")]
    public class RoomType
    {
        [Key]
        [Column("RT_ID")]
        public int RoomTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string TypeName { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
