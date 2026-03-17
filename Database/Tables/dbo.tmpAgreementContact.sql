SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tmpAgreementContact](
	[AgreementContactID] [int] IDENTITY(1,1) NOT NULL,
	[AgreementNumber] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Title] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[FirstName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastName] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[EmailAddress] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Organization] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[AgreementRole] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[F8] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[F9] [nvarchar](255) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_tmpAgreementContact_AgreementContactID] PRIMARY KEY CLUSTERED 
(
	[AgreementContactID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
