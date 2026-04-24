using ComputerStore.MVC.Models; // Đảm bảo đúng namespace Models của bạn
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký MVC Controllers & Views
builder.Services.AddControllersWithViews();

// 2. Đăng ký DbContext kết nối SQL Server
builder.Services.AddDbContext<ComputerStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Cấu hình Đăng nhập bằng Cookie thuần túy (Không dùng Identity)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn bị văng về khi chưa đăng nhập
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi không có quyền
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Nhớ đăng nhập trong 7 ngày
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.Run();