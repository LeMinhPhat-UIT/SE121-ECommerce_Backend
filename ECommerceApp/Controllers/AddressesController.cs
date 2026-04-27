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

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        // Creates a new address for a customer.
        [Authorize]
        [HttpPost("CreateAddress")]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> CreateAddress([FromBody] AddressCreateRequest addressDto)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != addressDto.CustomerId)
            {
                return Forbid();
            }

            var response = await _addressService.CreateAddressAsync(addressDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves an address by ID.
        [Authorize]
        [HttpGet("GetAddressById/{id}")]
        public async Task<ActionResult<ApiResponse<AddressResponse>>> GetAddressById(int id)
        {
            var response = await _addressService.GetAddressByIdAsync(id);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Updates an existing address.
        [Authorize]
        [HttpPut("UpdateAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> UpdateAddress([FromBody] AddressUpdateRequest addressDto)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != addressDto.CustomerId)
            {
                return Forbid();
            }

            var response = await _addressService.UpdateAddressAsync(addressDto);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Deletes an address by ID.
        [Authorize]
        [HttpDelete("DeleteAddress")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponse>>> DeleteAddress([FromBody] AddressDeleteRequest addressDeleteDTO)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != addressDeleteDTO.CustomerId)
            {
                return Forbid();
            }

            var response = await _addressService.DeleteAddressAsync(addressDeleteDTO);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all addresses for a specific customer.
        [Authorize]
        [HttpGet("GetAddressesByCustomer/{customerId}")]
        public async Task<ActionResult<ApiResponse<PagedResult<AddressResponse>>>> GetAddressesByCustomer(int customerId, [FromQuery] PaginationRequest paginationRequest)
        {
            var currentCustomerId = User.GetCustomerId();

            if (!User.IsAdmin() && currentCustomerId != customerId)
            {
                return Forbid();
            }

            var response = await _addressService.GetAddressesByCustomerAsync(customerId, paginationRequest);
            if (response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}