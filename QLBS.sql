CREATE DATABASE QLBS;
GO

USE QLBS;
GO

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QLBS')
BEGIN
    ALTER DATABASE QLBS SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLBS;
END
GO



CREATE TABLE Role (
    RoleId INT IDENTITY PRIMARY KEY,
    RoleName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL
);
GO

CREATE TABLE Permission (
    PermissionId INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(100) NOT NULL UNIQUE,
    PermissionName NVARCHAR(150) NOT NULL,
    Description NVARCHAR(255) NULL
);
GO

CREATE TABLE Role_Permission (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES Role(RoleId) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES Permission(PermissionId) ON DELETE CASCADE
);
GO

CREATE TABLE Account (
    AccountId INT IDENTITY PRIMARY KEY,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    IsActive BIT DEFAULT 1,
    IsEmailVerified BIT DEFAULT 0,
    RefreshToken NVARCHAR(512) NULL,
    ResetToken NVARCHAR(512) NULL,
    ResetTokenExpires DATETIME NULL,
    OTP CHAR(6) NULL,
    OtpExpires DATETIME NULL,
    LastLogin DATETIME NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleId) REFERENCES Role(RoleId)
);
GO

CREATE TABLE UserProfile (
    UserId INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(200) NULL,
    DateOfBirth DATE NULL,
    Address NVARCHAR(200) NULL,
    PhoneNumber VARCHAR(20) NULL,
    AvatarUrl NVARCHAR(255) NULL,
    IsDeleted BIT DEFAULT 0,
    AccountId INT NOT NULL UNIQUE,
    FOREIGN KEY (AccountId) REFERENCES Account(AccountId) ON DELETE CASCADE
);
GO

CREATE TABLE NotificationType (
    NotificationTypeId INT IDENTITY PRIMARY KEY,
    NotificationTypeName NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE Notification (
    NotificationId INT IDENTITY PRIMARY KEY,
    NotificationTypeId INT NOT NULL,
    UserId INT NOT NULL,
    Content NVARCHAR(500) NOT NULL,
    URL NVARCHAR(255) NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    ReadAt DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (NotificationTypeId) REFERENCES NotificationType(NotificationTypeId),
    FOREIGN KEY (UserId) REFERENCES UserProfile(UserId) ON DELETE CASCADE
);
GO



CREATE TABLE Category (
    CategoryId INT IDENTITY PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(1000) NULL,
    IsDeleted BIT DEFAULT 0
);
GO

CREATE TABLE Author (
    AuthorId INT IDENTITY PRIMARY KEY,
    AuthorName NVARCHAR(100) NOT NULL,
    BirthYear INT NULL,
    Nationality NVARCHAR(100) NULL,
    IsDeleted BIT DEFAULT 0
);
GO

CREATE TABLE Book (
    BookId INT IDENTITY PRIMARY KEY,
    BookTitle NVARCHAR(200) NOT NULL,
    CategoryId INT NOT NULL,
    AuthorId INT NOT NULL,
    PublishYear INT NOT NULL,
    Publisher NVARCHAR(150) NOT NULL,
    EntryDate DATE NOT NULL,
    Price DECIMAL(18,2) NOT NULL CHECK (Price >= 0),
    Quantity INT NOT NULL DEFAULT 0,
    Description NVARCHAR(1000) NULL,
    IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId),
    FOREIGN KEY (AuthorId) REFERENCES Author(AuthorId)
);
GO

CREATE TABLE BookImage (
    ImageId INT IDENTITY PRIMARY KEY,
    BookId INT NOT NULL,
    URL NVARCHAR(500) NOT NULL,
    Description NVARCHAR(255) NULL,
    OrderIndex INT NOT NULL DEFAULT 1,
    IsCover BIT DEFAULT 0,
    FOREIGN KEY (BookId) REFERENCES Book(BookId) ON DELETE CASCADE
);
GO

