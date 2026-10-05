using Duanbanhang.Data;
using Duanbanhang.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var newest = await _db.Products.AsNoTracking()
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();

            var vm = new HomeViewModel
            {
                NewProducts = newest,
                Featured = newest.FirstOrDefault(),
                Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync()
            };
            return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View();
    }
}
