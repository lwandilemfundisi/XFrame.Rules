using XFrame.Notifications;
using XFrame.Common.Extensions;
using XFrame.ValueObjects.XmlValueObjects;

namespace XFrame.Rules.Validations.Common
{
    public abstract class RequiredValidation<T> : 
        Validation<T>, IRequiredValidation where T : class
    {
        #region IRequiredValidation Members

        public bool IsRequired()
        {
            return OnIsRequired();
        }

        #endregion

        #region Virtual Members

        protected override Task<Notification> OnValidate(CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            if (IsRequired())
            {
                if (PropertyValue.IsNull())
                {
                    notification.AddMessage(OnCreateMessage());
                }
                else
                {
                    if (!RequiredPropertyHasValue())
                    {
                        notification.AddMessage(OnCreateMessage());
                    }
                }
            }

            return Task.FromResult(notification);
        }

        protected virtual bool OnIsRequired()
        {
            return true;
        }

        protected virtual Message OnCreateMessage()
        {
            return CreateMessage("{0} is required", DisplayName);
        }

        protected override Message CreateMessage(string message, params object[] values)
        {
            var result = base.CreateMessage(message, values);

            result.MessageType = MessageType.Required;

            return result;
        }

        protected virtual bool RequiredPropertyHasValue()
        {
            if (PropertyValueType.Equals(typeof(string)))
            {
                var stringValue = IsSpaceValidForString ? PropertyValue.AsString() : PropertyValue.AsString().Trim(' ');
                if (stringValue.IsNullOrEmpty())
                {
                    return false;
                }
            }
            else if (PropertyValueType.Equals(typeof(byte[])) && ((byte[])PropertyValue).Length == 0)
            {
                return false;
            }
            else if (PropertyValueType.IsSubclassOf(typeof(XmlValueObject)) && ((XmlValueObject)PropertyValue).Code.IsNullOrEmpty())
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether only a space is a valid value for string properties
        /// </summary>
        public virtual bool IsSpaceValidForString
        {
            get
            {
                return true;
            }
        }

        #endregion
    }

    public abstract class RequiredValidation<T, C> : RequiredValidation<T>
        where T : class
        where C : class
    {
        #region Methods

        public C GetContext()
        {
            return (C)base.Context;
        }

        #endregion 
    }

    public abstract class RequiredListRule<T, K> : RequiredValidation<T>
        where T : class
        where K : class
    {
        #region Properties

        public abstract IList<K> List { get; }

        protected override bool RequiredPropertyHasValue()
        {
            return List.Count > 0;
        }

        #endregion
    }
}
