CREATE TABLE [dbo].[ProjectImageTiming](
    [ProjectImageTimingID] [int] NOT NULL CONSTRAINT [PK_ProjectImageTiming_ProjectImageTimingID] PRIMARY KEY,
    [ProjectImageTimingName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectImageTiming_ProjectImageTimingName] UNIQUE,
    [ProjectImageTimingDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectImageTiming_ProjectImageTimingDisplayName] UNIQUE,
    [SortOrder] [int] NOT NULL
)
GO
