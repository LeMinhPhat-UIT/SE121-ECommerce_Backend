using ECommerceApp.DTOs.AddressesDTOs;
using ECommerceApp.Entites;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Addresses
{
    public interface IAddressMapper
    {
        AddressResponse Map(Address source);

        Address Map(AddressCreateRequest source);

        void Map(AddressUpdateRequest source, Address destination);
    }

    [Mapper]
    public partial class AddressMapper : IAddressMapper
    {
        public partial AddressResponse Map(Address source);

        public partial Address Map(AddressCreateRequest source);

        public partial void Map(AddressUpdateRequest source, Address destination);
    }
}