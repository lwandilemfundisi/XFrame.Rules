using XFrame.Common.Extensions;
using XFrame.Notifications;

namespace XFrame.Rules.Validations.Common
{
    public abstract class StringRangeValidation<T> 
        : Validation<T>, IRangeValidation where T : class
    {
        #region IRange Members

        public object GetMaximum()
        {
            return OnGetMaximum();
        }

        public object GetMinimum()
        {
            return OnGetMinimum();
        }

        #endregion

        #region Virtual Methods

        protected virtual int OnGetMaximum()
        {
            return 255;
        }

        protected virtual int OnGetMinimum()
        {
            return 1;
        }

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue.AsString();

            //rule should not check empty values. Required Rules must validate empty or null values
            if (propertyValue.IsNotNullOrEmpty())
            {
                var minimum = OnGetMinimum();
                var maximum = OnGetMaximum();

                if (propertyValue.Length < minimum || propertyValue.Length > maximum)
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} does not fall between the range of {1} and {2}", DisplayName, OnGetMinimum(), OnGetMaximum());
        }

        #endregion
    }

    public abstract class StringRangeValidation<T, C> 
        : StringRangeValidation<T>
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