CREATE TABLE Review (
    ReviewId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    Rating TINYINT NOT NULL CHECK (Rating BETWEEN 1 AND 5),
    Comment NVARCHAR(1000) NOT NULL,
    Status TINYINT NOT NULL DEFAULT 1,
    ReviewDate DATETIME DEFAULT GETDATE(),
    Note NVARCHAR(200) NULL,
    CONSTRAINT UQ_User_Book UNIQUE (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES UserProfile(UserId) ON DELETE CASCADE,
    FOREIGN KEY (BookId) REFERENCES Book(BookId) ON DELETE CASCADE
);
GO

CREATE TABLE FavoriteBook (
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    MarkedDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT DEFAULT 0,
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES UserProfile(UserId) ON DELETE CASCADE,
    FOREIGN KEY (BookId) REFERENCES Book(BookId) ON DELETE CASCADE
);
GO

CREATE TABLE DiscountCode (
    DiscountCodeId INT IDENTITY PRIMARY KEY,
    Code NVARCHAR(50) NOT NULL UNIQUE,
    DiscountType TINYINT NOT NULL,
    DiscountValue DECIMAL(18,2) NOT NULL CHECK (DiscountValue >= 0),
    MaxDiscountAmount DECIMAL(18,2) NULL CHECK (MaxDiscountAmount >= 0),
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity >= 0),
    MinOrderAmount DECIMAL(18,2) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE OrderTable (
    OrderId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL,
    ReceiverName NVARCHAR(200) NOT NULL,
    ReceiverPhone NVARCHAR(20) NOT NULL,
    ShippingAddress NVARCHAR(255) NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL CHECK (TotalAmount >= 0),
    TotalQuantity INT NOT NULL DEFAULT 0,
    OrderStatus TINYINT NOT NULL,
    DiscountCodeId INT NULL,
    FOREIGN KEY (UserId) REFERENCES UserProfile(UserId),
    FOREIGN KEY (DiscountCodeId) REFERENCES DiscountCode(DiscountCodeId)
);
GO

CREATE TABLE OrderDetail (
    OrderId INT NOT NULL,
    BookId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18, 2) NOT NULL,
    PRIMARY KEY (OrderId, BookId),
    FOREIGN KEY (OrderId) REFERENCES OrderTable(OrderId) ON DELETE CASCADE,
    FOREIGN KEY (BookId) REFERENCES Book(BookId)
);
GO

CREATE TABLE Supplier (
    SupplierId INT IDENTITY PRIMARY KEY,
    SupplierName NVARCHAR(200) NOT NULL,
    Address NVARCHAR(255) NULL,
    Email NVARCHAR(200) NULL,
    PhoneNumber VARCHAR(20) NULL
);
GO

CREATE TABLE BookReceipt (
    ReceiptId INT IDENTITY PRIMARY KEY,
    SupplierId INT NOT NULL,
    UserId INT NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL CHECK (TotalAmount >= 0),
    Status TINYINT NOT NULL DEFAULT 0,
    ConfirmedById INT NULL,
    ConfirmedAt DATETIME NULL,
    Note NVARCHAR(500) NULL,
    FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId),
    FOREIGN KEY (UserId) REFERENCES UserProfile(UserId),
    FOREIGN KEY (ConfirmedById) REFERENCES UserProfile(UserId)
);
GO

CREATE TABLE BookReceiptDetail (
    ReceiptId INT NOT NULL,
    RowNumber INT NOT NULL,
    BookId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    UnitPrice DECIMAL(18,2) NOT NULL,
    Note NVARCHAR(255) NULL,
    PRIMARY KEY (ReceiptId, RowNumber),
    FOREIGN KEY (ReceiptId) REFERENCES BookReceipt(ReceiptId) ON DELETE CASCADE,
    FOREIGN KEY (BookId) REFERENCES Book(BookId)
);


CREATE TABLE PaymentMethod (
    PaymentMethodId INT IDENTITY PRIMARY KEY,
    MethodName NVARCHAR(150) NOT NULL
);
GO

CREATE TABLE Payment (
    PaymentId INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL,
    PaymentMethodId INT NOT NULL,
    PaidAmount DECIMAL(18,2) NOT NULL CHECK (PaidAmount > 0),
    PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
    PaymentStatus TINYINT NOT NULL,
    TransactionCode NVARCHAR(255) NULL,
    FOREIGN KEY (OrderId) REFERENCES OrderTable(OrderId),
    FOREIGN KEY (PaymentMethodId) REFERENCES PaymentMethod(PaymentMethodId)
);
GO

