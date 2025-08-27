IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fRemoveLeadingZeroes'))
    drop function dbo.fRemoveLeadingZeroes
go
CREATE FUNCTION dbo.fRemoveLeadingZeroes
(
   @inputString NVARCHAR(MAX)
)
RETURNS NVARCHAR(MAX)
BEGIN
    RETURN SUBSTRING(@inputString, PATINDEX('%[^0]%', @inputString+'.'), LEN(@inputString))
END
go


/*

select dbo.fRemoveLeadingZeroes('00032')
select dbo.fRemoveLeadingZeroes('10032')
select dbo.fRemoveLeadingZeroes('AABADAD00000')
select dbo.fRemoveLeadingZeroes('00AABADAD00000')


*/

