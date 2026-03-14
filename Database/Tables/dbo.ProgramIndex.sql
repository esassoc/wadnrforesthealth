SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProgramIndex](
	[ProgramIndexID] [int] IDENTITY(1,1) NOT NULL,
	[ProgramIndexCode] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ProgramIndexTitle] [varchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Biennium] [int] NOT NULL,
	[Activity] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Program] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Subprogram] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Subactivity] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_ProgramIndex_ProgramIndexID] PRIMARY KEY CLUSTERED 
(
	[ProgramIndexID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ProgramIndex_ProgramIndexCode_Biennium] UNIQUE NONCLUSTERED 
(
	[ProgramIndexCode] ASC,
	[Biennium] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [AK_ProgramIndex_ProgramIndexTitle_Biennium] UNIQUE NONCLUSTERED 
(
	[ProgramIndexTitle] ASC,
	[Biennium] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
