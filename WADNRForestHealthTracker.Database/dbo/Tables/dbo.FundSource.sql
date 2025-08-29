CREATE TABLE [dbo].[FundSource](
    [FundSourceID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSource_FundSourceID] PRIMARY KEY,
    [FundSourceNumber] [varchar](30) NULL,
    [StartDate] [datetime] NULL,
    [EndDate] [datetime] NULL,
    [ConditionsAndRequirements] [varchar](max) NULL,
    [ComplianceNotes] [varchar](max) NULL,
    [CFDANumber] [varchar](10) NULL,
    [FundSourceName] [varchar](64) NOT NULL,
    [FundSourceTypeID] [int] NULL CONSTRAINT [FK_FundSource_FundSourceType_FundSourceTypeID] FOREIGN KEY REFERENCES [dbo].[FundSourceType]([FundSourceTypeID]),
    [ShortName] [varchar](64) NULL,
    [FundSourceStatusID] [int] NOT NULL CONSTRAINT [FK_FundSource_FundSourceStatus_FundSourceStatusID] FOREIGN KEY REFERENCES [dbo].[FundSourceStatus]([FundSourceStatusID]),
    [OrganizationID] [int] NOT NULL CONSTRAINT [FK_FundSource_Organization_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID]),
    [TotalAwardAmount] [money] NOT NULL
)
GO