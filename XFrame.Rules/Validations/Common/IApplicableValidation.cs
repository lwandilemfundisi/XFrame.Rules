namespace XFrame.Rules.Validations.Common
{
    public interface IApplicableValidation
        : IValidation
    {
        bool IsApplicable();
    }
}
