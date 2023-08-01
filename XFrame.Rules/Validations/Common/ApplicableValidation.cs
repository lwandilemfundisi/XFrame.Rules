using XFrame.Rules.Notifications;
using XFrame.Common.Extensions;

namespace XFrame.Rules.Validations.Common
{
    public abstract class ApplicableValidation<T>
        : Validation<T>, IApplicableValidation where T : class
    {
        private bool? isApplicable;

        #region IApplicableValidation Members

        public bool IsApplicable()
        {
            if (isApplicable.IsNull())
            {
                isApplicable = OnIsApplicable();
            }

            return isApplicable.Value;
        }

        #endregion

        #region Virtual Methods

        protected override async Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            if (!IsApplicable())
            {
                if (IsPropertyValidation)
                {
                    if (PropertyHasValue())
                    {
                        notification += await OnCreateApplicableMessage();
                    }
                }
                else
                {
                    notification += await OnCreateApplicableMessage();
                }
            }

            return notification;
        }

        protected abstract bool OnIsApplicable();

        protected virtual Task<Message> OnCreateApplicableMessage()
        {
            return Task.FromResult(CreateMessage("{0} is not applicable".FormatInvariantCulture(DisplayName)));
        }

        protected override Message CreateMessage(string message, params object[] values)
        {
            var result = base.CreateMessage(message, values);

            result.MessageType = MessageType.NotApplicable;

            return result;
        }

        #endregion
    }

    public abstract class ApplicableValidation<T, C>
        : ApplicableValidation<T>
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
