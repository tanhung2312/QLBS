using QLBS.Dtos.Review;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IUserProfileRepository _userRepo;

        public ReviewService(IReviewRepository reviewRepo, IUserProfileRepository userRepo)
        {
            _reviewRepo = reviewRepo;
            _userRepo = userRepo;
        }

        public async Task<string> AddReviewAsync(int accountId, AddReviewDto dto)
        {
            var profile = await _userRepo.GetByAccountIdAsync(accountId);
            if (profile == null) return "User không tồn tại";

            bool canReview = await _reviewRepo.CanReviewAsync(profile.UserId, dto.BookId);
            if (!canReview)
                return "Bạn chưa thể đánh giá sách này (Chưa mua hoặc Đơn hàng chưa hoàn thành).";

            if (await _reviewRepo.HasReviewedAsync(profile.UserId, dto.BookId))
                return "Bạn đã đánh giá sách này rồi.";

            var review = new Review
            {
                UserId = profile.UserId,
                BookId = dto.BookId,
                Rating = (byte)dto.Rating,
                Comment = dto.Comment,
                ReviewDate = DateTime.Now
            };

            await _reviewRepo.AddReviewAsync(review);
            return "Success";
        }

        public async Task<IEnumerable<ReviewResponseDto>> GetBookReviewsAsync(int bookId)
        {
            var reviews = await _reviewRepo.GetReviewsByBookIdAsync(bookId);

            return reviews.Select(r => new ReviewResponseDto
            {
                ReviewId = r.ReviewId,
                UserName = r.User?.FullName ?? "Người dùng ẩn danh",
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate
            });
        }
    }
}
