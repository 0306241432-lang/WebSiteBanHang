using System.Globalization;
using Duanbanhang.Data;
using Duanbanhang.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---- Database (EF Core + SQL Server) ----
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---- MVC (bật chống CSRF cho mọi POST) ----
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

    // Thông báo lỗi binding bằng tiếng Việt (ví dụ: để trống ô giá bán)
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => "Vui lòng nhập giá trị");
    options.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(field => $"Thiếu giá trị cho {field}");
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((value, field) => $"Giá trị '{value}' không hợp lệ cho {field}");
    options.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(field => $"Giá trị không hợp lệ cho {field}");
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(value => $"Giá trị '{value}' không hợp lệ");
});

// ---- Session: lưu giỏ hàng ----
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Duanbanhang.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromDays(7);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICartService, CartService>();

// ---- Authentication đơn giản bằng Cookie cho trang Admin ----
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".Duanbanhang.Auth";
        options.Cookie.HttpOnly = true;
        options.LoginPath = "/Admin/Account/Login";
        options.AccessDeniedPath = "/Admin/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// ---- Tạo DB + dữ liệu mẫu khi chạy lần đầu ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Initialize(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Cố định culture Invariant để form số (giá tiền "199000.00") luôn được đọc đúng,
// bất kể Windows/máy chủ đang đặt ngôn ngữ nào. Việc hiển thị tiền VNĐ do ToVnd() xử lý riêng.
var invariant = CultureInfo.InvariantCulture;
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(invariant),
    SupportedCultures = new List<CultureInfo> { invariant },
    SupportedUICultures = new List<CultureInfo> { invariant }
});

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Khu vực Admin: /Admin/{controller}/{action}
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
