using XFrame.Notifications;
using XFrame.Common.Extensions;
using System.Collections;

namespace XFrame.Rules.Validations.Common
{
    public abstract class AllowedValidation<T>
        : Validation<T>, IAllowedValidation where T : class
    {
        #region IAllowedValidation Members

        public IEnumerable GetAllowedValues()
        {
            return OnGetAllowedValues();
        }

        #endregion

        #region Virtual Methods

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            if (PropertyHasValue())
            {
                if (!OnContainsPropertyValue(GetAllowedValues(), PropertyValue))
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} is not allowed", DisplayName);
        }

        protected abstract IEnumerable OnGetAllowedValues();

        protected virtual bool OnContainsPropertyValue(
            IEnumerable allowedValues,
            object propertyValue)
        {
            return allowedValues.OfType<object>()
                .Contains(v => v.Equals(propertyValue));
        }

        #endregion
    }

    public abstract class AllowedValidation<T, C>
        : AllowedValidation<T>
        where T : class
        where C : class
    {
        #region Methods

        public C GetContext()
        {
            return (C)Context;
        }

        #endregion
    }
}
