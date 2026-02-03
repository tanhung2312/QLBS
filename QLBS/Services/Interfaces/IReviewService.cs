using QLBS.Dtos.Review;

namespace QLBS.Services.Interfaces
{
    public interface IReviewService
    {
        Task<string> AddReviewAsync(int accountId, AddReviewDto dto);
        Task<IEnumerable<ReviewResponseDto>> GetBookReviewsAsync(int bookId);
    }
}