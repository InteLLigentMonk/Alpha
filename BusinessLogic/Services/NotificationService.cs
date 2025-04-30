using BusinessLogic.Hubs;
using BusinessLogic.Interfaces;
using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services;

public class NotificationService(AppDbContext context, IHubContext<NotificationHub> notificationHub, UserManager<AppUser> userManager) : INotificationService
{
    private readonly AppDbContext _context = context;
    private readonly IHubContext<NotificationHub> _notificationHub = notificationHub;
    private readonly UserManager<AppUser> _userManager = userManager;
    public async Task AddNotificationAsync(NotificationEntity notificationEntity, string userId = "anonymous")
    {
        if (string.IsNullOrEmpty(notificationEntity.Icon))
        {
            switch (notificationEntity.NotificationTypeId)
            {
                case 1:
                    notificationEntity.Icon = "/icons/avatars/2.svg";
                    break;

                case 2:
                    notificationEntity.Icon = "/icons/projects/project-template.svg";
                    break;
            }
        }
        notificationEntity.CreatedByUserId = userId;
        _context.Add(notificationEntity);
        await _context.SaveChangesAsync();

        var targetGroupName = await _context.NotificationTargetGroups
            .Where(g => g.Id == notificationEntity.NotificationTargetGroupId)
            .Select(g => g.TargetGroup)
            .FirstOrDefaultAsync() ?? "All";

        if (targetGroupName == "All")
        {
            await _notificationHub.Clients.All.SendAsync("ReceiveNotification", notificationEntity);
        }
        else
        {
            await _notificationHub.Clients.Group(targetGroupName).SendAsync("ReceiveNotification", notificationEntity);
        }
    }

    public async Task<IEnumerable<NotificationEntity>> GetNotificationsAsync(string userId, int take = 5)
    {
        var user = await _userManager.FindByIdAsync(userId);

        List<string> userRoles = [];
        if (user != null)
        {
            userRoles = [.. (await _userManager.GetRolesAsync(user))];
        }

        var dismissedIds = await _context.DismissedNotifications
            .Where(d => d.UserId == Guid.Parse(userId))
            .Select(d => d.NotificationId)
            .ToListAsync();

        var query = _context.Notifications
            .Include(n => n.TargetGroup)
            .Where(x => !dismissedIds.Contains(x.Id) && x.CreatedByUserId != userId);

        query = query.Where(n =>
            n.TargetGroup.TargetGroup == "All" ||
            (n.TargetGroup.TargetGroup == "Admins" && userRoles.Contains("Admin")) ||
            (n.TargetGroup.TargetGroup == "Managers" && userRoles.Contains("Management"))
        );

        var notifications = await query
            .OrderByDescending(n => n.Created)
            .Take(take)
            .ToListAsync();



        return notifications;
    }

    public async Task DismissNotificationAsync(string notificationId, string userId)
    {
        var alreadyDismissed = await _context.DismissedNotifications
            .AnyAsync(d => d.UserId == Guid.Parse(userId) && d.NotificationId == notificationId);
        if (!alreadyDismissed)
        {
            var dismissed = new NotificationDismissedEntity
            {
                UserId = Guid.Parse(userId),
                NotificationId = notificationId
            };

            _context.Add(dismissed);
            await _context.SaveChangesAsync();
        }
    }
}
