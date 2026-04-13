using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BestSellingBookReportController : ControllerBase
    {
        private readonly IBestSellingBookReportService _service;

        public BestSellingBookReportController(IBestSellingBookReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null) return NotFound(new { message = "Không tìm thấy báo cáo." });
            return Ok(result);
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTop(
            [FromQuery] int month,
            [FromQuery] int year,
            [FromQuery] int top = 10)
        {
            if (month < 1 || month > 12)
                return BadRequest(new { message = "Tháng không hợp lệ (1-12)." });
            if (top < 1 || top > 100)
                return BadRequest(new { message = "Số lượng top phải từ 1-100." });

            var result = await _service.GetTopAsync(month, year, top);
            return Ok(result);
        }

        [HttpGet("book/{bookId:int}")]
        public async Task<IActionResult> GetByBook(int bookId)
        {
            var result = await _service.GetByBookIdAsync(bookId);
            return Ok(result);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateMonthly(
            [FromQuery] int? month,
            [FromQuery] int? year)
        {
            var m = month ?? DateTime.Now.Month;
            var y = year ?? DateTime.Now.Year;

            if (m < 1 || m > 12)
                return BadRequest(new { message = "Tháng không hợp lệ (1-12)." });

            var result = await _service.GenerateMonthlyReportAsync(m, y);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BestSellingBookReportCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.BestSellingReportId }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] BestSellingBookReportCreateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result is null) return NotFound(new { message = "Không tìm thấy báo cáo." });
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound(new { message = "Không tìm thấy báo cáo." });
            return NoContent();
        }
    }

}
