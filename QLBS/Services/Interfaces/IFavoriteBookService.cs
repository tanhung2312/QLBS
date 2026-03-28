using QLBS.DTOs;

namespace QLBS.Services.Interfaces
{
    public interface IFavoriteBookService
    {
        Task<FavoriteBookListResponseDto> GetFavoritesAsync(int userId);
        Task AddFavoriteAsync(int userId, int bookId);
        Task RemoveFavoriteAsync(int userId, int bookId);
    }
}