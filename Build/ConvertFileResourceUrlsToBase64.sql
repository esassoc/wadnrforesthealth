-- Convert legacy FileResource URLs in all dbo.html columns
-- Images  -> base64 data URIs (inline display)
-- Non-images (PDF, etc.) -> /file-resources/{GUID} API URLs
--
-- Handles all known URL variants:
--   https://foresthealthtracker.dnr.wa.gov/FileResource/DisplayResource/{GUID}
--   ../../FileResource/DisplayResource/{GUID}
--   /FileResource/DisplayResource/{GUID}
--
-- Run against WADNRForestHealthDB (restored legacy backup) BEFORE DatabaseBuild.ps1
-- drops the FileResourceData column.
--
-- Usage:
--   sqlcmd -S ".\" -d "WADNRForestHealthDB" -i "ConvertFileResourceUrlsToBase64.sql"

PRINT 'Starting migration: Convert FileResource URLs';
PRINT 'Images -> base64 data URIs, Non-images -> API URLs...';
PRINT '';

-- Create temp table for MIME type mapping (accessible in dynamic SQL)
CREATE TABLE #ExtensionToMime (Extension VARCHAR(10), MimeType VARCHAR(50), IsImage BIT);
INSERT INTO #ExtensionToMime VALUES
    ('.png', 'image/png', 1),
    ('.jpg', 'image/jpeg', 1),
    ('.jpeg', 'image/jpeg', 1),
    ('.gif', 'image/gif', 1),
    ('.bmp', 'image/bmp', 1),
    ('.svg', 'image/svg+xml', 1),
    ('.webp', 'image/webp', 1),
    ('.ico', 'image/x-icon', 1),
    ('.pdf', 'application/pdf', 0);

-- Find all columns with dbo.html type
DECLARE @HtmlColumns TABLE (
    TableName SYSNAME,
    ColumnName SYSNAME,
    PrimaryKeyColumn SYSNAME
);

INSERT INTO @HtmlColumns (TableName, ColumnName, PrimaryKeyColumn)
SELECT
    t.name AS TableName,
    c.name AS ColumnName,
    (SELECT TOP 1 col.name
     FROM sys.indexes i
     INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
     INNER JOIN sys.columns col ON ic.object_id = col.object_id AND ic.column_id = col.column_id
     WHERE i.is_primary_key = 1 AND i.object_id = t.object_id) AS PrimaryKeyColumn
FROM sys.columns c
INNER JOIN sys.types ty ON c.user_type_id = ty.user_type_id
INNER JOIN sys.tables t ON c.object_id = t.object_id
WHERE ty.name = 'html' AND ty.schema_id = SCHEMA_ID('dbo');

-- Check if FileResourceData column exists (only present on restored legacy backup before DatabaseBuild.ps1)
DECLARE @HasFileData BIT = 0;
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FileResource' AND TABLE_SCHEMA = 'dbo' AND COLUMN_NAME = 'FileResourceData')
    SET @HasFileData = 1;

IF @HasFileData = 0
    PRINT 'Note: FileResourceData column not present. Images will use /file-resources/{GUID} API URLs instead of base64.';

-- The pattern we search for (present in all variants)
DECLARE @SearchPattern VARCHAR(100) = 'FileResource/DisplayResource/';
DECLARE @SearchPatternLen INT = LEN(@SearchPattern);

-- Known URL prefixes (longest first for correct replacement)
DECLARE @Prefix1 VARCHAR(200) = 'https://foresthealthtracker.dnr.wa.gov/FileResource/DisplayResource/';
DECLARE @Prefix2 VARCHAR(200) = '../../FileResource/DisplayResource/';
DECLARE @Prefix3 VARCHAR(200) = '/FileResource/DisplayResource/';

-- Variables for processing
DECLARE @TableName SYSNAME;
DECLARE @ColumnName SYSNAME;
DECLARE @PrimaryKeyColumn SYSNAME;
DECLARE @SQL NVARCHAR(MAX);
DECLARE @UpdateCount INT;
DECLARE @TotalUpdates INT = 0;

-- Cursor through each html column
DECLARE column_cursor CURSOR FOR
    SELECT TableName, ColumnName, PrimaryKeyColumn FROM @HtmlColumns;

