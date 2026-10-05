using Duanbanhang.Data;
using Duanbanhang.Models;
using Duanbanhang.Services;
using Duanbanhang.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Controllers
{
    public class CheckoutController : Controller
    {
        private const string LastOrderKey = "LastOrderId";

        private readonly ICartService _cart;
        private readonly AppDbContext _db;

        public CheckoutController(ICartService cart, AppDbContext db)
        {
            _cart = cart;
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await _cart.BuildAsync();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Giỏ hàng đang trống, hãy chọn sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
            return View(new CheckoutViewModel { Cart = cart });
        }

        [HttpPost]
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            // Luôn dựng lại giỏ hàng từ Session + DB (không tin dữ liệu từ form)
            var cart = await _cart.BuildAsync();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                model.Cart = cart;
                return View(model);
            }

            var order = new Order
            {
                CustomerName = model.CustomerName.Trim(),
                Phone = model.Phone.Trim(),
                Address = model.Address.Trim(),
                Note = string.IsNullOrWhiteSpace(model.Note) ? null : model.Note.Trim(),
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
                TotalAmount = cart.Total
            };

            foreach (var line in cart.Lines)
            {
                order.Details.Add(new OrderDetail
                {
                    ProductId = line.ProductId,
                    ProductName = line.Name,
                    UnitPrice = line.Price,
                    Quantity = line.Quantity
                });
            }

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(); // Order + OrderDetails được lưu trong cùng 1 transaction

            _cart.Clear();
            HttpContext.Session.SetInt32(LastOrderKey, order.Id);

            return RedirectToAction(nameof(Success), new { id = order.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Success(int id)
        {
            // Chỉ cho xem đơn vừa đặt trong phiên hiện tại
            if (HttpContext.Session.GetInt32(LastOrderKey) != id)
                return RedirectToAction("Index", "Home");

            var order = await _db.Orders.AsNoTracking()
                .Include(o => o.Details)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }
    }
}
