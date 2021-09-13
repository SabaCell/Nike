using Nike.Framework.Domain.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nike.Framework.Domain
{
    public sealed class DomainImplementationValidator
    {
        private readonly List<Type> _modelTypes;
        private readonly List<Type> _snapshotTypes;

        private DomainImplementationValidator(List<Type> modelTypes, List<Type> snapshotTypes)
        {
            _modelTypes = modelTypes;
            _snapshotTypes = snapshotTypes;
        }

        public static DomainImplementationValidator SetAssembly(Assembly assembly = null)
        {
            assembly ??= Assembly.GetEntryAssembly();

            var modelTypes = assembly
            .GetTypes()
            .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) || typeof(IEntity).IsAssignableFrom(t) ||
                        typeof(ValueObject<>).IsAssignableFrom(t) && t.IsAbstract == false);

            var snapshotTypes = assembly
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(i => typeof(IHasSnapshot).IsAssignableFrom(i)));

            return new DomainImplementationValidator(modelTypes.ToList(), snapshotTypes.ToList());
        }

        /// <summary>
        ///     validates that each domain model has public or protected parameterless constructor
        /// </summary>
        /// <returns>returns the current instance of object (builder pattern)</returns>
        /// <exception cref="Exception">in case any invalid model is found, an exception is thrown</exception>
        public DomainImplementationValidator ValidateEmptyConstructors()
        {
            foreach (var modelType in _modelTypes)
            {
                var constructor = modelType
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance |
                                 BindingFlags.Instance)
                .SingleOrDefault(c => c.GetParameters().Length == 0);

                if (constructor == null || constructor.IsPrivate)
                    throw new Exception(
                    $"The [{modelType.Name}] type does not have a public or protected parameterless constructor");
            }

            return this;
        }

        /// <summary>
        ///     validates that properties of each model are privately set and are accessible publicly
        /// </summary>
        /// <returns>returns the current instance of object (builder pattern)</returns>
        /// <exception cref="Exception">in case any invalid model is found, an exception is thrown</exception>
        public DomainImplementationValidator ValidateGettersAndSetters()
        {
            foreach (var modelType in _modelTypes)
                foreach (var propertyInfo in modelType.GetProperties())
                {
                    if (propertyInfo.GetGetMethod(false) == null)
                        throw new Exception(
                        $"The [{propertyInfo.Name}] on [{modelType.Name}] does not have a public getter");

                    if (propertyInfo.CanWrite && propertyInfo.GetSetMethod() != null &&
                        propertyInfo.GetSetMethod(true).IsPublic)
                        throw new Exception(
                        $"The [{propertyInfo.Name}] setter on [{modelType.Name}] should not be public");
                }

            return this;
        }

        /// <summary>
        ///     validates that all properties of each domain model that inherits <see cref="IHasSnapshot" /> are present on its
        ///     snapshot model
        /// </summary>
        /// <returns>returns the current instance of object (builder pattern)</returns>
        /// <exception cref="Exception">in case any invalid model is found, an exception is thrown</exception>
        public DomainImplementationValidator ValidateSnapshots()
        {
            foreach (var snapshotType in _snapshotTypes)
            {
                var iHasSnapshotType = snapshotType.GetInterfaces()
                .Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IHasSnapshot<>));

                var snapshotModelType = iHasSnapshotType.GetGenericArguments().First();

                var domainProperties = snapshotType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var domainProperty in domainProperties)
                {
               //     if (domainProperty.Name.Equals(nameof(IAggregateRoot.Events))) continue;

                    var equivalentProperty = snapshotModelType.GetProperty(domainProperty.Name);

                    if (equivalentProperty == null)
                        throw new Exception(
                        $"The [{domainProperty.Name}] on [{snapshotType.Name}] does not exist on its snapshot model");
                }
            }

            return this;
        }
    }
}