using ECommerceApp.DTOs.FeedbackDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Feedbacks
{
    public interface IFeedbackMapper
    {
        FeedbackResponse MapFeedback(Feedback source);

        CustomerFeedback MapCustomerFeedback(Feedback source);
    }

    [Mapper]
    public partial class FeedbackMapper : IFeedbackMapper
    {
        public partial FeedbackResponse MapFeedback(Feedback source);

        public partial CustomerFeedback MapCustomerFeedback(Feedback source);
    }
}