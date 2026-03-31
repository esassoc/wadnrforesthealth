CREATE TABLE [dbo].[TrainingVideo](
    [TrainingVideoID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TrainingVideo_TrainingVideoID] PRIMARY KEY,
    [VideoName] [varchar](100) NOT NULL,
    [VideoDescription] [varchar](500) NULL,
    [VideoUploadDate] [date] NOT NULL,
    [VideoURL] [varchar](100) NOT NULL
)
GO
