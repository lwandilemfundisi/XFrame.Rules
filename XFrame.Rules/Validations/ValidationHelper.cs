using XFrame.Notifications;
using XFrame.Common.Extensions;

namespace XFrame.Rules.Validations
{
    public static class ValidationHelper
    {
        public static Message CreateMessage(
            IValidation validation, 
            string propertyName, 
            string message, 
            string tag, 
            params object[] values)
        {
            if (values.IsNotNull())
            {
                message = message.FormatInvariantCulture(values);
            }

            return new Message(message)
            {
                Severity = validation.Severity,
                PropertyName = propertyName,
                Tag = tag,
                RuleInfo = validation.Name
            };
        }

        public static Notification ValidateNullValue(
            string propertyName, 
            object propertyValue, 
            string message, 
            params object[] values)
        {
            var notification = Notification.CreateEmpty();

            if (propertyValue.IsNull())
            {
                notification += AddNotification(propertyName, message, values);
            }

            return notification;
        }

        public static Notification ValidateEmptyValue(
            string propertyName, 
            string propertyValue, 
            string message, 
            params object[] values)
        {
            var notification = Notification.CreateEmpty();

            if (propertyValue.IsNullOrEmpty())
            {
                notification += AddNotification(propertyName, message, values);
            }

            return notification;
        }

        public static Notification CreateRuleMessage(
            string propertyName, 
            string message)
        {
            return AddNotification(propertyName, message);
        }

        #region Private Methods

        private static Notification AddNotification(
            string propertyName, 
            string message, 
            params object[] values)
        {
            var notification = Notification.CreateEmpty();

            if (values.IsNotNull())
            {
                message = message.FormatInvariantCulture(values);
            }

            notification.AddMessage(
                    new Message(message)
                    {
                        PropertyName = propertyName,
                        Severity = SeverityType.Critical
                    });

            return notification;
        }

        #endregion
    }
}
