USE MyStore;
GO
USE MyStore;

GO


-- 1. Thêm các thuộc tính đặc trưng cho giày nếu chưa có
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Sizes')
    ALTER TABLE Products ADD Sizes NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Colors')
    ALTER TABLE Products ADD Colors NVARCHAR(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Brand')
    ALTER TABLE Products ADD Brand NVARCHAR(100) NULL;
GO
USE MyStore;
GO
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Brand')
    ALTER TABLE Products ADD Brand NVARCHAR(100) NULL;
GO
-- 1. Bảng Users (Quản trị)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserID] INT IDENTITY(1,1) PRIMARY KEY,
        [Username] NVARCHAR(50) NOT NULL,
        [Password] NVARCHAR(100) NOT NULL,
        [Role] NVARCHAR(20) DEFAULT 'Admin'
    );
END
GO

-- 2. Bảng Categories (Danh mục)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [CategoryID] INT IDENTITY(1,1) PRIMARY KEY,
        [CategoryName] NVARCHAR(100) NOT NULL
    );
END
GO

-- 3. Bảng Products (Sản phẩm - Tích hợp Shop Giày)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Products] (
        [ProductID] INT IDENTITY(1,1) PRIMARY KEY,
        [CategoryID] INT FOREIGN KEY REFERENCES [dbo].[Categories]([CategoryID]),
        [ProductName] NVARCHAR(200) NOT NULL,
        [ProductDescription] NVARCHAR(MAX) NULL,
        [UnitPrice] DECIMAL(18, 2) NOT NULL,
        [ProductImage] NVARCHAR(MAX) NULL,
        [Sizes] NVARCHAR(100) NULL,
        [Colors] NVARCHAR(100) NULL,
        [Brand] NVARCHAR(100) NULL
    );
END
ELSE
BEGIN
    -- Nếu bảng Products đã có sẵn, tự động bổ sung cột nếu thiếu
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Sizes')
        ALTER TABLE Products ADD Sizes NVARCHAR(100) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Colors')
        ALTER TABLE Products ADD Colors NVARCHAR(100) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND name = 'Brand')
        ALTER TABLE Products ADD Brand NVARCHAR(100) NULL;
END
GO

-- 4. Bảng Customer (Khách hàng)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customer]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Customer] (
        [CustomerID] INT IDENTITY(1,1) PRIMARY KEY,
        [CustomerName] NVARCHAR(100) NOT NULL,
        [Phone] NVARCHAR(15) NULL,
        [Email] NVARCHAR(100) NULL,
        [Address] NVARCHAR(255) NULL
    );

END
GO

-- 5. Bảng Orders (Đơn hàng)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Orders] (
        [OrderID] INT IDENTITY(1,1) PRIMARY KEY,
        [CustomerID] INT FOREIGN KEY REFERENCES [dbo].[Customer]([CustomerID]),
        [OrderDate] DATETIME DEFAULT GETDATE(),
        [AddressDeliver] NVARCHAR(255) NULL,
        [TotalAmount] DECIMAL(18, 2) NOT NULL,
        [Status] NVARCHAR(50) DEFAULT N'Chờ duyệt'
    );
END
GO

-- 6. Bảng OrderDetails (Chi tiết đơn hàng - Lưu Size/Màu đã chọn)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[OrderDetails] (
        [OrderDetailID] INT IDENTITY(1,1) PRIMARY KEY,
        [OrderID] INT FOREIGN KEY REFERENCES [dbo].[Orders]([OrderID]) ON DELETE CASCADE,
        [ProductID] INT FOREIGN KEY REFERENCES [dbo].[Products]([ProductID]),
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18, 2) NOT NULL,
        [SelectedSize] NVARCHAR(20) NULL,
        [SelectedColor] NVARCHAR(20) NULL
    );
END
ELSE
BEGIN
    -- Nếu bảng OrderDetails đã có sẵn, tự động bổ sung cột chọn size/màu nếu thiếu
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND name = 'SelectedSize')
        ALTER TABLE OrderDetails ADD SelectedSize NVARCHAR(20) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OrderDetails]') AND name = 'SelectedColor')
        ALTER TABLE OrderDetails ADD SelectedColor NVARCHAR(20) NULL;
END
GO
SELECT * FROM Products;