using Bookify.DataAccessLayer;
using Bookify.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Services
{
    public class CartService
    {
        private readonly BookifyDbContext _context;

        public CartService(BookifyDbContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(int roomNumber, int userId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.RoomNumber == roomNumber && c.UserId == userId);

            if (item == null)
            {
                item = new CartItem
                {
                    RoomNumber = roomNumber,
                    UserId = userId
                };

                _context.CartItems.Add(item);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CartItem>> GetCartAsync(int userId)
        {
            return await _context.CartItems
                .Include(c => c.Room)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task RemoveItemAsync(int id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = _context.CartItems.Where(c => c.UserId == userId);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }


}
