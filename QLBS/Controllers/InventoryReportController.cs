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
    public class InventoryReportController : ControllerBase
    {
        private readonly IInventoryReportService _service;

        public InventoryReportController(IInventoryReportService service)
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

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatest()
        {
            var result = await _service.GetLatestSummaryAsync();
            return Ok(result);
        }

        [HttpGet("low-stock")]
        public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 10)
        {
            if (threshold < 1)
                return BadRequest(new { message = "Ngưỡng tồn kho phải lớn hơn 0." });

            var result = await _service.GetLowStockAsync(threshold);
            return Ok(result);
        }

        [HttpGet("out-of-stock")]
        public async Task<IActionResult> GetOutOfStock()
        {
            var result = await _service.GetOutOfStockAsync();
            return Ok(result);
        }

        [HttpPost("snapshot")]
        public async Task<IActionResult> GenerateSnapshot()
        {
            var result = await _service.GenerateSnapshotAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InventoryReportCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.InventoryReportId }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryReportCreateDto dto)
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
