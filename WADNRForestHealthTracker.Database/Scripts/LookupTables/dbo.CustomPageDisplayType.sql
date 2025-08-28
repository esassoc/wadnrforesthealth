merge into dbo.CustomPageDisplayType as Target
using (values
(1, 'Disabled', 'Disabled', 'Not visible to any users'),
(2, 'Public', 'Public', 'Visible to all users'),
(3, 'Protected', 'Protected', 'Visible to logged in users only')
) as Source (CustomPageDisplayTypeID, CustomPageDisplayTypeName, CustomPageDisplayTypeDisplayName, CustomPageDisplayTypeDescription)
on Target.CustomPageDisplayTypeID = Source.CustomPageDisplayTypeID
when matched then
    update set
        CustomPageDisplayTypeName = Source.CustomPageDisplayTypeName,
        CustomPageDisplayTypeDisplayName = Source.CustomPageDisplayTypeDisplayName,
        CustomPageDisplayTypeDescription = Source.CustomPageDisplayTypeDescription
when not matched by target then
    insert (CustomPageDisplayTypeID, CustomPageDisplayTypeName, CustomPageDisplayTypeDisplayName, CustomPageDisplayTypeDescription)
    values (CustomPageDisplayTypeID, CustomPageDisplayTypeName, CustomPageDisplayTypeDisplayName, CustomPageDisplayTypeDescription)
when not matched by source then
    delete;

