merge into dbo.ProjectLocationType as Target
using (values
(1, 'ProjectArea', 'Project Area', '#2c96c3'),
(2, 'TreatmentArea', 'Treatment Area', '#2b7ac3'),
(3, 'ResearchPlot', 'Research Plot', '#2a44c3'),
(4, 'TestSite', 'Test Site', '#3e29c3'),
(5, 'Other', 'Other', '#6928c3')
) as Source (ProjectLocationTypeID, ProjectLocationTypeName, ProjectLocationTypeDisplayName, ProjectLocationTypeMapLayerColor)
on Target.ProjectLocationTypeID = Source.ProjectLocationTypeID
when matched then
    update set
        ProjectLocationTypeName = Source.ProjectLocationTypeName,
        ProjectLocationTypeDisplayName = Source.ProjectLocationTypeDisplayName,
        ProjectLocationTypeMapLayerColor = Source.ProjectLocationTypeMapLayerColor
when not matched by target then
    insert (ProjectLocationTypeID, ProjectLocationTypeName, ProjectLocationTypeDisplayName, ProjectLocationTypeMapLayerColor)
    values (ProjectLocationTypeID, ProjectLocationTypeName, ProjectLocationTypeDisplayName, ProjectLocationTypeMapLayerColor)
when not matched by source then
    delete;
