using Microsoft.AspNetCore.Mvc;
using QLBS.DTOs;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteBookController : ControllerBase
    {
        private readonly IFavoriteBookService _favoriteBookService;

        public FavoriteBookController(IFavoriteBookService favoriteBookService)
        {
            _favoriteBookService = favoriteBookService;
        }

        /// <summary>Lấy danh sách sách yêu thích của user</summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFavorites(int userId)
        {
            var result = await _favoriteBookService.GetFavoritesAsync(userId);
            return Ok(result);
        }

        /// <summary>Thêm sách vào yêu thích</summary>
        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteBookRequestDto dto)
        {
            await _favoriteBookService.AddFavoriteAsync(dto.UserId, dto.BookId);
            return Ok(new { message = "Thêm sách yêu thích thành công." });
        }

        /// <summary>Xóa mềm sách khỏi yêu thích</summary>
        [HttpDelete("{userId}/{bookId}")]
        public async Task<IActionResult> RemoveFavorite(int userId, int bookId)
        {
            await _favoriteBookService.RemoveFavoriteAsync(userId, bookId);
            return Ok(new { message = "Xóa sách yêu thích thành công." });
        }
    }
}