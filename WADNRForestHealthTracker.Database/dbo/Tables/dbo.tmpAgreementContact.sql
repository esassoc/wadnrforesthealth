CREATE TABLE [dbo].[tmpAgreementContact](
    [AgreementContactID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_TmpAgreementContact_AgreementContactID] PRIMARY KEY,
    [AgreementNumber] [varchar](255) NULL,
    [Title] [varchar](255) NULL,
    [FirstName] [varchar](255) NULL,
    [LastName] [varchar](255) NULL,
    [EmailAddress] [varchar](255) NULL,
    [Organization] [varchar](255) NULL,
    [AgreementRole] [varchar](255) NULL,
    [F8] [varchar](255) NULL,
    [F9] [varchar](255) NULL
)
GO
