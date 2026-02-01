using QLBS.Dtos;
using QLBS.Models;
using System.Threading.Tasks;

namespace QLBS.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfile?> UpdateProfileAsync(int accountId, UpdateProfileDto dto);
        Task<UserProfileDto?> GetUserProfileAsync(int accountId);
    }
}
