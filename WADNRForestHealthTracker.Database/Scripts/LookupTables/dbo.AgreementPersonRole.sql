
merge into dbo.AgreementPersonRole as Target
using (values

           (1, 'ContractManager', 'Contract Manager'),
           (2, 'ProjectManager', 'Project Manager'),
           (3, 'ProjectCoordinator', 'Project Coordinator'),
           (4, 'Signer', 'Signer'),
           (5, 'TechnicalContact', 'Technical Contact')
)
    as Source (AgreementPersonRoleID, AgreementPersonRoleName, AgreementPersonRoleDisplayName)
on Target.AgreementPersonRoleID = Source.AgreementPersonRoleID
when matched then
    update set
               AgreementPersonRoleName = Source.AgreementPersonRoleName,
               AgreementPersonRoleDisplayName = Source.AgreementPersonRoleDisplayName
when not matched by target then
    insert (AgreementPersonRoleID, AgreementPersonRoleName, AgreementPersonRoleDisplayName)
    values (AgreementPersonRoleID, AgreementPersonRoleName, AgreementPersonRoleDisplayName)
when not matched by source then
    delete;