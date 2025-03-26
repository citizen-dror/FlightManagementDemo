-- 1. Create the FlightsDemo database
CREATE DATABASE FlightsDemo;
GO

-- 2. Create a SQL Server login for the .NET app
CREATE LOGIN AppUser WITH PASSWORD = 'Aa123456789';
GO

-- 3. Switch to the FlightsDemo database
USE FlightsDemo;
GO

-- 4. Create a database user linked to the login
CREATE USER AppUser FOR LOGIN AppUser;
GO

-- 5. Grant read and write permissions
ALTER ROLE db_datareader ADD MEMBER AppUser; -- Allows SELECT queries
ALTER ROLE db_datawriter ADD MEMBER AppUser; -- Allows INSERT, UPDATE, DELETE
GO

GRANT CREATE TABLE TO [AppUser]; 
-- Grant ALTER and CONTROL on the dbo schema to the user
GRANT ALTER, CONTROL ON SCHEMA::dbo TO [AppUser];