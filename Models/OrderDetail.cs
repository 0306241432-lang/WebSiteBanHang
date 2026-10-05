using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duanbanhang.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        /// <summary>Null nếu sản phẩm đã bị xóa khỏi hệ thống (vẫn giữ lại ProductName/UnitPrice của đơn).</summary>
        public int? ProductId { get; set; }
        public Product Product { get; set; }

        [Required, StringLength(200)]
        public string ProductName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [NotMapped]
        public decimal LineTotal => UnitPrice * Quantity;
    }
}
