using System.ComponentModel.DataAnnotations;

namespace Duanbanhang.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
