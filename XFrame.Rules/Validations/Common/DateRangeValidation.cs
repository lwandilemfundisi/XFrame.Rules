using XFrame.Notifications;
using XFrame.Common.Extensions;

namespace XFrame.Rules.Validations.Common
{
    public abstract class DateRangeValidation<T> 
        : Validation<T>, IRangeValidation where T : class
    {
        #region IRange Members

        public object GetMinimum()
        {
            return OnGetMinimum();
        }

        public object GetMaximum()
        {
            return OnGetMaximum();
        }

        #endregion

        #region Virtual Members

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();
            var propertyValue = PropertyValue as DateTime?;

            if (propertyValue.IsNotNull())
            {
                var minimum = OnGetMinimum();

                var maximum = OnGetMaximum();

                if (propertyValue > maximum || propertyValue < minimum)
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual DateTime OnGetMaximum()
        {
            return DateTime.MaxValue;
        }

        protected virtual DateTime OnGetMinimum()
        {
            return DateTime.MinValue;
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} does not fall within the range of {1} and {2}", DisplayName, OnGetMinimum().ToLongDateString(), OnGetMaximum().ToLongDateString());
        }

        #endregion
    }
}
