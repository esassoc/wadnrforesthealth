using System.Linq.Expressions;
using WADNR.Models.DataTransferObjects.ProjectUpdate;

namespace WADNR.EFModels.Entities;

public static class ProjectUpdateConfigurationProjections
{
    public static readonly Expression<Func<ProjectUpdateConfiguration, ProjectUpdateConfigurationDetail>> AsDetail = x => new ProjectUpdateConfigurationDetail
    {
        ProjectUpdateConfigurationID = x.ProjectUpdateConfigurationID,
        EnableProjectUpdateReminders = x.EnableProjectUpdateReminders,
        ProjectUpdateKickOffDate = x.ProjectUpdateKickOffDate,
        ProjectUpdateKickOffIntroContent = x.ProjectUpdateKickOffIntroContent,
        SendPeriodicReminders = x.SendPeriodicReminders,
        ProjectUpdateReminderInterval = x.ProjectUpdateReminderInterval,
        ProjectUpdateReminderIntroContent = x.ProjectUpdateReminderIntroContent,
        SendCloseOutNotification = x.SendCloseOutNotification,
        ProjectUpdateCloseOutDate = x.ProjectUpdateCloseOutDate,
        ProjectUpdateCloseOutIntroContent = x.ProjectUpdateCloseOutIntroContent,
    };
}
