
merge into dbo.TreatmentDetailedActivityType as Target
using (values
(1, 'Chipping', 'Chipping'),
(2, 'Pruning', 'Pruning'),
(3, 'Thinning', 'Thinning'),
(4, 'Mastication', 'Mastication'),
(5, 'Grazing', 'Grazing'),
(6, 'LopAndScatter', 'Lop and Scatter'),
(7, 'BiomassRemoval', 'Biomass Removal'),
(8, 'HandPile', 'Hand Pile'),
(9, 'BroadcastBurn', 'Broadcast Burn'),
(10, 'HandPileBurn', 'Hand Pile Burn'),
(11, 'MachinePileBurn', 'Machine Pile Burn'),
(12, 'Slash', 'Slash'),
(13, 'Other', 'Other'),
(14, 'JackpotBurn', 'Jackpot Burn'),
(15, 'MachinePile', 'Machine Pile'),
(16, 'FuelBreak', 'Fuel Break'),
(17, 'Planting', 'Planting'),
(18, 'BrushControl', 'Brush Control'),
(19, 'Mowing', 'Mowing'),
(20, 'Regen', 'Regen'),
(21, 'PileBurn', 'Pile Burn')
)
    as Source (TreatmentDetailedActivityTypeID, TreatmentDetailedActivityTypeName, TreatmentDetailedActivityTypeDisplayName)
on Target.TreatmentDetailedActivityTypeID = Source.TreatmentDetailedActivityTypeID
when matched then
    update set
               TreatmentDetailedActivityTypeName = Source.TreatmentDetailedActivityTypeName,
               TreatmentDetailedActivityTypeDisplayName = Source.TreatmentDetailedActivityTypeDisplayName
when not matched by target then
    insert (TreatmentDetailedActivityTypeID, TreatmentDetailedActivityTypeName, TreatmentDetailedActivityTypeDisplayName)
    values (TreatmentDetailedActivityTypeID, TreatmentDetailedActivityTypeName, TreatmentDetailedActivityTypeDisplayName)
when not matched by source then
    delete;