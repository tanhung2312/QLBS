using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly QLBSDbContext _context;
        private readonly ILogger<AuthorRepository> _logger;

        public AuthorRepository(QLBSDbContext context, ILogger<AuthorRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _context.Authors
                .AsNoTracking()
                .Where(a => a.IsDeleted == false)
                .OrderBy(a => a.AuthorName)
                .ToListAsync();
        }

        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _context.Authors
                .FirstOrDefaultAsync(a => a.AuthorId == id && a.IsDeleted == false);
        }

        public async Task<Author> AddAsync(Author author)
        {
            author.IsDeleted = false;
            await _context.Authors.AddAsync(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> UpdateAsync(Author author)
        {
            try
            {
                _context.Authors.Update(author);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật tác giả ID: {Id}", author.AuthorId);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null) return false;

                author.IsDeleted = true;
                _context.Authors.Update(author);

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa tác giả ID: {Id}", id);
                return false;
            }
        }
    }
}