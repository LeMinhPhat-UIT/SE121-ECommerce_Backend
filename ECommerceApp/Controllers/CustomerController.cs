using ECommerceApp.Commons;
using ECommerceApp.DTOs.CustomerDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("RegisterCustomer")]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> RegisterCustomer([FromBody] CustomerRegistrationRequest customerDto)
        {
            _logger.LogInformation("RegisterCustomer request received for Email={Email}", customerDto.Email);
            var response = await _customerService.RegisterCustomerAsync(customerDto);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("RegisterCustomer failed for Email={Email} with StatusCode={StatusCode}", customerDto.Email, response.StatusCode);
                return StatusCode((int)response.StatusCode, response);
            }

            _logger.LogInformation("RegisterCustomer succeeded for Email={Email}", customerDto.Email);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login request received for Email={Email}", request.Email);
            var response = await _customerService.LoginAsync(request);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("Login failed for Email={Email} with StatusCode={StatusCode}", request.Email, response.StatusCode);
                return StatusCode((int)response.StatusCode, response);
            }

            _logger.LogInformation("Login succeeded for Email={Email}", request.Email);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("GetCustomerById/{id}")]
        public async Task<ActionResult<ApiResponse<CustomerResponse>>> GetCustomerById(int id)
        {
            _logger.LogInformation("GetCustomerById request received for CustomerId={CustomerId}", id);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != id)
            {
                _logger.LogWarning("GetCustomerById forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, id);
                return Forbid();
            }

            var response = await _customerService.GetCustomerByIdAsync(id);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetCustomerById failed for CustomerId={CustomerId} with StatusCode={StatusCode}", id, response.StatusCode);
                return StatusCode((int)response.StatusCode, response);
            }

            _logger.LogInformation("GetCustomerById succeeded for CustomerId={CustomerId}", id);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("UpdateCustomer")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateCustomer([FromBody] CustomerUpdateRequest request)
        {
            _logger.LogInformation("UpdateCustomer request received for CustomerId={CustomerId}", request.CustomerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != request.CustomerId)
            {
                _logger.LogWarning("UpdateCustomer forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, request.CustomerId);
                return Forbid();
            }

            var response = await _customerService.UpdateCustomerAsync(request);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("UpdateCustomer failed for CustomerId={CustomerId} with StatusCode={StatusCode}", request.CustomerId, response.StatusCode);
                return StatusCode((int)response.StatusCode, response);
            }

            _logger.LogInformation("UpdateCustomer succeeded for CustomerId={CustomerId}", request.CustomerId);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("DeleteCustomer/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteCustomer(int id)
        {
            _logger.LogInformation("DeleteCustomer request received for CustomerId={CustomerId}", id);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != id)
            {
                _logger.LogWarning("DeleteCustomer forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, id);
                return Forbid();
            }

            var response = await _customerService.DeleteCustomerAsync(id);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("DeleteCustomer failed for CustomerId={CustomerId} with StatusCode={StatusCode}", id, response.StatusCode);
                return StatusCode((int)response.StatusCode, response);
            }

            _logger.LogInformation("DeleteCustomer succeeded for CustomerId={CustomerId}", id);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            _logger.LogInformation("ChangePassword request received for CustomerId={CustomerId}", request.CustomerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != request.CustomerId)
            {
                _logger.LogWarning("ChangePassword forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, request.CustomerId);
                return Forbid();
            }

            var response = await _customerService.ChangePasswordAsync(request);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("ChangePassword failed for CustomerId={CustomerId} with StatusCode={StatusCode}", request.CustomerId, response.StatusCode);
                return StatusCode((int)response.StatusCode, response);
            }

            _logger.LogInformation("ChangePassword succeeded for CustomerId={CustomerId}", request.CustomerId);
            return Ok(response);
        }
    }
}