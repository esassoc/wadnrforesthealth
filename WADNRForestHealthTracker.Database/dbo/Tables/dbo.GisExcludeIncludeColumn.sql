CREATE TABLE [dbo].[GisExcludeIncludeColumn](
    [GisExcludeIncludeColumnID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_GisExcludeIncludeColumn_GisExcludeIncludeColumnID] PRIMARY KEY,
    [GisUploadSourceOrganizationID] [int] NOT NULL CONSTRAINT [FK_GisExcludeIncludeColumn_GisUploadSourceOrganization_GisUploadSourceOrganizationID] FOREIGN KEY REFERENCES [dbo].[GisUploadSourceOrganization]([GisUploadSourceOrganizationID]),
    [GisDefaultMappingColumnName] [varchar](300) NOT NULL,
    [IsWhitelist] [bit] NOT NULL,
    [IsBlacklist]  AS (CONVERT([bit],case when [IsWhitelist]=(1) then (0) else (1) end))
)
GO