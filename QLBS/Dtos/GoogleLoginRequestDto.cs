namespace QLBS.Dtos
{
    public class GoogleLoginRequestDto
    {
        public string IdToken { get; set; } = string.Empty;
    }

    public class GoogleUserInfoDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public bool EmailVerified { get; set; }
    }
}