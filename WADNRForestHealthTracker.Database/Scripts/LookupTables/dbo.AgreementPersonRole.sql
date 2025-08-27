
delete from dbo.AgreementPersonRole

insert dbo.AgreementPersonRole (AgreementPersonRoleID, AgreementPersonRoleName, AgreementPersonRoleDisplayName) 
values 
(1, 'ContractManager', 'Contract Manager'),
(2, 'ProjectManager', 'Project Manager'),
(3, 'ProjectCoordinator', 'Project Coordinator'),
(4, 'Signer', 'Signer'),
(5, 'TechnicalContact', 'Technical Contact')
