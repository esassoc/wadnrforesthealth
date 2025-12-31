SET IDENTITY_INSERT dbo.ForesterRole ON;

merge into dbo.ForesterRole as Target
using (values
(1, 'Service Forester', 'ServiceForester', 10),
(2, 'Service Forestry Specialist', 'ServiceForestrySpecialist', 160),
(3, 'Forest Practices Forester', 'ForestPracticesForester', 80),
(4, 'Stewardship Fish & Wildlife Biologist', 'StewardshipFishAndWildlifeBiologist', 20),
(5, 'Urban Forestry Technician', 'UrbanForestryTechnician', 120),
(6, 'Community Resilience Coordinator', 'CommunityResilienceCoordinator', 70),
(7, 'Regulation Assistance Forester', 'RegulationAssistanceForester', 30),
(8, 'Family Forest Fish Passage Program', 'FamilyForestFishPassageProgram', 90),
(9, 'Forestry Riparian Easement Program', 'ForestryRiparianEasementProgram', 100),
(10, 'Rivers and Habitat Open Space Program Manager', 'RiversAndHabitatOpenSpaceProgramManager', 110),
(11, 'Service Forestry Program Manager', 'ServiceForestryProgramManager', 130),
(12, 'UCF Statewide Specialist', 'UcfStatewideSpecialist', 150),
(13, 'Small Forest Landowner Office Program Manager', 'SmallForestLandownerOfficeProgramManager', 140),
(14, 'Forest Regulation Fish & Wildlife Biologist', 'ForestRegulationFishAndWildlifeBiologist', 60)
) as Source (ForesterRoleID, ForesterRoleDisplayName, ForesterRoleName, SortOrder)
on Target.ForesterRoleID = Source.ForesterRoleID
when matched then
    update set
        ForesterRoleDisplayName = Source.ForesterRoleDisplayName,
        ForesterRoleName = Source.ForesterRoleName,
        SortOrder = Source.SortOrder
when not matched by target then
    insert (ForesterRoleID, ForesterRoleDisplayName, ForesterRoleName, SortOrder)
    values (ForesterRoleID, ForesterRoleDisplayName, ForesterRoleName, SortOrder)
when not matched by source then
    delete;

SET IDENTITY_INSERT dbo.ForesterRole OFF;


/*

Service Forester
Stewardship Biologist
Regulation Assistance Forester
Family Forest Fish Passage Program
Forestry Riparian Easement Program
Rivers and Habitat Open Space Program
Community Wildfire Preparedness Specialist
Urban Forestry Tech
Forest Practices Forester
Service Forestry Program Manager
Small Forest Landowner Program Manager
UCF Statewide Specialist

select * from dbo.ForesterRole order by SortOrder


*/

