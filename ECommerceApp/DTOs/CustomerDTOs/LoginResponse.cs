namespace ECommerceApp.DTOs.CustomerDTOs
{
    public class LoginResponse
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public string Role { get; set; } = null!;
    }
}