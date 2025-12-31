merge into dbo.ActivityType as Target
using (values

(1, 'Travel', 'Travel'),
(2, 'StaffTime', 'Staff Time'),
(3, 'Treatment', 'Treatment'),
(4, 'ContractorTime', 'Contractor Time'),
(5, 'Supplies', 'Supplies')
)
as Source (ActivityTypeID, ActivityTypeName, ActivityTypeDisplayName)
on Target.ActivityTypeID = Source.ActivityTypeID
when matched then
update set
	ActivityTypeName = Source.ActivityTypeName,
	ActivityTypeDisplayName = Source.ActivityTypeDisplayName
when not matched by target then
	insert (ActivityTypeID, ActivityTypeName, ActivityTypeDisplayName)
	values (ActivityTypeID, ActivityTypeName, ActivityTypeDisplayName)
when not matched by source then
	delete;
