using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLBS.Dtos.Review;
using QLBS.Services.Interfaces;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddReview([FromBody] AddReviewDto dto)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null || !int.TryParse(claim.Value, out int accountId)) return Unauthorized();

        var result = await _reviewService.AddReviewAsync(accountId, dto);

        if (result != "Success") return BadRequest(new { message = result });

        return Ok(new { message = "Đánh giá thành công!" });
    }
}