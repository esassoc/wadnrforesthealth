DECLARE @MigrationName VARCHAR(200);
SET @MigrationName = '0004 - Update RTE links to new Angular routes'

IF NOT EXISTS(SELECT * FROM dbo.DatabaseMigration DM WHERE DM.ReleaseScriptFileName = @MigrationName)
BEGIN
    -- ============================================================
    -- This migration updates RTE HTML content that contains links
    -- to old MVC route patterns, replacing them with new Angular routes.
    --
    -- Strategy:
    --   1. Normalize absolute URLs to relative paths
    --   2. Replace route patterns using individual UPDATE statements
    --      (avoids SSDT parser nesting limits with chained REPLACE)
    --   3. Order: most-specific patterns first to avoid partial matches
    -- ============================================================

    -- ============================================================
    -- Build a temp table of all table/column combos and replacements
    -- to drive a cursor-based approach that avoids deep REPLACE nesting
    -- ============================================================

    -- Route mapping table: old pattern -> new pattern
    -- Order matters: most-specific first
    CREATE TABLE #RouteMap (
        SortOrder INT IDENTITY(1,1),
        OldPattern NVARCHAR(200),
        NewPattern NVARCHAR(200)
    );

    -- Step 1: Absolute URL normalization (must run first, https before http to avoid partial match)
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('https://foresthealthtracker.dnr.wa.gov/', '/'),
        ('http://foresthealthtracker.dnr.wa.gov/', '/');

    -- Step 2: Non-standard routes (most specific / longest first)
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('/ProgramInfo/ClassificationSystem/11', '/projects-by-theme'),
        ('/ProgramInfo/Taxonomy', '/projects-by-type'),
        ('/Results/ProjectMap', '/projects/map'),
        ('/Project/FeaturedList', '/projects/featured'),
        ('/Project/MyProjects', '/projects/my'),
        ('/Project/Pending', '/projects/pending'),
        ('/FindYourForester/Manage', '/manage-find-your-forester'),
        ('/FindYourForester/', '/find-your-forester'),
        ('/Home/ManageHomePageImages', '/homepage-configuration'),
        ('/Home/InternalSetupNotes', '/internal-setup-notes'),
        ('/Account/Login', '/login'),
        ('/Help/Support', '/support'),
        ('/FileResource/DisplayResource/', '/api/file-resources/');

    -- Step 3: Entity detail routes (/Entity/Detail/ -> /kebab-case/)
    -- FactSheet is handled separately below (needs /fact-sheet suffix after ID)
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

    -- Step 4: Entity index routes (/Entity/Index -> /kebab-case)
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

    -- Step 5: /About/ vanity URL prefix stripping (last, as it's a broad pattern)
    INSERT INTO #RouteMap (OldPattern, NewPattern) VALUES
        ('/About/', '/');

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
        -- FirmaPage.FirmaPageContent
        UPDATE dbo.FirmaPage
        SET FirmaPageContent = REPLACE(FirmaPageContent, @OldPattern, @NewPattern)
        WHERE FirmaPageContent LIKE '%' + @OldPattern + '%';

        -- CustomPage.CustomPageContent
        UPDATE dbo.CustomPage
        SET CustomPageContent = REPLACE(CustomPageContent, @OldPattern, @NewPattern)
        WHERE CustomPageContent LIKE '%' + @OldPattern + '%';

        -- FieldDefinition.DefaultDefinition
        UPDATE dbo.FieldDefinition
        SET DefaultDefinition = REPLACE(DefaultDefinition, @OldPattern, @NewPattern)
        WHERE DefaultDefinition LIKE '%' + @OldPattern + '%';

        -- FieldDefinitionDatum.FieldDefinitionDatumValue
        UPDATE dbo.FieldDefinitionDatum
        SET FieldDefinitionDatumValue = REPLACE(FieldDefinitionDatumValue, @OldPattern, @NewPattern)
        WHERE FieldDefinitionDatumValue LIKE '%' + @OldPattern + '%';

        -- PriorityLandscape.PriorityLandscapeDescription
        UPDATE dbo.PriorityLandscape
        SET PriorityLandscapeDescription = REPLACE(PriorityLandscapeDescription, @OldPattern, @NewPattern)
        WHERE PriorityLandscapeDescription LIKE '%' + @OldPattern + '%';

        -- PriorityLandscape.PriorityLandscapeExternalResources
        UPDATE dbo.PriorityLandscape
        SET PriorityLandscapeExternalResources = REPLACE(PriorityLandscapeExternalResources, @OldPattern, @NewPattern)
        WHERE PriorityLandscapeExternalResources LIKE '%' + @OldPattern + '%';

        -- FindYourForesterQuestion.ResultsBonusContent
        UPDATE dbo.FindYourForesterQuestion
        SET ResultsBonusContent = REPLACE(ResultsBonusContent, @OldPattern, @NewPattern)
        WHERE ResultsBonusContent LIKE '%' + @OldPattern + '%';

        -- ClassificationSystem.ClassificationSystemDefinition
        UPDATE dbo.ClassificationSystem
        SET ClassificationSystemDefinition = REPLACE(ClassificationSystemDefinition, @OldPattern, @NewPattern)
        WHERE ClassificationSystemDefinition LIKE '%' + @OldPattern + '%';

        -- ClassificationSystem.ClassificationSystemListPageContent
        UPDATE dbo.ClassificationSystem
        SET ClassificationSystemListPageContent = REPLACE(ClassificationSystemListPageContent, @OldPattern, @NewPattern)
        WHERE ClassificationSystemListPageContent LIKE '%' + @OldPattern + '%';

        -- DNRUplandRegion.RegionContent
        UPDATE dbo.DNRUplandRegion
        SET RegionContent = REPLACE(RegionContent, @OldPattern, @NewPattern)
        WHERE RegionContent LIKE '%' + @OldPattern + '%';

        -- ProjectType.ProjectTypeDescription
        UPDATE dbo.ProjectType
        SET ProjectTypeDescription = REPLACE(ProjectTypeDescription, @OldPattern, @NewPattern)
        WHERE ProjectTypeDescription LIKE '%' + @OldPattern + '%';

        -- ProjectUpdateConfiguration.ProjectUpdateKickOffIntroContent
        UPDATE dbo.ProjectUpdateConfiguration
        SET ProjectUpdateKickOffIntroContent = REPLACE(ProjectUpdateKickOffIntroContent, @OldPattern, @NewPattern)
        WHERE ProjectUpdateKickOffIntroContent LIKE '%' + @OldPattern + '%';

        -- ProjectUpdateConfiguration.ProjectUpdateReminderIntroContent
        UPDATE dbo.ProjectUpdateConfiguration
        SET ProjectUpdateReminderIntroContent = REPLACE(ProjectUpdateReminderIntroContent, @OldPattern, @NewPattern)
        WHERE ProjectUpdateReminderIntroContent LIKE '%' + @OldPattern + '%';

        -- ProjectUpdateConfiguration.ProjectUpdateCloseOutIntroContent
        UPDATE dbo.ProjectUpdateConfiguration
        SET ProjectUpdateCloseOutIntroContent = REPLACE(ProjectUpdateCloseOutIntroContent, @OldPattern, @NewPattern)
        WHERE ProjectUpdateCloseOutIntroContent LIKE '%' + @OldPattern + '%';

        -- TaxonomyBranch.TaxonomyBranchDescription
        UPDATE dbo.TaxonomyBranch
        SET TaxonomyBranchDescription = REPLACE(TaxonomyBranchDescription, @OldPattern, @NewPattern)
        WHERE TaxonomyBranchDescription LIKE '%' + @OldPattern + '%';

        -- TaxonomyTrunk.TaxonomyTrunkDescription
        UPDATE dbo.TaxonomyTrunk
        SET TaxonomyTrunkDescription = REPLACE(TaxonomyTrunkDescription, @OldPattern, @NewPattern)
        WHERE TaxonomyTrunkDescription LIKE '%' + @OldPattern + '%';

        FETCH NEXT FROM route_cursor INTO @OldPattern, @NewPattern;
    END

    CLOSE route_cursor;
    DEALLOCATE route_cursor;

    DROP TABLE #RouteMap;

    -- ============================================================
    -- Handle FactSheet links: /Project/FactSheet/{ID} -> /projects/{ID}/fact-sheet
    -- Simple REPLACE can't append a suffix after a variable-length ID,
    -- so we use per-column string surgery with PATINDEX to find the pattern,
    -- extract the numeric ID, and rebuild the URL.
    -- LEN('/Project/FactSheet/') = 20
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

    DECLARE @FSTable NVARCHAR(100), @FSColumn NVARCHAR(100), @SQL NVARCHAR(MAX);

    DECLARE fs_cursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT TableName, ColumnName FROM #FSColumns;

    OPEN fs_cursor;
    FETCH NEXT FROM fs_cursor INTO @FSTable, @FSColumn;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SQL = N'
        WHILE EXISTS (SELECT 1 FROM dbo.' + @FSTable + N' WHERE ' + @FSColumn + N' LIKE ''%/Project/FactSheet/[0-9]%'')
        BEGIN
            UPDATE f
            SET f.' + @FSColumn + N' =
                LEFT(f.' + @FSColumn + N', m.MatchStart - 1)
                + ''/projects/'' + parts.FactSheetID + ''/fact-sheet''
                + SUBSTRING(f.' + @FSColumn + N', parts.MatchEnd, LEN(f.' + @FSColumn + N'))
            FROM dbo.' + @FSTable + N' f
            CROSS APPLY (
                SELECT PATINDEX(''%/Project/FactSheet/[0-9]%'', f.' + @FSColumn + N') AS MatchStart
            ) m
            CROSS APPLY (
                SELECT
                    CASE
                        WHEN PATINDEX(''%[^0-9]%'', SUBSTRING(f.' + @FSColumn + N', m.MatchStart + 20, 20)) > 0
                        THEN PATINDEX(''%[^0-9]%'', SUBSTRING(f.' + @FSColumn + N', m.MatchStart + 20, 20)) - 1
                        ELSE LEN(SUBSTRING(f.' + @FSColumn + N', m.MatchStart + 20, 20))
                    END AS IdLen
            ) il
            CROSS APPLY (
                SELECT
                    SUBSTRING(f.' + @FSColumn + N', m.MatchStart + 20, il.IdLen) AS FactSheetID,
                    m.MatchStart + 20 + il.IdLen AS MatchEnd
            ) parts
            WHERE f.' + @FSColumn + N' LIKE ''%/Project/FactSheet/[0-9]%'';
        END';

        EXEC sp_executesql @SQL;

        FETCH NEXT FROM fs_cursor INTO @FSTable, @FSColumn;
    END

    CLOSE fs_cursor;
    DEALLOCATE fs_cursor;
    DROP TABLE #FSColumns;

    -- ============================================================
    -- Record migration
    -- ============================================================
    INSERT INTO dbo.DatabaseMigration(MigrationAuthorName, ReleaseScriptFileName, MigrationReason)
    SELECT 'Claude (Mack Peters approved)', @MigrationName, 'Update RTE content links from old MVC route patterns to new Angular routes'
END
