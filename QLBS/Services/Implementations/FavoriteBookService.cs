using QLBS.DTOs;
using QLBS.Models;
using QLBS.Repository.Interfaces;
using QLBS.Services.Interfaces;

namespace QLBS.Services.Implementations
{
    public class FavoriteBookService : IFavoriteBookService
    {
        private readonly IFavoriteBookRepository _favoriteBookRepo;
        private readonly IUserProfileRepository _userRepo;
        private readonly IBookRepository _bookRepo;

        public FavoriteBookService(
            IFavoriteBookRepository favoriteBookRepo,
            IUserProfileRepository userRepo,
            IBookRepository bookRepo)
        {
            _favoriteBookRepo = favoriteBookRepo;
            _userRepo = userRepo;
            _bookRepo = bookRepo;
        }

        public async Task<FavoriteBookListResponseDto> GetFavoritesAsync(int userId)
        {
            // Dùng GetByAccountIdAsync để kiểm tra user tồn tại
            var user = await _userRepo.GetByAccountIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User không tồn tại.");

            var favorites = await _favoriteBookRepo.GetByUserIdAsync(userId);
            var total = await _favoriteBookRepo.CountByUserIdAsync(userId);

            return new FavoriteBookListResponseDto
            {
                TotalCount = total,
                Items = favorites.Select(f => new FavoriteBookResponseDto
                {
                    BookId = f.BookId,
                    BookTitle = f.Book.BookTitle,
                    AuthorName = f.Book.Author?.AuthorName,
                    CategoryName = f.Book.Category?.CategoryName,
                    Price = f.Book.Price,
                    // Lấy ảnh bìa (IsCover = true), nếu không có thì lấy ảnh đầu tiên
                    CoverImageUrl = f.Book.BookImages?
                                     .FirstOrDefault(i => i.IsCover)?.URL
                                   ?? f.Book.BookImages?.FirstOrDefault()?.URL,
                    MarkedDate = f.MarkedDate
                })
            };
        }

        public async Task AddFavoriteAsync(int userId, int bookId)
        {
            var user = await _userRepo.GetByAccountIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User không tồn tại.");

            var book = await _bookRepo.GetBookByIdAsync(bookId);
            if (book == null)
                throw new KeyNotFoundException("Book không tồn tại.");

            var existing = await _favoriteBookRepo.GetByUserAndBookAsync(userId, bookId);

            if (existing != null)
            {
                if (!existing.IsDeleted)
                    throw new InvalidOperationException("Sách đã có trong danh sách yêu thích.");

                // Khôi phục nếu đã xóa mềm
                existing.IsDeleted = false;
                existing.MarkedDate = DateTime.UtcNow;
                await _favoriteBookRepo.UpdateAsync(existing);
            }
            else
            {
                await _favoriteBookRepo.AddAsync(new FavoriteBook
                {
                    UserId = userId,
                    BookId = bookId,
                    MarkedDate = DateTime.UtcNow,
                    IsDeleted = false
                });
            }

            await _favoriteBookRepo.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(int userId, int bookId)
        {
            var existing = await _favoriteBookRepo.GetByUserAndBookAsync(userId, bookId);

            if (existing == null || existing.IsDeleted)
                throw new KeyNotFoundException("Không tìm thấy sách yêu thích.");

            existing.IsDeleted = true;
            await _favoriteBookRepo.UpdateAsync(existing);
            await _favoriteBookRepo.SaveChangesAsync();
        }
    }
}