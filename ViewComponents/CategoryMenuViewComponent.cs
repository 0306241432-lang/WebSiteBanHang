using Duanbanhang.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.ViewComponents
{
    /// <summary>Menu danh mục (dùng ở side menu và sidebar trang Shop).</summary>
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly AppDbContext _db;

        public CategoryMenuViewComponent(AppDbContext db) => _db = db;

        public async Task<IViewComponentResult> InvokeAsync(string menuId = "menu-content")
        {
            ViewBag.MenuId = menuId;
            int.TryParse(Request.Query["categoryId"], out var activeId);
            ViewBag.ActiveId = activeId;

            var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }
    }
}
