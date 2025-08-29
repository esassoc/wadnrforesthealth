CREATE TABLE [dbo].[ForesterWorkUnit](
	[ForesterWorkUnitID] [int] IDENTITY(1,1) NOT NULL CONSTRAINT [PK_ForesterWorkUnit_ForesterWorkUnitID] PRIMARY KEY,
	[ForesterRoleID] [int] NOT NULL CONSTRAINT [FK_ForesterWorkUnit_ForesterRole_ForesterRoleID] FOREIGN KEY REFERENCES [dbo].[ForesterRole]([ForesterRoleID]),
	[PersonID] [int] NULL CONSTRAINT [FK_ForesterWorkUnit_Person_PersonID] FOREIGN KEY REFERENCES [dbo].[Person]([PersonID]),
	[ForesterWorkUnitName] [varchar](100) NOT NULL,
	[RegionName] [varchar](100) NULL,
	[ForesterWorkUnitLocation] [geometry] NOT NULL
)
GO
CREATE SPATIAL INDEX [SPATIAL_ForesterWorkUnit_ForesterWorkUnitLocation] ON [dbo].[ForesterWorkUnit]
(
	[ForesterWorkUnitLocation]
)USING  GEOMETRY_AUTO_GRID 
WITH (BOUNDING_BOX =(-125, 45, -116, 50), 
CELLS_PER_OBJECT = 8) 
GO