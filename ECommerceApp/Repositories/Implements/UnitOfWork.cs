using ECommerceApp.Data;
using ECommerceApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerceApp.Repositories.Implements;

public class UnitOfWork(
        ApplicationDbContext context,
        IAddressRepository addressRepo,
        ICustomerRepository customerRepo,
        ICategoryRepository categoryRepo,
        IProductRepository productRepo,
        ICartRepository cartRepo,
        ICartItemRepository cartItemRepo,
        IOrderRepository orderRepo
    ) : IUnitOfWork
{
    public IAddressRepository AddressRepository => addressRepo;
    public ICustomerRepository CustomerRepository => customerRepo;
    public ICategoryRepository CategoryRepository => categoryRepo;
    public IProductRepository ProductRepository => productRepo;
    public ICartRepository CartRepository => cartRepo;
    public ICartItemRepository CartItemRepository => cartItemRepo;
    public IOrderRepository OrderRepository => orderRepo;
    
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
    
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await context.Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await context.Database.RollbackTransactionAsync();
    }
    
    public void Dispose()
    {
        context.Dispose();
        GC.SuppressFinalize(this);
    }
}