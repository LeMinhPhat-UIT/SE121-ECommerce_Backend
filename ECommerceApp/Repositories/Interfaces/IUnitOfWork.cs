namespace ECommerceApp.Repositories.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IAddressRepository AddressRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    
    Task<int> SaveChangesAsync();
}