CREATE TABLE [dbo].[FirmaPage](
    [FirmaPageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FirmaPage_FirmaPageID] PRIMARY KEY,
    [FirmaPageTypeID] [int] NOT NULL CONSTRAINT [AK_FirmaPage_FirmaPageTypeID] UNIQUE,
    [FirmaPageContent] [dbo].[html] NULL,
    CONSTRAINT [FK_FirmaPage_FirmaPageType_FirmaPageTypeID] FOREIGN KEY ([FirmaPageTypeID]) REFERENCES [dbo].[FirmaPageType]([FirmaPageTypeID])
)
GO