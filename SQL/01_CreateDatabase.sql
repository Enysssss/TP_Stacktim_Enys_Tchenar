IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Stacktim')
BEGIN
    CREATE DATABASE Stacktim;
END
GO

USE Stacktim;
GO