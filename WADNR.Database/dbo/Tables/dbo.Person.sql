CREATE TABLE [dbo].[Person](
    [PersonID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_Person_PersonID] PRIMARY KEY,
    [FirstName] [varchar](100) NOT NULL,
    [LastName] [varchar](100) NULL,
    [Email] [varchar](255) NULL,
    [Phone] [varchar](30) NULL,
    [CreateDate] [datetime] NOT NULL,
    [UpdateDate] [datetime] NULL,
    [LastActivityDate] [datetime] NULL,
    [IsActive] [bit] NOT NULL,
    [OrganizationID] [int] NULL CONSTRAINT [FK_Person_Organization_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID]),
    [ReceiveSupportEmails] [bit] NOT NULL,
    [WebServiceAccessToken] [uniqueidentifier] NULL,
    [MiddleName] [varchar](100) NULL,
    [Notes] [varchar](500) NULL,
    [PersonAddress] [varchar](255) NULL,
    [AddedByPersonID] [int] NULL CONSTRAINT [FK_Person_Person_AddedByPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [VendorID] [int] NULL CONSTRAINT [FK_Person_Vendor_VendorID] FOREIGN KEY REFERENCES [dbo].[Vendor]([VendorID]),
    [IsProgramManager] [bit] NULL,
    [CreatedAsPartOfBulkImport] [bit] NULL,
    [GlobalID] VARCHAR(100) NULL,
    [ImpersonatedPersonID] [int] NULL CONSTRAINT [FK_Person_Person_ImpersonatedPersonID_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Person_Email_UniqueWhenNotNull] ON [dbo].[Person]([Email]) WHERE ([Email] IS NOT NULL)
GO