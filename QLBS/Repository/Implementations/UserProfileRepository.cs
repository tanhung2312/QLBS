using Microsoft.EntityFrameworkCore;
using QLBS.Models;
using QLBS.Repository.Interfaces;

namespace QLBS.Repository.Implementations
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly QLBSDbContext _context;

        public UserProfileRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetByAccountIdAsync(int accountId)
        {
            return await _context.UserProfiles
                .FirstOrDefaultAsync(up => up.AccountId == accountId);
        }

        public async Task<bool> UpdateProfileAsync(UserProfile profile)
        {
            try
            {
                _context.UserProfiles.Update(profile);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<UserProfile?> GetProfileDetailsAsync(int accountId)
        {
            return await _context.UserProfiles
                .AsNoTracking()
                .Include(p => p.Account)
                .ThenInclude(a => a.Role)
                .FirstOrDefaultAsync(up => up.AccountId == accountId);
        }
    }
}