CREATE TABLE Cart (
    UserId INT NOT NULL,
    BookId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    AddedAt DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY (UserId, BookId),
    FOREIGN KEY (UserId) REFERENCES UserProfile(UserId),
    FOREIGN KEY (BookId) REFERENCES Book(BookId)
);
GO

CREATE TABLE RevenueReport (
    RevenueReportId INT IDENTITY PRIMARY KEY,
    ReportDate DATE NOT NULL UNIQUE,
    OrderCount INT NOT NULL DEFAULT 0,
    SoldBookCount INT NOT NULL DEFAULT 0,
    TotalRevenue DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

CREATE TABLE BestSellingBookReport (
    BestSellingReportId INT IDENTITY PRIMARY KEY,
    Month INT NOT NULL,
    Year INT NOT NULL,
    BookId INT NOT NULL,
    QuantitySold INT NOT NULL DEFAULT 0,
    Revenue DECIMAL(18,2) NOT NULL DEFAULT 0,
    CONSTRAINT UQ_BestSellingBook UNIQUE (Month, Year, BookId),
    FOREIGN KEY (BookId) REFERENCES Book(BookId) ON DELETE CASCADE
);
GO

CREATE TABLE InventoryReport (
    InventoryReportId INT IDENTITY PRIMARY KEY,
    UpdateDate DATE NOT NULL,
    BookId INT NOT NULL,
    StockQuantity INT NOT NULL,
    CONSTRAINT UQ_Inventory UNIQUE (UpdateDate, BookId),
    FOREIGN KEY (BookId) REFERENCES Book(BookId) ON DELETE CASCADE
);
GO



CREATE INDEX IDX_Book_Title ON Book(BookTitle);
CREATE INDEX IDX_Book_Category ON Book(CategoryId);
CREATE INDEX IDX_Book_Author ON Book(AuthorId);
CREATE INDEX IDX_Book_Publisher ON Book(Publisher);
CREATE INDEX IDX_Book_PublishYear ON Book(PublishYear);

CREATE INDEX IDX_Author_Name ON Author(AuthorName);
CREATE INDEX IDX_Category_Name ON Category(CategoryName);

CREATE INDEX IDX_Review_BookId ON Review(BookId);
CREATE INDEX IDX_Review_UserId ON Review(UserId);
CREATE INDEX IDX_Review_Rating ON Review(Rating);
CREATE INDEX IDX_FavoriteBook_UserId ON FavoriteBook(UserId);
CREATE INDEX IDX_FavoriteBook_BookId ON FavoriteBook(BookId);

CREATE INDEX IDX_Order_UserId ON OrderTable(UserId);
CREATE INDEX IDX_Order_Status ON OrderTable(OrderStatus);
CREATE INDEX IDX_OrderDetail_OrderId ON OrderDetail(OrderId);
CREATE INDEX IDX_OrderDetail_BookId ON OrderDetail(BookId);

CREATE INDEX IDX_Payment_OrderId ON Payment(OrderId);
CREATE INDEX IDX_Payment_MethodId ON Payment(PaymentMethodId);

CREATE INDEX IDX_Cart_UserId ON Cart(UserId);
CREATE INDEX IDX_Cart_BookId ON Cart(BookId);

CREATE INDEX IDX_BookReceipt_Supplier ON BookReceipt(SupplierId);
CREATE INDEX IDX_BookReceipt_UserId ON BookReceipt(UserId);
CREATE INDEX IDX_BookReceiptDetail_BookId ON BookReceiptDetail(BookId);

CREATE INDEX IDX_RevenueReport_Date ON RevenueReport(ReportDate);
CREATE INDEX IDX_BestSellingBookReport_MonthYear ON BestSellingBookReport(Month, Year);
CREATE INDEX IDX_InventoryReport_Date ON InventoryReport(UpdateDate);


SELECT * FROM NguoiDung;
SELECT * FROM TaiKhoan;
SELECT * FROM Sach;
SELECT * FROM DonHang;
SELECT * FROM Author;
SELECT * FROM TheLoai;
SELECT * FROM DanhGia_NhanXet;

