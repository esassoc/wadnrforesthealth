CREATE TABLE [dbo].[tmpAgreement](
	[TmpAgreementID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementType] [nvarchar](50) NULL,
	[AgreementNumber] [nvarchar](50) NULL,
	[SourceOfFunding] [nvarchar](100) NULL,
	[AgencyEntity] [nvarchar](100) NULL,
	[PM] [nvarchar](50) NULL,
	[ProgramIndexProjectCode] [nvarchar](50) NULL,
	[StartDate] [nvarchar](50) NULL,
	[EndDate] [nvarchar](50) NULL,
	[AgreementAmount] [nvarchar](50) NULL,
	[Expended] [nvarchar](50) NULL,
	[Balance] [nvarchar](50) NULL,
	[INCLUDED_IN_BALANCE_INV_AMT_SUBMITTED_ON_7_12_OR_AFTER] [nvarchar](1) NULL,
	[Region_Div] [nvarchar](50) NULL,
	[ACTIVITY] [nvarchar](50) NULL,
	[1ST_BILL_DUE_ON] [nvarchar](50) NULL,
	[Notes] [nvarchar](150) NULL,
	[PARTNER_CONTACT] [nvarchar](50) NULL,
 CONSTRAINT [PK_tmpAgreement_TmpAgreementID] PRIMARY KEY CLUSTERED 
(
	[TmpAgreementID] ASC
)

