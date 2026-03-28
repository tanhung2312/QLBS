using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class FavoriteBookRepository : IFavoriteBookRepository
    {
        private readonly QLBSDbContext _context;

        public FavoriteBookRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<FavoriteBook?> GetByUserAndBookAsync(int userId, int bookId)
        {
            return await _context.FavoriteBooks
                .FirstOrDefaultAsync(f => f.UserId == userId && f.BookId == bookId);
        }

        public async Task<IEnumerable<FavoriteBook>> GetByUserIdAsync(int userId)
        {
            return await _context.FavoriteBooks
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .Include(f => f.Book)
                    .ThenInclude(b => b.Author)
                .Include(f => f.Book)
                    .ThenInclude(b => b.Category)
                .Include(f => f.Book)
                    .ThenInclude(b => b.BookImages)
                .OrderByDescending(f => f.MarkedDate)
                .ToListAsync();
        }

        public async Task<int> CountByUserIdAsync(int userId)
        {
            return await _context.FavoriteBooks
                .CountAsync(f => f.UserId == userId && !f.IsDeleted);
        }

        public async Task AddAsync(FavoriteBook favoriteBook)
        {
            await _context.FavoriteBooks.AddAsync(favoriteBook);
        }

        public async Task UpdateAsync(FavoriteBook favoriteBook)
        {
            _context.FavoriteBooks.Update(favoriteBook);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}