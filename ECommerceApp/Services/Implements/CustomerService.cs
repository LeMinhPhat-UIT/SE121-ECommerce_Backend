using ECommerceApp.Commons;
using ECommerceApp.Data;
using ECommerceApp.DTOs.CustomerDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Customers;
using ECommerceApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerceApp.Services.Implements
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ICustomerMapper _mapper;

        public CustomerService(ApplicationDbContext context, IConfiguration configuration, ICustomerMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CustomerResponse>> RegisterCustomerAsync(CustomerRegistrationRequest customerDto)
        {
            try
            {
                if (await _context.Customers.AnyAsync(c => c.Email.ToLower() == customerDto.Email.ToLower()))
                {
                    return new ApiResponse<CustomerResponse>(400, "Email is already in use.");
                }

                var customer = _mapper.Map(customerDto);
                customer.IsActive = true;
                customer.Password = BCrypt.Net.BCrypt.HashPassword(customerDto.Password);

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                var customerResponse = _mapper.Map(customer);

                return new ApiResponse<CustomerResponse>(200, customerResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CustomerResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var customer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Email == request.Email);

                if (customer == null)
                {
                    return new ApiResponse<LoginResponse>(401, "Invalid email or password.");
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, customer.Password);
                if (!isPasswordValid)
                {
                    return new ApiResponse<LoginResponse>(401, "Invalid email or password.");
                }

                var role = IsAdminCustomer(customer.Email) ? "Admin" : "Customer";
                var expiresAtUtc = DateTime.UtcNow.AddMinutes(GetTokenLifetimeMinutes());

                var loginResponse = new LoginResponse
                {
                    Message = "Login successful.",
                    CustomerId = customer.Id,
                    CustomerName = $"{customer.FirstName} {customer.LastName}",
                    AccessToken = GenerateJwtToken(customer, role, expiresAtUtc),
                    ExpiresAtUtc = expiresAtUtc,
                    Role = role
                };

                return new ApiResponse<LoginResponse>(200, loginResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CustomerResponse>> GetCustomerByIdAsync(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id && c.IsActive == true);

                if (customer == null)
                {
                    return new ApiResponse<CustomerResponse>(404, "Customer not found.");
                }

                var customerResponse = _mapper.Map(customer);

                return new ApiResponse<CustomerResponse>(200, customerResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CustomerResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateCustomerAsync(CustomerUpdateRequest customerDto)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(customerDto.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Customer not found.");
                }

                if (customer.Email != customerDto.Email && await _context.Customers.AnyAsync(c => c.Email == customerDto.Email))
                {
                    return new ApiResponse<ConfirmationResponse>(400, "Email is already in use.");
                }

                customer.FirstName = customerDto.FirstName;
                customer.LastName = customerDto.LastName;
                customer.Email = customerDto.Email;
                customer.PhoneNumber = customerDto.PhoneNumber;
                customer.DateOfBirth = customerDto.DateOfBirth;

                await _context.SaveChangesAsync();

                var confirmationMessage = new ConfirmationResponse
                {
                    Message = $"Customer with Id {customerDto.CustomerId} updated successfully."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (customer == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Customer not found.");
                }

                customer.IsActive = false;
                await _context.SaveChangesAsync();

                var confirmationMessage = new ConfirmationResponse
                {
                    Message = $"Customer with Id {id} deleted successfully."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(request.CustomerId);
                if (customer == null || !customer.IsActive)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Customer not found or inactive.");
                }

                bool isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, customer.Password);
                if (!isCurrentPasswordValid)
                {
                    return new ApiResponse<ConfirmationResponse>(401, "Current password is incorrect.");
                }

                customer.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                await _context.SaveChangesAsync();

                var confirmationMessage = new ConfirmationResponse
                {
                    Message = "Password changed successfully."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        private int GetTokenLifetimeMinutes()
        {
            return int.TryParse(_configuration["Jwt:AccessTokenMinutes"], out var minutes) && minutes > 0 ? minutes : 480;
        }

        private bool IsAdminCustomer(string email)
        {
            var adminEmails = _configuration["Jwt:AdminEmails"];

            if (string.IsNullOrWhiteSpace(adminEmails))
            {
                return false;
            }

            return adminEmails
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Any(adminEmail => adminEmail.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        private string GenerateJwtToken(Customer customer, string role, DateTime expiresAtUtc)
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing.");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is missing.");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is missing.");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new("customer_id", customer.Id.ToString()),
                new(ClaimTypes.Name, $"{customer.FirstName} {customer.LastName}"),
                new(ClaimTypes.Email, customer.Email),
                new(ClaimTypes.Role, role)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}