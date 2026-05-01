using ECommerceApp.Data;
using ECommerceApp.Repositories.Interfaces;

namespace ECommerceApp.Repositories.Implements;

public class UnitOfWork(
        ApplicationDbContext context,
        IAddressRepository addressRepo,
        ICustomerRepository customerRepo,
        ICategoryRepository categoryRepo,
        IProductRepository productRepo,
        ICartRepository cartRepo,
        ICartItemRepository cartItemRepo
    ) : IUnitOfWork
{
    public IAddressRepository AddressRepository => addressRepo;
    public ICustomerRepository CustomerRepository => customerRepo;
    public ICategoryRepository CategoryRepository => categoryRepo;
    public IProductRepository ProductRepository => productRepo;
    public ICartRepository CartRepository => cartRepo;
    public ICartItemRepository CartItemRepository => cartItemRepo;
    
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