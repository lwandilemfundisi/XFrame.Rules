using XFrame.Rules.Extensions;
using XFrame.Rules.Notifications;
using XFrame.Common.Extensions;

namespace XFrame.Rules.Validations.Common
{
    public abstract class AlphanumericPropertyValidation<T>
        : Validation<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            return ValidatePropertyValue(PropertyValue.AsString());
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} must be alphanumeric", DisplayName);
        }

        #endregion

        #region Methods

        protected Task<Notification> ValidatePropertyValue(string propertyValue)
        {
            var notification = Notification.CreateEmpty();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (!StringValidationHelper.ValidateString(
                    propertyValue,
                    AllowedCharacter.Alpha,
                    AllowedCharacter.Numeric))
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        #endregion
    }
}
