
create procedure dbo.procImportGisTableToGisFeature
(
    @piGisUploadAttemptID int
)
as

DECLARE @SQL varchar(1000)
SET @SQL = 'insert into dbo.GisFeature (GisUploadAttemptID, GisFeatureGeometry, GisImportFeatureKey) select ' + cast(@piGisUploadAttemptID as varchar) + ', Shape, '+ (select COLUMN_NAME from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'gisimport' and ORDINAL_POSITION = 1 )+ ' from dbo.gisimport' 

EXEC (@SQL)
  


/*

exec dbo.procImportGisTableToGisFeature @tableNameToImportFrom = 'gisimport1' , @piGisUploadAttemptID = 1

*/