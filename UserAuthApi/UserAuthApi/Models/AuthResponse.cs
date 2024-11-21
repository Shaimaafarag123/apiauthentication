namespace UserAuthApi.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }

        public int UserId { get; set; }

        public string TokenType { get; set; } = "Bearer";

        public DateTime Expiration { get; set; }
    }
}