CREATE TABLE dbo.ExternalMapLayer
(
    ExternalMapLayerID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalMapLayer_ExternalMapLayerID PRIMARY KEY,
    LayerName varchar(100) NOT NULL,
    LayerUrl varchar(256) NOT NULL,
    LayerType varchar(50) NOT NULL
)
GO
