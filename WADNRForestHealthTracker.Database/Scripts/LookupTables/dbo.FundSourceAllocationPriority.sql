merge into dbo.FundSourceAllocationPriority as Target
using (values
(1, '#ff0000', 1),
(2, '#ff9933', 2),
(3, '#ffff99', 3),
(4, '#c5d9f1', 4),
(5, '#c4d79b', 5)
) as Source (FundSourceAllocationPriorityID, FundSourceAllocationPriorityColor, FundSourceAllocationPriorityNumber)
on Target.FundSourceAllocationPriorityID = Source.FundSourceAllocationPriorityID
when matched then
    update set
        FundSourceAllocationPriorityColor = Source.FundSourceAllocationPriorityColor,
        FundSourceAllocationPriorityNumber = Source.FundSourceAllocationPriorityNumber
when not matched by target then
    insert (FundSourceAllocationPriorityID, FundSourceAllocationPriorityColor, FundSourceAllocationPriorityNumber)
    values (FundSourceAllocationPriorityID, FundSourceAllocationPriorityColor, FundSourceAllocationPriorityNumber)
when not matched by source then
    delete;




