using Duanbanhang.Data;
using Duanbanhang.Helpers;
using Duanbanhang.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Areas.Admin.Controllers
{
    public class ProductsController : AdminControllerBase
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Index(string q, int? categoryId, int page = 1)
        {
            var query = _db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(p => p.Name.Contains(q));
            }
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            ViewBag.Q = q;
            await LoadCategoriesAsync(categoryId);

            var paged = await PagedList<Product>.CreateAsync(query.OrderByDescending(p => p.Id), page, 10);
            return View(paged);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _db.Products.AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            ViewBag.OrderedQuantity = await _db.OrderDetails
                .Where(d => d.ProductId == id)
                .SumAsync(d => (int?)d.Quantity) ?? 0;

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new Product());
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,Price,CategoryId")] Product product, IFormFile imageFile)
        {
            var uploadError = ValidateImage(imageFile);
            if (uploadError != null) ModelState.AddModelError("imageFile", uploadError);

            if (product.CategoryId > 0 && !await _db.Categories.AnyAsync(c => c.Id == product.CategoryId))
                ModelState.AddModelError(nameof(Product.CategoryId), "Danh mục không tồn tại");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(product.CategoryId);
                return View(product);
            }

            if (imageFile != null && imageFile.Length > 0)
                product.ImageUrl = await SaveImageAsync(imageFile);

            product.Name = product.Name.Trim();
            product.CreatedAt = DateTime.Now;
            _db.Products.Add(product);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Đã thêm sản phẩm mới.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            await LoadCategoriesAsync(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            int id, [Bind("Id,Name,Description,Price,CategoryId")] Product model, IFormFile imageFile)
        {
            if (id != model.Id) return BadRequest();

            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            var uploadError = ValidateImage(imageFile);
            if (uploadError != null) ModelState.AddModelError("imageFile", uploadError);

            if (model.CategoryId > 0 && !await _db.Categories.AnyAsync(c => c.Id == model.CategoryId))
                ModelState.AddModelError(nameof(Product.CategoryId), "Danh mục không tồn tại");

            if (!ModelState.IsValid)
            {
                model.ImageUrl = product.ImageUrl; // để view vẫn hiển thị ảnh hiện tại
                await LoadCategoriesAsync(model.CategoryId);
                return View(model);
            }

            product.Name = model.Name.Trim();
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;

            if (imageFile != null && imageFile.Length > 0)
            {
                var oldImage = product.ImageUrl;
                product.ImageUrl = await SaveImageAsync(imageFile);
                await _db.SaveChangesAsync();
                await DeleteImageIfUnusedAsync(oldImage);
            }
            else
            {
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "Đã cập nhật sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _db.Products.AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound();

            var image = product.ImageUrl;
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            await DeleteImageIfUnusedAsync(image);

            TempData["Success"] = "Đã xóa sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        // ---------- Helpers ----------

        private async Task LoadCategoriesAsync(int? selected = null)
        {
            var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selected);
        }

        private static string ValidateImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return null; // ảnh là tùy chọn

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return "Chỉ chấp nhận file ảnh .jpg, .jpeg, .png, .gif, .webp";
            if (file.Length > MaxFileSize)
                return "Dung lượng ảnh tối đa 5 MB";
            return null;
        }

        /// <summary>Lưu file vào wwwroot/images với tên ngẫu nhiên, trả về tên file.</summary>
        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "images");
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid():N}{ext}";

            using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await file.CopyToAsync(stream);
            return fileName;
        }

        private async Task DeleteImageIfUnusedAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            if (await _db.Products.AnyAsync(p => p.ImageUrl == fileName)) return;

            // Path.GetFileName chặn path traversal
            var path = Path.Combine(_env.WebRootPath, "images", Path.GetFileName(fileName));
            if (System.IO.File.Exists(path))
            {
                try { System.IO.File.Delete(path); } catch (IOException) { /* bỏ qua nếu file đang bị khóa */ }
            }
        }
    }
}