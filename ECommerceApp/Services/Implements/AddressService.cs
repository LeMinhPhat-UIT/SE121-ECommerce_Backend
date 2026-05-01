using ECommerceApp.Commons;
using ECommerceApp.DTOs.AddressesDTOs;
using ECommerceApp.Mappings.Addresses;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services.Interfaces;

namespace ECommerceApp.Services.Implements;

public class AddressService(IUnitOfWork unitOfWork, IAddressMapper mapper) : IAddressService
{
    public async Task<ApiResponse<AddressResponse>> CreateAddressAsync(AddressCreateRequest addressDto)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(addressDto.CustomerId);
            if (customer == null)
            {
                return new ApiResponse<AddressResponse>(404, "Customer not found.");
            }

            var address = mapper.Map(addressDto);

            unitOfWork.AddressRepository.Add(address);
            await unitOfWork.SaveChangesAsync();

            var addressResponse = mapper.Map(address);

            return new ApiResponse<AddressResponse>(200, addressResponse);
        }
        catch (Exception ex)
        {
            return new ApiResponse<AddressResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AddressResponse>> GetAddressByIdAsync(int id)
    {
        try
        {
            var address = await unitOfWork.AddressRepository.GetByIdAsync(id);

            if (address == null)
            {
                return new ApiResponse<AddressResponse>(404, "Address not found.");
            }

            var addressResponse = mapper.Map(address);
            return new ApiResponse<AddressResponse>(200, addressResponse);
        }
        catch (Exception ex)
        {
            return new ApiResponse<AddressResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> UpdateAddressAsync(AddressUpdateRequest addressDto)
    {
        try
        {
            var address = await unitOfWork.AddressRepository.GetByIdAndCustomerIdAsync(
                addressDto.AddressId, 
                addressDto.CustomerId, 
                trackChanges: true);

            if (address == null)
            {
                return new ApiResponse<ConfirmationResponse>(404, "Address not found.");
            }

            address.AddressLine1 = addressDto.AddressLine1;
            address.AddressLine2 = addressDto.AddressLine2;
            address.City = addressDto.City;
            address.State = addressDto.State;
            address.PostalCode = addressDto.PostalCode;
            address.Country = addressDto.Country;

            await unitOfWork.SaveChangesAsync(); 

            var confirmationMessage = new ConfirmationResponse
            {
                Message = $"Address with Id {addressDto.AddressId} updated successfully."
            };

            return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
        }
        catch (Exception ex)
        {
            return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ConfirmationResponse>> DeleteAddressAsync(AddressDeleteRequest addressDeleteDto)
    {
        try
        {
            var address = await unitOfWork.AddressRepository.GetByIdAndCustomerIdAsync(
                addressDeleteDto.AddressId, 
                addressDeleteDto.CustomerId, 
                trackChanges: true);

            if (address == null)
            {
                return new ApiResponse<ConfirmationResponse>(404, "Address not found.");
            }

            unitOfWork.AddressRepository.Remove(address);
            await unitOfWork.SaveChangesAsync();
            
            var confirmationMessage = new ConfirmationResponse
            {
                Message = $"Address with Id {addressDeleteDto.AddressId} deleted successfully."
            };

            return new ApiResponse<ConfirmationResponse>(200, confirmationMessage);
        }
        catch (Exception ex)
        {
            return new ApiResponse<ConfirmationResponse>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PagedResult<AddressResponse>>> GetAddressesByCustomerAsync(int customerId, PaginationRequest paginationRequest)
    {
        try
        {
            var customer = await unitOfWork.CustomerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return new ApiResponse<PagedResult<AddressResponse>>(404, "Customer not found.");
            }

            var addresses = (await unitOfWork.AddressRepository.GetByCustomerIdAsync(customerId))
                .Select(mapper.Map)
                .ToPagedResult(paginationRequest);

            return new ApiResponse<PagedResult<AddressResponse>>(200, addresses);
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedResult<AddressResponse>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
        }
    }
}