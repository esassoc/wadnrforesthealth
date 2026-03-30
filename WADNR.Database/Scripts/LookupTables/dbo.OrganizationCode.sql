merge into dbo.OrganizationCode as Target
using (values
('1', 'Forest Resilience Division', '5900'),
('2', 'NE region', '2300'),
('3', 'SE region', '0100'),
('4', 'NW region', '1900'),
('5', 'SPS region', '0900'),
('6', 'OLY region', '0200'),
('7', 'PC region', '0400')
) as Source (OrganizationCodeID, OrganizationCodeName, OrganizationCodeValue)
on Target.OrganizationCodeID = Source.OrganizationCodeID
when matched then
    update set
        OrganizationCodeName = Source.OrganizationCodeName,
        OrganizationCodeValue = Source.OrganizationCodeValue
when not matched by target then
    insert (OrganizationCodeID, OrganizationCodeName, OrganizationCodeValue)
    values (OrganizationCodeID, OrganizationCodeName, OrganizationCodeValue)
when not matched by source then
    delete;


