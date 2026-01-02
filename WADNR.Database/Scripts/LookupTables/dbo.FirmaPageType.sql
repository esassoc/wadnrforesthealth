merge into dbo.FirmaPageType as Target
using (values
(1, 'HomePage', 'Home Page', 2),
(4, 'DemoScript', 'Demo Script', 2),
(5, 'InternalSetupNotes', 'Internal Setup Notes', 2),
(6, 'FullProjectList', 'Full Project List', 1),
(11, 'ProjectTypeList', 'Project Type List', 1),
(13, 'TaxonomyBranchList', 'Taxonomy Branch List', 1),
(14, 'TaxonomyTrunkList', 'Taxonomy Trunk List', 1),
(16, 'OrganizationsList', 'Organizations List', 1),
(18, 'MyProjects', 'My Projects', 1),
(22, 'ProjectMap', 'Project Map', 1),
(24, 'HomeMapInfo', 'Home Page Map Info', 2),
(25, 'HomeAdditionalInfo', 'Home Page Additional Info', 2),
(26, 'FeaturedProjectList', 'Featured Project List', 1),
(33, 'FullProjectListSimple', 'Full Project List (Simple)', 1),
(34, 'Taxonomy', 'Taxonomy', 1),
(36, 'TagList', 'Tag List', 1),
(41, 'ManageUpdateNotifications', 'Manage Project Update Notifications', 1),
(46, 'ProjectStewardOrganizationList', 'ProjectStewardOrganizationList', 1),
(47, 'EnterHistoricProjectInstructions', 'Enter Historic Project Instructions', 2),
(48, 'PendingProjects', 'Pending Projects', 1),
(49, 'Training','Training', 2),
(51, 'CustomFooter', 'Custom Footer', 1),
(54, 'FactSheetCustomText', 'Fact Sheet Custom Text', 2),
(55, 'FocusAreasList', 'Focus Areas List', 1),
(56, 'FullFundSourceList', 'Full Fund Source List', 1),
(57, 'FullFundSourceAllocationList', 'Full Fund Source Allocation List', 1),
(58, 'RegionsList', 'Regions List', 1),
(59, 'PriorityLandscapesList', 'Priority Landscapes List', 1),
(60, 'FullAgreementList', 'Full Agreement List', 1),
(62, 'InteractionEventList', 'Interaction/Event List', 1),
(63, 'GisUploadAttemptInstructions', 'GIS Upload Attempt Instructions', 1),
(65, 'ProgramsList', 'Programs List', 1),
(66, 'UploadLoaTabularDataExcel', 'Upload Loa Tabular Data', 1),
(67, 'ManageFindYourForester', 'Manage Find Your Forester', 1),
(68, 'FindYourForester', 'Find Your Forester', 1),
(69, 'ExternalMapLayers', 'External Map Layers', 1),
(70, 'County', 'Counties', 1),
(71, 'Reports', 'Reports', 1),
(72, 'ReportProjects', 'Report Projects', 1),
(73, 'ReportAddReport', 'Add a Report', 2),
(74, 'Vendor', 'Vendors', 1),
(75, 'ProjectCreateInstructions', 'Project Create Instructions', 2),
(76, 'ProjectUpdateInstructions', 'Project Update Instructions', 2)
) as Source (FirmaPageTypeID, FirmaPageTypeName, FirmaPageTypeDisplayName, FirmaPageRenderTypeID)
on Target.FirmaPageTypeID = Source.FirmaPageTypeID
when matched then
    update set
        FirmaPageTypeName = Source.FirmaPageTypeName,
        FirmaPageTypeDisplayName = Source.FirmaPageTypeDisplayName,
        FirmaPageRenderTypeID = Source.FirmaPageRenderTypeID
when not matched by target then
    insert (FirmaPageTypeID, FirmaPageTypeName, FirmaPageTypeDisplayName, FirmaPageRenderTypeID)
    values (FirmaPageTypeID, FirmaPageTypeName, FirmaPageTypeDisplayName, FirmaPageRenderTypeID)
when not matched by source then
    delete;
