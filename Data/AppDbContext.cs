using Duanbanhang.Models;
using Microsoft.EntityFrameworkCore;

namespace Duanbanhang.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Không cho xóa danh mục khi còn sản phẩm
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Xóa đơn hàng thì xóa luôn chi tiết đơn
            modelBuilder.Entity<OrderDetail>()
                .HasOne(d => d.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Xóa sản phẩm: giữ lại chi tiết đơn (ProductId = NULL)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
