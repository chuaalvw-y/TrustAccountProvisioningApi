using System;
using System.Web.Http;
using System.Web.Mvc;
using ChuA.DatabaseLegacy;
using ChuA.ObservabilityLegacy.Abstractions;
using TrustAccountProvisioningApi.Infrastructure;
using TrustAccountProvisioningApi.Services;

namespace TrustAccountProvisioningApi
{
    public static class DependencyConfig
    {
        private static SimpleContainer _container;

        public static void Register()
        {
            var container = new SimpleContainer();

            container.RegisterSingleton<IChuADatabase>(CreateDatabase);
            container.RegisterSingleton<IApplicationLogger>(() => ObservabilityConfig.Logger);
            container.Register<IAccountNumberListService, AccountNumberListService>();

            var resolver = new AppDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;

            _container = container;
        }

        public static void Dispose()
        {
            _container?.Dispose();
        }

        private static IChuADatabase CreateDatabase()
        {
            return ChuADatabase.CreateSqlServerFromAppSettings();
        }
    }
}
