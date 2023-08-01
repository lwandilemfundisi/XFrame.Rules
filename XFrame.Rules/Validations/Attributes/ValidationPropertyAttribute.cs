using XFrame.Common.Extensions;

namespace XFrame.Rules.Validations.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ValidationPropertyAttribute : Attribute
    {
        #region Constructors

        public ValidationPropertyAttribute(string propertyName)
        {
            PropertyName = propertyName;
            DisplayName = propertyName.RemoveCamelCasing();
        }

        public ValidationPropertyAttribute(string propertyName, Type owner)
        {
            PropertyName = propertyName;
            DisplayName = propertyName.RemoveCamelCasing();
            Owner = owner;
        }

        public ValidationPropertyAttribute(string propertyName, string displayName)
        {
            PropertyName = propertyName;
            DisplayName = displayName;
        }

        #endregion

        #region Properties

        public string PropertyName { get; }

        public Type Owner { get; }

        public string DisplayName { get; }

        public string ReadOnlyMessage { get; set; }

        #endregion
    }
}
