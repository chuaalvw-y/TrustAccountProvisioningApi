using System;
using System.Collections.Generic;

namespace TrustAccountProvisioningApi.Infrastructure
{
    public sealed class AppDependencyResolver :
        System.Web.Mvc.IDependencyResolver,
        System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly SimpleContainer _container;

        public AppDependencyResolver(SimpleContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public object GetService(Type serviceType)
        {
            return _container.TryResolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var service = GetService(serviceType);
            return service == null
                ? Array.Empty<object>()
                : new[] { service };
        }

        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return this;
        }

        public void Dispose()
        {
        }
    }
}
