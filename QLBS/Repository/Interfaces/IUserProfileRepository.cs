using QLBS.Models;
using System.Threading.Tasks;

namespace QLBS.Repository.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByAccountIdAsync(int accountId);
        Task<bool> UpdateProfileAsync(UserProfile profile);
        Task<UserProfile> GetProfileByAccountIdAsync(int accountId);
    }
}