OPEN column_cursor;
FETCH NEXT FROM column_cursor INTO @TableName, @ColumnName, @PrimaryKeyColumn;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Processing ' + @TableName + '.' + @ColumnName + '...';

    -- Check if any rows contain the pattern
    SET @SQL = N'SELECT @cnt = COUNT(*) FROM dbo.' + QUOTENAME(@TableName) +
               N' WHERE ' + QUOTENAME(@ColumnName) + N' LIKE ''%'' + @sp + ''%''';

    DECLARE @RowCount INT;
    EXEC sp_executesql @SQL, N'@sp VARCHAR(100), @cnt INT OUTPUT', @SearchPattern, @RowCount OUTPUT;

    IF @RowCount > 0
    BEGIN
        PRINT '  Found ' + CAST(@RowCount AS VARCHAR) + ' row(s) with FileResource URLs';

        -- Build the image handling block conditionally (FileResourceData column must exist for base64)
        DECLARE @ImageBlock NVARCHAR(MAX) = N'';
        IF @HasFileData = 1
        BEGIN
            SET @ImageBlock = N'
                IF @Mime IS NOT NULL AND @Img = 1
                BEGIN
                    SELECT @B64 = CAST(N'''''''' AS XML).value(''xs:base64Binary(sql:column("f.FileResourceData"))'', ''VARCHAR(MAX)'')
                    FROM dbo.FileResource f WHERE f.FileResourceGUID = TRY_CAST(@Guid AS UNIQUEIDENTIFIER);
                    SET @Rpl = ''data:'' + @Mime + '';base64,'' + @B64;
                END
                ELSE ';
        END

        SET @SQL = N'
        DECLARE @PK INT;
        DECLARE @Content NVARCHAR(MAX);
        DECLARE @NewContent NVARCHAR(MAX);
        DECLARE @Pos INT;
        DECLARE @Guid VARCHAR(36);
        DECLARE @B64 VARCHAR(MAX);
        DECLARE @Mime VARCHAR(50);
        DECLARE @Img BIT;
        DECLARE @Rpl VARCHAR(MAX);
        DECLARE @Cnt INT = 0;

        DECLARE rc CURSOR FOR
            SELECT ' + QUOTENAME(@PrimaryKeyColumn) + N', ' + QUOTENAME(@ColumnName) + N'
            FROM dbo.' + QUOTENAME(@TableName) + N'
            WHERE ' + QUOTENAME(@ColumnName) + N' LIKE ''%'' + @sp + ''%'';
        OPEN rc;
        FETCH NEXT FROM rc INTO @PK, @Content;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @NewContent = @Content;
            WHILE CHARINDEX(@sp, @NewContent) > 0
            BEGIN
                SET @Pos = CHARINDEX(@sp, @NewContent);
                SET @Guid = SUBSTRING(@NewContent, @Pos + @spLen, 36);
                SET @Mime = NULL; SET @Img = NULL; SET @B64 = NULL;

                SELECT @Mime = ISNULL(e.MimeType, ''application/octet-stream''), @Img = ISNULL(e.IsImage, 0)
                FROM dbo.FileResource f LEFT JOIN #ExtensionToMime e ON LOWER(f.OriginalFileExtension) = e.Extension
                WHERE f.FileResourceGUID = TRY_CAST(@Guid AS UNIQUEIDENTIFIER);

                ' + @ImageBlock + N'IF @Mime IS NOT NULL
                BEGIN
                    SET @Rpl = ''/file-resources/'' + @Guid;
                END
                ELSE
                BEGIN
                    SET @Rpl = ''[MISSING:'' + @Guid + '']'';
                END

                -- Replace longest prefix first, then shorter, then core pattern as fallback
                IF CHARINDEX(@p1 + @Guid, @NewContent) > 0
                    SET @NewContent = REPLACE(@NewContent, @p1 + @Guid, @Rpl);
                ELSE IF CHARINDEX(@p2 + @Guid, @NewContent) > 0
                    SET @NewContent = REPLACE(@NewContent, @p2 + @Guid, @Rpl);
                ELSE IF CHARINDEX(@p3 + @Guid, @NewContent) > 0
                    SET @NewContent = REPLACE(@NewContent, @p3 + @Guid, @Rpl);
                ELSE
                    SET @NewContent = REPLACE(@NewContent, @sp + @Guid, @Rpl);
            END

            IF @NewContent <> @Content
            BEGIN
                UPDATE dbo.' + QUOTENAME(@TableName) + N' SET ' + QUOTENAME(@ColumnName) + N' = @NewContent WHERE ' + QUOTENAME(@PrimaryKeyColumn) + N' = @PK;
                SET @Cnt = @Cnt + 1;
            END
            FETCH NEXT FROM rc INTO @PK, @Content;
        END
        CLOSE rc; DEALLOCATE rc;
        SET @UpdOut = @Cnt;
        ';

        EXEC sp_executesql @SQL,
            N'@sp VARCHAR(100), @spLen INT, @p1 VARCHAR(200), @p2 VARCHAR(200), @p3 VARCHAR(200), @UpdOut INT OUTPUT',
            @SearchPattern, @SearchPatternLen, @Prefix1, @Prefix2, @Prefix3, @UpdateCount OUTPUT;

        IF @UpdateCount > 0
        BEGIN
            PRINT '  Updated ' + CAST(@UpdateCount AS VARCHAR) + ' row(s)';
            SET @TotalUpdates = @TotalUpdates + @UpdateCount;
        END
    END
    ELSE
    BEGIN
        PRINT '  No FileResource URLs found';
    END

    PRINT '';
    FETCH NEXT FROM column_cursor INTO @TableName, @ColumnName, @PrimaryKeyColumn;
END

CLOSE column_cursor;
DEALLOCATE column_cursor;

DROP TABLE #ExtensionToMime;

PRINT '================================';
PRINT 'Migration complete!';
PRINT 'Total rows updated: ' + CAST(@TotalUpdates AS VARCHAR);
PRINT '================================';
