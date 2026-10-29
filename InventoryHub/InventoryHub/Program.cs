using InventoryHub.Client.Pages;
using InventoryHub.Components;
using InventoryHub.Components.Account;
using InventoryHub.Data;
using InventoryHub.Models; 
using InventoryHub.Repositories;
using InventoryHub.Repository;
using InventoryHub.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//============================================
// 1. Serilog Setup
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("Logs/inventoryhub-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
//builder.Host.UseSerilog();

//Log.Information("Earl Wilkerson - Starting Blazor application host...");

// 2. Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

connectionString = builder.Configuration.GetConnectionString("AppDb") ?? throw new InvalidOperationException("Connection string 'AppDb' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();



// 3. Dependency Injection
//builder.Services.AddScoped<IProductApiRepository, ProductApiRepository>();
//builder.Services.AddScoped<IInventoryReportingApiRepository, InventoryReportingApiRepository>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<IInventoryReportingRepository, InventoryReportingRepository>();
builder.Services.AddScoped<IProductApiRepository, ProductJSONApiRepository>();
builder.Services.AddScoped<IInventoryReportingApiRepository, InventoryReportingJSONApiRepository>();
builder.Services.AddScoped<IProductRepository, ProductJSONRepository>();
builder.Services.AddScoped<IInventoryReportingRepository, InventoryReportingJSONRepository>();


builder.Services.AddScoped<IProductApiService, ProductApiService>();
builder.Services.AddScoped<IInventoryReportingApiService, InventoryReportingApiService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryReportingService, InventoryReportingService>();

// 4. Authentication (JWT)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddControllers();

builder.Services.AddHttpClient();

// Locate the HttpClient injection and change it from the custom domain:
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://inventoryhub.dev.localhost:7124") });

// To standard localhost:
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7124") });

//builder.Services.AddDefaultIdentity()
//    .AddRoles() // Enables native role support
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => {
    o.LoginPath = "/Account/Login"; o.AccessDeniedPath = "/Account/AccessDenied";
    o.Cookie.Name = "SafeVault.Auth"; o.Cookie.HttpOnly = true; o.Cookie.SameSite = SameSiteMode.Strict;
    o.Cookie.SecurePolicy = CookieSecurePolicy.Always; o.SlidingExpiration = true; o.ExpireTimeSpan = TimeSpan.FromMinutes(30);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole",
         policy => policy.RequireRole("Admin"));

    options.AddPolicy("RequireSuperUserRole",
         policy => policy.RequireRole("SuperUser"));
});

////Configure EF Core with an in-memory database
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//   options.UseInMemoryDatabase("AppDb"));


////Add Indentity services - 
//builder.Services.AddIdentityApiEndpoints<IdentityUser>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

//============================================


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(InventoryHub.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

//============================================
//app.UseSerilogRequestLogging();
//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//app.MapIdentityApi<IdentityUser>();
app.MapIdentityApi<ApplicationUser>();
//============================================


//================================================================
// Basic routes
//================================================================

app.MapGet("/hello", () => "Hey Earl!!!");


app.MapGet("/products", (IProductService productService) => productService.GetAllProducts());

app.MapGet("/api/minimal/products", (IProductService productService) => productService.GetAllProducts());

app.MapGet("/api/productlist", () =>
{
    return new[]
    {
        new
        {
            Id = 1,
            Name = "Laptop",
            Price = 1200.50,
            Stock = 25,
            Category = new { Id = 101, Name = "Electronics" }
        },
        new
        {
            Id = 2,
            Name = "Headphones",
            Price = 50.00,
            Stock = 100,
            Category = new { Id = 102, Name = "Accessories" }
        }
    };
});


//================================================================

app.Run();
