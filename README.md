# Duanbanhang – Website giới thiệu sản phẩm và đặt hàng

Dự án ASP.NET Core MVC (.NET 8) + Entity Framework Core + SQL Server.

- **Client (người dùng):** giao diện *Karl* – xem sản phẩm, lọc theo danh mục, tìm kiếm, giỏ hàng (Session), đặt hàng.
- **Admin (quản trị):** giao diện *Startmin* – quản lý danh mục, sản phẩm (upload ảnh), đơn hàng (đổi trạng thái).

## 1. Yêu cầu

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB đi kèm Visual Studio là đủ, hoặc SQL Server Express/Developer)
- Visual Studio 2022 hoặc VS Code

> Dùng .NET 9/10? Sửa `<TargetFramework>` trong `Duanbanhang.csproj` và nâng version 2 package `Microsoft.EntityFrameworkCore.*` tương ứng.

## 2. Cấu hình chuỗi kết nối (Connection String)

Mở `appsettings.json`, sửa mục `ConnectionStrings:DefaultConnection`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DuanbanhangDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Một số ví dụ khác:

| Trường hợp | Giá trị `Server=` |
|---|---|
| SQL Server Express | `.\\SQLEXPRESS` |
| SQL Server mặc định | `localhost` hoặc `.` |
| Đăng nhập bằng tài khoản SQL | thêm `User Id=sa;Password=...;` và bỏ `Trusted_Connection=True;` |

## 3. Cách chạy

```bash
cd Duanbanhang
dotnet restore
dotnet run
```

Mở trình duyệt theo địa chỉ in ra ở terminal (mặc định `https://localhost:7080` hoặc `http://localhost:5080`).

**Lần chạy đầu tiên** ứng dụng tự tạo database `DuanbanhangDb`, các bảng và dữ liệu mẫu (3 danh mục, 9 sản phẩm).

### Tạo database bằng script SQL (không bắt buộc)

Nếu muốn tạo DB thủ công, mở `database.sql` trong SSMS/Azure Data Studio và Execute. Script tạo DB, 4 bảng và dữ liệu mẫu, có thể chạy lại nhiều lần. Sau đó ứng dụng sẽ dùng luôn DB này.

### Dùng EF Core Migrations thay cho tự tạo DB (không bắt buộc)

Mặc định `Data/DbSeeder.cs` gọi `EnsureCreated()`. Nếu thầy cô yêu cầu `Update-Database`:

1. Trong `DbSeeder.cs` đổi `db.Database.EnsureCreated();` thành `db.Database.Migrate();`
2. Package Manager Console (hoặc terminal):
   ```bash
   dotnet tool install --global dotnet-ef      # chỉ cần cài 1 lần
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
3. Xóa database cũ trước nếu nó đã được tạo bằng `EnsureCreated()`.

## 4. Tài khoản quản trị

- Địa chỉ: `/Admin` (hoặc menu **Quản trị** trên website)
- Tên đăng nhập: `admin`
- Mật khẩu: `Admin@123`

Đổi tài khoản/mật khẩu ở mục `AdminAccount` trong `appsettings.json`.
Mọi trang `/Admin/...` (trừ trang đăng nhập) đều yêu cầu đăng nhập (Cookie Authentication).

## 5. Chức năng

**Người dùng**
- Trang chủ: slider, sản phẩm mới, lọc nhanh theo danh mục.
- `/Shop`: danh sách sản phẩm, lọc theo danh mục (`?categoryId=`), tìm theo tên (`?q=`).
- `/Shop/Details/{id}`: ảnh, giá, mô tả, sản phẩm liên quan.
- Giỏ hàng (lưu bằng **Session**, không mất khi tải lại trang): thêm từ trang danh sách/chi tiết, cập nhật số lượng, xóa dòng, xóa cả giỏ, tổng tiền. Giỏ hàng thu nhỏ hiển thị ở header.
- Đặt hàng: form Họ tên / Số điện thoại / Địa chỉ / Ghi chú → lưu `Order` + `OrderDetail` → trang thành công và làm sạch giỏ.

**Quản trị**
- Danh mục: thêm / xem / sửa / xóa (không cho xóa danh mục còn sản phẩm).
- Sản phẩm: thêm / xem / sửa / xóa, upload ảnh vào `wwwroot/images` (jpg, png, gif, webp, tối đa 5 MB).
- Đơn hàng: danh sách (lọc theo trạng thái), chi tiết, đổi trạng thái theo luồng
  `Chờ xử lý → Đang giao → Hoàn thành`, hoặc `→ Đã hủy`.
- Trang tổng quan: số liệu nhanh và đơn mới nhất.

**Validation (Data Annotations, server-side):** tên không để trống, giá trong khoảng cho phép, số điện thoại đúng định dạng (`0912345678` hoặc `+84912345678`), độ dài tối đa các trường, kiểm tra loại và dung lượng file ảnh.

## 6. Cấu trúc thư mục

```
Duanbanhang/
├── Areas/Admin/            # Phân hệ quản trị (Controllers + Views, layout Startmin)
├── Controllers/            # Home, Shop, Cart, Checkout (client)
├── Data/                   # AppDbContext, DbSeeder
├── Helpers/                # Session JSON, ToVnd(), tiện ích view
├── Models/                 # Category, Product, Order, OrderDetail (+ Data Annotations)
├── Services/               # CartService (giỏ hàng trong Session)
├── ViewComponents/         # CartSummary (header), CategoryMenu (menu danh mục)
├── ViewModels/             # Cart, Checkout, Login, Home/Shop
├── Views/                  # Razor Views client; Shared/_Layout, _Header, _Footer, _ProductCard
├── wwwroot/
│   ├── client/             # CSS/JS/fonts/ảnh của giao diện Karl
│   ├── admin/              # CSS/JS/fonts của giao diện Startmin
│   └── images/             # Ảnh sản phẩm (ảnh upload cũng lưu ở đây)
├── appsettings.json
├── database.sql
└── Program.cs
```

## 7. Lược đồ cơ sở dữ liệu

```
Category (1) ──< (N) Product (1) ──< (N) OrderDetail (N) >── (1) Order
```

- `OrderDetail` lưu thêm `ProductName`, `UnitPrice` tại thời điểm mua nên đơn cũ không đổi khi sản phẩm bị sửa/xóa.
- Xóa sản phẩm → `OrderDetail.ProductId` = NULL; xóa đơn → xóa luôn chi tiết đơn; không xóa được danh mục còn sản phẩm.

## 8. Trước khi nộp bài

Xóa thư mục `bin` và `obj` để giảm dung lượng (đã có `.gitignore`):

```bash
dotnet clean
rm -rf bin obj        # Windows PowerShell: Remove-Item -Recurse -Force bin, obj
```

## 9. Ghi chú bản quyền

Giao diện client dựa trên template **Karl** (Colorlib, CC BY 3.0 – giữ nguyên link ghi công ở footer),
giao diện admin dựa trên **Startmin** (MIT).
