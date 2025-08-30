CREATE TABLE [dbo].[ProjectType](
    [ProjectTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProjectType_ProjectTypeID] PRIMARY KEY,
    [ProjectTypeName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectType_ProjectTypeName] UNIQUE,
    [ProjectTypeDisplayName] [varchar](100) NOT NULL CONSTRAINT [AK_ProjectType_ProjectTypeDisplayName] UNIQUE
)
GO