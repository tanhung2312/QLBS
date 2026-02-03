using System;
using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos.Review
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }

    public class AddReviewDto
    {
        [Required] public int BookId { get; set; }
        [Required][Range(1, 5)] public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}