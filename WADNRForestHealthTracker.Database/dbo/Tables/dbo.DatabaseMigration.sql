CREATE TABLE dbo.DatabaseMigration
(
    DatabaseMigrationID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_DatabaseMigration_DatabaseMigrationID PRIMARY KEY,
    MigrationName varchar(100) NOT NULL,
    MigrationDate datetime NOT NULL
)
GO
