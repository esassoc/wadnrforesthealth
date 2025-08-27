IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'dbo.procImportGisTableToGisFeature'))
    drop procedure dbo.procImportGisTableToGisFeature
go
create procedure dbo.procImportGisTableToGisFeature
(
    @piGisUploadAttemptID int
)
as

DECLARE @SQL nvarchar(1000)
SET @SQL = 'insert into dbo.GisFeature (GisUploadAttemptID, GisFeatureGeometry, GisImportFeatureKey) select ' + cast(@piGisUploadAttemptID as varchar) + ', Shape, '+ (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'gisimport' and ORDINAL_POSITION = 1 )+ ' from dbo.gisimport' 

EXEC (@SQL)
  


/*

exec dbo.procImportGisTableToGisFeature @tableNameToImportFrom = 'gisimport1' , @piGisUploadAttemptID = 1

*/