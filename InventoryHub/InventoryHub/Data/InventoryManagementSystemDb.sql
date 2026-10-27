USE [InventoryManagementSystemDb]
GO

-- Categories Table
CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

-- Suppliers Table
CREATE TABLE Suppliers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName NVARCHAR(100) NOT NULL,
    ContactEmail NVARCHAR(MAX) NOT NULL
);

-- Products Table
CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    SKU NVARCHAR(50) NOT NULL,
    UnitPrice DECIMAL(18,2) NOT NULL,
    StockQuantity INT NOT NULL,
    ReorderLevel INT NOT NULL,
    CategoryId INT NOT NULL,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
);

-- Transactions Table
CREATE TABLE Transactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductId INT NOT NULL,
    TransactionType NVARCHAR(MAX) NOT NULL DEFAULT 'Sale',
    Quantity INT NOT NULL,
    TransactionDate DATETIME2 NOT NULL,
    CONSTRAINT FK_Transactions_Products FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
);

-- Insert Categories
INSERT INTO Categories (Name)
VALUES 
    ('Electronics'),
    ('Office Supplies'),
    ('Breakroom');

-- Insert Suppliers
INSERT INTO Suppliers (CompanyName, ContactEmail)
VALUES 
    ('TechDistributors Inc.', 'sales@techdistributors.com'),
    ('Global Office Solutions', 'orders@globaloffice.com'),
    ('BevAndSnack Co.', 'supply@bevandsnack.com');

-- Insert Products (Assuming Categories IDs are 1, 2, and 3)
INSERT INTO Products (Name, SKU, UnitPrice, StockQuantity, ReorderLevel, CategoryId)
VALUES 
    ('Wireless Mouse', 'WM-1001', 25.99, 150, 20, 1),
    ('Mechanical Keyboard', 'MK-2005', 85.50, 75, 10, 1),
    ('Printer Paper (Ream)', 'PP-500', 6.49, 300, 50, 2),
    ('Whiteboard Markers (4-Pack)', 'WM-4PK', 4.99, 200, 30, 2),
    ('Dark Roast Coffee Beans', 'DR-COFF-1LB', 14.99, 80, 15, 3);

-- Insert Transactions (Assuming Product IDs 1 through 5 were generated)
INSERT INTO Transactions (ProductId, TransactionType, Quantity, TransactionDate)
VALUES 
    (1, 'Restock', 50, '2026-10-01 08:30:00'),
    (2, 'Sale', 2, '2026-10-15 14:45:00'),
    (3, 'Sale', 10, '2026-10-18 09:15:00'),
    (4, 'Restock', 100, '2026-10-20 11:00:00'),
    (5, 'Sale', 3, '2026-10-22 16:20:00');

    -- 1. View all products with their assigned category names
SELECT 
    p.Id AS ProductId,
    p.Name AS ProductName,
    p.SKU,
    p.UnitPrice,
    p.StockQuantity,
    c.Name AS CategoryName
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.Id;

-- 2. View all transactions with the associated product details
SELECT 
    t.Id AS TransactionId,
    t.TransactionDate,
    t.TransactionType,
    t.Quantity,
    p.Name AS ProductName,
    p.SKU,
    (p.UnitPrice * t.Quantity) AS TotalTransactionValue
FROM Transactions t
INNER JOIN Products p ON t.ProductId = p.Id
ORDER BY t.TransactionDate DESC;

-- 3. Identify products that need restocking (Stock Quantity at or below Reorder Level)
SELECT 
    p.Name AS ProductName,
    p.SKU,
    p.StockQuantity,
    p.ReorderLevel,
    c.Name AS CategoryName
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.Id
WHERE p.StockQuantity <= p.ReorderLevel;

-- 4. Calculate total inventory value per category
SELECT 
    c.Name AS CategoryName,
    SUM(p.StockQuantity) AS TotalItemsInStock,
    SUM(p.StockQuantity * p.UnitPrice) AS TotalInventoryValue
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.Id
GROUP BY c.Name;


