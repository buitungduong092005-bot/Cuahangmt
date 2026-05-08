using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComputerStore.MVC.Models;

namespace ComputerStore.MVC.Controllers
{
    [Authorize(Roles = "Manager, Staff")]
    public class AdminController : Controller
    {
        private readonly ComputerStoreDbContext _context;
        public AdminController(ComputerStoreDbContext context) => _context = context;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var data = new
            {
                revenue = await _context.Orders.Where(o => o.Status != "Cancelled").SumAsync(o => (decimal?)o.TotalAmount) ?? 0,
                orders = await _context.Orders.CountAsync(),
                products = await _context.Products.CountAsync(),
                customers = await _context.AppUsers.CountAsync(u => u.Role == "Member")
            };
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetRevenueData()
        {
            var currentYear = DateTime.Now.Year;
            var rawData = await _context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Year == currentYear && o.Status != "Cancelled")
                .GroupBy(o => o.OrderDate.Value.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(o => o.TotalAmount) }).ToListAsync();

            var result = Enumerable.Range(1, 12).Select(m => new {
                label = $"Tháng {m}",
                value = rawData.FirstOrDefault(d => d.Month == m)?.Total ?? 0
            });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTopProducts()
        {
            var data = await _context.OrderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new {
                    productName = _context.Products.Where(p => p.ProductId == g.Key).Select(p => p.ProductName).FirstOrDefault() ?? "Ẩn",
                    quantity = g.Sum(od => od.Quantity)
                }).OrderByDescending(x => x.quantity).Take(5).ToListAsync();
            return Json(data);
        }
    }
}