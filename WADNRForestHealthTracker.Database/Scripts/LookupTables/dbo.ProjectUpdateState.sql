

merge into dbo.ProjectUpdateState as Target
using (values

           (1, 'Created', 'Created'),
           (2, 'Submitted', 'Submitted'),
           (3, 'Returned', 'Returned'),
           (4, 'Approved', 'Approved')
)
    as Source (ProjectUpdateStateID, ProjectUpdateStateName, ProjectUpdateStateDisplayName)
on Target.ActivityTypeID = Source.ActivityTypeID
when matched then
    update set
               ActivityTypeName = Source.ActivityTypeName,
               ActivityTypeDisplayName = Source.ActivityTypeDisplayName
when not matched by target then
    insert (ProjectUpdateStateID, ProjectUpdateStateName, ProjectUpdateStateDisplayName)
    values (ProjectUpdateStateID, ProjectUpdateStateName, ProjectUpdateStateDisplayName)
when not matched by source then
    delete;