using QLBS.Dtos;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileService(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        public async Task<UserProfileDto?> UpdateProfileAsync(int accountId, UpdateProfileDto dto)
        {
            var profile = await _userProfileRepository.GetByAccountIdAsync(accountId);
            if (profile == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                profile.FullName = dto.FullName;

            if (dto.DateOfBirth.HasValue)
                profile.DateOfBirth = dto.DateOfBirth.Value;

            if (!string.IsNullOrWhiteSpace(dto.Address))
                profile.Address = dto.Address;

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                profile.PhoneNumber = dto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl))
                profile.AvatarUrl = dto.AvatarUrl;

            var success = await _userProfileRepository.UpdateProfileAsync(profile);
            if (!success)
            {
                return null;
            }

            return await GetUserProfileAsync(accountId);
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int accountId)
        {
            var profile = await _userProfileRepository.GetProfileDetailsAsync(accountId);

            if (profile == null)
            {
                return null;
            }

            var userProfileDto = new UserProfileDto
            {
                Email = profile.Account.Email,
                FullName = profile.FullName,
                PhoneNumber = profile.PhoneNumber,
                Address = profile.Address,
                DateOfBirth = profile.DateOfBirth,
                AvatarUrl = profile.AvatarUrl,
                RoleName = profile.Account.Role.RoleName
            };
            return userProfileDto;
        }
    }
}
