using Duanbanhang.Data;
using Duanbanhang.Services;
using Duanbanhang.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cart;
        private readonly AppDbContext _db;

        public CartController(ICartService cart, AppDbContext db)
        {
            _cart = cart;
            _db = db;
        }

        public async Task<IActionResult> Index() => View(await _cart.BuildAsync());

        // POST /Cart/Add  (từ trang danh sách hoặc trang chi tiết)
        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1, string returnUrl = null)
        {
            var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return NotFound();

            _cart.Add(productId, quantity);
            TempData["Success"] = $"Đã thêm \"{product.Name}\" vào giỏ hàng.";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction(nameof(Index));
        }

        // POST /Cart/UpdateAll : cập nhật số lượng tất cả dòng trong giỏ
        [HttpPost]
        public IActionResult UpdateAll(List<CartUpdateItem> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                    _cart.SetQuantity(item.ProductId, item.Quantity);
            }
            TempData["Success"] = "Đã cập nhật giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int productId)
        {
            _cart.Remove(productId);
            TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cart.Clear();
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng.";
            return RedirectToAction(nameof(Index));
        }
    }
}
