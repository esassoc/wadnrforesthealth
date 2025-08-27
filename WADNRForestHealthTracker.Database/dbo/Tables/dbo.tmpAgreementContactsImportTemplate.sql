SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tmpAgreementContactsImportTemplate](
	[tmpAgreementContactsImportTemplateID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementNumber] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[Title] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[FirstName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[LastName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[EmailAddress] [nvarchar](1) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Organization] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AgreementRole] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_tmpAgreementContactsImportTemplate_tmpAgreementContactsImportTemplateID] PRIMARY KEY CLUSTERED 
(
	[tmpAgreementContactsImportTemplateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
