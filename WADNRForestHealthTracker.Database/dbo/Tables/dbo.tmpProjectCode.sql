SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tmpProjectCode](
	[agency] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subagency] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[project] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[subproject] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[project_phase] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[title] [varchar](2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[project_type] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[project_start_date] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[project_end_date] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[federal_catalog] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[sub_grantee] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[indirect_cost_rate] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[indirect_cost_limit] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[reimbursement_method] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[federal_agency] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[object_posting_level_ind] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[revenue_posting_level_ind] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[approp_cntrl_type_ind] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[cash_cntrl_type_ind] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[gl_project_level] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[create_date] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
