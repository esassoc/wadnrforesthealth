merge into dbo.Division as Target
using (values
(1, 'ForestHealth', 'DNR Headquarters - Forest Health'),
(2, 'Wildfire', 'DNR Headquarters - Wildfire')
) as Source (DivisionID, DivisionName, DivisionDisplayName)
on Target.DivisionID = Source.DivisionID
when matched then
    update set
        DivisionName = Source.DivisionName,
        DivisionDisplayName = Source.DivisionDisplayName
when not matched by target then
    insert (DivisionID, DivisionName, DivisionDisplayName)
    values (DivisionID, DivisionName, DivisionDisplayName)
when not matched by source then
    delete;