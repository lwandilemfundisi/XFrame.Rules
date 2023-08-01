using XFrame.Common.Extensions;
using XFrame.Notifications;

namespace XFrame.Rules.Validations.Common
{
    public abstract class IntRangeValidation<T> 
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

        #region Virtual Members

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            var propertyValue = PropertyValue as int?;

            if (propertyValue.IsNotNull())
            {
                var minimum = GetMinimum() as int?;
                var maximum = GetMaximum() as int?;

                if (propertyValue < minimum || propertyValue > maximum)
                {
                    notification.AddMessage(OnCreateMessage());
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual int OnGetMinimum()
        {
            return int.MinValue;
        }

        protected virtual int OnGetMaximum()
        {
            return int.MaxValue;
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} does not fall within the range of {1} and {2}", DisplayName, OnGetMaximum(), OnGetMinimum());
        }

        #endregion
    }

    public abstract class IntRangeValidation<T, C> 
        : IntRangeValidation<T>
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
