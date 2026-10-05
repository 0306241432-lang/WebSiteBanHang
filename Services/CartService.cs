using Duanbanhang.Data;
using Duanbanhang.Helpers;
using Duanbanhang.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Services
{
    /// <summary>Một dòng trong giỏ hàng, lưu trong Session (chỉ lưu Id + số lượng).</summary>
    public class CartEntry
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public interface ICartService
    {
        List<CartEntry> GetEntries();
        void Add(int productId, int quantity);
        void SetQuantity(int productId, int quantity);
        void Remove(int productId);
        void Clear();
        Task<CartViewModel> BuildAsync();
    }

    public class CartService : ICartService
    {
        private const string SessionKey = "Cart";
        public const int MaxQuantity = 99;

        private readonly IHttpContextAccessor _http;
        private readonly AppDbContext _db;

        public CartService(IHttpContextAccessor http, AppDbContext db)
        {
            _http = http;
            _db = db;
        }

        private ISession Session => _http.HttpContext.Session;

        public List<CartEntry> GetEntries() =>
            Session.GetJson<List<CartEntry>>(SessionKey) ?? new List<CartEntry>();

        private void Save(List<CartEntry> entries) => Session.SetJson(SessionKey, entries);

        public void Add(int productId, int quantity)
        {
            if (quantity < 1) quantity = 1;
            var entries = GetEntries();
            var existing = entries.FirstOrDefault(e => e.ProductId == productId);
            if (existing == null)
                entries.Add(new CartEntry { ProductId = productId, Quantity = Math.Min(quantity, MaxQuantity) });
            else
                existing.Quantity = Math.Min(existing.Quantity + quantity, MaxQuantity);
            Save(entries);
        }

        public void SetQuantity(int productId, int quantity)
        {
            var entries = GetEntries();
            var existing = entries.FirstOrDefault(e => e.ProductId == productId);
            if (existing == null) return;

            if (quantity < 1) entries.Remove(existing);
            else existing.Quantity = Math.Min(quantity, MaxQuantity);
            Save(entries);
        }

        public void Remove(int productId)
        {
            var entries = GetEntries();
            entries.RemoveAll(e => e.ProductId == productId);
            Save(entries);
        }

        public void Clear() => Session.Remove(SessionKey);

        public async Task<CartViewModel> BuildAsync()
        {
            var entries = GetEntries();
            var vm = new CartViewModel();
            if (entries.Count == 0) return vm;

            var ids = entries.Select(e => e.ProductId).ToList();
            var products = await _db.Products.AsNoTracking()
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();

            foreach (var entry in entries)
            {
                var p = products.FirstOrDefault(x => x.Id == entry.ProductId);
                if (p == null) continue; // sản phẩm đã bị xóa

                vm.Lines.Add(new CartLineViewModel
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    ImageUrl = p.ImageUrl,
                    Price = p.Price,       // luôn lấy giá mới nhất từ DB
                    Quantity = entry.Quantity
                });
            }

            // Dọn các sản phẩm không còn tồn tại khỏi session
            if (vm.Lines.Count != entries.Count)
                Save(entries.Where(e => vm.Lines.Any(l => l.ProductId == e.ProductId)).ToList());

            return vm;
        }
    }
}
