using Duanbanhang.Data;
using Duanbanhang.Helpers;
using Duanbanhang.Models;
using Duanbanhang.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Controllers
{
    public class ShopController : Controller
    {
        private const int PageSize = 9;
        private readonly AppDbContext _db;

        public ShopController(AppDbContext db) => _db = db;

        // GET /Shop?categoryId=1&q=ao&page=2
        public async Task<IActionResult> Index(int? categoryId, string q, int page = 1)
        {
            var query = _db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();
            string categoryName = null;

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                categoryName = await _db.Categories
                    .Where(c => c.Id == categoryId.Value)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
            }

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p => p.Name.Contains(q));
            }

            var paged = await PagedList<Product>.CreateAsync(
                query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id), page, PageSize);

            var vm = new ShopViewModel
            {
                CategoryId = categoryId,
                CategoryName = categoryName,
                Query = q,
                Products = paged.Items,
                Paging = paged,
                Recommended = await _db.Products.AsNoTracking()
                    .OrderByDescending(p => p.Id).Take(3).ToListAsync()
            };
            return View(vm);
        }

        // GET /Shop/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var vm = new ProductDetailsViewModel
            {
                Product = product,
                Related = await _db.Products.AsNoTracking()
                    .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)
                    .OrderByDescending(p => p.Id)
                    .Take(3)
                    .ToListAsync()
            };
            return View(vm);
        }
    }
}