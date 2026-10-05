using Duanbanhang.Data;
using Duanbanhang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Areas.Admin.Controllers
{
    public class CategoriesController : AdminControllerBase
    {
        private readonly AppDbContext _db;

        public CategoriesController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var list = await _db.Categories.AsNoTracking()
                .Include(c => c.Products)
                .OrderBy(c => c.Name)
                .ToListAsync();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View(new Category());

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            if (!ModelState.IsValid) return View(category);

            if (await _db.Categories.AnyAsync(c => c.Name == category.Name.Trim()))
            {
                ModelState.AddModelError(nameof(Category.Name), "Tên danh mục đã tồn tại");
                return View(category);
            }

            category.Name = category.Name.Trim();
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã thêm danh mục mới.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var category = await _db.Categories.FindAsync(id);
            if (category == null) return NotFound();

            var name = model.Name.Trim();
            if (await _db.Categories.AnyAsync(c => c.Name == name && c.Id != id))
            {
                ModelState.AddModelError(nameof(Category.Name), "Tên danh mục đã tồn tại");
                return View(model);
            }

            category.Name = name;
            category.Description = model.Description;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã cập nhật danh mục.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _db.Categories.AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _db.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            if (category.Products.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm. Hãy xóa hoặc chuyển sản phẩm sang danh mục khác trước.";
                return RedirectToAction(nameof(Index));
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa danh mục.";
            return RedirectToAction(nameof(Index));
        }
    }
}
