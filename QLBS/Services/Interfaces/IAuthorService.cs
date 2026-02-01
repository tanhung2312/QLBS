using QLBS.Dtos.Author;

namespace QLBS.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<IEnumerable<AuthorResponseDto>> GetAllAuthorsAsync();
        Task<AuthorResponseDto?> GetAuthorByIdAsync(int id);
        Task<AuthorResponseDto?> CreateAuthorAsync(CreateAuthorDto createDto);
        Task<bool> UpdateAuthorAsync(int id, UpdateAuthorDto updateDto);
        Task<bool> DeleteAuthorAsync(int id);
    }
}