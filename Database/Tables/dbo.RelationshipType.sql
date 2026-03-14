SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelationshipType](
	[RelationshipTypeID] [int] IDENTITY(1,1) NOT NULL,
	[RelationshipTypeName] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[CanStewardProjects] [bit] NOT NULL,
	[IsPrimaryContact] [bit] NOT NULL,
	[CanOnlyBeRelatedOnceToAProject] [bit] NOT NULL,
	[RelationshipTypeDescription] [varchar](360) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ReportInAccomplishmentsDashboard] [bit] NOT NULL,
	[ShowOnFactSheet] [bit] NOT NULL,
 CONSTRAINT [PK_RelationshipType_RelationshipTypeID] PRIMARY KEY CLUSTERED 
(
	[RelationshipTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
