using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos.Author;
using QLBS.Services.Implementations;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null)
            {
                return NotFound(new { message = "Không tìm thấy tác giả." });
            }
            return Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authorService.CreateAuthorAsync(dto);
            if (result == null)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo tác giả." });
            }
            return StatusCode(201, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _authorService.UpdateAuthorAsync(id, dto);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy tác giả hoặc cập nhật thất bại." });
            }
            return Ok(new { message = "Cập nhật thành công." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _authorService.DeleteAuthorAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy tác giả để xóa." });
            }
            return Ok(new { message = "Xóa thành công." });
        }
    }
}