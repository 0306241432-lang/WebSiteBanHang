using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duanbanhang.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(1000, 1000000000, ErrorMessage = "Giá bán phải từ 1.000 đến 1.000.000.000")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá bán (VNĐ)")]
        public decimal Price { get; set; }

        /// <summary>Tên file ảnh, lưu trong wwwroot/images</summary>
        [StringLength(260)]
        [Display(Name = "Hình ảnh")]
        public string ImageUrl { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
