SET IDENTITY_INSERT dbo.FundSourceStatus ON;
merge into dbo.FundSourceStatus as Target
using (values
(1, 'Active'),
(2, 'Pending'),
(3, 'Planned'),
(4, 'Closeout')
) as Source (FundSourceStatusID, FundSourceStatusName)
on Target.FundSourceStatusID = Source.FundSourceStatusID
when matched then
    update set
        FundSourceStatusName = Source.FundSourceStatusName
when not matched by target then
    insert (FundSourceStatusID, FundSourceStatusName)
    values (FundSourceStatusID, FundSourceStatusName)
when not matched by source then
    delete;
SET IDENTITY_INSERT dbo.FundSourceStatus OFF;

