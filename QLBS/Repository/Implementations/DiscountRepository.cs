using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly QLBSDbContext _context;

        public DiscountRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DiscountCode>> GetAllAsync()
        {
            return await _context.DiscountCodes
                .AsNoTracking()
                .OrderByDescending(d => d.DiscountCodeId)
                .ToListAsync();
        }

        public async Task<DiscountCode?> GetByIdAsync(int id)
        {
            return await _context.DiscountCodes.FindAsync(id);
        }

        public async Task<DiscountCode?> GetByCodeAsync(string code)
        {
            return await _context.DiscountCodes
                .FirstOrDefaultAsync(d => d.Code == code);
        }

        public async Task<DiscountCode> AddAsync(DiscountCode discountCode)
        {
            await _context.DiscountCodes.AddAsync(discountCode);
            await _context.SaveChangesAsync();
            return discountCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.DiscountCodes.FindAsync(id);
            if (entity == null) return false;

            try
            {
                _context.DiscountCodes.Remove(entity);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _context.DiscountCodes.AnyAsync(d => d.Code == code);
        }
    }
}