using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WADNR.API.Tests.Helpers;
using WADNR.EFModels.Entities;

namespace WADNR.API.Tests.Integration;

/// <summary>
/// Regression tests for WADNR-2161: imported data with a Program Index but no Project Code
/// must be attributed to the corresponding PI-only Fund Source Allocation. Covers both the
/// Expenditures stored procedure (pArcOnlineFundSourceExpenditureImportJson) and the LOA
/// matching view (vLoaStageFundSourceAllocationByProgramIndexProjectCode).
/// </summary>
[TestClass]
[DoNotParallelize]
public class FundSourceExpenditureImportTests
{
    // Far-future biennium to isolate test rows from production-shaped data.
    private const int TestBiennium = 2099;

    private int _testOrganizationID;
    private int _testFundSourceID;
    private int _piPcAllocationID;
    private int _piOnlyAllocationID;
    private int _programIndexID;
    private int _projectCodeID;
    private int _costTypeDatamartMappingID;
    private int _rawJsonImportID;
    private string _objCd = null!;
    private string _objName = null!;
    private string _subObjCd = null!;
    private string _subObjName = null!;
    private string _programIndexCode = null!;
    private string _projectCodeName = null!;
    private string _loaProjectIdentifierPiPc = null!;
    private string _loaProjectIdentifierPiOnly = null!;

