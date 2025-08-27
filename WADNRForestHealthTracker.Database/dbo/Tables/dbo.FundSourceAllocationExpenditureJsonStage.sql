SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSourceAllocationExpenditureJsonStage](
	[FundSourceAllocationExpenditureJsonStageID] [int] IDENTITY(1,1) NOT NULL,
	[Biennium] [int] NULL,
	[FiscalMo] [int] NULL,
	[FiscalAdjMo] [int] NULL,
	[CalYr] [int] NULL,
	[MoString] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SourceSystem] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DocNo] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DocSuffix] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DocDate] [datetime] NULL,
	[InvoiceDesc] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[InvoiceDate] [datetime] NULL,
	[GlAcctNo] [int] NULL,
	[ObjCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ObjName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubObjCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubObjName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubSubObjCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubSubObjName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ApprnCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ApprnName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FundCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FundName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrgCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[OrgName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProgIdxCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProgIdxName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProgCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProgName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubProgCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubProgName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ActivityCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ActivityName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubActivityCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SubActivityName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectCd] [varchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProjectName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorNo] [varchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[VendorName] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ExpendAccrued] [money] NULL,
 CONSTRAINT [PK_FundSourceAllocationExpenditureJsonStage_FundSourceAllocationExpenditureJsonStageID] PRIMARY KEY CLUSTERED 
(
	[FundSourceAllocationExpenditureJsonStageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
