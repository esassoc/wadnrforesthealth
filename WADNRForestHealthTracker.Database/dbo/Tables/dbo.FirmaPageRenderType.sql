CREATE TABLE [dbo].[FirmaPageRenderType](
    [FirmaPageRenderTypeID] [int] NOT NULL CONSTRAINT [PK_FirmaPageRenderType_FirmaPageRenderTypeID] PRIMARY KEY,
    [FirmaPageRenderTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_FirmaPageRenderType_FirmaPageRenderTypeName] UNIQUE,
    [FirmaPageRenderTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_FirmaPageRenderType_FirmaPageRenderTypeDisplayName] UNIQUE
)
GO
