Create database BackendTest

CREATE TABLE Supplier (
    SupplierId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SupplierName VARCHAR(50),
    CreatedOn DATETIME,
    IsActive BIT
);

CREATE TABLE Product (
    ProductId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProductName VARCHAR(50),
    UnitPrice DECIMAL,
    SupplierId UNIQUEIDENTIFIER,
    CreatedOn DATETIME,
    IsActive BIT,
    FOREIGN KEY (SupplierId) REFERENCES Supplier(SupplierId)
);

CREATE TABLE Customer (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username VARCHAR(30),
    Email VARCHAR(20),
    FirstName VARCHAR(20),
    LastName VARCHAR(20),
    CreatedOn DATETIME,
    IsActive BIT
);

CREATE TABLE Orders (
    OrderId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ProductId UNIQUEIDENTIFIER,
    OrderStatus INT,
    OrderType INT,
    OrderBy UNIQUEIDENTIFIER,
    OrderedOn DATETIME,
    ShippedOn DATETIME,
    IsActive BIT,
    FOREIGN KEY (ProductId) REFERENCES Product(ProductId),
    FOREIGN KEY (OrderBy) REFERENCES Customer(UserId)
);


INSERT INTO Supplier (SupplierName, CreatedOn, IsActive)
VALUES 
    ('Supplier A', GETDATE(), 1),
    ('Supplier B', GETDATE(), 1),
    ('Supplier C', GETDATE(), 1);


INSERT INTO Product (ProductName, UnitPrice, SupplierId, CreatedOn, IsActive)
VALUES 
    ('Product 1', 10.99, (SELECT SupplierId FROM Supplier WHERE SupplierName = 'Supplier A'), GETDATE(), 1),
    ('Product 2', 15.99, (SELECT SupplierId FROM Supplier WHERE SupplierName = 'Supplier B'), GETDATE(), 1),
    ('Product 3', 20.99, (SELECT SupplierId FROM Supplier WHERE SupplierName = 'Supplier C'), GETDATE(), 1);



INSERT INTO Customer (Username, Email, FirstName, LastName, CreatedOn, IsActive)
VALUES 
    ('user1', 'user1@example.com', 'John', 'Doe', GETDATE(), 1),
    ('user2', 'user2@example.com', 'Jane', 'Smith', GETDATE(), 1),
    ('user3', 'user3@example.com', 'Alice', 'Johnson', GETDATE(), 1);



INSERT INTO Orders (ProductId, OrderStatus, OrderType, OrderBy, OrderedOn, ShippedOn, IsActive)
VALUES 
    ((SELECT ProductId FROM Product WHERE ProductName = 'Product 1'), 1, 1, (SELECT UserId FROM Customer WHERE Username = 'user1'), GETDATE(), GETDATE(), 1),
    ((SELECT ProductId FROM Product WHERE ProductName = 'Product 2'), 2, 2, (SELECT UserId FROM Customer WHERE Username = 'user2'), GETDATE(), GETDATE(), 0),
    ((SELECT ProductId FROM Product WHERE ProductName = 'Product 3'), 1, 1, (SELECT UserId FROM Customer WHERE Username = 'user3'), GETDATE(), GETDATE(), 1);



SELECT * FROM Customer
SELECT * FROM Supplier
SELECT * FROM Orders
SELECT * FROM Product


DROP TABLE IF EXISTS Orders;