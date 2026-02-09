using Microsoft.EntityFrameworkCore;
using WADNR.Models.DataTransferObjects;

namespace WADNR.EFModels.Entities;

public static class Notifications
{
    public static async Task<List<ProjectNotificationGridRow>> ListForProjectAsGridRowAsync(WADNRDbContext dbContext, int projectID)
    {
        var rawNotifications = await dbContext.NotificationProjects
            .AsNoTracking()
            .Where(np => np.ProjectID == projectID)
            .Select(np => new
            {
                np.Notification.NotificationID,
                np.Notification.NotificationDate,
                np.Notification.NotificationTypeID,
                PersonFirstName = np.Notification.Person.FirstName,
                PersonLastName = np.Notification.Person.LastName
            })
            .OrderByDescending(np => np.NotificationDate)
            .ToListAsync();

        var notifications = rawNotifications
            .Select(n => new ProjectNotificationGridRow
            {
                NotificationID = n.NotificationID,
                NotificationDate = n.NotificationDate,
                PersonName = $"{n.PersonFirstName} {n.PersonLastName}",
                NotificationTypeName = NotificationType.AllLookupDictionary.TryGetValue(n.NotificationTypeID, out var notifType)
                    ? notifType.NotificationTypeDisplayName
                    : $"Unknown ({n.NotificationTypeID})"
            })
            .ToList();

        return notifications;
    }
}
