using BLOOM.Business.Services;
using BLOOM.Business.Services.IServices;
using BLOOM.DataAccess.Data;
using BLOOM.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
});


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;                   //Must have at least one number(0-9)
    options.Password.RequireLowercase = true;               //Must have at least one lowercase letter(a-z)
    options.Password.RequireUppercase = true;               //Must have at least onel owercase letter(A-Z)
    options.Password.RequireNonAlphanumeric = true;         //Must have at least one Special Character(!@#$^%$)
    options.Password.RequiredLength = 6;                    //Must have minimum 6 character
    options.Password.RequiredUniqueChars = 1;              //Must have 1 unique character

}).AddDefaultTokenProviders()
    
    .AddEntityFrameworkStores<ApplicationDbContext>();   
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IShoppingCartServices, ShoppingCartService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly= true;
    options.Cookie.IsEssential= true;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/identity/account/login";
    options.LogoutPath = "/identity/account/logout";
    options.AccessDeniedPath = "/identity/account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "MyArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { area = "Customer" })
    .WithStaticAssets();



app.Run();
