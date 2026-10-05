using Duanbanhang.Data;
using Duanbanhang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Areas.Admin.Controllers
{
    public class DashboardController : AdminControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            ViewBag.CategoryCount = await _db.Categories.CountAsync();
            ViewBag.ProductCount = await _db.Products.CountAsync();
            ViewBag.OrderCount = await _db.Orders.CountAsync();
            ViewBag.PendingCount = await _db.Orders.CountAsync(o => o.Status == OrderStatus.Pending);
            ViewBag.Revenue = await _db.Orders
                .Where(o => o.Status == OrderStatus.Completed)
                .SumAsync(o => o.TotalAmount);

            var latest = await _db.Orders.AsNoTracking()
                .OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id)
                .Take(5)
                .ToListAsync();
            return View(latest);
        }
    }
}
