merge into dbo.FocusAreaStatus as Target
using (values
(1, 'Planned', 'Planned'),
(2, 'In Progress', 'In Progress'),
(3, 'Completed', 'Completed')
) as Source (FocusAreaStatusID, FocusAreaStatusName, FocusAreaStatusDisplayName)
on Target.FocusAreaStatusID = Source.FocusAreaStatusID
when matched then
    update set
        FocusAreaStatusName = Source.FocusAreaStatusName,
        FocusAreaStatusDisplayName = Source.FocusAreaStatusDisplayName
when not matched by target then
    insert (FocusAreaStatusID, FocusAreaStatusName, FocusAreaStatusDisplayName)
    values (FocusAreaStatusID, FocusAreaStatusName, FocusAreaStatusDisplayName)
when not matched by source then
    delete;
