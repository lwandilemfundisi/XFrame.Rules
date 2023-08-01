using XFrame.Rules.Notifications;
using XFrame.Common.Extensions;

namespace XFrame.Rules.Extensions
{
    public static class NotificationExtensions
    {
        public static bool HasMessages(this Notification notification)
        {
            return notification.Messages.Count > 0;
        }

        public static bool HasErrors(this Notification notification)
        {
            return notification
                .Messages
                .Contains(m => m.Severity == SeverityType.Critical);
        }

        public static bool HasWarnings(this Notification notification)
        {
            return notification
                .Messages
                .Contains(m => m.Severity == SeverityType.Warning);
        }

        public static bool HasInformation(this Notification notification)
        {
            return notification
                .Messages
                .Contains(m => m.Severity == SeverityType.Information);
        }

        public static IEnumerable<Message> GetErrors(this Notification notification)
        {
            return notification
                .Messages
                .Where(m => m.Severity == SeverityType.Critical);
        }

        public static IEnumerable<Message> GetWarnings(this Notification notification)
        {
            return notification
                .Messages
                .Where(m => m.Severity == SeverityType.Warning);
        }

        public static IEnumerable<Message> GetInformation(this Notification notification)
        {
            return notification
                .Messages
                .Where(m => m.Severity == SeverityType.Information);
        }
    }
}
