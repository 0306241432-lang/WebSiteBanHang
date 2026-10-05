using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Duanbanhang.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duanbanhang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;

        public AccountController(IConfiguration config) => _config = config;

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Dashboard");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var adminUser = _config["AdminAccount:Username"] ?? "";
            var adminPass = _config["AdminAccount:Password"] ?? "";

            if (!FixedEquals(model.Username, adminUser) | !FixedEquals(model.Password, adminPass))
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, adminUser),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = model.RememberMe });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // So sánh chuỗi thời gian hằng số (tránh timing attack)
        private static bool FixedEquals(string a, string b)
        {
            var ba = SHA256.HashData(Encoding.UTF8.GetBytes(a ?? ""));
            var bb = SHA256.HashData(Encoding.UTF8.GetBytes(b ?? ""));
            return CryptographicOperations.FixedTimeEquals(ba, bb);
        }
    }
}
