CREATE PROCEDURE GetActiveOrdersByCustomer
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Orders.OrderId,
           Orders.ProductId,
           Orders.OrderStatus,
           Orders.OrderType,
           Orders.OrderedOn,
           Orders.ShippedOn,
           Product.ProductName,
           Product.UnitPrice
    FROM Orders
    INNER JOIN Product ON Orders.ProductId = Product.ProductId
    WHERE Orders.OrderBy = @UserId
          AND Orders.IsActive = 1;
END;





DECLARE @UserId UNIQUEIDENTIFIER
SET @UserId = '5F93C1F9-A400-4271-9EB9-F149BF02CB54'
EXEC GetActiveOrdersByCustomer @UserId


SELECT * FROM sys.procedures WHERE name = 'GetActiveOrdersByCustomer';



DROP PROCEDURE BackendTest.GetActiveOrdersByCustomer;

