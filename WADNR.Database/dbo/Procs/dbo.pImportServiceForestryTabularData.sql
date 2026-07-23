CREATE PROCEDURE dbo.pImportServiceForestryTabularData
AS
BEGIN
    SET NOCOUNT ON;

    -- =========================================================================
    -- STUB / TODO: Service Forestry "publishing" process.
    --
    -- This mirrors dbo.pImportLoaTabularData, which maps staged LOA rows into the
    -- real domain tables (Project, ProjectFundSourceAllocationRequest,
    -- ProjectPerson, etc.). The equivalent business mapping for the
    -- ServiceForestryStage data (treatments, vendors, costs, allocated amounts,
    -- etc.) has not yet been defined.
    --
    -- Until the target mapping is specified, this procedure is intentionally a
    -- no-op so the upload + staging pipeline works end-to-end. Fill in the
    -- staging -> domain mapping here. The staged rows are available in
    -- dbo.ServiceForestryStage (wiped and repopulated on each upload).
    --
    -- See dbo.pImportLoaTabularData for the pattern (temp tables, joins on
    -- Project.ProjectGisIdentifier = stage.ProjectIdentifier, etc.).
    -- =========================================================================

    -- No-op for now.
    SELECT 1;
END

/*

EXEC dbo.pImportServiceForestryTabularData

*/
