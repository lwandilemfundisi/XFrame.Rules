using XFrame.Notifications;

namespace XFrame.Rules.Validations.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ValidationContextAttribute : Attribute
    {
        #region Constructors

        public ValidationContextAttribute(Type contextType)
            : this(contextType, SeverityType.Critical)
        {
        }

        public ValidationContextAttribute(Type contextType, SeverityType severity)
        {
            ContextType = contextType;
            Severity = severity;
        }

        #endregion

        #region Properties

        public Type ContextType { get; }

        public SeverityType Severity { get; }

        #endregion
    }
}
