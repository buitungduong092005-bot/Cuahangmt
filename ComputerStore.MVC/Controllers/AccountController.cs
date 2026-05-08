using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using ComputerStore.MVC.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ComputerStore.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly ComputerStoreDbContext _context;

        // Bơm DbContext vào để gọi Database
        public AccountController(ComputerStoreDbContext context)
        {
            _context = context;
        }

        // Hàm này để hiển thị cái Form giao diện Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Hàm này chạy khi khách hàng bấm nút "Đăng nhập"
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Kiểm tra xem khách có nhập trống không
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ tài khoản và mật khẩu!");
                return View();
            }

            // Tìm user trong DB (Đã fix u.PasswordHash cho khớp với file AppUser.cs của cậu)
            var user = _context.AppUsers.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu!");
                return View();
            }

            // Kiểm tra xem tài khoản có bị khóa không
            if (user.IsLocked)
            {
                ModelState.AddModelError("", "Tài khoản này đã bị khóa do vi phạm chính sách!");
                return View();
            }

            // ==========================================
            // CẤP QUYỀN (NẠP ROLE VÀO COOKIE)
            // ==========================================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Manager")  // Role này phải là "Manager" hoặc "Staff" để vào được Admin
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Lưu Cookie vào trình duyệt và Đăng nhập thành công
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Đăng nhập xong thì đá về Trang chủ
            return RedirectToAction("Index", "Home");
        }

        // Hàm xử lý khi bấm nút Đăng xuất
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}