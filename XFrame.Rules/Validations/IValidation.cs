using XFrame.Notifications;
using XFrame.Common.Extensions;
using XFrame.Common;

namespace XFrame.Rules.Validations
{
    public interface IValidation
    {
        SeverityType Severity { get; set; }

        SystemCulture Culture { get; set; }

        object Context { get; set; }

        object Owner { get; set; }

        Task<Notification> Validate(CancellationToken cancellationToken);

        bool IsPropertyValidation { get; set; }

        bool MustValidate();

        bool CanExecuteParallel();

        string Name { get; set; }

        string PropertyName { get; set; }

        string ReadOnlyMessage { get; set; }

        string DisplayName { get; set; }
    }
}
