namespace XFrame.Rules.Validations.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ValidationAttribute : Attribute
    {
        #region Constructors

        public ValidationAttribute(
            Type entityType, 
            Type entityOwnerType)
        {
            EntityType = entityType;
            EntityOwnerType = entityOwnerType;
        }

        public ValidationAttribute(
            Type entityType)
        {
            EntityType = entityType;
        }

        public ValidationAttribute()
        {
        }

        #endregion

        #region Public Properties

        public Type EntityType { get; }

        public Type EntityOwnerType { get; }

        #endregion
    }
}
