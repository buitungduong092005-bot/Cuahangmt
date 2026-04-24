using ComputerStore.MVC.Models; // Đảm bảo đúng namespace của bạn
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // Hàm này để hiển thị cái Form giao diện
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Hàm này chạy khi khách hàng bấm nút "Đăng nhập"
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password) // Sửa chữ 'email' thành 'username' ở đây
        {
            // Kiểm tra an toàn và loại bỏ khoảng trắng thừa 
            string inputUsername = string.IsNullOrEmpty(username) ? "" : username.Trim();
            string inputPassword = string.IsNullOrEmpty(password) ? "" : password.Trim();

            // 1. Tìm user trong Database theo Username
            var user = _context.AppUsers.FirstOrDefault(u => u.Username == inputUsername && u.PasswordHash == inputPassword);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("Username", user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không chính xác!";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        //Đăng ký
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string username, string password)
        {
            // 1. Kiểm tra xem username hoặc email đã tồn tại chưa
            var existingUser = _context.AppUsers.Any(u => u.Username == username || u.Email == email);
            if (existingUser)
            {
                ViewBag.Error = "Tên đăng nhập hoặc Email đã được sử dụng!";
                return View();
            }

            // 2. Tạo đối tượng người dùng mới
            var newUser = new AppUser
            {
                FullName = fullName,
                Email = email,
                Username = username,
                PasswordHash = password, // Lưu ý: thực tế nên mã hóa mật khẩu
                Role = "Member", // Mặc định là thành viên
                CreatedAt = DateTime.Now
            };

            // 3. Lưu vào Database
            _context.AppUsers.Add(newUser);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}