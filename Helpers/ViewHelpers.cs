using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using Duanbanhang.Models;

namespace Duanbanhang.Helpers
{
    public static class ViewHelpers
    {
        private static readonly CultureInfo Vn = new CultureInfo("vi-VN");

        /// <summary>Định dạng tiền VNĐ: 1.250.000 đ</summary>
        public static string ToVnd(this decimal value) => string.Format(Vn, "{0:N0} đ", value);

        /// <summary>Đường dẫn ảnh sản phẩm (dùng với Url.Content).</summary>
        public static string ImagePath(string fileName) =>
            string.IsNullOrWhiteSpace(fileName) ? "~/images/no-image.svg" : "~/images/" + fileName;

        /// <summary>Lấy tên hiển thị từ [Display(Name = ...)] của enum.</summary>
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var attr = member?.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }

        /// <summary>Class badge Bootstrap 3 (trang Admin) theo trạng thái đơn.</summary>
        public static string BadgeClass(this OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Pending: return "label-warning";
                case OrderStatus.Shipping: return "label-info";
                case OrderStatus.Completed: return "label-success";
                case OrderStatus.Cancelled: return "label-danger";
                default: return "label-default";
            }
        }
    }
}
