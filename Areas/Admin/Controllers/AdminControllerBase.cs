using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duanbanhang.Areas.Admin.Controllers
{
    /// <summary>Controller cơ sở: mọi trang quản trị đều yêu cầu đăng nhập.</summary>
    [Area("Admin")]
    [Authorize]
    public abstract class AdminControllerBase : Controller
    {
    }
}
