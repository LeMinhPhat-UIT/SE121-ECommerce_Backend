using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressDTOs;
using ECommerceApp.Entities;
using ECommerceApp.Mappings.Addresses;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Services.Implements
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAddressMapper _mapper;

        public AddressService(IAddressRepository addressRepository, ICustomerRepository customerRepository, IAddressMapper mapper)
        {
            _addressRepository = addressRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AddressResponse>> CreateAddressAsync(AddressCreateRequest addressDto)
        {
            try
            {
                // Check if customer exists
                var customer = await _customerRepository.GetByIdAsync(addressDto.CustomerId);
                if (customer == null)
                {
                    return new ApiResponse<AddressResponse>(404, "Customer not found.");
                }

                // Manual mapping from DTO to Model
                var address = _mapper.Map(addressDto);

                // Add address to the database
                await _addressRepository.AddAsync(address);

                // Map to AddressResponseDTO
                var addressResponse = _mapper.Map(address);

                return new ApiResponse<AddressResponse>(200, addressResponse);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<AddressResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int id, int currentCustomerId, bool isAdmin)
        {
            try
            {
                var address = await _addressRepository.GetByIdAsync(id);

                if (address == null)
                {
                    return new ApiResponse<AddressResponse>(404, "Address not found.");
                }

                if (!isAdmin && address.CustomerId != currentCustomerId)
                {
                    return new ApiResponse<AddressResponse>(403, "You do not have permission to access this address.");
                }

                // Map to AddressResponseDTO
                var addressResponse = _mapper.Map(address);

                return new ApiResponse<AddressResponse>(200, addressResponse);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<AddressResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> UpdateAddressAsync(AddressUpdateRequest addressDto)
        {
            try
            {
                var address = await _addressRepository.GetByIdAndCustomerIdAsync(addressDto.AddressId, addressDto.CustomerId);

                if (address == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Address not found.");
                }

                // Update address properties
                address.AddressLine1 = addressDto.AddressLine1;
                address.AddressLine2 = addressDto.AddressLine2;
                address.City = addressDto.City;
                address.State = addressDto.State;
                address.PostalCode = addressDto.PostalCode;
                address.Country = addressDto.Country;

                await _addressRepository.UpdateAsync(address);

                // Prepare confirmation message
                var confirmationMessage = new ConfirmationResponse
                {
                    Message = $"Address with Id {addressDto.AddressId} updated successfully."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponse>> DeleteAddressAsync(AddressDeleteRequest addressDeleteDTO)
        {
            try
            {
                var address = await _addressRepository.GetByIdAndCustomerIdAsync(addressDeleteDTO.AddressId, addressDeleteDTO.CustomerId);

                if (address == null)
                {
                    return new ApiResponse<ConfirmationResponse>(404, "Address not found.");
                }

                await _addressRepository.RemoveAsync(address);

                // Prepare confirmation message
                var confirmationMessage = new ConfirmationResponse
                {
                    Message = $"Address with Id {addressDeleteDTO.AddressId} deleted successfully."
                };

                return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<PagedResult<AddressResponse>>> GetAddressesByCustomerAsync(int customerId, PaginationRequest paginationRequest)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(customerId);

                if (customer == null)
                {
                    return new ApiResponse<PagedResult<AddressResponse>>(404, "Customer not found.");
                }

                var safeRequest = new PaginationRequest
                {
                    PageIndex = paginationRequest?.PageIndex > 0 ? paginationRequest.PageIndex : 1,
                    PageSize = paginationRequest?.PageSize > 0 ? paginationRequest.PageSize : 10
                };

                var addressQuery = _addressRepository.QueryByCustomerId(customerId);
                var totalCount = await addressQuery.CountAsync();
                var addresses = await addressQuery
                    .Skip((safeRequest.PageIndex - 1) * safeRequest.PageSize)
                    .Take(safeRequest.PageSize)
                    .ToListAsync();

                var addressList = new PagedResult<AddressResponse>(addresses.Select(_mapper.Map), safeRequest, totalCount);

                return new ApiResponse<PagedResult<AddressResponse>>(200, addressList);
            }
            catch (Exception ex)
            {
                // Log the exception
                return new ApiResponse<PagedResult<AddressResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }
    }
}