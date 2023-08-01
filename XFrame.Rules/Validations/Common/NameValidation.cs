using XFrame.Common.Extensions;
using XFrame.Rules.Extensions;
using XFrame.Notifications;

namespace XFrame.Rules.Validations.Common
{
    public abstract class NameValidation<T> 
        : Validation<T> where T : class
    {
        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            if (propertyValue.IsNotNullOrEmpty())
            {
                if (!StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Alpha, AllowedCharacter.Space, AllowedCharacter.Apostrophe, AllowedCharacter.Dash, AllowedCharacter.ForwardSlash, AllowedCharacter.Exclamation, AllowedCharacter.Ampersand, AllowedCharacter.RoundBrackets, AllowedCharacter.AccentedVowelsLower))
                {
                    notification.AddMessage(CreateMessage("{0} is not valid", DisplayName));
                    return Task.FromResult(notification);
                }

                if (StringValidationHelper.ValidateString(propertyValue, AllowedCharacter.Apostrophe, AllowedCharacter.Dash, AllowedCharacter.ForwardSlash, AllowedCharacter.Exclamation))
                {
                    notification.AddMessage(CreateMessage("{0} must contain at least one letter of the alphabet", DisplayName));
                    return Task.FromResult(notification);
                }

                var terminationCharacter = PropertyValue.AsString().Last().ToString();

                if (!StringValidationHelper.ValidateString(terminationCharacter, AllowedCharacter.Alpha, AllowedCharacter.Space, AllowedCharacter.RoundBrackets, AllowedCharacter.AccentedVowelsLower))
                {
                    notification.AddMessage(CreateMessage("{0} may not terminate in a {1}", DisplayName, terminationCharacter));
                    return Task.FromResult(notification);
                }

            }

            return Task.FromResult(notification);
        }

        #endregion
    }
}
