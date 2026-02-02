using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly QLBSDbContext _context;

        public CartRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .AsNoTracking()
                .Include(c => c.Book)
                .ThenInclude(b => b.BookImages)
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.AddedAt)
                .ToListAsync();
        }

        public async Task<Cart?> GetCartItemAsync(int userId, int bookId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
        }

        public async Task AddToCartAsync(Cart cartItem)
        {
            await _context.Carts.AddAsync(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(Cart cartItem)
        {
            _context.Carts.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCartItemAsync(Cart cartItem)
        {
            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(int userId)
        {
            var items = await _context.Carts.Where(c => c.UserId == userId).ToListAsync();
            if (items.Any())
            {
                _context.Carts.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }
    }
}