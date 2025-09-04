

CREATE FUNCTION dbo.fGetColumnNamesForTable
(
    @psTableName varchar(100)
)
RETURNS @columnNameTable TABLE
(
    PrimaryKey int not null primary key,
    ColumnName varchar(500) not null
)
AS
BEGIN


    INSERT @columnNameTable
    SELECT
	ORDINAL_POSITION as PrimaryKey,
    COLUMN_NAME as ColumnName
FROM
  	INFORMATION_SCHEMA.COLUMNS
WHERE
	TABLE_NAME = @psTableName
	RETURN
END
go

