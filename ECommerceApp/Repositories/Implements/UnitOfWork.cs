using ECommerceApp.Data;
using ECommerceApp.Repositories.Interfaces;

namespace ECommerceApp.Repositories.Implements;

public class UnitOfWork(
        ApplicationDbContext context,
        IAddressRepository addressRepo,
        ICustomerRepository customerRepo
    ) : IUnitOfWork
{
    public IAddressRepository AddressRepository => addressRepo;
    public ICustomerRepository CustomerRepository => customerRepo;
    
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
    
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}