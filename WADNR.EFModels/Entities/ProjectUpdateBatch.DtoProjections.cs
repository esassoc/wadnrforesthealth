using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.EFModels.Entities;

public static class ProjectUpdateBatchProjections
{
    public static readonly Expression<Func<ProjectUpdateBatch, ProjectUpdateBatchDetail>> AsDetail = b => new ProjectUpdateBatchDetail
    {
        ProjectUpdateBatchID = b.ProjectUpdateBatchID,
        ProjectID = b.ProjectID,
        ProjectName = b.Project.ProjectName,
        ProjectUpdateStateID = b.ProjectUpdateStateID,
        ProjectUpdateStateName = null, // Resolved client-side (static lookup dict can't be translated by EF)
        LastUpdateDate = b.LastUpdateDate,
        LastUpdatedByPersonName = b.LastUpdatePerson.FirstName + " " + b.LastUpdatePerson.LastName
    };

    public static readonly Expression<Func<ProjectUpdateHistory, ProjectUpdateHistoryEntry>> AsHistoryEntry = h => new ProjectUpdateHistoryEntry
    {
        TransitionDate = h.TransitionDate,
        ProjectUpdateStateID = h.ProjectUpdateStateID,
        ProjectUpdateStateName = null, // Resolved client-side
        UpdatePersonName = h.UpdatePerson.FirstName + " " + h.UpdatePerson.LastName + " - " + h.UpdatePerson.Organization.OrganizationName,
    };
}
