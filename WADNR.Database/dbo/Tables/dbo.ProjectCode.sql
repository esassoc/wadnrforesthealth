CREATE TABLE [dbo].[ProjectCode](
    [ProjectCodeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectCode_ProjectCodeID] PRIMARY KEY,
    [ProjectCodeName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectCode_ProjectCodeName] UNIQUE,
    [ProjectCodeTitle] [varchar](255) NULL,
    [CreateDate] [datetime] NULL,
    [ProjectStartDate] [datetime] NULL,
    [ProjectEndDate] [datetime] NULL
)
GO
