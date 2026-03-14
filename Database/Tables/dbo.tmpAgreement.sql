SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tmpAgreement](
	[TmpAgreementID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementType] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AgreementNumber] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[SourceOfFunding] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AgencyEntity] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PM] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ProgramIndexProjectCode] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StartDate] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EndDate] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AgreementAmount] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Expended] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Balance] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[INCLUDED_IN_BALANCE_INV_AMT_SUBMITTED_ON_7_12_OR_AFTER] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Region_Div] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ACTIVITY] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[1ST_BILL_DUE_ON] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Notes] [nvarchar](150) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PARTNER_CONTACT] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_tmpAgreement_TmpAgreementID] PRIMARY KEY CLUSTERED 
(
	[TmpAgreementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
