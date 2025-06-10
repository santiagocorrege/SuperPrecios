
CREATE USER [superprecios_user] WITH PASSWORD = 'SuperPass123!';
GO

-- Dar permisos necesarios
ALTER ROLE db_datareader ADD MEMBER [superprecios_user];
ALTER ROLE db_datawriter ADD MEMBER [superprecios_user];
ALTER ROLE db_ddladmin ADD MEMBER [superprecios_user];
GO

-- Verificar que se creó
SELECT name FROM sys.database_principals WHERE type = 'S';