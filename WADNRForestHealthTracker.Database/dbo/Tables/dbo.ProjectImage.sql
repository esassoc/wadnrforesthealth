CREATE TABLE [dbo].[ProjectImage](
    [ProjectImageID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectImage_ProjectImageID] PRIMARY KEY,
    [FileResourceID] [int] NOT NULL CONSTRAINT [FK_ProjectImage_FileResource_FileResourceID] FOREIGN KEY REFERENCES [dbo].[FileResource]([FileResourceID]),
    [ProjectID] [int] NOT NULL CONSTRAINT [FK_ProjectImage_Project_ProjectID] FOREIGN KEY REFERENCES [dbo].[Project]([ProjectID]),
    [ProjectImageTimingID] [int] NULL CONSTRAINT [FK_ProjectImage_ProjectImageTiming_ProjectImageTimingID] FOREIGN KEY REFERENCES [dbo].[ProjectImageTiming]([ProjectImageTimingID]),
    [Caption] [varchar](200) NOT NULL,
    [Credit] [varchar](200) NOT NULL,
    [IsKeyPhoto] [bit] NOT NULL,
    [ExcludeFromFactSheet] [bit] NOT NULL,
    CONSTRAINT [AK_ProjectImage_FileResourceID_ProjectID] UNIQUE ([FileResourceID], [ProjectID])
)
GO