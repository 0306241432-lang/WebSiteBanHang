using Duanbanhang.Services;
using Microsoft.AspNetCore.Mvc;

namespace Duanbanhang.ViewComponents
{
    /// <summary>Giỏ hàng thu nhỏ trên header.</summary>
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ICartService _cart;

        public CartSummaryViewComponent(ICartService cart) => _cart = cart;

        public async Task<IViewComponentResult> InvokeAsync() => View(await _cart.BuildAsync());
    }
}
