merge into dbo.ProjectImageTiming as Target
using (values
(1, 'After', 'After', 30),
(2, 'Before', 'Before', 10),
(3, 'During', 'During', 20),
(4, 'Unknown', 'Unknown', 40),
(5, 'DesiredFutureConditions', 'Desired Future Conditions', 35)
) as Source (ProjectImageTimingID, ProjectImageTimingName, ProjectImageTimingDisplayName, SortOrder)
on Target.ProjectImageTimingID = Source.ProjectImageTimingID
when matched then
    update set
        ProjectImageTimingName = Source.ProjectImageTimingName,
        ProjectImageTimingDisplayName = Source.ProjectImageTimingDisplayName,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (ProjectImageTimingID, ProjectImageTimingName, ProjectImageTimingDisplayName, SortOrder)
    values (ProjectImageTimingID, ProjectImageTimingName, ProjectImageTimingDisplayName, SortOrder)
when not matched by source then
    delete;
