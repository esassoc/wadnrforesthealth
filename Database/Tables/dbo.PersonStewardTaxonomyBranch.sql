SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonStewardTaxonomyBranch](
	[PersonStewardTaxonomyBranchID] [int] IDENTITY(1,1) NOT NULL,
	[PersonID] [int] NOT NULL,
	[TaxonomyBranchID] [int] NOT NULL,
 CONSTRAINT [PK_PersonStewardTaxonomyBranch_PersonStewardTaxonomyBranchID] PRIMARY KEY CLUSTERED 
(
	[PersonStewardTaxonomyBranchID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[PersonStewardTaxonomyBranch]  WITH CHECK ADD  CONSTRAINT [FK_PersonStewardTaxonomyBranch_Person_PersonID] FOREIGN KEY([PersonID])
REFERENCES [dbo].[Person] ([PersonID])
GO
ALTER TABLE [dbo].[PersonStewardTaxonomyBranch] CHECK CONSTRAINT [FK_PersonStewardTaxonomyBranch_Person_PersonID]
GO
ALTER TABLE [dbo].[PersonStewardTaxonomyBranch]  WITH CHECK ADD  CONSTRAINT [FK_PersonStewardTaxonomyBranch_TaxonomyBranch_TaxonomyBranchID] FOREIGN KEY([TaxonomyBranchID])
REFERENCES [dbo].[TaxonomyBranch] ([TaxonomyBranchID])
GO
ALTER TABLE [dbo].[PersonStewardTaxonomyBranch] CHECK CONSTRAINT [FK_PersonStewardTaxonomyBranch_TaxonomyBranch_TaxonomyBranchID]