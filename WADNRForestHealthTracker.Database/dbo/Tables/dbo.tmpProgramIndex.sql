SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tmpProgramIndex](
	[fy_biennium] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[agency] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subagency] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[program_index] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[title] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[program_function] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[program] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subprogram] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[activity] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subactivity] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[task] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[allot_pgm_level_ind] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[gl_program_ind] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[program_index_restrictor] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[last_process_date] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[program_index_used_ind] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
