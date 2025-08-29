CREATE TABLE [dbo].[FirmaPageType](
	[FirmaPageTypeID] [int] NOT NULL CONSTRAINT [PK_FirmaPageType_FirmaPageTypeID] PRIMARY KEY,
	[FirmaPageTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_FirmaPageType_FirmaPageTypeName] UNIQUE,
	[FirmaPageTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_FirmaPageType_FirmaPageTypeDisplayName] UNIQUE,
	[FirmaPageRenderTypeID] [int] NOT NULL CONSTRAINT [FK_FirmaPageType_FirmaPageRenderType_FirmaPageRenderTypeID] FOREIGN KEY REFERENCES [dbo].[FirmaPageRenderType]([FirmaPageRenderTypeID])
)
GO