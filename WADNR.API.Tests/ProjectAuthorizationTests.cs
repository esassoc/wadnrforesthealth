using WADNR.EFModels.Entities;
using WADNR.Models.DataTransferObjects;

namespace WADNR.API.Tests;

[TestClass]
public class ProjectAuthorizationTests
{
    #region Helpers

    private static PersonDetail MakePerson(
        int personID = 100,
        int baseRoleID = (int)RoleEnum.Normal,
        int? organizationID = null,
        int[]? supplementalRoleIDs = null,
        List<int>? assignedProgramIDs = null,
        List<int>? stewardOrganizationIDs = null,
        List<int>? stewardTaxonomyBranchIDs = null,
        List<int>? stewardRegionIDs = null)
    {
        return new PersonDetail
        {
            PersonID = personID,
            OrganizationID = organizationID,
            BaseRole = new RoleLookupItem { RoleID = baseRoleID, RoleName = $"Role{baseRoleID}" },
            SupplementalRoleList = supplementalRoleIDs?
                .Select(id => new RoleLookupItem { RoleID = id, RoleName = $"Role{id}" })
                .ToList() ?? [],
            AssignedPrograms = assignedProgramIDs?
                .Select(id => new ProgramLookupItem { ProgramID = id, ProgramName = $"Program{id}" })
                .ToList() ?? [],
            StewardOrganizations = stewardOrganizationIDs?
                .Select(id => new StewardshipAreaItem { ID = id, Name = $"Org{id}" })
                .ToList() ?? [],
            StewardTaxonomyBranches = stewardTaxonomyBranchIDs?
                .Select(id => new StewardshipAreaItem { ID = id, Name = $"Branch{id}" })
                .ToList() ?? [],
            StewardRegions = stewardRegionIDs?
                .Select(id => new StewardshipAreaItem { ID = id, Name = $"Region{id}" })
                .ToList() ?? [],
        };
    }

    private static ProjectAuthorizationData MakeAuthData(
        int approvalStatusID = (int)ProjectApprovalStatusEnum.Approved,
        int? primaryContactPersonID = null,
        int? leadImplementerOrganizationID = null,
        int? stewardingOrganizationID = null,
        int? proposingPersonOrganizationID = null,
        int? taxonomyBranchID = null,
        List<int>? regionIDs = null,
        List<int>? programIDs = null)
    {
        return new ProjectAuthorizationData
        {
            ProjectID = 1,
            ProjectApprovalStatusID = approvalStatusID,
            PrimaryContactPersonID = primaryContactPersonID,
            LeadImplementerOrganizationID = leadImplementerOrganizationID,
            StewardingOrganizationID = stewardingOrganizationID,
            ProposingPersonOrganizationID = proposingPersonOrganizationID,
            TaxonomyBranchID = taxonomyBranchID,
            RegionIDs = regionIDs ?? [],
            ProgramIDs = programIDs ?? [],
        };
    }

    #endregion

    #region CanProgramEditorManageProject

    [TestMethod]
    public void CanProgramEditorManageProject_ReturnsFalse_WhenNoCanEditProgramRole()
    {
        var person = MakePerson();
        var authData = MakeAuthData(programIDs: [1, 2]);

        Assert.IsFalse(ProjectAuthorization.CanProgramEditorManageProject(person, authData));
    }

    [TestMethod]
    public void CanProgramEditorManageProject_ReturnsFalse_WhenHasRoleButNoAssignedPrograms()
    {
        var person = MakePerson(supplementalRoleIDs: [(int)RoleEnum.CanEditProgram]);
        var authData = MakeAuthData(programIDs: [1, 2]);

        Assert.IsFalse(ProjectAuthorization.CanProgramEditorManageProject(person, authData));
    }

    [TestMethod]
    public void CanProgramEditorManageProject_ReturnsFalse_WhenProgramsDontOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(programIDs: [3, 4]);

