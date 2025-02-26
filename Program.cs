using System.Net.Mail;
using BookStore.Models;
using BookStore.Services;
using BookStore.Services.Book;
using BookStore.Services.Jwt;
using BookStore.Services.Mail;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<Prn222BookshopContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<AuthService>();
builder.Services.AddTransient<MailService>();
builder.Services.AddSingleton<IJwtService, JwtService>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
