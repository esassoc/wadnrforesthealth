
merge into dbo.RecurrenceInterval as Target
using (values

           (1, 1, '1 Year', 'OneYear'),
           (2, 5, '5 Years', 'FiveYears'),
           (3, 10, '10 Years', 'TenYears'),
           (4, 15, '15 Years', 'FifteenYears')
)
    as Source (RecurrenceIntervalID, RecurrenceIntervalInYears, RecurrenceIntervalDisplayName, RecurrenceIntervalName)
on Target.RecurrenceIntervalID = Source.RecurrenceIntervalID
when matched then
    update set
               RecurrenceIntervalInYears = Source.RecurrenceIntervalInYears,
               RecurrenceIntervalDisplayName = Source.RecurrenceIntervalDisplayName,
               RecurrenceIntervalName = Source.RecurrenceIntervalName
when not matched by target then
    insert (RecurrenceIntervalID, RecurrenceIntervalInYears, RecurrenceIntervalDisplayName, RecurrenceIntervalName)
    values (RecurrenceIntervalID, RecurrenceIntervalInYears, RecurrenceIntervalDisplayName, RecurrenceIntervalName)
when not matched by source then
    delete;