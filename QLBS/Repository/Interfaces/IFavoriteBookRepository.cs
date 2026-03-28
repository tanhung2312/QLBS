using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IFavoriteBookRepository
    {
        Task<FavoriteBook?> GetByUserAndBookAsync(int userId, int bookId);
        Task<IEnumerable<FavoriteBook>> GetByUserIdAsync(int userId);
        Task<int> CountByUserIdAsync(int userId);
        Task AddAsync(FavoriteBook favoriteBook);
        Task UpdateAsync(FavoriteBook favoriteBook);
        Task SaveChangesAsync();
    }
}