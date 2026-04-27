using System.Text.Json.Serialization;

namespace ECommerceApp.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RefundStatus
    {
        Pending = 1,
        Completed = 6,
        Failed = 7
    }
}