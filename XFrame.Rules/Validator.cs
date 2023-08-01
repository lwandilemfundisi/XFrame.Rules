using XFrame.Rules.Extensions;
using XFrame.Notifications;
using XFrame.Rules.Validations;
using XFrame.Rules.Validations.Attributes;
using XFrame.Rules.Validations.Common;
using XFrame.Common.Extensions;
using System.Collections.Concurrent;
using System.Reflection;
using XFrame.Common;
using XFrame.Notifications.Extensions;

namespace XFrame.Rules
{
    public class Validator : IValidator
    {
        private static object resolveValidationsLock = new object();
        private static object resolveEntityValidationsLock = new object();
        private readonly IServiceProvider _serviceProvider;

        private static IDictionary<Assembly, IList<ValidationTypeEntry>> assemblyValidations = 
            new Dictionary<Assembly, IList<ValidationTypeEntry>>();

        private static Dictionary<Type, Dictionary<Assembly, IList<ValidationTypeEntry>>> entityValidations = 
            new Dictionary<Type, Dictionary<Assembly, IList<ValidationTypeEntry>>>();

        #region Constructors

        public Validator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        #endregion

        #region Methods

        public Task<Notification> Validate(
            IEnumerable<IValidation> validations,
            CancellationToken cancellationToken)
        {
            var applicableValidations = validations.OfType<IApplicableValidation>();
            var requiredValidations = validations.OfType<IRequiredValidation>();
            var validationValidations = validations
                .Except(applicableValidations.Cast<IValidation>())
                .Except(requiredValidations.Cast<IValidation>());

            var failedRequiredValidations = new List<IRequiredValidation>();
            var failedApplicableValidations = new List<IApplicableValidation>();

            var notification = Notification.CreateEmpty();
            ConcurrentDictionary<string, Task> executingTasks = new();

            //applicability validations
            foreach (var applicableValidation in applicableValidations.Where(r => r.MustValidate()))
            {
                executingTasks.TryAdd(
                    Guid.NewGuid().ToString(),
                    applicableValidation
                    .Validate(cancellationToken)
                    .ContinueWith(r =>
                    {
                        if (r.Result.HasErrors())
                            notification += r.Result;
                        if (!applicableValidation.IsApplicable())
                            failedApplicableValidations.Add(applicableValidation);
                    }));
            }

            //required validations
            foreach (var requiredValidation in requiredValidations)
            {
                if (!failedApplicableValidations
                    .Contains(r => r.PropertyName == requiredValidation.PropertyName && r.Owner == requiredValidation.Owner))
                {
                    if (requiredValidation.MustValidate())
                    {
                        executingTasks.TryAdd(
                            Guid.NewGuid().ToString(),
                            requiredValidation
                            .Validate(cancellationToken)
                            .ContinueWith(r =>
                            {
                                notification += r.Result;
                                if (r.Result.HasErrors())
                                    failedRequiredValidations.Add(requiredValidation);
                            }));
                    }
                }
            }

            var parallelValidations = validationValidations.Where(c => c.CanExecuteParallel()).ToList();
            var nonParallelValidations = validationValidations.Where(c => !c.CanExecuteParallel()).ToList();

            //business and validation validations
            Parallel.ForEach(parallelValidations, new ParallelOptions { MaxDegreeOfParallelism = 5 }, Validation =>
            {
                executingTasks.TryAdd(
                    Guid.NewGuid().ToString(),
                    ValidateValidation(Validation, failedApplicableValidations, failedRequiredValidations, cancellationToken)
                    .ContinueWith(r =>
                    {
                        notification += r.Result;
                    }));
            });

            foreach (var Validation in nonParallelValidations)
            {
                executingTasks.TryAdd(
                    Guid.NewGuid().ToString(),
                    ValidateValidation(Validation, failedApplicableValidations, failedRequiredValidations, cancellationToken)
                    .ContinueWith(r =>
                    {
                        notification += r.Result;
                    }));
            }

            Task.WaitAll(executingTasks.Values.ToArray());
            return Task.FromResult(notification);
        }

        public async Task<Notification> Validate(
            object instance,
            object context,
            SystemCulture culture,
            Assembly assembly,
            CancellationToken cancellationToken)
        {
            return await Validate(
                await ResolveValidations(
                    instance,
                    context,
                    culture,
                    assembly,
                    cancellationToken),
                cancellationToken);
        }

        public async Task<Notification> Validate(
            object instance,
            object context,
            SystemCulture culture,
            IEnumerable<Assembly> assemblies,
            CancellationToken cancellationToken)
        {
            var validations = new List<IValidation>();

            foreach (var assembly in assemblies)
            {
                validations.AddRange(
                    await ResolveValidations(
                        instance,
                        context,
                        culture,
                        assembly,
                        cancellationToken));
            }

            return await Validate(
                validations,
                cancellationToken);
        }

        public Task<IEnumerable<IValidation>> CreateValidations(
            object context,
            SystemCulture culture,
            object owner,
            Type ownerType,
            ValidationTypeEntry ValidationTypeEntry,
            CancellationToken cancellationToken)
        {
            var validatableType = ownerType;
            var validations = new List<IValidation>();
            var ValidationAttribute = ValidationTypeEntry.ValidationAttribute;
            var contextList = ValidationTypeEntry.ValidationContexts;
            var ValidationPropertyAttributes = ValidationTypeEntry.ValidationProperties;
            var ValidationType = ValidationTypeEntry.ValidationType;

            if (ValidationAttribute.EntityType.IsNull() ||
                ValidationAttribute.EntityType.Equals(validatableType) ||
                validatableType.IsSubclassOf(ValidationAttribute.EntityType))
            {
                var contextType = context.GetType();
                var contextEntry = contextList.FirstOrDefault(c => contextType.Equals(c.ContextType) || contextType.IsSubclassOf(c.ContextType));

                if (contextList.Count == 0 || contextEntry.IsNotNull())
                {
                    if (ValidationPropertyAttributes.Count > 0)
                    {
                        foreach (var ValidationPropertyAttribute in ValidationPropertyAttributes)
                        {
                            if (ValidationPropertyAttribute.Owner.IsNull() || ValidationPropertyAttribute.Owner == validatableType)
                            {
                                validations.Add(CreateValidation(ValidationType, context, culture, contextEntry, owner, ValidationPropertyAttribute));
                            }
                        }
                    }
                    else
                    {
                        validations.Add(CreateValidation(ValidationType, context, culture, contextEntry, owner));
                    }
                }
            }

            return Task.FromResult(validations.AsEnumerable());
        }

