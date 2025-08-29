
merge into dbo.TreatmentType as Target
using (values
(1, 'Commercial', 'Commercial'),
(2, 'PrescribedFire', 'Prescribed Fire'),
(3, 'NonCommercial', 'Non-Commercial'),
(4, 'Other', 'Other')

)
    as Source (TreatmentTypeID, TreatmentTypeName, TreatmentTypeDisplayName)
on Target.TreatmentTypeID = Source.TreatmentTypeID
when matched then
    update set
               TreatmentTypeName = Source.TreatmentTypeName,
               TreatmentTypeDisplayName = Source.TreatmentTypeDisplayName
when not matched by target then
    insert (TreatmentTypeID, TreatmentTypeName, TreatmentTypeDisplayName)
    values (TreatmentTypeID, TreatmentTypeName, TreatmentTypeDisplayName)
when not matched by source then
    delete;
    