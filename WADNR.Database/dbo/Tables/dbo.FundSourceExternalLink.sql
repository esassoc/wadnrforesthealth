CREATE TABLE [dbo].[FundSourceExternalLink](
	[FundSourceExternalLinkID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_FundSourceExternalLink_FundSourceExternalLinkID] PRIMARY KEY,
	[FundSourceID] [int] NOT NULL CONSTRAINT [FK_FundSourceExternalLink_FundSource_FundSourceID] FOREIGN KEY REFERENCES [dbo].[FundSource]([FundSourceID]),
	[ExternalLinkLabel] [varchar](300) NOT NULL,
	[ExternalLinkUrl] [varchar](300) NOT NULL
)
GO
CREATE NONCLUSTERED INDEX [IX_FundSourceExternalLink_FundSourceID] ON [dbo].[FundSourceExternalLink]([FundSourceID])
GO
