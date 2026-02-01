using Microsoft.EntityFrameworkCore;
using QLBS.Repository.Interfaces;
using QLBS.Models;

namespace QLBS.Repository.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly QLBSDbContext _context;

        public AccountRepository(QLBSDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Account?> GetAccountByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Accounts
                .Include(a => a.Role)
                .Include(a => a.UserProfile)
                .FirstOrDefaultAsync(a => a.RefreshToken == refreshToken);
        }

        public async Task<bool> CreateAccountWithProfileAsync(Account account, UserProfile profile)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                account.CreatedAt = DateTime.UtcNow;
                account.UpdatedAt = DateTime.UtcNow;
                account.IsActive = true;
                account.IsEmailVerified = false;

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                profile.AccountId = account.AccountId;
                profile.IsDeleted = false;

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            try
            {
                account.UpdatedAt = DateTime.UtcNow;
                _context.Accounts.Update(account);
                return await SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<int?> GetRoleIdByNameAsync(string roleName)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == roleName);
            return role?.RoleId;
        }

        public async Task<bool> UpdateAccountOtpAsync(Account account)
        {
            try
            {
                account.UpdatedAt = DateTime.UtcNow;
                _context.Accounts.Update(account);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
