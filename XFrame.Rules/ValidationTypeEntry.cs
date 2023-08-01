using XFrame.Rules.Validations.Attributes;
using XFrame.Common.Extensions;

namespace XFrame.Rules
{
    public class ValidationTypeEntry
    {
        #region Properties

        public Type ValidationType { get; set; }

        public ValidationAttribute ValidationAttribute { get; set; }

        public IList<ValidationContextAttribute> ValidationContexts { get; set; }

        public IList<ValidationPropertyAttribute> ValidationProperties { get; set; }

        #endregion

        #region Methods

        public static ValidationTypeEntry CreateEntry(Type ValidationType)
        {
            var ValidationEntry = new ValidationTypeEntry();

            ValidationEntry.ValidationType = ValidationType;
            ValidationEntry.ValidationAttribute = ValidationType
                .GetCustomAttributes(typeof(ValidationAttribute), false)
                .FirstOrDefault() as ValidationAttribute;

            if (ValidationEntry.ValidationAttribute.IsNotNull())
            {
                ValidationEntry.ValidationContexts = new List<ValidationContextAttribute>();
                ValidationType.GetCustomAttributes(typeof(ValidationContextAttribute), false)
                    .ForEach(rc => ValidationEntry.ValidationContexts.Add((ValidationContextAttribute)rc));

                ValidationEntry.ValidationProperties = new List<ValidationPropertyAttribute>();
                ValidationType.GetCustomAttributes(typeof(ValidationPropertyAttribute), false)
                    .ForEach(rc => ValidationEntry.ValidationProperties.Add((ValidationPropertyAttribute)rc));

                return ValidationEntry;
            }

            return null;
        }

        #endregion
    }
}
