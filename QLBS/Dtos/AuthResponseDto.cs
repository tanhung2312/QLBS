namespace QLBS.Dtos
{
    public class AuthResponseDto
    {
        public int UserId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } 
    }
}
