using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Duanbanhang.Models
{
    public enum OrderStatus
    {
        [Display(Name = "Chờ xử lý")] Pending = 0,
        [Display(Name = "Đang giao")] Shipping = 1,
        [Display(Name = "Hoàn thành")] Completed = 2,
        [Display(Name = "Đã hủy")] Cancelled = 3
    }

    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [StringLength(20)]
        [RegularExpression(@"^(0|\+84)\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0912345678)")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [StringLength(300, ErrorMessage = "Địa chỉ tối đa 300 ký tự")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string Address { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Ngày đặt")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();

        /// <summary>Các trạng thái được phép chuyển tới từ trạng thái hiện tại.</summary>
        public IEnumerable<OrderStatus> NextStatuses()
        {
            switch (Status)
            {
                case OrderStatus.Pending:
                    return new[] { OrderStatus.Shipping, OrderStatus.Cancelled };
                case OrderStatus.Shipping:
                    return new[] { OrderStatus.Completed, OrderStatus.Cancelled };
                default:
                    return Array.Empty<OrderStatus>();
            }
        }
    }
}
