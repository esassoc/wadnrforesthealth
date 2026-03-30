merge into dbo.CustomPageNavigationSection as Target
using (values
(1, 'About'),
(2, 'Projects'),
(3, 'Financials'),
(4, 'ProgramInfo')
) as Source (CustomPageNavigationSectionID, CustomPageNavigationSectionName)
on Target.CustomPageNavigationSectionID = Source.CustomPageNavigationSectionID
when matched then
    update set
        CustomPageNavigationSectionName = Source.CustomPageNavigationSectionName
when not matched by target then
    insert (CustomPageNavigationSectionID, CustomPageNavigationSectionName)
    values (CustomPageNavigationSectionID, CustomPageNavigationSectionName)
when not matched by source then
    delete;

