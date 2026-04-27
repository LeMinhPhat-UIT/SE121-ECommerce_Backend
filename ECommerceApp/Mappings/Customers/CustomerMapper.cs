using ECommerceApp.DTOs.CustomerDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Customers
{
    public interface ICustomerMapper
    {
        CustomerResponse Map(Customer source);

        Customer Map(CustomerRegistrationRequest source);
    }

    [Mapper]
    public partial class CustomerMapper : ICustomerMapper
    {
        public partial CustomerResponse Map(Customer source);

        public partial Customer Map(CustomerRegistrationRequest source);
    }
}