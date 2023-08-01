using System.Collections;

namespace XFrame.Rules.Validations.Common
{
    public interface IAllowedValidation
        : IValidation
    {
        IEnumerable GetAllowedValues();
    }
}
