CREATE TABLE [dbo].[FocusAreaStatus](
    [FocusAreaStatusID] [int] NOT NULL CONSTRAINT [PK_FocusAreaStatus_FocusAreaStatusID] PRIMARY KEY,
    [FocusAreaStatusName] [varchar](50) NULL CONSTRAINT [AK_FocusAreaStatus_FocusAreaStatusName] UNIQUE,
    [FocusAreaStatusDisplayName] [varchar](50) NULL CONSTRAINT [AK_FocusAreaStatus_FocusAreaStatusDisplayName] UNIQUE
)
GO
