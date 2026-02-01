using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos.Book;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _bookService.GetAllBooksAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy sách" });
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromForm] CreateBookDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _bookService.CreateBookAsync(dto);
            if (result == null) return BadRequest(new { message = "Thêm thất bại hoặc sách đã tồn tại" });

            return StatusCode(201, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateBookDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _bookService.UpdateBookAsync(id, dto);
            if (!result) return NotFound(new { message = "Cập nhật thất bại hoặc không tìm thấy sách" });

            return Ok(new { message = "Cập nhật thành công" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            if (!result) return NotFound(new { message = "Xóa thất bại hoặc không tìm thấy sách" });

            return Ok(new { message = "Xóa sách thành công" });
        }
    }
}