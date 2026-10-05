using Duanbanhang.Data;
using Duanbanhang.Helpers;
using Duanbanhang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Areas.Admin.Controllers
{
    public class OrdersController : AdminControllerBase
    {
        private readonly AppDbContext _db;

        public OrdersController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index(OrderStatus? status, string q, int page = 1)
        {
            var query = _db.Orders.AsNoTracking().AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                int.TryParse(q.TrimStart('#'), out var orderId);   // cho phép gõ "12" hoặc "#12"
                query = query.Where(o => o.CustomerName.Contains(q)
                                      || o.Phone.Contains(q)
                                      || (orderId > 0 && o.Id == orderId));
            }

            ViewBag.Status = status;
            ViewBag.Q = q;

            var paged = await PagedList<Order>.CreateAsync(
                query.OrderByDescending(o => o.OrderDate).ThenByDescending(o => o.Id), page, 10);
            return View(paged);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _db.Orders.AsNoTracking()
                .Include(o => o.Details)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(id);
            if (order == null) return NotFound();

            if (!order.NextStatuses().Contains(status))
            {
                TempData["Error"] = "Không thể chuyển đơn hàng sang trạng thái này.";
                return RedirectToAction(nameof(Details), new { id });
            }

            order.Status = status;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}