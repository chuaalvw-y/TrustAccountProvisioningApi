using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ChuA.ObservabilityLegacy;
using ChuA.ObservabilityLegacy.Abstractions;
using ChuA.ObservabilityLegacy.Utilities;
using Serilog;
using Serilog.Events;

namespace TrustAccountProvisioningApi
{
    public static class ObservabilityConfig
    {
        public static void Register()
        {
            ChuAObservabilityLegacy.Configure();
            AddSensitiveKey("AccountNumber");
            ConfigureFileLogger();

            Logger.Information("Trust Account Provisioning API observability configured.");
        }

        public static void EnsureFallbackLogger()
        {
            ConfigureFileLogger();
        }

        public static void Dispose()
        {
            if (ChuAObservabilityLegacy.Provider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public static IApplicationLogger Logger =>
            ChuAObservabilityLegacy.Provider.ApplicationLogger;

        private static void ConfigureFileLogger()
        {
            var logDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "App_Data",
                "Logs");

            Directory.CreateDirectory(logDirectory);

            var logPath = Path.Combine(logDirectory, "app-.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "TrustAccountProvisioningApi")
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    shared: true,
                    outputTemplate: "timestamp=\"{Timestamp:yyyy-MM-ddTHH:mm:ss.fffZ}\" level=\"{Level:u3}\" application=\"TrustAccountProvisioningApi\" message=\"{Message:lj}\" properties=\"{Properties:j}\" exception=\"{Exception}\"{NewLine}")
                .CreateLogger();
        }

        private static void AddSensitiveKey(string key)
        {
            var field = typeof(SensitiveDataRedactor).GetField(
                "SensitiveKeys",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (field?.GetValue(null) is string[] sensitiveKeys &&
                !sensitiveKeys.Any(
                    existingKey => string.Equals(
                        existingKey,
                        key,
                        StringComparison.OrdinalIgnoreCase)))
            {
                field.SetValue(null, sensitiveKeys.Concat(new[] { key }).ToArray());
            }
        }
    }
}
