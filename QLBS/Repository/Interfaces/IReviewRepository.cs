using QLBS.Models;

namespace QLBS.Repository.Interfaces
{
    public interface IReviewRepository
    {
        Task AddReviewAsync(Review review);
        Task<bool> HasReviewedAsync(int userId, int bookId);
        Task<bool> CanReviewAsync(int userId, int bookId);
        Task<IEnumerable<Review>> GetReviewsByBookIdAsync(int bookId);
    }
}