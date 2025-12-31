CREATE TABLE [dbo].[PersonStewardOrganization](
    [PersonStewardOrganizationID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_PersonStewardOrganization_PersonStewardOrganizationID] PRIMARY KEY,
    [PersonID] [int] NOT NULL CONSTRAINT [FK_PersonStewardOrganization_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
    [OrganizationID] [int] NOT NULL CONSTRAINT [FK_PersonStewardOrganization_Organization_OrganizationID] FOREIGN KEY REFERENCES [dbo].[Organization]([OrganizationID])
)
GO