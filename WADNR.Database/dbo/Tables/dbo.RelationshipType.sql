CREATE TABLE [dbo].[RelationshipType](
    [RelationshipTypeID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_RelationshipType_RelationshipTypeID] PRIMARY KEY,
    [RelationshipTypeName] [varchar](200) NOT NULL,
    [CanStewardProjects] [bit] NOT NULL,
    [IsPrimaryContact] [bit] NOT NULL,
    [CanOnlyBeRelatedOnceToAProject] [bit] NOT NULL,
    [RelationshipTypeDescription] [varchar](360) NULL,
    [ReportInAccomplishmentsDashboard] [bit] NOT NULL,
    [ShowOnFactSheet] [bit] NOT NULL
)
GO
