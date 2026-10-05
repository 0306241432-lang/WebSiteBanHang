using Duanbanhang.Models;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Data
{
    public static class DbSeeder
    {
        public static void Initialize(AppDbContext db)
        {
            // Tạo database + bảng nếu chưa có (không cần chạy migration)
            db.Database.EnsureCreated();

            if (db.Categories.Any()) return;

            var ao = new Category { Name = "Áo nữ", Description = "Áo thun, áo sweater và các mẫu áo thời trang dành cho nữ" };
            var khoac = new Category { Name = "Áo khoác & Blazer", Description = "Áo khoác, cardigan, blazer cho cả nam và nữ" };
            var dam = new Category { Name = "Đầm & Jumpsuit", Description = "Đầm dài, đầm quấn và jumpsuit" };
            db.Categories.AddRange(ao, khoac, dam);

            db.Products.AddRange(
                new Product { Name = "Áo thun đen in họa tiết", Price = 199000, ImageUrl = "product-2.jpg", Category = ao,
                    Description = "Áo thun cotton form rộng, in họa tiết cá tính. Dễ phối cùng quần jeans hoặc chân váy." },
                new Product { Name = "Áo sweater trắng Homegirl", Price = 279000, ImageUrl = "product-3.jpg", Category = ao,
                    Description = "Áo sweater nỉ mềm, ấm, phù hợp đi học và đi chơi. Chất vải dày dặn, ít xù lông." },
                new Product { Name = "Blazer trắng nữ", Price = 650000, ImageUrl = "product-1.jpg", Category = khoac,
                    Description = "Blazer trắng dáng suông thanh lịch, phù hợp đi làm và dự tiệc." },
                new Product { Name = "Cardigan be dáng dài", Price = 459000, ImageUrl = "product-4.jpg", Category = khoac,
                    Description = "Cardigan dáng dài màu be nhẹ nhàng, chất liệu mềm mại, giữ ấm tốt." },
                new Product { Name = "Blazer xám nam", Price = 790000, ImageUrl = "product-6.jpg", Category = khoac,
                    Description = "Blazer nam màu xám xanh, phom ôm vừa vặn, phối được với cả áo thun và sơ mi." },
                new Product { Name = "Áo khoác dạ hồng", Price = 899000, ImageUrl = "product-8.jpg", Category = khoac,
                    Description = "Áo khoác dạ màu hồng pastel, dáng dài, phù hợp thời tiết se lạnh." },
                new Product { Name = "Jumpsuit xanh ngọc", Price = 520000, ImageUrl = "product-5.jpg", Category = dam,
                    Description = "Jumpsuit cổ V màu xanh ngọc nổi bật, chất liệu satin mềm, tôn dáng." },
                new Product { Name = "Đầm quấn cam đất", Price = 590000, ImageUrl = "product-7.jpg", Category = dam,
                    Description = "Đầm quấn xẻ tà màu cam đất, chất liệu mát, phù hợp đi biển và dạo phố." },
                new Product { Name = "Đầm dài vàng", Price = 620000, ImageUrl = "product-9.jpg", Category = dam,
                    Description = "Đầm dài màu vàng tươi sáng, thiết kế nhẹ nhàng, thích hợp đi chơi và chụp ảnh." }
            );

            db.SaveChanges();
        }
    }
}
