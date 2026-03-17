IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.fGetForesterWorkUnitsByPoint'))
drop function dbo.fGetForesterWorkUnitsByPoint
GO
CREATE FUNCTION dbo.fGetForesterWorkUnitsByPoint
(
    @Point GEOMETRY
)
RETURNS TABLE
AS
RETURN
(
    SELECT 
        fwu.ForesterWorkUnitID,
        fwu.ForesterRoleID,
        fr.ForesterRoleDisplayName,
        fwu.PersonID,
        p.FirstName,
        p.LastName,
        p.Email,
        p.Phone,
        fwu.ForesterWorkUnitName,
        fr.SortOrder
    FROM 
        dbo.ForesterWorkUnit fwu
        INNER JOIN dbo.ForesterRole fr ON fwu.ForesterRoleID = fr.ForesterRoleID
        LEFT JOIN dbo.Person p ON fwu.PersonID = p.PersonID
    WHERE 
        fwu.ForesterWorkUnitLocation.STIntersects(@Point) = 1
);
GO


/*



DECLARE @Point GEOMETRY = GEOMETRY::STPointFromText('POINT(-123.411241 46.980636)', 4326);

-- Call the function
SELECT * 
FROM dbo.fGetForesterWorkUnitsByPoint(@Point);


*/