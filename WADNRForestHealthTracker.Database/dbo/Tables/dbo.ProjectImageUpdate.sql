CREATE TABLE [dbo].[ProjectImageUpdate](
    [ProjectImageUpdateID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectImageUpdate_ProjectImageUpdateID] PRIMARY KEY,
    [FileResourceID] [int] NULL CONSTRAINT [FK_ProjectImageUpdate_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [ProjectUpdateBatchID] [int] NOT NULL CONSTRAINT [FK_ProjectImageUpdate_ProjectUpdateBatch_ProjectUpdateBatchID] FOREIGN KEY REFERENCES [dbo].[ProjectUpdateBatch]([ProjectUpdateBatchID]),
    [ProjectImageTimingID] [int] NULL CONSTRAINT [FK_ProjectImageUpdate_ProjectImageTiming_ProjectImageTimingID] FOREIGN KEY REFERENCES [dbo].[ProjectImageTiming]([ProjectImageTimingID]),
    [Caption] [varchar](200) NOT NULL,
    [Credit] [varchar](200) NOT NULL,
    [IsKeyPhoto] [bit] NOT NULL,
    [ExcludeFromFactSheet] [bit] NOT NULL,
    [ProjectImageID] [int] NULL CONSTRAINT [FK_ProjectImageUpdate_ProjectImage_ProjectImageID] FOREIGN KEY REFERENCES [dbo].[ProjectImage]([ProjectImageID])
)
GO