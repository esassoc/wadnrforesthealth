CREATE PROCEDURE dbo.pImportServiceForestryTabularData
AS
BEGIN
    SET NOCOUNT ON;

    -- =========================================================================
    -- Publishes staged Service Forestry rows (dbo.ServiceForestryStage) into the
    -- real domain tables. Modeled on dbo.pImportLoaTabularData.
    --
    -- Scope: fund-source allocations + project field updates (incl. County and
    -- the Forester primary contact). Treatments are handled by the ARC GIS import
    -- job; vendor/invoice mapping is out of scope.
    --
    -- IMPORTANT: Service Forestry IS Landowner Assistance (ProgramID = 3), the same
    -- program dbo.pImportLoaTabularData targets. Every delete/update here is scoped
    -- to the ProjectIDs present in the CURRENT ServiceForestryStage so we never
    -- clobber LOA-imported data for projects that are not in this Service Forestry
    -- file. This also keeps the proc idempotent (safe to re-run / run nightly).
    -- =========================================================================

    DECLARE @LandownerAssistanceProgramID int = 3;
    DECLARE @PrimaryContactRelationshipTypeID int =
        (SELECT TOP 1 ProjectPersonRelationshipTypeID
         FROM dbo.ProjectPersonRelationshipType
         WHERE ProjectPersonRelationshipTypeName = 'PrimaryContact');

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Projects referenced by the current Service Forestry staging snapshot.
        if object_id('tempdb.dbo.#sfProjects') is not null drop table #sfProjects
        select distinct p.ProjectID
        into #sfProjects
        from dbo.ServiceForestryStage sf
        join dbo.Project p on p.ProjectGisIdentifier = sf.ProjectIdentifier

        -- -----------------------------------------------------------------
        -- 1) Resolve staged rows -> (Project, FundSourceAllocation, amounts, dates)
        -- -----------------------------------------------------------------
        if object_id('tempdb.dbo.#projectFundSourceAllocation') is not null drop table #projectFundSourceAllocation
        select x.ProjectID
        , x.FundSourceAllocationID
        , x.DCMatchAmount
        , x.DCPayAmount
        , x.DCLetterDate
        , x.DCExpirationDate
        , x.ApprovalDate
        , x.PercentMatch
        , x.ServiceForestryStageID
        into #projectFundSourceAllocation
        from dbo.vServiceForestryStageProjectFundSourceAllocation x

        -- Per-row request parts (only rows that matched an allocation).
        if object_id('tempdb.dbo.#projectFundSourceAllocationRequestPart') is not null drop table #projectFundSourceAllocationRequestPart
        select x.ProjectID,
               x.FundSourceAllocationID
               , isnull(x.DCMatchAmount, 0) + isnull(x.DCPayAmount, 0) as TotalAmount
               , x.DCMatchAmount as MatchAmount
               , x.DCPayAmount as PayAmount
        into #projectFundSourceAllocationRequestPart
        from #projectFundSourceAllocation x where x.FundSourceAllocationID is not null

        -- Aggregate to one row per (Project, FundSourceAllocation) -- the unique key
        -- on dbo.ProjectFundSourceAllocationRequest.
        if object_id('tempdb.dbo.#projectFundSourceAllocationRequest') is not null drop table #projectFundSourceAllocationRequest
        select x.ProjectID,
               x.FundSourceAllocationID
               , sum(x.TotalAmount) as TotalAmount
               , sum(x.MatchAmount) as MatchAmount
               , sum(x.PayAmount) as PayAmount
        into #projectFundSourceAllocationRequest
        from #projectFundSourceAllocationRequestPart x group by x.ProjectID, x.FundSourceAllocationID

        -- -----------------------------------------------------------------
        -- 2) Replace fund-source allocation requests (scoped to SF projects)
        -- -----------------------------------------------------------------
        -- Remove prior tabular-imported requests for SF projects only.
        delete from dbo.ProjectFundSourceAllocationRequest
        where ImportedFromTabularData = 1
          and ProjectID in (select ProjectID from #sfProjects)

        -- Replace any user-entered request that collides with an incoming
        -- (Project, FundSourceAllocation) pair (matches the LOA "replace user data" step).
        delete from dbo.ProjectFundSourceAllocationRequest
        where ProjectFundSourceAllocationRequestID in (
            select pgar.ProjectFundSourceAllocationRequestID
            from dbo.ProjectFundSourceAllocationRequest pgar
            join #projectFundSourceAllocationRequest tpgar
                on pgar.ProjectID = tpgar.ProjectID
                and pgar.FundSourceAllocationID = tpgar.FundSourceAllocationID
            where pgar.ImportedFromTabularData = 0
        )

        insert into dbo.ProjectFundSourceAllocationRequest(ProjectID, FundSourceAllocationID, TotalAmount, MatchAmount, PayAmount, CreateDate, ImportedFromTabularData)
        select x.ProjectID,
               x.FundSourceAllocationID
               , x.TotalAmount
               , x.MatchAmount
               , x.PayAmount
               , getdate()
               , 1
        from #projectFundSourceAllocationRequest x

        -- -----------------------------------------------------------------
        -- 3) Update Project financial / date fields (scoped to SF projects)
        -- -----------------------------------------------------------------
        update dbo.Project
        set EstimatedTotalCost = y.EstimatedTotalCost
        ,   ExpirationDate = y.ExpirationDate
        ,   PlannedDate = y.LetterDate
        ,   ApprovalDate = y.ApprovalDate
        from dbo.Project p
        join (select x.ProjectID
            , sum(isnull(x.DCMatchAmount, 0)) + sum(isnull(x.DCPayAmount, 0)) as EstimatedTotalCost
            , max(x.DCExpirationDate) as ExpirationDate
            , min(x.DCLetterDate) as LetterDate
            , min(x.ApprovalDate) as ApprovalDate
              from #projectFundSourceAllocation x group by x.ProjectID) y on y.ProjectID = p.ProjectID

        -- PercentageMatch (Project column is int) <- ServiceForestryStage.PercentMatch (decimal(9,4)).
        -- ASSUMPTION: PercentMatch is stored as a whole percent (e.g. 25.0000 = 25%).
        -- If it turns out to be a fraction (0.2500), multiply by 100 here.
        update dbo.Project
        set PercentageMatch = y.PercentMatch
        from dbo.Project p
        join (select p2.ProjectID, max(cast(round(sf.PercentMatch, 0) as int)) as PercentMatch
              from dbo.ServiceForestryStage sf
              join dbo.Project p2 on p2.ProjectGisIdentifier = sf.ProjectIdentifier
              where sf.PercentMatch is not null
              group by p2.ProjectID) y on y.ProjectID = p.ProjectID

        -- -----------------------------------------------------------------
        -- 4) County -> ProjectCounty (insert-if-not-exists, WA only)
        -- -----------------------------------------------------------------
        insert into dbo.ProjectCounty(ProjectID, CountyID)
        select distinct p.ProjectID, c.CountyID
        from dbo.ServiceForestryStage sf
        join dbo.Project p on p.ProjectGisIdentifier = sf.ProjectIdentifier
        join dbo.County c on ltrim(rtrim(c.CountyName)) = ltrim(rtrim(sf.County))
        join dbo.StateProvince stp on stp.StateProvinceID = c.StateProvinceID and stp.StateProvinceAbbreviation = 'WA'
        where sf.County is not null and ltrim(rtrim(sf.County)) != ''
          and not exists (select 1 from dbo.ProjectCounty pc where pc.ProjectID = p.ProjectID and pc.CountyID = c.CountyID)

        -- -----------------------------------------------------------------
        -- 5) Forester -> ProjectPerson (PrimaryContact). MATCH ONLY -- never
        --    create a Person, because the staged Forester is a single free-text
        --    name with no email to dedupe on (unlike LOA).
        -- -----------------------------------------------------------------
        if object_id('tempdb.dbo.#projectForesterInfo') is not null drop table #projectForesterInfo
        select distinct p.ProjectID
        , person.PersonID
        , pp.ProjectPersonID
        , ppOld.ProjectPersonID as OldProjectPersonID
        into #projectForesterInfo
        from dbo.ServiceForestryStage sf
        join dbo.Project p on p.ProjectGisIdentifier = sf.ProjectIdentifier
        -- Inner join => only foresters that match an existing active Person survive.
        join dbo.Person person on person.IsActive = 1
            and (
                ltrim(rtrim(sf.Forester)) = ltrim(rtrim(isnull(person.FirstName, '') + ' ' + isnull(person.LastName, '')))
                or ltrim(rtrim(sf.Forester)) = ltrim(rtrim(isnull(person.LastName, '') + ', ' + isnull(person.FirstName, '')))
            )
        left join dbo.ProjectPerson pp on pp.PersonID = person.PersonID and pp.ProjectID = p.ProjectID and pp.ProjectPersonRelationshipTypeID = @PrimaryContactRelationshipTypeID
        left join dbo.ProjectPerson ppOld on ppOld.ProjectID = p.ProjectID and ppOld.ProjectPersonRelationshipTypeID = @PrimaryContactRelationshipTypeID
        where sf.Forester is not null and ltrim(rtrim(sf.Forester)) != ''

        -- Remove the existing primary contact when we matched a different person.
        delete from dbo.ProjectPerson where ProjectPersonID in
        (select x.OldProjectPersonID from #projectForesterInfo x
         where x.OldProjectPersonID is not null
           and (x.ProjectPersonID is null or x.ProjectPersonID != x.OldProjectPersonID))

        -- Assign the matched person as primary contact where not already assigned.
        insert into dbo.ProjectPerson(ProjectID, PersonID, ProjectPersonRelationshipTypeID, CreatedAsPartOfBulkImport)
        select x.ProjectID
        , x.PersonID
        , @PrimaryContactRelationshipTypeID
        , 1
        from #projectForesterInfo x
        where x.PersonID is not null and x.ProjectPersonID is null

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END

/*

EXEC dbo.pImportServiceForestryTabularData

*/
