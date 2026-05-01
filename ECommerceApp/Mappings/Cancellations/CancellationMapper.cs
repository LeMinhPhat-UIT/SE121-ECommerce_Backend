using ECommerceApp.DTOs.CancellationDTOs;
using ECommerceApp.Entities;
using Riok.Mapperly.Abstractions;

namespace ECommerceApp.Mappings.Cancellations
{
    public interface ICancellationMapper
    {
        CancellationResponse Map(Cancellation source);
    }

    [Mapper]
    public partial class CancellationMapper : ICancellationMapper
    {
        public partial CancellationResponse Map(Cancellation source);
    }
}