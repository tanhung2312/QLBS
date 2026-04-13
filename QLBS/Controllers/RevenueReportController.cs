using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos;
using QLBS.Services.Interfaces;

namespace QLBS.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RevenueReportController : ControllerBase
    {
        private readonly IRevenueReportService _service;

        public RevenueReportController(IRevenueReportService service)
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

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (from > to)
                return BadRequest(new { message = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc." });

            var result = await _service.GetSummaryAsync(from, to);
            return Ok(result);
        }

        [HttpGet("summary/month")]
        public async Task<IActionResult> GetMonthSummary(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            if (month < 1 || month > 12)
                return BadRequest(new { message = "Tháng không hợp lệ (1-12)." });
            if (year < 2000 || year > DateTime.Now.Year + 1)
                return BadRequest(new { message = "Năm không hợp lệ." });

            var result = await _service.GetMonthSummaryAsync(month, year);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RevenueReportCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.RevenueReportId }, result);
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDaily([FromQuery] DateTime? date)
        {
            var targetDate = date ?? DateTime.Today;
            var result = await _service.GenerateDailyReportAsync(targetDate);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RevenueReportCreateDto dto)
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


    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportDashboardController : ControllerBase
    {
        private readonly IReportDashboardService _service;

        public ReportDashboardController(IReportDashboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _service.GetDashboardAsync();
            return Ok(result);
        }
    }
}