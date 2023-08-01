using System.Reflection;
using XFrame.Common;
using XFrame.Common.Extensions;
using XFrame.Rules.Notifications;
using XFrame.Rules.Validations;

namespace XFrame.Rules
{
    public interface IValidator
    {
        Task<Notification> Validate(
            IEnumerable<IValidation> validations,
            CancellationToken cancellationToken);

        Task<Notification> Validate(
            object instance,
            object context,
            SystemCulture culture,
            Assembly assembly,
            CancellationToken cancellationToken);

        Task<Notification> Validate(
            object instance,
            object context,
            SystemCulture culture,
            IEnumerable<Assembly> assemblies,
            CancellationToken cancellationToken);
    }
}
