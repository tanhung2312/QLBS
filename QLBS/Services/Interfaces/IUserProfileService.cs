using QLBS.Dtos;
using QLBS.Models;
using System.Threading.Tasks;

namespace QLBS.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto?> UpdateProfileAsync(int accountId, UpdateProfileDto dto);
        Task<UserProfileDto?> GetUserProfileAsync(int accountId);
    }
}
