using ECommerceApp.Data;
using ECommerceApp.Repositories.Implements;
using ECommerceApp.Repositories.Interfaces;
using ECommerceApp.Services;
using ECommerceApp.Services.Implements;
using ECommerceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerceApp.Mappings.Addresses;
using ECommerceApp.Mappings.Cancellations;
using ECommerceApp.Mappings.Carts;
using ECommerceApp.Mappings.Categories;
using ECommerceApp.Mappings.Customers;
using ECommerceApp.Mappings.Feedbacks;
using ECommerceApp.Mappings.Orders;
using ECommerceApp.Mappings.Payments;
using ECommerceApp.Mappings.Products;
using ECommerceApp.Mappings.Refunds;
using ECommerceApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("EFCoreDBConnection")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT configuration is missing.")))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IAddressMapper, AddressMapper>();
builder.Services.AddScoped<IProductMapper, ProductMapper>();
builder.Services.AddScoped<ICategoryMapper, CategoryMapper>();
builder.Services.AddScoped<ICustomerMapper, CustomerMapper>();
builder.Services.AddScoped<IOrderMapper, OrderMapper>();
builder.Services.AddScoped<ICartMapper, CartMapper>();
builder.Services.AddScoped<IFeedbackMapper, FeedbackMapper>();
builder.Services.AddScoped<ICancellationMapper, CancellationMapper>();
builder.Services.AddScoped<IRefundMapper, RefundMapper>();
builder.Services.AddScoped<IPaymentMapper, PaymentMapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseLoggingMiddleware();
app.UseErrorHandlingMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
