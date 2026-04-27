namespace ECommerceApp.DTOs.FeedbackDTOs
{
    public class CustomerFeedback
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } // Combines FirstName and LastName
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
