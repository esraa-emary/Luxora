using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.DataAccessLayer.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        [ForeignKey("Room")]
        public int RoomNumber { get; set; }

        public int UserId { get; set; } 

        public Room Room { get; set; }
    }
}
