CREATE TABLE dbo.ExternalMapLayer
(
    ExternalMapLayerID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalMapLayer_ExternalMapLayerID PRIMARY KEY,
    [DisplayName] [varchar](75) NOT NULL CONSTRAINT [AK_ExternalMapLayer_DisplayName] UNIQUE,
    [LayerUrl] [varchar](500) NOT NULL CONSTRAINT [AK_ExternalMapLayer_LayerUrl] UNIQUE,
    [LayerDescription] [varchar](200) NULL,
    [FeatureNameField] [varchar](100) NULL,
    [DisplayOnPriorityLandscape] [bit] NOT NULL,
    [DisplayOnProjectMap] [bit] NOT NULL,
    [DisplayOnAllOthers] [bit] NOT NULL,
    [IsActive] [bit] NOT NULL,
    [IsTiledMapService] [bit] NOT NULL
)
GO
