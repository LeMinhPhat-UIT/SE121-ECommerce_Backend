using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressesDTOs;
using ECommerceApp.Security;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AddressesController> _logger;

        public AddressesController(IAddressService addressService, ILogger<AddressesController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        // Creates a new address for a customer.
        [Authorize]
        [HttpPost("CreateAddress")]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> CreateAddress([FromBody] AddressCreateRequest addressDto)
        {
            _logger.LogInformation("CreateAddress request received for CustomerId={CustomerId}", addressDto.CustomerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != addressDto.CustomerId)
            {
                _logger.LogWarning("CreateAddress forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, addressDto.CustomerId);
                return Forbid();
            }

            var response = await _addressService.CreateAddressAsync(addressDto);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("CreateAddress failed for CustomerId={CustomerId} with StatusCode={StatusCode}", addressDto.CustomerId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("CreateAddress succeeded for CustomerId={CustomerId}", addressDto.CustomerId);
            return Ok(response);
        }

        // Retrieves an address by ID.
        [Authorize]
        [HttpGet("GetAddressById/{id}")]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> GetAddressById(int id)
        {
            _logger.LogInformation("GetAddressById request received for AddressId={AddressId}", id);
            var response = await _addressService.GetAddressByIdAsync(id);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetAddressById failed for AddressId={AddressId} with StatusCode={StatusCode}", id, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetAddressById succeeded for AddressId={AddressId}", id);
            return Ok(response);
        }

        // Updates an existing address.
        [Authorize]
        [HttpPut("UpdateAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateAddress([FromBody] AddressUpdateRequest addressDto)
        {
            _logger.LogInformation("UpdateAddress request received for AddressId={AddressId} CustomerId={CustomerId}", addressDto.AddressId, addressDto.CustomerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != addressDto.CustomerId)
            {
                _logger.LogWarning("UpdateAddress forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, addressDto.CustomerId);
                return Forbid();
            }

            var response = await _addressService.UpdateAddressAsync(addressDto);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("UpdateAddress failed for AddressId={AddressId} with StatusCode={StatusCode}", addressDto.AddressId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("UpdateAddress succeeded for AddressId={AddressId}", addressDto.AddressId);
            return Ok(response);
        }

        // Deletes an address by ID.
        [Authorize]
        [HttpDelete("DeleteAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteAddress([FromBody] AddressDeleteRequest addressDeleteDTO)
        {
            _logger.LogInformation("DeleteAddress request received for AddressId={AddressId} CustomerId={CustomerId}", addressDeleteDTO.AddressId, addressDeleteDTO.CustomerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != addressDeleteDTO.CustomerId)
            {
                _logger.LogWarning("DeleteAddress forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, addressDeleteDTO.CustomerId);
                return Forbid();
            }

            var response = await _addressService.DeleteAddressAsync(addressDeleteDTO);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("DeleteAddress failed for AddressId={AddressId} with StatusCode={StatusCode}", addressDeleteDTO.AddressId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("DeleteAddress succeeded for AddressId={AddressId}", addressDeleteDTO.AddressId);
            return Ok(response);
        }

        // Retrieves all addresses for a specific customer.
        [Authorize]
        [HttpGet("GetAddressesByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<PagedResult<AddressResponse>>>> GetAddressesByCustomer(int customerId, [FromQuery] PaginationRequest paginationRequest)
        {
            _logger.LogInformation("GetAddressesByCustomer request received for CustomerId={CustomerId}", customerId);
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                _logger.LogWarning("GetAddressesByCustomer forbidden for CurrentCustomerId={CurrentCustomerId} TargetCustomerId={TargetCustomerId}", currentCustomerId, customerId);
                return Forbid();
            }

            var response = await _addressService.GetAddressesByCustomerAsync(customerId, paginationRequest);
            if (response.StatusCode != 200)
            {
                _logger.LogWarning("GetAddressesByCustomer failed for CustomerId={CustomerId} with StatusCode={StatusCode}", customerId, response.StatusCode);
                return StatusCode(response.StatusCode, response);
            }

            _logger.LogInformation("GetAddressesByCustomer succeeded for CustomerId={CustomerId}", customerId);
            return Ok(response);
        }
    }
}