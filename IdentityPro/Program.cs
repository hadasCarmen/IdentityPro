using IdentityPro.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IceCreamShopGateway.Services;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Ice_cream_shopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Ice_cream_shopContext") ?? throw new InvalidOperationException("Connection string 'Ice_cream_shopContext' not found.")));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient();/////////////

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Home}/{id?}");
app.MapRazorPages();

app.Run();

using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;

//var gatewayAddressService = serviceProvider.GetRequiredService<AddressService>();

// Configure base URL for HttpClient using gateway address
//var httpClient = new HttpClient();
//var addressExists = await gatewayAddressService.CheckAddressExistence("ירושלים", "דגל ראובן");
//if (addressExists == true)
//{
//    var uri = new Uri(gatewayAddressService.GetGatewayAddress());
//    httpClient.BaseAddress = uri;
//}

// Resolve your HttpClient from the service provider
//var httpClient = serviceProvider.GetRequiredService<HttpClient>();

// Configure the HTTP request pipeline.
