merge into dbo.InteractionEventType as Target
using (values
(1, 'Complaint', 'Complaint'),
(2, 'FireSafetyPresentation', 'Fire Safety Presentation'),
(3, 'ForestLandownerFieldDay', 'Forest Landowner Field Day'),
(4, 'Other', 'Other'),
(5, 'Outreach', 'Education and Outreach'),
(6, 'PhoneCall', 'Phone Call'),
(7, 'SiteVisit', 'Site Visit or Field Trip'),
(8, 'TechnicalAssistance', 'Technical Assistance'),
(9, 'Workshop', 'Workshop'),
(10, 'ResearchMonitoring', 'Research and Monitoring')
) as Source (InteractionEventTypeID, InteractionEventTypeName, InteractionEventTypeDisplayName)
on Target.InteractionEventTypeID = Source.InteractionEventTypeID
when matched then
    update set
        InteractionEventTypeName = Source.InteractionEventTypeName,
        InteractionEventTypeDisplayName = Source.InteractionEventTypeDisplayName
when not matched by target then
    insert (InteractionEventTypeID, InteractionEventTypeName, InteractionEventTypeDisplayName)
    values (InteractionEventTypeID, InteractionEventTypeName, InteractionEventTypeDisplayName)
when not matched by source then
    delete;
