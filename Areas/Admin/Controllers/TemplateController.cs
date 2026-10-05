using Microsoft.AspNetCore.Mvc;

namespace Duanbanhang.Areas.Admin.Controllers
{
    /// <summary>Các trang mẫu của giao diện Startmin (chỉ để hiển thị giao diện).</summary>
    public class TemplateController : AdminControllerBase
    {
        public IActionResult Flot() => View();
        public IActionResult Morris() => View();
        public IActionResult Tables() => View();
        public IActionResult Forms() => View();
        public IActionResult Panels() => View();
        public IActionResult Buttons() => View();
        public IActionResult Notifications() => View();
        public IActionResult Typography() => View();
        public IActionResult Icons() => View();
        public IActionResult Grid() => View();
        public IActionResult Blank() => View();
        public IActionResult LoginDemo() => View();
    }
}