        public async Task<IEnumerable<IValidation>> ResolveValidations(
            object instance,
            object context,
            SystemCulture culture,
            Assembly assembly,
            CancellationToken cancellationToken)
        {
            var validations = new List<IValidation>();
            var instanceType = instance.GetType();

            foreach (var ValidationClass in ResolveEntityValidations(assembly, instanceType))
            {
                validations.AddRange(
                    await CreateValidations(
                        context,
                        culture,
                        instance,
                        instanceType,
                        ValidationClass,
                        cancellationToken));
            }

            return validations;
        }

        public Task<IEnumerable<IValidation>> ResolveValidations(
            object instance,
            object context,
            SystemCulture culture,
            CancellationToken cancellationToken)
        {
            return ResolveValidations(
                instance,
                context,
                culture,
                instance.GetType().Assembly,
                cancellationToken);
        }

        #endregion

        #region Private Methods

        private static IList<ValidationTypeEntry> ResolveEntityValidations(
            Assembly assembly,
            Type entityType)
        {
            if (!entityValidations.ContainsKey(entityType))
            {
                lock (resolveEntityValidationsLock)
                {
                    if (!entityValidations.ContainsKey(entityType))
                    {
                        entityValidations.Add(entityType, new Dictionary<Assembly, IList<ValidationTypeEntry>>());
                    }
                }
            }

            if (!entityValidations[entityType].ContainsKey(assembly))
            {
                lock (resolveEntityValidationsLock)
                {
                    if (!entityValidations[entityType].ContainsKey(assembly))
                    {
                        var assemblyEntries = ResolveValidationEntries(assembly);

                        var ValidationEntries = new List<ValidationTypeEntry>();

                        foreach (var ValidationEntry in assemblyEntries)
                        {
                            if (ValidationEntry.ValidationAttribute.EntityType.IsNull() ||
                            ValidationEntry.ValidationAttribute.EntityType.Equals(entityType) ||
                            entityType.IsSubclassOf(ValidationEntry.ValidationAttribute.EntityType))
                            {
                                ValidationEntries.Add(ValidationEntry);
                            }
                        }

                        entityValidations[entityType].Add(assembly, ValidationEntries);
                    }
                }
            }

            return entityValidations[entityType][assembly];
        }

        private static IList<ValidationTypeEntry> ResolveValidationEntries(Assembly assembly)
        {
            if (!assemblyValidations.ContainsKey(assembly))
            {
                lock (resolveValidationsLock)
                {
                    if (!assemblyValidations.ContainsKey(assembly))
                    {
                        var iValidationType = typeof(IValidation);

                        var ValidationTypeList = new List<ValidationTypeEntry>();

                        foreach (var ValidationType in assembly.GetTypes().Where(t => iValidationType.IsAssignableFrom(t) && !t.IsAbstract))
                        {
                            var ValidationEntry = ValidationTypeEntry.CreateEntry(ValidationType);

                            if (ValidationEntry.IsNotNull())
                            {
                                ValidationTypeList.Add(ValidationEntry);
                            }
                        }

                        assemblyValidations.Add(assembly, ValidationTypeList);
                    }
                }
            }

            return assemblyValidations[assembly];
        }

        private IValidation CreateValidation(
            Type ValidationType,
            object context,
            SystemCulture culture,
            ValidationContextAttribute contextType,
            object owner)
        {
            var Validation = (IValidation)_serviceProvider.GetService(ValidationType);
            Validation.Context = context;
            Validation.Culture = culture;
            Validation.Owner = owner;
            Validation.Severity = contextType.IsNull() ? SeverityType.Critical : contextType.Severity;
            Validation.Name = ValidationType.Name;
            return Validation;
        }

        private IValidation CreateValidation(
            Type ValidationType,
            object context,
            SystemCulture culture,
            ValidationContextAttribute contextType,
            object owner,
            ValidationPropertyAttribute propertyType)
        {
            var Validation = CreateValidation(ValidationType, context, culture, contextType, owner);
            Validation.IsPropertyValidation = true;
            Validation.PropertyName = propertyType.PropertyName;
            Validation.DisplayName = propertyType.DisplayName;
            Validation.ReadOnlyMessage = propertyType.ReadOnlyMessage;
            return Validation;
        }

        private static async Task<Notification> ValidateValidation(
            IValidation Validation,
            IEnumerable<IApplicableValidation> applicableValidations,
            IEnumerable<IRequiredValidation> requiredValidations,
            CancellationToken cancellationToken)
        {
            var notification = Notification.CreateEmpty();

            if (!applicableValidations.Contains(r => r.PropertyName == Validation.PropertyName && r.Owner == Validation.Owner)
                    && !requiredValidations.Contains(r => r.PropertyName == Validation.PropertyName && r.Owner == Validation.Owner))
            {
                if (Validation.MustValidate())
                {
                    notification += await Validation.Validate(cancellationToken);
                }
            }

            return notification;
        }

        #endregion
    }
}
