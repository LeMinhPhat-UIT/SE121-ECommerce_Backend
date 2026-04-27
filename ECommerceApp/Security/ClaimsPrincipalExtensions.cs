using System.Security.Claims;

namespace ECommerceApp.Security
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetCustomerId(this ClaimsPrincipal principal)
        {
            var customerIdClaim = principal.FindFirstValue("customer_id") ?? principal.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(customerIdClaim, out var customerId) ? customerId : null;
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Admin");
        }
    }
}