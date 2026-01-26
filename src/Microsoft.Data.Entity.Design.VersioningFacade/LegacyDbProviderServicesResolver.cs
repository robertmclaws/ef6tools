// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Legacy = System.Data.Common;

namespace Microsoft.Data.Entity.Design.VersioningFacade
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Infrastructure.DependencyResolution;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.Data.Entity.Design.VersioningFacade.LegacyProviderWrapper;

    internal class LegacyDbProviderServicesResolver : IDbDependencyResolver
    {
        private const string MicrosoftDataSqlClient = "Microsoft.Data.SqlClient";

        public object GetService(Type type, object key)
        {
            var providerInvariantName = key as string;

            Debug.WriteLine($"[EF6Tools] LegacyDbProviderServicesResolver.GetService: type={type?.Name}, key='{key}'");

            if (type == typeof(DbProviderServices)
                && providerInvariantName != null)
            {
                // For SQL Server providers, try to use System.Data.SqlClient which has proper EF6 support
                // Microsoft.Data.SqlClient is for EF Core and doesn't support the legacy provider model
                var effectiveProviderName = providerInvariantName;
                if (string.Equals(providerInvariantName, MicrosoftDataSqlClient, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine($"[EF6Tools] LegacyDbProviderServicesResolver: Microsoft.Data.SqlClient requested, using System.Data.SqlClient instead");
                    effectiveProviderName = "System.Data.SqlClient";
                }

                Legacy.DbProviderFactory factory;
                try
                {
                    factory = Legacy.DbProviderFactories.GetFactory(effectiveProviderName);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "The ADO.NET provider '{0}' is not registered on this machine. " +
                            "Please ensure the provider is installed and registered in the machine.config or app.config file.",
                            effectiveProviderName),
                        ex);
                }

                if (factory == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "DbProviderFactories.GetFactory returned null for provider '{0}'.",
                            effectiveProviderName));
                }

                var serviceProvider = factory as IServiceProvider;
                if (serviceProvider == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "The ADO.NET provider '{0}' does not implement IServiceProvider and cannot be used with the EF6 designer. " +
                            "This typically occurs when using Microsoft.Data.SqlClient, which is designed for EF Core. " +
                            "Consider using System.Data.SqlClient instead for EF6 projects.",
                            effectiveProviderName));
                }

                var legacyProviderServices = serviceProvider.GetService(typeof(Legacy.DbProviderServices)) as Legacy.DbProviderServices;

                if (legacyProviderServices == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            System.Globalization.CultureInfo.CurrentCulture,
                            "The ADO.NET provider '{0}' does not support the legacy Entity Framework 6 provider model. " +
                            "The provider factory does not return DbProviderServices when queried. " +
                            "This typically occurs when using Microsoft.Data.SqlClient, which is designed for EF Core. " +
                            "Consider using System.Data.SqlClient instead for EF6 projects, or ensure the EF6 provider is properly installed.",
                            effectiveProviderName));
                }

                Debug.WriteLine($"[EF6Tools] LegacyDbProviderServicesResolver: Creating LegacyDbProviderServicesWrapper for provider '{effectiveProviderName}' (requested as '{providerInvariantName}')");
                return new LegacyDbProviderServicesWrapper(legacyProviderServices);
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type type, object key)
        {
            var service = GetService(type, key);
            return service != null ? new[] { service } : Enumerable.Empty<object>();
        }
    }
}
