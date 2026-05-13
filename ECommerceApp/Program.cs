using ECommerceApp.Data;
using ECommerceApp.Repositories.Implements;
using ECommerceApp.Repositories.Interfaces;
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
using Elasticsearch.Net;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Enrichers;
using System.Reflection;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Serilog:Properties:Application"] = builder.Configuration["SERILOG_APPLICATION"],
    ["Serilog:Properties:Server"] = builder.Configuration["SERILOG_SERVER"],
    ["Serilog:MinimumLevel:Default"] = builder.Configuration["SERILOG_MINIMUM_LEVEL"],
    ["Serilog:MinimumLevel:Override:Microsoft"] = builder.Configuration["SERILOG_MINIMUM_LEVEL_MICROSOFT"],
    ["Serilog:MinimumLevel:Override:System"] = builder.Configuration["SERILOG_MINIMUM_LEVEL_SYSTEM"]
});

var elasticUri = builder.Configuration["SERILOG_ELASTIC_URI"] ?? "http://elasticsearch:9200";
var elasticIndexFormat = builder.Configuration["SERILOG_ELASTIC_INDEX_FORMAT"] ?? "ecommerceapp-logs-{0:yyyy.MM}";
var autoRegisterTemplate = bool.TryParse(builder.Configuration["SERILOG_ELASTIC_AUTO_REGISTER_TEMPLATE"], out var parsedAutoRegisterTemplate) && parsedAutoRegisterTemplate;
var elasticUsername = builder.Configuration["SERILOG_ELASTIC_USERNAME"];
var elasticPassword = builder.Configuration["SERILOG_ELASTIC_PASSWORD"];
var elasticAuthHeader = CreateBasicAuthHeader(elasticUsername, elasticPassword);

builder.Host.UseSerilog((context, services, loggerConfiguration) =>
{
    var elasticSinkOptions = new ElasticsearchSinkOptions(new Uri(elasticUri))
    {
        AutoRegisterTemplate = autoRegisterTemplate,
        IndexFormat = elasticIndexFormat
    };

    if (!string.IsNullOrWhiteSpace(elasticAuthHeader))
    {
        elasticSinkOptions.ModifyConnectionSettings = connectionConfiguration => connectionConfiguration.GlobalHeaders(
            new System.Collections.Specialized.NameValueCollection
            {
                ["Authorization"] = elasticAuthHeader
            });
    }

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithEnvironmentName()
        .Enrich.WithProperty("Application", context.Configuration["Serilog:Properties:Application"] ?? "ECommerceApp")
        .Enrich.WithProperty("Server", context.Configuration["Serilog:Properties:Server"] ?? Environment.MachineName)
        .WriteTo.Elasticsearch(elasticSinkOptions);
});

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
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<ICancellationRepository, CancellationRepository>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IRefundRepository, RefundRepository>();

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICancellationService, CancellationService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IRefundService, RefundService>();

builder.Services.AddHostedService<PendingPaymentService>();
builder.Services.AddHostedService<RefundProcessingBackgroundService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IAddressMapper, AddressMapper>();
builder.Services.AddScoped<IProductMapper, ProductMapper>();
builder.Services.AddScoped<ICategoryMapper, CategoryMapper>();
builder.Services.AddScoped<ICustomerMapper, CustomerMapper>();
// TODO: These mappers support in-progress Order, Payment, Cancellation, Refund, and Feedback features.
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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static string? CreateBasicAuthHeader(string? userName, string? password)
{
    if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
    {
        return null;
    }

    var credentials = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{userName}:{password}"));
    return $"Basic {credentials}";
}
