merge into dbo.InvoiceStatus as Target
using (values
(1, 'Pending', 'Pending'),
(2, 'Paid', 'Paid'),
(3, 'Canceled', 'Canceled')
) as Source (InvoiceStatusID, InvoiceStatusName, InvoiceStatusDisplayName)
on Target.InvoiceStatusID = Source.InvoiceStatusID
when matched then
    update set
        InvoiceStatusName = Source.InvoiceStatusName,
        InvoiceStatusDisplayName = Source.InvoiceStatusDisplayName
when not matched by target then
    insert (InvoiceStatusID, InvoiceStatusName, InvoiceStatusDisplayName)
    values (InvoiceStatusID, InvoiceStatusName, InvoiceStatusDisplayName)
when not matched by source then
    delete;