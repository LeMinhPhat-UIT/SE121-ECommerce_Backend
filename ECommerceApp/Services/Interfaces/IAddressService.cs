using ECommerceApp.Commons;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.AddressDTOs;

namespace ECommerceApp.Services.Interfaces
{
    public interface IAddressService
    {
        Task<ApiResponse<AddressResponse>> CreateAddressAsync(AddressCreateRequest addressDto);
        Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int id, int currentCustomerId, bool isAdmin);
        Task<ApiResponse<ConfirmationResponse>> UpdateAddressAsync(AddressUpdateRequest addressDto);
        Task<ApiResponse<ConfirmationResponse>> DeleteAddressAsync(AddressDeleteRequest addressDeleteDTO);
        Task<ApiResponse<PagedResult<AddressResponse>>> GetAddressesByCustomerAsync(int customerId, PaginationRequest paginationRequest);
    }
}