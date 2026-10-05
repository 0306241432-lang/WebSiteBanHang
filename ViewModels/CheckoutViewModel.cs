using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Duanbanhang.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
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

        // Giỏ hàng luôn được đọc lại từ Session ở server, không lấy từ form
        [BindNever, ValidateNever]
        public CartViewModel Cart { get; set; } = new CartViewModel();
    }
}
