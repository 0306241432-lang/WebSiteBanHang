using System.Globalization;
using Duanbanhang.Data;
using Duanbanhang.Helpers;
using Duanbanhang.Models;
using Duanbanhang.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Areas.Admin.Controllers
{
    public class StatisticsController : AdminControllerBase
    {
        private readonly AppDbContext _db;

        public StatisticsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            // ---- Doanh thu 6 tháng gần nhất (chỉ tính đơn đã hoàn thành) ----
            var firstMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-5);

            var grouped = await _db.Orders.AsNoTracking()
                .Where(o => o.Status == OrderStatus.Completed && o.OrderDate >= firstMonth)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(x => x.TotalAmount),
                    Orders = g.Count()
                })
                .ToListAsync();

            var months = new List<MonthRevenueItem>();
            for (var i = 0; i < 6; i++)
            {
                var m = firstMonth.AddMonths(i);
                var found = grouped.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                months.Add(new MonthRevenueItem
                {
                    Label = m.ToString("MM/yyyy", CultureInfo.InvariantCulture),
                    Revenue = found == null ? 0 : found.Revenue,
                    Orders = found == null ? 0 : found.Orders
                });
            }

            // ---- Số đơn theo trạng thái ----
            var counts = await _db.Orders.AsNoTracking()
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Total = g.Count() })
                .ToListAsync();

            var statuses = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>()
                .Select(s =>
                {
                    var c = counts.FirstOrDefault(x => x.Status == s);
                    return new StatusCountItem { Label = s.GetDisplayName(), Value = c == null ? 0 : c.Total };
                })
                .ToList();

            // ---- Top 5 sản phẩm bán chạy (không tính đơn đã hủy) ----
            var top = await _db.OrderDetails.AsNoTracking()
                .Where(d => d.Order.Status != OrderStatus.Cancelled)
                .GroupBy(d => d.ProductName)
                .Select(g => new TopProductItem
                {
                    Name = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    Revenue = g.Sum(x => x.UnitPrice * x.Quantity)
                })
                .OrderByDescending(x => x.Quantity)
                .Take(5)
                .ToListAsync();

            var vm = new StatisticsViewModel
            {
                Months = months,
                Statuses = statuses,
                TopProducts = top,
                TotalOrders = await _db.Orders.CountAsync(),
                TotalRevenue = await _db.Orders
                    .Where(o => o.Status == OrderStatus.Completed)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0
            };
            return View(vm);
        }
    }
}