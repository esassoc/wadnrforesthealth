CREATE TABLE [dbo].[tmpAgreementContactsImportTemplate](
	[tmpAgreementContactsImportTemplateID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementNumber] [nvarchar](50) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[LastName] [nvarchar](50) NOT NULL,
	[EmailAddress] [nvarchar](1) NULL,
	[Organization] [nvarchar](100) NULL,
	[AgreementRole] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_tmpAgreementContactsImportTemplate_tmpAgreementContactsImportTemplateID] PRIMARY KEY CLUSTERED 
(
	[tmpAgreementContactsImportTemplateID] ASC
)
) 
GO
