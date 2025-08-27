SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FundSource](
	[FundSourceID] [int] IDENTITY(1,1) NOT NULL,
	[FundSourceNumber] [varchar](30) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[ConditionsAndRequirements] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ComplianceNotes] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[CFDANumber] [varchar](10) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FundSourceName] [varchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FundSourceTypeID] [int] NULL,
	[ShortName] [varchar](64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FundSourceStatusID] [int] NOT NULL,
	[OrganizationID] [int] NOT NULL,
	[TotalAwardAmount] [money] NOT NULL,
 CONSTRAINT [PK_FundSource_FundSourceID] PRIMARY KEY CLUSTERED 
(
	[FundSourceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[FundSource]  WITH CHECK ADD  CONSTRAINT [FK_FundSource_FundSourceStatus_FundSourceStatusID] FOREIGN KEY([FundSourceStatusID])
REFERENCES [dbo].[FundSourceStatus] ([FundSourceStatusID])
GO
ALTER TABLE [dbo].[FundSource] CHECK CONSTRAINT [FK_FundSource_FundSourceStatus_FundSourceStatusID]
GO
ALTER TABLE [dbo].[FundSource]  WITH CHECK ADD  CONSTRAINT [FK_FundSource_FundSourceType_FundSourceTypeID] FOREIGN KEY([FundSourceTypeID])
REFERENCES [dbo].[FundSourceType] ([FundSourceTypeID])
GO
ALTER TABLE [dbo].[FundSource] CHECK CONSTRAINT [FK_FundSource_FundSourceType_FundSourceTypeID]
GO
ALTER TABLE [dbo].[FundSource]  WITH CHECK ADD  CONSTRAINT [FK_FundSource_Organization_OrganizationID] FOREIGN KEY([OrganizationID])
REFERENCES [dbo].[Organization] ([OrganizationID])
GO
ALTER TABLE [dbo].[FundSource] CHECK CONSTRAINT [FK_FundSource_Organization_OrganizationID]