    [TestInitialize]
    public async Task TestInitialize()
    {
        AssemblySteps.DbContext.ChangeTracker.Clear();
        AssemblySteps.SetCurrentUser(AssemblySteps.TestAdminPersonID);

        var suffix = DateTime.UtcNow.Ticks % 1000000;
        _objCd = $"TOBJ{suffix}";
        _objName = $"Test Obj {suffix}";
        _subObjCd = $"TSUB{suffix}";
        _subObjName = $"Test SubObj {suffix}";
        _programIndexCode = $"TPI{suffix}";
        _projectCodeName = $"TPC{suffix}";
        _loaProjectIdentifierPiPc = $"WADNR-2161-PIPC-{suffix}";
        _loaProjectIdentifierPiOnly = $"WADNR-2161-PI-{suffix}";

        // Start from a clean slate for our isolated biennium in case a prior failed test left data behind.
        await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC dbo.pClearFundSourceAllocationExpenditureTables @bienniumFiscalYear = {TestBiennium}");

        var organization = await OrganizationHelper.CreateOrganizationAsync(AssemblySteps.DbContext);
        _testOrganizationID = organization.OrganizationID;

        var fundSource = new FundSource
        {
            FundSourceName = $"FS WADNR-2161 {suffix}",
            FundSourceStatusID = (int)FundSourceStatusEnum.Active,
            OrganizationID = _testOrganizationID,
            TotalAwardAmount = 100000,
        };
        AssemblySteps.DbContext.FundSources.Add(fundSource);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _testFundSourceID = fundSource.FundSourceID;

        var piPcAllocation = new FundSourceAllocation
        {
            FundSourceID = _testFundSourceID,
            FundSourceAllocationName = $"PI+PC alloc {suffix}",
            AllocationAmount = 10000,
        };
        var piOnlyAllocation = new FundSourceAllocation
        {
            FundSourceID = _testFundSourceID,
            FundSourceAllocationName = $"PI-only alloc {suffix}",
            AllocationAmount = 10000,
        };
        AssemblySteps.DbContext.FundSourceAllocations.AddRange(piPcAllocation, piOnlyAllocation);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _piPcAllocationID = piPcAllocation.FundSourceAllocationID;
        _piOnlyAllocationID = piOnlyAllocation.FundSourceAllocationID;

        var programIndex = new ProgramIndex
        {
            ProgramIndexCode = _programIndexCode,
            ProgramIndexTitle = $"Test PI {_programIndexCode}",
            Biennium = TestBiennium,
        };
        AssemblySteps.DbContext.ProgramIndices.Add(programIndex);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _programIndexID = programIndex.ProgramIndexID;

        var projectCode = new ProjectCode
        {
            ProjectCodeName = _projectCodeName,
            ProjectCodeTitle = $"Test PC {_projectCodeName}",
        };
        AssemblySteps.DbContext.ProjectCodes.Add(projectCode);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _projectCodeID = projectCode.ProjectCodeID;

        AssemblySteps.DbContext.FundSourceAllocationProgramIndexProjectCodes.AddRange(
            new FundSourceAllocationProgramIndexProjectCode
            {
                FundSourceAllocationID = _piPcAllocationID,
                ProgramIndexID = _programIndexID,
                ProjectCodeID = _projectCodeID,
            },
            new FundSourceAllocationProgramIndexProjectCode
            {
                FundSourceAllocationID = _piOnlyAllocationID,
                ProgramIndexID = _programIndexID,
                ProjectCodeID = null,
            });
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();

        var costMapping = new CostTypeDatamartMapping
        {
            CostTypeID = (int)CostTypeEnum.Travel,
            DatamartObjectCode = _objCd,
            DatamartObjectName = _objName,
            DatamartSubObjectCode = _subObjCd,
            DatamartSubObjectName = _subObjName,
        };
        AssemblySteps.DbContext.CostTypeDatamartMappings.Add(costMapping);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _costTypeDatamartMappingID = costMapping.CostTypeDatamartMappingID;

        var json = JsonSerializer.Serialize(new[]
        {
            new
            {
                BIENNIUM = TestBiennium,
                FISCAL_MONTH = 1,
                FISCAL_ADJUSTMENT_MONTH = 0,
                CALENDAR_YEAR = TestBiennium,
                MONTH_NAME = "July",
                SOURCE_SYSTEM = "TEST",
                DOCUMENT_NUMBER = "DOC-PIPC",
                OBJECT_CODE = _objCd,
                OBJECT_NAME = _objName,
                SUB_OBJECT_CODE = _subObjCd,
                SUB_OBJECT_NAME = _subObjName,
                PROGRAM_INDEX_CODE = _programIndexCode,
                PROGRAM_INDEX_NAME = "Test PI Name",
                PROJECT_CODE = (string?)_projectCodeName,
                PROJECT_NAME = (string?)"Test PC Name",
                EXPENDITURE_ACCURED = 1000m,
            },
            new
            {
                BIENNIUM = TestBiennium,
                FISCAL_MONTH = 2,
                FISCAL_ADJUSTMENT_MONTH = 0,
                CALENDAR_YEAR = TestBiennium,
                MONTH_NAME = "August",
                SOURCE_SYSTEM = "TEST",
                DOCUMENT_NUMBER = "DOC-PIONLY",
                OBJECT_CODE = _objCd,
                OBJECT_NAME = _objName,
                SUB_OBJECT_CODE = _subObjCd,
                SUB_OBJECT_NAME = _subObjName,
                PROGRAM_INDEX_CODE = _programIndexCode,
                PROGRAM_INDEX_NAME = "Test PI Name",
                PROJECT_CODE = (string?)null,
                PROJECT_NAME = (string?)null,
                EXPENDITURE_ACCURED = 2000m,
            },
        });

        var rawImport = new ArcOnlineFinanceApiRawJsonImport
        {
            CreateDate = DateTime.UtcNow,
            ArcOnlineFinanceApiRawJsonImportTableTypeID = (int)ArcOnlineFinanceApiRawJsonImportTableTypeEnum.FundSourceExpenditure,
            BienniumFiscalYear = TestBiennium,
            FinanceApiLastLoadDate = DateTime.UtcNow,
            RawJsonString = json,
            JsonImportStatusTypeID = (int)JsonImportStatusTypeEnum.NotYetProcessed,
        };
        AssemblySteps.DbContext.ArcOnlineFinanceApiRawJsonImports.Add(rawImport);
        await AssemblySteps.DbContext.SaveChangesWithNoAuditingAsync();
        _rawJsonImportID = rawImport.ArcOnlineFinanceApiRawJsonImportID;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        try
        {
            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.LoaStage WHERE ProjectIdentifier IN ({_loaProjectIdentifierPiPc}, {_loaProjectIdentifierPiOnly})");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC dbo.pClearFundSourceAllocationExpenditureTables @bienniumFiscalYear = {TestBiennium}");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ArcOnlineFinanceApiRawJsonImport WHERE ArcOnlineFinanceApiRawJsonImportID = {_rawJsonImportID}");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.CostTypeDatamartMapping WHERE CostTypeDatamartMappingID = {_costTypeDatamartMappingID}");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.FundSourceAllocationProgramIndexProjectCode WHERE FundSourceAllocationID IN ({_piPcAllocationID}, {_piOnlyAllocationID})");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.FundSourceAllocation WHERE FundSourceAllocationID IN ({_piPcAllocationID}, {_piOnlyAllocationID})");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.FundSource WHERE FundSourceID = {_testFundSourceID}");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProgramIndex WHERE ProgramIndexID = {_programIndexID}");

            await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
                $"DELETE FROM dbo.ProjectCode WHERE ProjectCodeID = {_projectCodeID}");

            await OrganizationHelper.DeleteOrganizationAsync(AssemblySteps.DbContext, _testOrganizationID);
        }
        catch { }
    }

    [TestMethod]
    public async Task ExpenditureImport_AttributesPiPcRowToPiPcAllocation()
    {
        await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC dbo.pArcOnlineFundSourceExpenditureImportJson @ArcOnlineFinanceApiRawJsonImportID = {_rawJsonImportID}, @BienniumToImport = {TestBiennium}");

        var expenditures = await AssemblySteps.DbContext.FundSourceAllocationExpenditures
            .AsNoTracking()
            .Where(e => e.FundSourceAllocationID == _piPcAllocationID && e.Biennium == TestBiennium)
            .ToListAsync();

        Assert.AreEqual(1, expenditures.Count, "PI+PC allocation should receive exactly one imported expenditure.");
        Assert.AreEqual(1000m, expenditures[0].ExpenditureAmount);
    }

    [TestMethod]
    public async Task ExpenditureImport_AttributesPiOnlyRowToPiOnlyAllocation()
    {
        await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC dbo.pArcOnlineFundSourceExpenditureImportJson @ArcOnlineFinanceApiRawJsonImportID = {_rawJsonImportID}, @BienniumToImport = {TestBiennium}");

        // WADNR-2161: Before the fix, the INNER JOIN on ProjectCode dropped this row entirely.
        var expenditures = await AssemblySteps.DbContext.FundSourceAllocationExpenditures
            .AsNoTracking()
            .Where(e => e.FundSourceAllocationID == _piOnlyAllocationID && e.Biennium == TestBiennium)
            .ToListAsync();

        Assert.AreEqual(1, expenditures.Count, "PI-only allocation should receive the import row that has no PROJECT_CODE.");
        Assert.AreEqual(2000m, expenditures[0].ExpenditureAmount);
    }

    [TestMethod]
    public async Task ExpenditureImport_DoesNotCrossAttributeExpenditures()
    {
        await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
            $"EXEC dbo.pArcOnlineFundSourceExpenditureImportJson @ArcOnlineFinanceApiRawJsonImportID = {_rawJsonImportID}, @BienniumToImport = {TestBiennium}");

        var piPcCount = await AssemblySteps.DbContext.FundSourceAllocationExpenditures
            .AsNoTracking()
            .CountAsync(e => e.FundSourceAllocationID == _piPcAllocationID && e.Biennium == TestBiennium);

        var piOnlyCount = await AssemblySteps.DbContext.FundSourceAllocationExpenditures
            .AsNoTracking()
            .CountAsync(e => e.FundSourceAllocationID == _piOnlyAllocationID && e.Biennium == TestBiennium);

        // Each allocation gets exactly its own row — the PI-only row must not duplicate onto the PI+PC allocation, and vice versa.
        Assert.AreEqual(1, piPcCount);
        Assert.AreEqual(1, piOnlyCount);
    }

    [TestMethod]
    public async Task LoaView_MatchesPiOnlyLoaStageRowToPiOnlyAllocation()
    {
        await AssemblySteps.DbContext.Database.ExecuteSqlInterpolatedAsync(
            $@"INSERT INTO dbo.LoaStage (ProjectIdentifier, IsNortheast, ProgramIndex, ProjectCode)
               VALUES ({_loaProjectIdentifierPiPc}, 1, {_programIndexCode}, {_projectCodeName}),
                      ({_loaProjectIdentifierPiOnly}, 1, {_programIndexCode}, NULL)");

        var view = await AssemblySteps.DbContext.vLoaStageFundSourceAllocationByProgramIndexProjectCodes
            .AsNoTracking()
            .Where(v => v.ProgramIndex == _programIndexCode)
            .ToListAsync();

        Assert.AreEqual(2, view.Count, "Both PI+PC and PI-only LoaStage rows should resolve to an allocation.");
        Assert.IsTrue(
            view.Any(v => v.FundSourceAllocationID == _piPcAllocationID && v.ProjectCode == _projectCodeName),
            "PI+PC LoaStage row should match the PI+PC allocation.");
        Assert.IsTrue(
            view.Any(v => v.FundSourceAllocationID == _piOnlyAllocationID && v.ProjectCode == null),
            "WADNR-2161: PI-only LoaStage row should match the PI-only allocation.");
    }
}
