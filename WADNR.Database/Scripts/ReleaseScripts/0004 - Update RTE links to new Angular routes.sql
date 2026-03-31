DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0004 - Update RTE links to new Angular routes'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    PRINT @MigrationName;
    SET XACT_ABORT ON;
    BEGIN TRANSACTION;

    -- ============================================================
    -- This migration updates RTE HTML content that contains links
    -- to old MVC route patterns, replacing them with new Angular routes.
    --
    -- Strategy:
    --   1. Normalize absolute URLs to relative paths
    --   2. Replace route patterns WITH leading slash first
    --   3. Replace route patterns WITHOUT leading slash (relative URLs)
    --   4. Order: most-specific patterns first to avoid partial matches
    -- ============================================================

    CREATE TABLE #RouteMap (
        SortOrder INT IDENTITY(1,1),
        OldPattern NVARCHAR(200),
        NewPattern NVARCHAR(200)
    );

    -- Step 1: Absolute URL normalization (must run first, https before http to avoid partial match)
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('https://foresthealthtracker.dnr.wa.gov/', '/'),
        ('http://foresthealthtracker.dnr.wa.gov/', '/');

    -- Step 2: Non-standard routes WITH leading slash (most specific / longest first)
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('/ProgramInfo/ClassificationSystem/11', '/projects-by-theme'),
        ('/ProgramInfo/Taxonomy', '/projects-by-type'),
        ('/Results/ProjectMap', '/projects/map'),
        ('/Project/FeaturedList', '/projects/featured'),
        ('/Project/MyProjects', '/projects/my'),
        ('/Project/Pending', '/projects/pending'),
        ('/FindYourForester/Manage', '/manage-find-your-forester'),
        ('/FindYourForester/Index', '/find-your-forester'),
        ('/FindYourForester/', '/find-your-forester'),
        ('/Home/ManageHomePageImages', '/homepage-configuration'),
        ('/Home/InternalSetupNotes', '/internal-setup-notes'),
        ('/ExcelUpload/ManageExcelUploadsAndEtl', '/upload-excel-files'),
        ('/Account/Login', '/login'),
        ('/Help/Support', '/support'),
        ('/FileResource/DisplayResource/', '/api/file-resources/');

    -- Step 3: Entity detail routes WITH leading slash
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('/Project/Detail/', '/projects/'),
        ('/Agreement/Detail/', '/agreements/'),
        ('/FundSourceAllocation/Detail/', '/fund-source-allocations/'),
        ('/FundSource/Detail/', '/fund-sources/'),
        ('/Program/Detail/', '/programs/'),
        ('/Organization/Detail/', '/organizations/'),
        ('/InteractionEvent/Detail/', '/interactions-events/'),
        ('/Tag/Detail/', '/tags/'),
        ('/County/Detail/', '/counties/'),
        ('/PriorityLandscape/Detail/', '/priority-landscapes/'),
        ('/DNRUplandRegion/Detail/', '/dnr-upland-regions/'),
        ('/FocusArea/Detail/', '/focus-areas/'),
        ('/Classification/Detail/', '/classifications/'),
        ('/User/Detail/', '/people/'),
        ('/Person/Detail/', '/people/');

    -- Step 4: Entity index routes WITH leading slash
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('/Project/Index', '/projects'),
        ('/Agreement/Index', '/agreements'),
        ('/FundSource/Index', '/fund-sources'),
        ('/Program/Index', '/programs'),
        ('/Organization/Index', '/organizations'),
        ('/InteractionEvent/Index', '/interactions-events'),
        ('/Tag/Index', '/tags'),
        ('/County/Index', '/counties'),
        ('/PriorityLandscape/Index', '/priority-landscapes'),
        ('/DNRUplandRegion/Index', '/dnr-upland-regions'),
        ('/FocusArea/Index', '/focus-areas'),
        ('/FieldDefinition/Index', '/labels-and-definitions'),
        ('/Person/Index', '/people'),
        ('/User/Index', '/people');

    -- Step 5: /About/ vanity URL WITH leading slash
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('/About/', '/');

    -- Step 6: Non-standard routes WITHOUT leading slash (relative URLs)
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('ProgramInfo/ClassificationSystem/11', '/projects-by-theme'),
        ('ProgramInfo/Taxonomy', '/projects-by-type'),
        ('Results/ProjectMap', '/projects/map'),
        ('Project/FeaturedList', '/projects/featured'),
        ('Project/MyProjects', '/projects/my'),
        ('Project/Pending', '/projects/pending'),
        ('FindYourForester/Manage', '/manage-find-your-forester'),
        ('FindYourForester/Index', '/find-your-forester'),
        ('Home/ManageHomePageImages', '/homepage-configuration'),
        ('Home/InternalSetupNotes', '/internal-setup-notes'),
        ('ExcelUpload/ManageExcelUploadsAndEtl', '/upload-excel-files'),
        ('Account/Login', '/login'),
        ('Help/Support', '/support'),
        ('FileResource/DisplayResource/', '/api/file-resources/');

    -- Step 7: Entity detail routes WITHOUT leading slash
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('Project/Detail/', '/projects/'),
        ('Agreement/Detail/', '/agreements/'),
        ('FundSourceAllocation/Detail/', '/fund-source-allocations/'),
        ('FundSource/Detail/', '/fund-sources/'),
        ('Program/Detail/', '/programs/'),
        ('Organization/Detail/', '/organizations/'),
        ('InteractionEvent/Detail/', '/interactions-events/'),
        ('Tag/Detail/', '/tags/'),
        ('County/Detail/', '/counties/'),
        ('PriorityLandscape/Detail/', '/priority-landscapes/'),
        ('DNRUplandRegion/Detail/', '/dnr-upland-regions/'),
        ('FocusArea/Detail/', '/focus-areas/'),
        ('Classification/Detail/', '/classifications/'),
        ('User/Detail/', '/people/'),
        ('Person/Detail/', '/people/');

    -- Step 8: Entity index routes WITHOUT leading slash
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('Project/Index', '/projects'),
        ('Agreement/Index', '/agreements'),
        ('FundSource/Index', '/fund-sources'),
        ('Program/Index', '/programs'),
        ('Organization/Index', '/organizations'),
        ('InteractionEvent/Index', '/interactions-events'),
        ('Tag/Index', '/tags'),
        ('County/Index', '/counties'),
        ('PriorityLandscape/Index', '/priority-landscapes'),
        ('DNRUplandRegion/Index', '/dnr-upland-regions'),
        ('FocusArea/Index', '/focus-areas'),
        ('FieldDefinition/Index', '/labels-and-definitions'),
        ('Person/Index', '/people'),
        ('User/Index', '/people');

    -- ============================================================
    -- Apply replacements to each RTE column
    -- ============================================================

    DECLARE @OldPattern NVARCHAR(200), @NewPattern NVARCHAR(200);

    DECLARE route_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT OldPattern, NewPattern FROM #RouteMap ORDER BY SortOrder;

    OPEN route_cursor;
    FETCH NEXT FROM route_cursor INTO @OldPattern, @NewPattern;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        UPDATE dbo.FirmaPage SET FirmaPageContent = REPLACE(FirmaPageContent, @OldPattern, @NewPattern) WHERE FirmaPageContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.CustomPage SET CustomPageContent = REPLACE(CustomPageContent, @OldPattern, @NewPattern) WHERE CustomPageContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.FieldDefinition SET DefaultDefinition = REPLACE(DefaultDefinition, @OldPattern, @NewPattern) WHERE DefaultDefinition LIKE '%' + @OldPattern + '%';

        UPDATE dbo.FieldDefinitionDatum SET FieldDefinitionDatumValue = REPLACE(FieldDefinitionDatumValue, @OldPattern, @NewPattern) WHERE FieldDefinitionDatumValue LIKE '%' + @OldPattern + '%';

        UPDATE dbo.PriorityLandscape SET PriorityLandscapeDescription = REPLACE(PriorityLandscapeDescription, @OldPattern, @NewPattern) WHERE PriorityLandscapeDescription LIKE '%' + @OldPattern + '%';

        UPDATE dbo.PriorityLandscape SET PriorityLandscapeExternalResources = REPLACE(PriorityLandscapeExternalResources, @OldPattern, @NewPattern) WHERE PriorityLandscapeExternalResources LIKE '%' + @OldPattern + '%';

        UPDATE dbo.FindYourForesterQuestion SET ResultsBonusContent = REPLACE(ResultsBonusContent, @OldPattern, @NewPattern) WHERE ResultsBonusContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.ClassificationSystem SET ClassificationSystemDefinition = REPLACE(ClassificationSystemDefinition, @OldPattern, @NewPattern) WHERE ClassificationSystemDefinition LIKE '%' + @OldPattern + '%';

        UPDATE dbo.ClassificationSystem SET ClassificationSystemListPageContent = REPLACE(ClassificationSystemListPageContent, @OldPattern, @NewPattern) WHERE ClassificationSystemListPageContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.DNRUplandRegion SET RegionContent = REPLACE(RegionContent, @OldPattern, @NewPattern) WHERE RegionContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.ProjectType SET ProjectTypeDescription = REPLACE(ProjectTypeDescription, @OldPattern, @NewPattern) WHERE ProjectTypeDescription LIKE '%' + @OldPattern + '%';

        UPDATE dbo.ProjectUpdateConfiguration SET ProjectUpdateKickOffIntroContent = REPLACE(ProjectUpdateKickOffIntroContent, @OldPattern, @NewPattern) WHERE ProjectUpdateKickOffIntroContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.ProjectUpdateConfiguration SET ProjectUpdateReminderIntroContent = REPLACE(ProjectUpdateReminderIntroContent, @OldPattern, @NewPattern) WHERE ProjectUpdateReminderIntroContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.ProjectUpdateConfiguration SET ProjectUpdateCloseOutIntroContent = REPLACE(ProjectUpdateCloseOutIntroContent, @OldPattern, @NewPattern) WHERE ProjectUpdateCloseOutIntroContent LIKE '%' + @OldPattern + '%';

        UPDATE dbo.TaxonomyBranch SET TaxonomyBranchDescription = REPLACE(TaxonomyBranchDescription, @OldPattern, @NewPattern) WHERE TaxonomyBranchDescription LIKE '%' + @OldPattern + '%';

        UPDATE dbo.TaxonomyTrunk SET TaxonomyTrunkDescription = REPLACE(TaxonomyTrunkDescription, @OldPattern, @NewPattern) WHERE TaxonomyTrunkDescription LIKE '%' + @OldPattern + '%';

        FETCH NEXT FROM route_cursor INTO @OldPattern, @NewPattern;
    END

    CLOSE route_cursor;
    DEALLOCATE route_cursor;
    DROP TABLE #RouteMap;

    -- ============================================================
    -- Handle FactSheet links: /Project/FactSheet/{ID} -> /projects/{ID}/fact-sheet
    -- Also handles without leading slash: Project/FactSheet/{ID}
    -- ============================================================

    CREATE TABLE #FSColumns (TableName NVARCHAR(100), ColumnName NVARCHAR(100));
    INSERT INTO #FSColumns VALUES
        ('FirmaPage', 'FirmaPageContent'),
        ('CustomPage', 'CustomPageContent'),
        ('FieldDefinition', 'DefaultDefinition'),
        ('FieldDefinitionDatum', 'FieldDefinitionDatumValue'),
        ('PriorityLandscape', 'PriorityLandscapeDescription'),
        ('PriorityLandscape', 'PriorityLandscapeExternalResources'),
        ('FindYourForesterQuestion', 'ResultsBonusContent'),
        ('ClassificationSystem', 'ClassificationSystemDefinition'),
        ('ClassificationSystem', 'ClassificationSystemListPageContent'),
        ('DNRUplandRegion', 'RegionContent'),
        ('ProjectType', 'ProjectTypeDescription'),
        ('ProjectUpdateConfiguration', 'ProjectUpdateKickOffIntroContent'),
        ('ProjectUpdateConfiguration', 'ProjectUpdateReminderIntroContent'),
        ('ProjectUpdateConfiguration', 'ProjectUpdateCloseOutIntroContent'),
        ('TaxonomyBranch', 'TaxonomyBranchDescription'),
        ('TaxonomyTrunk', 'TaxonomyTrunkDescription');

    -- Process both variants: with leading slash (LEN = 20) and without (LEN = 19)
    CREATE TABLE #FSPatterns (Pattern NVARCHAR(50), PrefixLen INT);
    INSERT INTO #FSPatterns VALUES
        ('/Project/FactSheet/', 20),
        ('Project/FactSheet/', 19);

    DECLARE @FSTable NVARCHAR(100), @FSColumn NVARCHAR(100), @SQL NVARCHAR(MAX);
    DECLARE @FSPattern NVARCHAR(50), @FSPrefixLen INT;

    DECLARE fs_pattern_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT Pattern, PrefixLen FROM #FSPatterns;

    OPEN fs_pattern_cursor;
    FETCH NEXT FROM fs_pattern_cursor INTO @FSPattern, @FSPrefixLen;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE fs_cursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT TableName, ColumnName FROM #FSColumns;

        OPEN fs_cursor;
        FETCH NEXT FROM fs_cursor INTO @FSTable, @FSColumn;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @SQL = N'
            WHILE EXISTS (SELECT 1 FROM dbo.' + @FSTable + N' WHERE ' + @FSColumn + N' LIKE ''%' + @FSPattern + N'[0-9]%'')
            BEGIN
                UPDATE f
                SET f.' + @FSColumn + N' =
                    LEFT(f.' + @FSColumn + N', m.MatchStart - 1)
                    + ''/projects/'' + parts.FactSheetID + ''/fact-sheet''
                    + SUBSTRING(f.' + @FSColumn + N', parts.MatchEnd, LEN(f.' + @FSColumn + N'))
                FROM dbo.' + @FSTable + N' f
                CROSS APPLY (
                    SELECT PATINDEX(''%' + @FSPattern + N'[0-9]%'', f.' + @FSColumn + N') AS MatchStart
                ) m
                CROSS APPLY (
                    SELECT
                        CASE
                            WHEN PATINDEX(''%[^0-9]%'', SUBSTRING(f.' + @FSColumn + N', m.MatchStart + ' + CAST(@FSPrefixLen AS NVARCHAR) + N', 20)) > 0
                            THEN PATINDEX(''%[^0-9]%'', SUBSTRING(f.' + @FSColumn + N', m.MatchStart + ' + CAST(@FSPrefixLen AS NVARCHAR) + N', 20)) - 1
                            ELSE LEN(SUBSTRING(f.' + @FSColumn + N', m.MatchStart + ' + CAST(@FSPrefixLen AS NVARCHAR) + N', 20))
                        END AS IdLen
                ) il
                CROSS APPLY (
                    SELECT
                        SUBSTRING(f.' + @FSColumn + N', m.MatchStart + ' + CAST(@FSPrefixLen AS NVARCHAR) + N', il.IdLen) AS FactSheetID,
                        m.MatchStart + ' + CAST(@FSPrefixLen AS NVARCHAR) + N' + il.IdLen AS MatchEnd
                ) parts
                WHERE f.' + @FSColumn + N' LIKE ''%' + @FSPattern + N'[0-9]%'';
            END';

            EXEC sp_executesql @SQL;

            FETCH NEXT FROM fs_cursor INTO @FSTable, @FSColumn;
        END

        CLOSE fs_cursor;
        DEALLOCATE fs_cursor;

        FETCH NEXT FROM fs_pattern_cursor INTO @FSPattern, @FSPrefixLen;
    END

    CLOSE fs_pattern_cursor;
    DEALLOCATE fs_pattern_cursor;
    DROP TABLE #FSPatterns;
    DROP TABLE #FSColumns;

    -- ============================================================
    -- Record migration
    -- ============================================================
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Claude (Mack Peters approved)', @MigrationName, 'Update RTE content links from old MVC route patterns to new Angular routes'

    COMMIT TRANSACTION;
END