        Assert.IsFalse(ProjectAuthorization.CanProgramEditorManageProject(person, authData));
    }

    [TestMethod]
    public void CanProgramEditorManageProject_ReturnsTrue_WhenProgramsOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(programIDs: [2, 3]);

        Assert.IsTrue(ProjectAuthorization.CanProgramEditorManageProject(person, authData));
    }

    [TestMethod]
    public void CanProgramEditorManageProject_ReturnsFalse_WhenProjectHasNoPrograms()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(programIDs: []);

        Assert.IsFalse(ProjectAuthorization.CanProgramEditorManageProject(person, authData));
    }

    #endregion

    #region CanStewardProject

    [TestMethod]
    public void CanStewardProject_ReturnsFalse_WhenNotProjectSteward()
    {
        var person = MakePerson();
        var authData = MakeAuthData();

        Assert.IsFalse(ProjectAuthorization.CanStewardProject(person, authData, null));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsTrue_WhenStewardAndNoStewardshipAreaType()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.ProjectSteward);
        var authData = MakeAuthData();

        Assert.IsTrue(ProjectAuthorization.CanStewardProject(person, authData, null));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsTrue_WhenOrgStewardshipAndOrgMatches()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10, 20]);
        var authData = MakeAuthData(stewardingOrganizationID: 20);

        Assert.IsTrue(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsFalse_WhenOrgStewardshipAndOrgDoesNotMatch()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10, 20]);
        var authData = MakeAuthData(stewardingOrganizationID: 30);

        Assert.IsFalse(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsFalse_WhenOrgStewardshipAndStewardingOrgIsNull()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10]);
        var authData = MakeAuthData(stewardingOrganizationID: null);

        Assert.IsFalse(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsTrue_WhenTaxonomyBranchAndBranchMatches()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardTaxonomyBranchIDs: [5, 10]);
        var authData = MakeAuthData(taxonomyBranchID: 10);

        Assert.IsTrue(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.TaxonomyBranches));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsFalse_WhenTaxonomyBranchAndBranchDoesNotMatch()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardTaxonomyBranchIDs: [5, 10]);
        var authData = MakeAuthData(taxonomyBranchID: 15);

        Assert.IsFalse(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.TaxonomyBranches));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsFalse_WhenTaxonomyBranchAndBranchIsNull()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardTaxonomyBranchIDs: [5]);
        var authData = MakeAuthData(taxonomyBranchID: null);

        Assert.IsFalse(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.TaxonomyBranches));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsTrue_WhenRegionAndRegionMatches()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardRegionIDs: [1, 2, 3]);
        var authData = MakeAuthData(regionIDs: [3, 4]);

        Assert.IsTrue(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.Regions));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsFalse_WhenRegionAndRegionDoesNotMatch()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardRegionIDs: [1, 2]);
        var authData = MakeAuthData(regionIDs: [3, 4]);

        Assert.IsFalse(ProjectAuthorization.CanStewardProject(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.Regions));
    }

    [TestMethod]
    public void CanStewardProject_ReturnsFalse_WhenUnknownStewardshipAreaType()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.ProjectSteward);
        var authData = MakeAuthData();

        Assert.IsFalse(ProjectAuthorization.CanStewardProject(person, authData, 99));
    }

    #endregion

    #region CanEditAsAdmin

    [TestMethod]
    public void CanEditAsAdmin_ReturnsTrue_WhenAdminAndApprovedProject()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.Admin);
        var authData = MakeAuthData(approvalStatusID: (int)ProjectApprovalStatusEnum.Approved);

        Assert.IsTrue(ProjectAuthorization.CanEditAsAdmin(person, authData, null));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsFalse_WhenAdminAndPendingProject()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.Admin);
        var authData = MakeAuthData(approvalStatusID: (int)ProjectApprovalStatusEnum.Draft);

        Assert.IsFalse(ProjectAuthorization.CanEditAsAdmin(person, authData, null));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsTrue_WhenEsaAdminAndApprovedProject()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.EsaAdmin);
        var authData = MakeAuthData(approvalStatusID: (int)ProjectApprovalStatusEnum.Approved);

        Assert.IsTrue(ProjectAuthorization.CanEditAsAdmin(person, authData, null));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsFalse_WhenEsaAdminAndPendingProject()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.EsaAdmin);
        var authData = MakeAuthData(approvalStatusID: (int)ProjectApprovalStatusEnum.PendingApproval);

        Assert.IsFalse(ProjectAuthorization.CanEditAsAdmin(person, authData, null));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsFalse_WhenNonAdminAndPendingProject()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10]);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Draft,
            stewardingOrganizationID: 10);

        Assert.IsFalse(ProjectAuthorization.CanEditAsAdmin(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsTrue_WhenStewardCanStewardApprovedProject()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10]);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Approved,
            stewardingOrganizationID: 10);

        Assert.IsTrue(ProjectAuthorization.CanEditAsAdmin(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsFalse_WhenStewardCannotStewardProject()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10]);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Approved,
            stewardingOrganizationID: 30);

        Assert.IsFalse(ProjectAuthorization.CanEditAsAdmin(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsTrue_WhenProgramEditorWithOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Approved,
            programIDs: [2, 3]);

        Assert.IsTrue(ProjectAuthorization.CanEditAsAdmin(person, authData, null));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsFalse_WhenProgramEditorWithNoOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Approved,
            programIDs: [3, 4]);

        Assert.IsFalse(ProjectAuthorization.CanEditAsAdmin(person, authData, null));
    }

    [TestMethod]
    public void CanEditAsAdmin_ReturnsFalse_WhenNormalUser()
    {
        var person = MakePerson();
        var authData = MakeAuthData(approvalStatusID: (int)ProjectApprovalStatusEnum.Approved);

        Assert.IsFalse(ProjectAuthorization.CanEditAsAdmin(person, authData, null));
    }

    #endregion

    #region CanApprove

    [TestMethod]
    public void CanApprove_ReturnsTrue_WhenAdmin()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.Admin);
        var authData = MakeAuthData();

        Assert.IsTrue(ProjectAuthorization.CanApprove(person, authData, null));
    }

    [TestMethod]
    public void CanApprove_ReturnsTrue_WhenEsaAdmin()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.EsaAdmin);
        var authData = MakeAuthData();

        Assert.IsTrue(ProjectAuthorization.CanApprove(person, authData, null));
    }

    [TestMethod]
    public void CanApprove_ReturnsTrue_WhenStewardCanStewardProject()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10]);
        var authData = MakeAuthData(stewardingOrganizationID: 10);

        Assert.IsTrue(ProjectAuthorization.CanApprove(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanApprove_ReturnsFalse_WhenStewardCannotStewardProject()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10]);
        var authData = MakeAuthData(stewardingOrganizationID: 30);

        Assert.IsFalse(ProjectAuthorization.CanApprove(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanApprove_ReturnsTrue_WhenProgramEditorWithOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(programIDs: [2, 3]);

        Assert.IsTrue(ProjectAuthorization.CanApprove(person, authData, null));
    }

    [TestMethod]
    public void CanApprove_ReturnsFalse_WhenProgramEditorWithNoOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(programIDs: [3, 4]);

        Assert.IsFalse(ProjectAuthorization.CanApprove(person, authData, null));
    }

    [TestMethod]
    public void CanApprove_ReturnsFalse_WhenNormalUser()
    {
        var person = MakePerson();
        var authData = MakeAuthData();

        Assert.IsFalse(ProjectAuthorization.CanApprove(person, authData, null));
    }

    #endregion

    #region IsMyProject

    [TestMethod]
    public void IsMyProject_ReturnsFalse_WhenAnonymous()
    {
        var person = MakePerson(personID: PersonDetail.AnonymousPersonID);
        var authData = MakeAuthData();

        Assert.IsFalse(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsFalse_WhenUnassigned()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.Unassigned);
        var authData = MakeAuthData();

        Assert.IsFalse(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsTrue_WhenPrimaryContactMatches()
    {
        var person = MakePerson(personID: 42);
        var authData = MakeAuthData(primaryContactPersonID: 42);

        Assert.IsTrue(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsTrue_WhenOrgMatchesLeadImplementer()
    {
        var person = MakePerson(organizationID: 10);
        var authData = MakeAuthData(leadImplementerOrganizationID: 10);

        Assert.IsTrue(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsTrue_WhenOrgMatchesStewardingOrg()
    {
        var person = MakePerson(organizationID: 10);
        var authData = MakeAuthData(stewardingOrganizationID: 10);

        Assert.IsTrue(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsTrue_WhenOrgMatchesProposingPersonOrg()
    {
        var person = MakePerson(organizationID: 10);
        var authData = MakeAuthData(proposingPersonOrganizationID: 10);

        Assert.IsTrue(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsTrue_WhenStewardOrgMatchesLeadImplementer()
    {
        var person = MakePerson(stewardOrganizationIDs: [10, 20]);
        var authData = MakeAuthData(leadImplementerOrganizationID: 20);

        Assert.IsTrue(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsTrue_WhenStewardOrgMatchesStewardingOrg()
    {
        var person = MakePerson(stewardOrganizationIDs: [10, 20]);
        var authData = MakeAuthData(stewardingOrganizationID: 10);

        Assert.IsTrue(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsTrue_WhenStewardOrgMatchesProposingPersonOrg()
    {
        var person = MakePerson(stewardOrganizationIDs: [10, 20]);
        var authData = MakeAuthData(proposingPersonOrganizationID: 20);

        Assert.IsTrue(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsFalse_WhenNoMatchesExist()
    {
        var person = MakePerson(personID: 100, organizationID: 999);
        var authData = MakeAuthData(
            primaryContactPersonID: 1,
            leadImplementerOrganizationID: 2,
            stewardingOrganizationID: 3,
            proposingPersonOrganizationID: 4);

        Assert.IsFalse(ProjectAuthorization.IsMyProject(person, authData));
    }

    [TestMethod]
    public void IsMyProject_ReturnsFalse_WhenOrgIsNullAndNoOtherMatch()
    {
        var person = MakePerson(personID: 100, organizationID: null);
        var authData = MakeAuthData(
            primaryContactPersonID: 1,
            leadImplementerOrganizationID: 2,
            stewardingOrganizationID: 3,
            proposingPersonOrganizationID: 4);

        Assert.IsFalse(ProjectAuthorization.IsMyProject(person, authData));
    }

    #endregion

    #region CanEditViaWorkflow

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsTrue_WhenAdmin()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.Admin);
        var authData = MakeAuthData(approvalStatusID: (int)ProjectApprovalStatusEnum.Approved);

        Assert.IsTrue(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsTrue_WhenEsaAdmin()
    {
        var person = MakePerson(baseRoleID: (int)RoleEnum.EsaAdmin);
        var authData = MakeAuthData(approvalStatusID: (int)ProjectApprovalStatusEnum.Rejected);

        Assert.IsTrue(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsFalse_WhenApprovedAndNonAdmin()
    {
        var person = MakePerson(personID: 42);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Approved,
            primaryContactPersonID: 42);

        Assert.IsFalse(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsFalse_WhenRejectedAndNonAdmin()
    {
        var person = MakePerson(personID: 42);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Rejected,
            primaryContactPersonID: 42);

        Assert.IsFalse(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsTrue_WhenDraftAndIsMyProject()
    {
        var person = MakePerson(personID: 42);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Draft,
            primaryContactPersonID: 42);

        Assert.IsTrue(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsTrue_WhenPendingApprovalAndIsMyProject()
    {
        var person = MakePerson(personID: 42);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.PendingApproval,
            primaryContactPersonID: 42);

        Assert.IsTrue(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsTrue_WhenReturnedAndIsMyProject()
    {
        var person = MakePerson(personID: 42);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Returned,
            primaryContactPersonID: 42);

        Assert.IsTrue(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsTrue_WhenDraftAndCanApprove()
    {
        var person = MakePerson(
            baseRoleID: (int)RoleEnum.ProjectSteward,
            stewardOrganizationIDs: [10]);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Draft,
            stewardingOrganizationID: 10);

        Assert.IsTrue(ProjectAuthorization.CanEditViaWorkflow(person, authData,
            (int)ProjectStewardshipAreaTypeEnum.ProjectStewardingOrganizations));
    }

    [TestMethod]
    public void CanEditViaWorkflow_ReturnsFalse_WhenDraftAndNotMyProjectAndCannotApprove()
    {
        var person = MakePerson(personID: 100, organizationID: 999);
        var authData = MakeAuthData(
            approvalStatusID: (int)ProjectApprovalStatusEnum.Draft,
            primaryContactPersonID: 1,
            leadImplementerOrganizationID: 2);

        Assert.IsFalse(ProjectAuthorization.CanEditViaWorkflow(person, authData, null));
    }

    #endregion

    #region CanProgramEditorViewPending

    [TestMethod]
    public void CanProgramEditorViewPending_ReturnsFalse_WhenNoCanEditProgramRole()
    {
        var person = MakePerson();
        var authData = MakeAuthData(programIDs: [1, 2]);

        Assert.IsFalse(ProjectAuthorization.CanProgramEditorViewPending(person, authData));
    }

    [TestMethod]
    public void CanProgramEditorViewPending_ReturnsTrue_WhenHasRoleAndProgramsOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(programIDs: [2, 3]);

        Assert.IsTrue(ProjectAuthorization.CanProgramEditorViewPending(person, authData));
    }

    [TestMethod]
    public void CanProgramEditorViewPending_ReturnsFalse_WhenHasRoleButProgramsDontOverlap()
    {
        var person = MakePerson(
            supplementalRoleIDs: [(int)RoleEnum.CanEditProgram],
            assignedProgramIDs: [1, 2]);
        var authData = MakeAuthData(programIDs: [3, 4]);

        Assert.IsFalse(ProjectAuthorization.CanProgramEditorViewPending(person, authData));
    }

    #endregion
}
