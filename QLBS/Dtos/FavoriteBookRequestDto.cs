namespace QLBS.DTOs
{
    public class FavoriteBookRequestDto
    {
        public int UserId { get; set; }
        public int BookId { get; set; }
    }

    public class FavoriteBookResponseDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime MarkedDate { get; set; }
    }

    public class FavoriteBookListResponseDto
    {
        public int TotalCount { get; set; }
        public IEnumerable<FavoriteBookResponseDto> Items { get; set; }
    }
}