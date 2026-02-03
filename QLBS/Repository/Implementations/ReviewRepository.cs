using Microsoft.EntityFrameworkCore;
using QLBS.Constants;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly QLBSDbContext _context;

        public ReviewRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasReviewedAsync(int userId, int bookId)
        {
            return await _context.Reviews.AnyAsync(r => r.UserId == userId && r.BookId == bookId);
        }

        public async Task<bool> CanReviewAsync(int userId, int bookId)
        {
            return await _context.OrderTables
                .Include(o => o.OrderDetails)
                .AnyAsync(o =>
                    o.UserId == userId &&
                    o.OrderStatus == OrderStatusConstants.Completed &&
                    o.OrderDetails.Any(d => d.BookId == bookId)
                );
        }


        public async Task<IEnumerable<Review>> GetReviewsByBookIdAsync(int bookId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }
    }
}
