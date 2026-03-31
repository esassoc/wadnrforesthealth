merge into dbo.FundingSource as Target
using (values
(1, 'Federal', 'Federal'),
(2, 'State', 'State'),
(3, 'Private', 'Private'),
(4, 'Other', 'Other')
) as Source (FundingSourceID, FundingSourceName, FundingSourceDisplayName)
on Target.FundingSourceID = Source.FundingSourceID
when matched then
    update set
        FundingSourceName = Source.FundingSourceName,
        FundingSourceDisplayName = Source.FundingSourceDisplayName
when not matched by target then
    insert (FundingSourceID, FundingSourceName, FundingSourceDisplayName)
    values (FundingSourceID, FundingSourceName, FundingSourceDisplayName)
when not matched by source then
    delete;