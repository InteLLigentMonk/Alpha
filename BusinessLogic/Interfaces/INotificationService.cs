using Data.Entities;

namespace BusinessLogic.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(NotificationEntity notificationEntity, string userId = "anonymous");
        Task DismissNotificationAsync(string notificationId, string userId);
        Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 5);
    }
}