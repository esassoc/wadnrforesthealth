merge into dbo.ProjectLocationSimpleType as Target
using (values
(1, 'PointOnMap', 'Plot a point on the map', 1),
(2, 'LatLngInput', 'Enter lat/lng coordinates (DD)', 2),
(3, 'None', 'No location', 3)
) as Source (ProjectLocationSimpleTypeID, ProjectLocationSimpleTypeName, DisplayInstructions, DisplayOrder)
on Target.ProjectLocationSimpleTypeID = Source.ProjectLocationSimpleTypeID
when matched then
    update set
        ProjectLocationSimpleTypeName = Source.ProjectLocationSimpleTypeName,
        DisplayInstructions = Source.DisplayInstructions,
        DisplayOrder = Source.DisplayOrder
when not matched by target then
    insert (ProjectLocationSimpleTypeID, ProjectLocationSimpleTypeName, DisplayInstructions, DisplayOrder)
    values (ProjectLocationSimpleTypeID, ProjectLocationSimpleTypeName, DisplayInstructions, DisplayOrder)
when not matched by source then
    delete;