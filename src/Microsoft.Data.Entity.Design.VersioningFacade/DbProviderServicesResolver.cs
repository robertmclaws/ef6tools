// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.VersioningFacade
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Infrastructure.DependencyResolution;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    internal class DbProviderServicesResolver : IDbDependencyResolver
    {
        private static readonly LegacyDbProviderServicesResolver LegacyDbProviderServicesResolver =
            new LegacyDbProviderServicesResolver();

        // NOTE: This dictionary uses default (case-SENSITIVE) comparison.
        // The provider name in EDMX files is "Microsoft.Data.SqlClient" (exact casing),
        // and we register with the same casing, so case-sensitivity is NOT an issue.
        // If lookups fail, check: 1) static initialization order, 2) stale VSIX builds
        private readonly Dictionary<string, Type> _providerServicesRegistrar = new Dictionary<string, Type>();

        public void Register(Type type, string invariantName)
        {
            Debug.Assert(type != null, "type != null");
            Debug.Assert(
                IsDbProviderServicesType(type),
                "expected type derived from DbProviderServices");
            Debug.Assert(!string.IsNullOrWhiteSpace(invariantName), "invariantName cannot be null or empty string");

            _providerServicesRegistrar[invariantName] = type;
        }

        // Check if a type derives from DbProviderServices
        private static bool IsDbProviderServicesType(Type type)
        {
            if (type == null) return false;
            if (typeof(DbProviderServices).IsAssignableFrom(type)) return true;

            // Fallback: check by type name for cross-assembly scenarios
            var current = type;
            while (current != null)
            {
                if (current.FullName == "System.Data.Entity.Core.Common.DbProviderServices")
                    return true;
                current = current.BaseType;
            }
            return false;
        }

        public void Unregister(string invariantName)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(invariantName), "invariantName cannot be null or empty string");
            _providerServicesRegistrar.Remove(invariantName);
        }

        public object GetService(Type type, object key)
        {
            var providerInvariantName = key as string;
            if (type == typeof(DbProviderServices)
                && providerInvariantName != null)
            {
                System.Diagnostics.Debug.WriteLine($"[EF6Tools] DbProviderServicesResolver.GetService: providerInvariantName='{providerInvariantName}'");
                System.Diagnostics.Debug.WriteLine($"[EF6Tools] DbProviderServicesResolver: Registered providers count={_providerServicesRegistrar.Count}");
                foreach (var kvp in _providerServicesRegistrar)
                {
                    System.Diagnostics.Debug.WriteLine($"[EF6Tools] DbProviderServicesResolver: Registered '{kvp.Key}' -> {kvp.Value.Name}");
                }

                Type providerServicesType;
                if (_providerServicesRegistrar.TryGetValue(providerInvariantName, out providerServicesType))
                {
                    System.Diagnostics.Debug.WriteLine($"[EF6Tools] DbProviderServicesResolver: FOUND in registrar, using {providerServicesType.Name}");
                    var instance = CreateProviderInstance(providerServicesType);
                    if (instance != null)
                    {
                        return instance;
                    }
                    // CreateProviderInstance returned null (cross-assembly type mismatch), fall through to legacy resolver
                    System.Diagnostics.Debug.WriteLine($"[EF6Tools] DbProviderServicesResolver: CreateProviderInstance returned null, falling back to LegacyDbProviderServicesResolver");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[EF6Tools] DbProviderServicesResolver: NOT FOUND in registrar, falling back to LegacyDbProviderServicesResolver");
                }
                return LegacyDbProviderServicesResolver.GetService(type, key);
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type type, object key)
        {
            var service = GetService(type, key);
            return service != null ? new[] { service } : Enumerable.Empty<object>();
        }

        private static DbProviderServices CreateProviderInstance(Type providerType)
        {
            Debug.Assert(providerType != null, "providerType != null");
            Debug.Assert(
                IsDbProviderServicesType(providerType),
                "expected type derived from DbProviderServices");

            const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var instanceMember = providerType.GetProperty("Instance", bindingFlags)
                                 ?? (MemberInfo)providerType.GetField("Instance", bindingFlags);

            if (instanceMember == null)
            {
                // Return null to allow fallback to legacy resolver
                System.Diagnostics.Debug.WriteLine($"[EF6Tools] CreateProviderInstance: No Instance member found on {providerType.AssemblyQualifiedName}");
                return null;
            }

            var instanceValue = GetInstanceValue(instanceMember);

            // Try direct cast first (works when types are from same assembly)
            var providerInstance = instanceValue as DbProviderServices;
            if (providerInstance != null)
            {
                return providerInstance;
            }

            // If the type doesn't match directly, fall back to the legacy resolver.
            if (instanceValue != null && IsDbProviderServicesType(instanceValue.GetType()))
            {
                System.Diagnostics.Debug.WriteLine($"[EF6Tools] CreateProviderInstance: Cross-assembly type mismatch for {providerType.Name}, falling back to legacy resolver");
                return null;
            }

            // Instance exists but is not a DbProviderServices type at all
            System.Diagnostics.Debug.WriteLine($"[EF6Tools] CreateProviderInstance: Instance is not DbProviderServices type for {providerType.AssemblyQualifiedName}");
            return null;
        }

        private static object GetInstanceValue(MemberInfo memberInfo)
        {
            var asPropertyInfo = memberInfo as PropertyInfo;
            if (asPropertyInfo != null)
            {
                return asPropertyInfo.GetValue(null, null);
            }
            return ((FieldInfo)memberInfo).GetValue(null);
        }
    }
}
