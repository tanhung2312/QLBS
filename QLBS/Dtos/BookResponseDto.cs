using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos.Book
{
    public class BookResponseDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int PublishYear { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreateBookDto
    {
        [Required]
        public string BookTitle { get; set; } = string.Empty;
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public int PublishYear { get; set; }
        [Required]
        public string Publisher { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class UpdateBookDto : CreateBookDto{}
}