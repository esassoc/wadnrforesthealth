CREATE TABLE [dbo].[FundSourceImage](
    [FundSourceImageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceImage_FundSourceImageID] PRIMARY KEY,
    [FileResourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceImage_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [FundSourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceImage_FundSource_FundSourceID] FOREIGN KEY REFERENCES [dbo].[FundSource]([FundSourceID]),
    [Caption] [varchar](200) NOT NULL,
    [Credit] [varchar](200) NOT NULL,
    [IsKeyPhoto] [bit] NOT NULL,
    CONSTRAINT [AK_FundSourceImage_FileResourceID_FundSourceID] UNIQUE ([FileResourceID], [FundSourceID])
)
GO
