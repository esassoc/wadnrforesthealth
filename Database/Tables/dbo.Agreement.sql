SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agreement](
	[AgreementID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementTypeID] [int] NOT NULL,
	[AgreementNumber] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[AgreementAmount] [money] NULL,
	[ExpendedAmount] [money] NULL,
	[BalanceAmount] [money] NULL,
	[DNRUplandRegionID] [int] NULL,
	[FirstBillDueOn] [datetime] NULL,
	[Notes] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AgreementTitle] [varchar](256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[OrganizationID] [int] NOT NULL,
	[AgreementStatusID] [int] NULL,
	[AgreementFileResourceID] [int] NULL,
	[tmpAgreement2ID] [int] NULL,
 CONSTRAINT [PK_Agreement_AgreementID] PRIMARY KEY CLUSTERED 
(
	[AgreementID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Agreement]  WITH CHECK ADD  CONSTRAINT [FK_Agreement_AgreementStatus_AgreementStatusID] FOREIGN KEY([AgreementStatusID])
REFERENCES [dbo].[AgreementStatus] ([AgreementStatusID])
GO
ALTER TABLE [dbo].[Agreement] CHECK CONSTRAINT [FK_Agreement_AgreementStatus_AgreementStatusID]
GO
ALTER TABLE [dbo].[Agreement]  WITH CHECK ADD  CONSTRAINT [FK_Agreement_AgreementType_AgreementTypeID] FOREIGN KEY([AgreementTypeID])
REFERENCES [dbo].[AgreementType] ([AgreementTypeID])
GO
ALTER TABLE [dbo].[Agreement] CHECK CONSTRAINT [FK_Agreement_AgreementType_AgreementTypeID]
GO
ALTER TABLE [dbo].[Agreement]  WITH CHECK ADD  CONSTRAINT [FK_Agreement_DNRUplandRegion_DNRUplandRegionID] FOREIGN KEY([DNRUplandRegionID])
REFERENCES [dbo].[DNRUplandRegion] ([DNRUplandRegionID])
GO
ALTER TABLE [dbo].[Agreement] CHECK CONSTRAINT [FK_Agreement_DNRUplandRegion_DNRUplandRegionID]
GO
ALTER TABLE [dbo].[Agreement]  WITH CHECK ADD  CONSTRAINT [FK_Agreement_FileResource_AgreementFileResourceID_FileResourceID] FOREIGN KEY([AgreementFileResourceID])
REFERENCES [dbo].[FileResource] ([FileResourceID])
GO
ALTER TABLE [dbo].[Agreement] CHECK CONSTRAINT [FK_Agreement_FileResource_AgreementFileResourceID_FileResourceID]
GO
ALTER TABLE [dbo].[Agreement]  WITH CHECK ADD  CONSTRAINT [FK_Agreement_Organization_OrganizationID] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO
ALTER TABLE [dbo].[Agreement] CHECK CONSTRAINT [FK_Agreement_Organization_OrganizationID]
GO
ALTER TABLE [dbo].[Agreement]  WITH CHECK ADD  CONSTRAINT [FK_Agreement_tmpAgreement2_tmpAgreement2ID] FOREIGN KEY([tmpAgreement2ID])
REFERENCES [dbo].[tmpAgreement2] ([TmpAgreement2ID])
GO
ALTER TABLE [dbo].[Agreement] CHECK CONSTRAINT [FK_Agreement_tmpAgreement2_tmpAgreement2ID]