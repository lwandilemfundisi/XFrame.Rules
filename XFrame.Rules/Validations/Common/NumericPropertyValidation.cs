using XFrame.Common.Extensions;
using XFrame.Rules.Extensions;
using XFrame.Notifications;

namespace XFrame.Rules.Validations.Common
{
    public abstract class NumericPropertyValidation<T> 
        : Validation<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (!StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Numeric, AllowedCharacter.Dot, AllowedCharacter.Comma))
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual Message OnCreateMessage()
        {
            var propertyValue = PropertyValue.AsString();
            if (propertyValue.Contains(" "))
            {
                return CreateMessage("{0} may not contain any spaces", DisplayName);
            }

            return CreateMessage("{0} must be numeric", DisplayName);
        }

        #endregion
    }
}
