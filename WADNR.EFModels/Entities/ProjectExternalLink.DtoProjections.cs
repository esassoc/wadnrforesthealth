using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class ProjectExternalLinkProjections
{
    public static Expression<Func<ProjectExternalLink, ProjectExternalLinkGridRow>> AsGridRow => x => new ProjectExternalLinkGridRow
    {
        ProjectExternalLinkID = x.ProjectExternalLinkID,
        ExternalLinkLabel = x.ExternalLinkLabel,
        ExternalLinkUrl = x.ExternalLinkUrl
    };
}
