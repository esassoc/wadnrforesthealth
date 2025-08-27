SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LoaStage](
	[LoaStageID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectIdentifier] [varchar](600) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ProjectStatus] [varchar](600) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FundSourceNumber] [varchar](600) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FocusAreaName] [varchar](600) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectExpirationDate] [datetime] NULL,
	[LetterDate] [datetime] NULL,
	[MatchAmount] [money] NULL,
	[PayAmount] [money] NULL,
	[ProgramIndex] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectCode] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[IsNortheast] [bit] NOT NULL,
	[IsSoutheast]  AS (CONVERT([bit],case when [IsNortheast]=(1) then (0) else (1) end)) PERSISTED NOT NULL,
	[ForesterLastName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ForesterFirstName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ForesterPhone] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ForesterEmail] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ApplicationDate] [datetime] NULL,
	[DecisionDate] [datetime] NULL,
 CONSTRAINT [PK_LoaStage_LoaStageID] PRIMARY KEY CLUSTERED 
(
	[LoaStageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING ON

GO
CREATE NONCLUSTERED INDEX [IDX_LoaStageGrantNumber] ON [dbo].[LoaStage]
(
	[FundSourceNumber] ASC
)
INCLUDE([FocusAreaName],[IsSoutheast]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]