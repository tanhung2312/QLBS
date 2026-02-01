using System.ComponentModel.DataAnnotations;

namespace QLBS.Dtos.Author
{
    public class AuthorResponseDto
    {
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public int? BirthYear { get; set; }
        public string? Nationality { get; set; }
    }

    public class CreateAuthorDto
    {
        [Required(ErrorMessage = "Tên tác giả không được để trống")]
        [MaxLength(100, ErrorMessage = "Tên tác giả tối đa 100 ký tự")]
        public string AuthorName { get; set; } = string.Empty;

        [Range(1000, 9999, ErrorMessage = "Năm sinh không hợp lệ")]
        public int? BirthYear { get; set; }

        [MaxLength(100)]
        public string? Nationality { get; set; }
    }

    public class UpdateAuthorDto : CreateAuthorDto{}
}