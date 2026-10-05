/* =====================================================================
   DUANBANHANG - Script khởi tạo cơ sở dữ liệu + dữ liệu mẫu (SQL Server)
   ---------------------------------------------------------------------
   Cách dùng: mở file trong SSMS / Azure Data Studio rồi bấm Execute.
   Lưu ý: ứng dụng cũng TỰ tạo DB + dữ liệu mẫu khi chạy lần đầu
   (xem Data/DbSeeder.cs), file này dùng khi bạn muốn tạo DB thủ công.
   Script có thể chạy lại nhiều lần (không tạo trùng).
   ===================================================================== */

IF DB_ID(N'DuanbanhangDb') IS NULL
    CREATE DATABASE DuanbanhangDb;
GO

USE DuanbanhangDb;
GO

/* ------------------------------ Categories ------------------------------ */
IF OBJECT_ID(N'dbo.Categories', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categories (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL
    );
END
GO

/* ------------------------------ Products -------------------------------- */
IF OBJECT_ID(N'dbo.Products', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Products (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
        Name        NVARCHAR(200)  NOT NULL,
        Description NVARCHAR(MAX)  NULL,
        Price       DECIMAL(18,2)  NOT NULL,
        ImageUrl    NVARCHAR(260)  NULL,            -- tên file trong wwwroot/images
        CreatedAt   DATETIME2      NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT SYSDATETIME(),
        CategoryId  INT            NOT NULL,
        CONSTRAINT FK_Products_Categories_CategoryId FOREIGN KEY (CategoryId)
            REFERENCES dbo.Categories (Id) ON DELETE NO ACTION   -- không xóa danh mục còn sản phẩm
    );
    CREATE INDEX IX_Products_CategoryId ON dbo.Products (CategoryId);
END
GO

/* ------------------------------- Orders --------------------------------- */
IF OBJECT_ID(N'dbo.Orders', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Orders (
        Id           INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
        CustomerName NVARCHAR(100) NOT NULL,
        Phone        NVARCHAR(20)  NOT NULL,
        Address      NVARCHAR(300) NOT NULL,
        Note         NVARCHAR(500) NULL,
        OrderDate    DATETIME2     NOT NULL CONSTRAINT DF_Orders_OrderDate DEFAULT SYSDATETIME(),
        Status       INT           NOT NULL CONSTRAINT DF_Orders_Status DEFAULT 0,  -- 0 Chờ xử lý, 1 Đang giao, 2 Hoàn thành, 3 Đã hủy
        TotalAmount  DECIMAL(18,2) NOT NULL
    );
END
GO

/* ---------------------------- OrderDetails ------------------------------ */
IF OBJECT_ID(N'dbo.OrderDetails', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderDetails (
        Id          INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_OrderDetails PRIMARY KEY,
        OrderId     INT           NOT NULL,
        ProductId   INT           NULL,             -- NULL nếu sản phẩm đã bị xóa
        ProductName NVARCHAR(200) NOT NULL,         -- lưu lại tên/giá tại thời điểm mua
        UnitPrice   DECIMAL(18,2) NOT NULL,
        Quantity    INT           NOT NULL,
        CONSTRAINT FK_OrderDetails_Orders_OrderId FOREIGN KEY (OrderId)
            REFERENCES dbo.Orders (Id) ON DELETE CASCADE,
        CONSTRAINT FK_OrderDetails_Products_ProductId FOREIGN KEY (ProductId)
            REFERENCES dbo.Products (Id) ON DELETE SET NULL
    );
    CREATE INDEX IX_OrderDetails_OrderId   ON dbo.OrderDetails (OrderId);
    CREATE INDEX IX_OrderDetails_ProductId ON dbo.OrderDetails (ProductId);
END
GO

/* ----------------------------- Dữ liệu mẫu ------------------------------ */
IF NOT EXISTS (SELECT 1 FROM dbo.Categories)
BEGIN
    INSERT INTO dbo.Categories (Name, Description) VALUES
        (N'Áo nữ',            N'Áo thun, áo sweater và các mẫu áo thời trang dành cho nữ'),
        (N'Áo khoác & Blazer', N'Áo khoác, cardigan, blazer cho cả nam và nữ'),
        (N'Đầm & Jumpsuit',   N'Đầm dài, đầm quấn và jumpsuit');

    DECLARE @ao INT    = (SELECT Id FROM dbo.Categories WHERE Name = N'Áo nữ');
    DECLARE @khoac INT = (SELECT Id FROM dbo.Categories WHERE Name = N'Áo khoác & Blazer');
    DECLARE @dam INT   = (SELECT Id FROM dbo.Categories WHERE Name = N'Đầm & Jumpsuit');

    INSERT INTO dbo.Products (Name, Description, Price, ImageUrl, CategoryId) VALUES
        (N'Áo thun đen in họa tiết', N'Áo thun cotton form rộng, in họa tiết cá tính. Dễ phối cùng quần jeans hoặc chân váy.', 199000, N'product-2.jpg', @ao),
        (N'Áo sweater trắng Homegirl', N'Áo sweater nỉ mềm, ấm, phù hợp đi học và đi chơi. Chất vải dày dặn, ít xù lông.', 279000, N'product-3.jpg', @ao),
        (N'Blazer trắng nữ', N'Blazer trắng dáng suông thanh lịch, phù hợp đi làm và dự tiệc.', 650000, N'product-1.jpg', @khoac),
        (N'Cardigan be dáng dài', N'Cardigan dáng dài màu be nhẹ nhàng, chất liệu mềm mại, giữ ấm tốt.', 459000, N'product-4.jpg', @khoac),
        (N'Blazer xám nam', N'Blazer nam màu xám xanh, phom ôm vừa vặn, phối được với cả áo thun và sơ mi.', 790000, N'product-6.jpg', @khoac),
        (N'Áo khoác dạ hồng', N'Áo khoác dạ màu hồng pastel, dáng dài, phù hợp thời tiết se lạnh.', 899000, N'product-8.jpg', @khoac),
        (N'Jumpsuit xanh ngọc', N'Jumpsuit cổ V màu xanh ngọc nổi bật, chất liệu satin mềm, tôn dáng.', 520000, N'product-5.jpg', @dam),
        (N'Đầm quấn cam đất', N'Đầm quấn xẻ tà màu cam đất, chất liệu mát, phù hợp đi biển và dạo phố.', 590000, N'product-7.jpg', @dam),
        (N'Đầm dài vàng', N'Đầm dài màu vàng tươi sáng, thiết kế nhẹ nhàng, thích hợp đi chơi và chụp ảnh.', 620000, N'product-9.jpg', @dam);
END
GO

PRINT N'Hoàn tất khởi tạo cơ sở dữ liệu DuanbanhangDb.';
GO
