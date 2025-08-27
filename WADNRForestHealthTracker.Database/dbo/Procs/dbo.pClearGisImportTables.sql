IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.pClearGisImportTables'))
    DROP PROCEDURE dbo.pClearGisImportTables
GO

CREATE PROCEDURE dbo.pClearGisImportTables
AS
begin

    delete from dbo.GisFeatureMetadataAttribute;
    delete from dbo.GisFeature;

	--reset seed number and 
	--DBCC CHECKIDENT ('dbo.GisFeatureMetadataAttribute', RESEED, 0);
	--DBCC CHECKIDENT ('dbo.GisFeature', RESEED, 0);

	--DBCC CHECKIDENT ('dbo.GisFeatureMetadataAttribute', RESEED);
	--DBCC CHECKIDENT ('dbo.GisFeature', RESEED);
	
	--rebuild index on these 2 tables
	alter index all on dbo.GisFeatureMetadataAttribute rebuild;
	alter index all on dbo.GisFeature rebuild;

	--update statistics
	update statistics dbo.GisFeatureMetadataAttribute with fullscan;
	update statistics dbo.GisFeature with fullscan;
	

end

/*

exec dbo.pClearGisImportTables

*/

