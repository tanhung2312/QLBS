using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> GetAccountByEmailAsync(string email);
        Task<Account?> GetAccountByRefreshTokenAsync(string refreshToken);
        Task<bool> CreateAccountWithProfileAsync(Account account, UserProfile profile);
        Task<bool> UpdateAccountAsync(Account account);
        Task<int> SaveChangesAsync();
        Task<int?> GetRoleIdByNameAsync(string roleName);
    }
}
