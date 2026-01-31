namespace QLBS.Dtos
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
