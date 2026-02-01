using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos.Category;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Không tìm thấy thể loại." });
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create([FromBody] CategoryResponseDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _categoryService.CreateCategoryAsync(dto);
            if (result == null)
            {
                return BadRequest(new { message = "Tạo thất bại (có thể do tên trùng lặp)." });
            }

            return StatusCode(201, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _categoryService.UpdateCategoryAsync(id, dto);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy thể loại hoặc cập nhật thất bại." });
            }

            return Ok(new { message = "Cập nhật thành công." });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteCategoryAsync(id);
            if (!success)
            {
                return NotFound(new { message = "Không tìm thấy thể loại để xóa." });
            }

            return Ok(new { message = "Xóa thành công." });
        }
    }
}