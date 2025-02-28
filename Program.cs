using System.Net.Mail;
using BookStore.Filters;
using BookStore.Models;
using BookStore.Services;
using BookStore.Services.Book;
using BookStore.Services.Jwt;
using BookStore.Services.Mail;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Prn222BookshopContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<MailService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<AdminFilter>();


builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// session config
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.LoginPath = "/Auth/Index";  // Trang đăng nhập
//        options.LogoutPath = "/Logout"; // Trang đăng xuất
//        options.AccessDeniedPath = "/Account/AccessDenied"; // Khi không đủ quyền
//    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
