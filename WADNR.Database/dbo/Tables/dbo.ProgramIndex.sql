CREATE TABLE [dbo].[ProgramIndex](
    [ProgramIndexID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ProgramIndex_ProgramIndexID] PRIMARY KEY,
    [ProgramIndexCode] [varchar](255) NOT NULL,
    [ProgramIndexTitle] [varchar](255) NOT NULL,
    [Biennium] [int] NOT NULL,
    [Activity] [varchar](200) NULL,
    [Program] [varchar](200) NULL,
    [Subprogram] [varchar](200) NULL,
    [Subactivity] [varchar](max) NULL,
    CONSTRAINT [AK_ProgramIndex_ProgramIndexCode_Biennium] UNIQUE ([ProgramIndexCode], [Biennium]),
    CONSTRAINT [AK_ProgramIndex_ProgramIndexTitle_Biennium] UNIQUE ([ProgramIndexTitle], [Biennium])
)
GO
