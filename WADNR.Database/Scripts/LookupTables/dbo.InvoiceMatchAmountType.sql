merge into dbo.InvoiceMatchAmountType as Target
using (values
(1, 'DollarAmount', 'Dollar Amount (enter amount in input below)'),
(2, 'N/A', 'N/A'),
(3, 'DNR', 'DNR')
) as Source (InvoiceMatchAmountTypeID, InvoiceMatchAmountTypeName, InvoiceMatchAmountTypeDisplayName)
on Target.InvoiceMatchAmountTypeID = Source.InvoiceMatchAmountTypeID
when matched then
    update set
        InvoiceMatchAmountTypeName = Source.InvoiceMatchAmountTypeName,
        InvoiceMatchAmountTypeDisplayName = Source.InvoiceMatchAmountTypeDisplayName
when not matched by target then
    insert (InvoiceMatchAmountTypeID, InvoiceMatchAmountTypeName, InvoiceMatchAmountTypeDisplayName)
    values (InvoiceMatchAmountTypeID, InvoiceMatchAmountTypeName, InvoiceMatchAmountTypeDisplayName)
when not matched by source then
    delete;