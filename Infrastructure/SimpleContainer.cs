using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TrustAccountProvisioningApi.Infrastructure
{
    public sealed class SimpleContainer : IDisposable
    {
        private readonly Dictionary<Type, Func<object>> _registrations =
            new Dictionary<Type, Func<object>>();

        private readonly List<IDisposable> _trackedDisposables =
            new List<IDisposable>();

        public void RegisterSingleton<TService>(Func<TService> factory)
            where TService : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            var lazy = new Lazy<TService>(() =>
            {
                var instance = factory();
                TrackDisposable(instance);
                return instance;
            });

            _registrations[typeof(TService)] = () => lazy.Value;
        }

        public void Register<TService, TImplementation>()
            where TImplementation : TService
        {
            _registrations[typeof(TService)] = () => Resolve(typeof(TImplementation));
        }

        public object TryResolve(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            try
            {
                return Resolve(serviceType);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        private object Resolve(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var factory))
            {
                return factory();
            }

            if (!CanConstruct(serviceType))
            {
                throw new InvalidOperationException(
                    $"Type '{serviceType.FullName}' has not been registered.");
            }

            var constructor = SelectConstructor(serviceType);
            var parameters = constructor
                .GetParameters()
                .Select(parameter => Resolve(parameter.ParameterType))
                .ToArray();

            return constructor.Invoke(parameters);
        }

        private static bool CanConstruct(Type serviceType)
        {
            return serviceType.IsClass
                && !serviceType.IsAbstract
                && !serviceType.ContainsGenericParameters;
        }

        private static ConstructorInfo SelectConstructor(Type serviceType)
        {
            var constructors = serviceType.GetConstructors();

            if (constructors.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Type '{serviceType.FullName}' has no public constructors.");
            }

            return constructors
                .OrderByDescending(constructor => constructor.GetParameters().Length)
                .First();
        }

        private void TrackDisposable(object instance)
        {
            if (instance is IDisposable disposable)
            {
                _trackedDisposables.Add(disposable);
            }
        }

        public void Dispose()
        {
            foreach (var disposable in _trackedDisposables.AsEnumerable().Reverse())
            {
                disposable.Dispose();
            }

            _trackedDisposables.Clear();
        }
    }
}
