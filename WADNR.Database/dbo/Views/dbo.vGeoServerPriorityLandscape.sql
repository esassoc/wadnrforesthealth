
create view dbo.vGeoServerPriorityLandscape
as
select
	pa.PriorityLandscapeID,
	pa.PriorityLandscapeID as PrimaryKey,
	pa.PriorityLandscapeName,
	pa.PriorityLandscapeLocation,
	pa.PriorityLandscapeLocation as Ogr_Geometry,
	pa.PriorityLandscapeCategoryID,
	plt.PriorityLandscapeCategoryName,
	plt.PriorityLandscapeCategoryMapLayerColor as MapColor
from
	dbo.PriorityLandscape as pa
	join dbo.PriorityLandscapeCategory as plt on pa.PriorityLandscapeCategoryID = plt.PriorityLandscapeCategoryID
