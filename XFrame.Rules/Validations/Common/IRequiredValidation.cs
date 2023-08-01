namespace XFrame.Rules.Validations.Common
{
    public interface IRequiredValidation
        : IValidation
    {
        bool IsRequired();
    }
}
