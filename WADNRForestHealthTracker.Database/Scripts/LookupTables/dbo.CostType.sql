
--If you are adding a new CostType, make sure you update the following check constraints(if needed):
--CK_InvoiceLineItem_CostTypeValues
SET IDENTITY_INSERT dbo.CostType ON;
merge into dbo.CostType as Target
using (values
(1, 'Indirect Costs', 'IndirectCosts', 1, 60),
(2, 'Supplies', 'Supplies', 1, 40),
(3, 'Personnel', 'Personnel', 1, 10),
(4, 'Benefits', 'Benefits', 1, 20),
(5, 'Travel', 'Travel', 1, 30),
(6, 'Contractual', 'Contractual', 1, 50),
(7, 'Agreements', 'Agreements', 0, 90),
(8, 'Equipment', 'Equipment', 1, 80),
(9, 'Other', 'Other', 1, 70)
) as Source (CostTypeID, CostTypeDisplayName, CostTypeName, IsValidInvoiceLineItemCostType, SortOrder)
on Target.CostTypeID = Source.CostTypeID
when matched then
    update set
        CostTypeDisplayName = Source.CostTypeDisplayName,
        CostTypeName = Source.CostTypeName,
        IsValidInvoiceLineItemCostType = Source.IsValidInvoiceLineItemCostType,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (CostTypeID, CostTypeDisplayName, CostTypeName, IsValidInvoiceLineItemCostType, SortOrder)
    values (CostTypeID, CostTypeDisplayName, CostTypeName, IsValidInvoiceLineItemCostType, SortOrder)
when not matched by source then
    delete;
SET IDENTITY_INSERT dbo.CostType OFF;

