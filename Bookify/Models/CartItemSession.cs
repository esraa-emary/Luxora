namespace Bookify.Models
{
    public class CartItemSession
    {

        public int RoomNumber { get; set; }
        public string RoomName { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;

        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }

        public decimal LineTotal => Price * Quantity;
    }
}
