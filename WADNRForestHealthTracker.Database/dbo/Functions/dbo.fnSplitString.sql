IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fnSplitString'))
    drop function dbo.fnSplitString
go
CREATE FUNCTION dbo.fnSplitString
(
   @stringToSplit NVARCHAR(MAX),
   @delimiter CHAR(1)
)
RETURNS @output TABLE(splitdata NVARCHAR(MAX)
)
BEGIN
   DECLARE @start INT, @end INT
   SELECT @start = 1, @end = CHARINDEX(@delimiter, @stringToSplit)
   WHILE @start < LEN(@stringToSplit) + 1 BEGIN
       IF @end = 0
           SET @end = LEN(@stringToSplit) + 1

       INSERT INTO @output (splitdata)
       VALUES(SUBSTRING(@stringToSplit, @start, @end - @start))
       SET @start = @end + 1
       SET @end = CHARINDEX(@delimiter, @stringToSplit, @start)
   END
   RETURN
END
go