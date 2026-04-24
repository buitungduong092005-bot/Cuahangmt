using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerStore.MVC.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerStore.MVC.Controllers
{
    // Cứ là Manager hoặc Staff thì mới được vào khu vực Admin này
    [Authorize(Roles = "Manager, Staff")]
    public class AdminController : Controller
    {
        private readonly ComputerStoreDbContext _context;

        // Tiêm DbContext vào đây để có thể gọi xuống Database
        public AdminController(ComputerStoreDbContext context)
        {
            _context = context;
        }

        // Trang chủ Dashboard - Nơi hiển thị biểu đồ và danh sách
        public IActionResult Index()
        {
            return View();
        }

        // ==========================================
        // 1. QUẢN LÝ NGƯỜI DÙNG (APIs)
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> LockAccount(int userId)
        {
            var user = await _context.AppUsers.FindAsync(userId);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            // Không cho phép khóa tài khoản của Quản lý
            if (user.Role == "Manager")
                return BadRequest("Không thể khóa tài khoản của Quản lý!");

            user.IsLocked = true; // Bật cờ khóa
            await _context.SaveChangesAsync(); // Lưu xuống DB

            return Json(new { success = true, message = "Đã khóa tài khoản thành công!" });
        }

        [HttpPost]
        public async Task<IActionResult> UnlockAccount(int userId)
        {
            var user = await _context.AppUsers.FindAsync(userId);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            user.IsLocked = false; // Tắt cờ khóa
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã mở khóa tài khoản!" });
        }

        [HttpPost]
        // Chỉ riêng Quản lý (Manager) mới có quyền đi phong tước cho Staff
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignStaffRole(int userId)
        {
            var user = await _context.AppUsers.FindAsync(userId);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            if (user.Role == "Manager")
                return BadRequest("Người này đã là Quản lý rồi!");

            user.Role = "Staff";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã cấp quyền Nhân viên thành công!" });
        }

        // ==========================================
        // 2. THỐNG KÊ DOANH THU & SẢN PHẨM (LINQ)
        // ==========================================

        // Lấy dữ liệu doanh thu 12 tháng (Đã vá lỗi DateTime?)
        [HttpGet]
        public async Task<IActionResult> GetRevenueData()
        {
            var currentYear = DateTime.Now.Year;

            var data = await _context.Orders
                // Đã thêm HasValue và .Value ở đây để tránh lỗi Null
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == currentYear && o.Status != "Cancelled")
                .GroupBy(o => o.OrderDate.Value.Month)
                .Select(g => new {
                    Month = g.Key,
                    Total = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            // Đổ dữ liệu ra mảng 12 phần tử để vẽ biểu đồ
            var result = Enumerable.Range(1, 12).Select(m => new {
                label = $"Tháng {m}",
                value = data.FirstOrDefault(d => d.Month == m)?.Total ?? 0
            });

            return Json(result);
        }

        // Lấy Top 5 sản phẩm bán chạy
        [HttpGet]
        public async Task<IActionResult> GetTopProducts()
        {
            var topProducts = await _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new {
                    ProductName = _context.Products.FirstOrDefault(p => p.ProductId == g.Key).ProductName,
                    Quantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToListAsync();

            return Json(topProducts);
        }

        // Thống kê tổng hợp (Dùng cho các thẻ Dashboard)
        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var totalOrders = await _context.Orders.CountAsync();
            var totalRevenue = await _context.Orders.Where(o => o.Status != "Cancelled").SumAsync(o => o.TotalAmount);
            var totalCustomers = await _context.AppUsers.Where(u => u.Role == "Member").CountAsync();
            var totalProducts = await _context.Products.CountAsync();

            return Json(new
            {
                Orders = totalOrders,
                Revenue = totalRevenue,
                Customers = totalCustomers,
                Products = totalProducts
            });
        }
    }
}