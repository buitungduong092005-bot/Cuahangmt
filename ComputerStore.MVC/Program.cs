using ComputerStore.MVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ComputerStore.MVC.Hubs; // Khai báo namespace chứa ChatHub

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký MVC Controllers & Views
builder.Services.AddControllersWithViews();

// 2. Đăng ký DbContext kết nối SQL Server
builder.Services.AddDbContext<ComputerStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Cấu hình Đăng nhập bằng Cookie thuần túy 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

// 4. Đăng ký dịch vụ SignalR
builder.Services.AddSignalR();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CỰC KỲ QUAN TRỌNG: Hai dòng này phải nằm theo đúng thứ tự
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Định tuyến cho ChatHub
app.MapHub<ChatHub>("/chatHub");

app.Run();