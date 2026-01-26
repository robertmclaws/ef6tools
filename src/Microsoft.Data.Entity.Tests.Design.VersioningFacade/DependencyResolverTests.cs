// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade
{
    using System;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Infrastructure.Pluralization;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.Data.Entity.Design.VersioningFacade.LegacyProviderWrapper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class DependencyResolverTests
    {
        [TestMethod]
        public void DependencyResolver_resolves_IDbProviderServicesFactory()
        {
            DependencyResolver.GetService<DbProviderServices>("System.Data.SqlClient").Should().NotBeNull();
        }

        [TestMethod]
        public void DependencyResolver_resolves_IPluralizationService()
        {
            DependencyResolver.GetService<IPluralizationService>().Should().NotBeNull();
        }

        [TestMethod]
        public void DependencyResolver_does_not_resolve_Object()
        {
            DependencyResolver.GetService<object>().Should().BeNull();
        }

        [TestMethod]
        public void DependencyResolver_can_register_unregister_provider()
        {
            // Use a unique provider name to avoid interfering with pre-registered providers
            const string testProviderName = "Test.Provider.ForUnregisterTest";

            // Registration should succeed
            DependencyResolver.RegisterProvider(Utils.SqlProviderServicesType, testProviderName);
            try
            {
                // The registered type can be resolved directly now that we use EntityFramework directly.
                // However, SqlProviderServices.Instance checks the provider invariant name against ADO.NET,
                // so a fake provider name will still throw.
                Action act = () => DependencyResolver.GetService<DbProviderServices>(testProviderName);
                act.Should().Throw<InvalidOperationException>("fake provider is not registered with ADO.NET");
            }
            finally
            {
                DependencyResolver.UnregisterProvider(testProviderName);
            }

            // After unregistering, the same exception should still occur
            Action afterUnregister = () => DependencyResolver.GetService<DbProviderServices>(testProviderName);
            afterUnregister.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void EnsureProvider_registers_provider()
        {
            // Use a unique provider name to avoid interfering with pre-registered providers
            const string testProviderName = "Test.Provider.ForEnsureProviderTest";

            // EnsureProvider should succeed (just registers the type)
            DependencyResolver.EnsureProvider(testProviderName, Utils.SqlProviderServicesType);
            try
            {
                // A fake provider name will throw because it's not registered with ADO.NET DbProviderFactories.
                Action act = () => DependencyResolver.GetService<DbProviderServices>(testProviderName);
                act.Should().Throw<InvalidOperationException>("fake provider is not registered with ADO.NET");
            }
            finally
            {
                DependencyResolver.UnregisterProvider(testProviderName);
            }
        }

        [TestMethod]
        public void EnsureProvider_unregisters_provider_when_null()
        {
            // Use a unique provider name to avoid interfering with pre-registered providers
            const string testProviderName = "Test.Provider.ForEnsureProviderNullTest";

            DependencyResolver.RegisterProvider(Utils.SqlProviderServicesType, testProviderName);

            DependencyResolver.EnsureProvider(testProviderName, null);

            // After unregistering a fake provider with null, the legacy resolver throws because
            // the provider isn't registered with ADO.NET DbProviderFactories
            Action act = () => DependencyResolver.GetService<DbProviderServices>(testProviderName);
            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void DependencyResolver_preregisters_MicrosoftDataSqlClient()
        {
            // Microsoft.Data.SqlClient should be resolvable via SqlProviderServices
            var providerServices = DependencyResolver.GetService<DbProviderServices>("Microsoft.Data.SqlClient");

            providerServices.Should().NotBeNull("Microsoft.Data.SqlClient should be resolvable");
        }

        [TestMethod]
        public void DependencyResolver_preregisters_SystemDataSqlClient()
        {
            // System.Data.SqlClient should be resolvable
            var providerServices = DependencyResolver.GetService<DbProviderServices>("System.Data.SqlClient");

            providerServices.Should().NotBeNull("System.Data.SqlClient should be resolvable");
        }

        [TestMethod]
        public void DependencyResolver_MicrosoftDataSqlClient_resolves_to_sql_provider()
        {
            // Verify Microsoft.Data.SqlClient resolves to SqlProviderServices
            var providerServices = DependencyResolver.GetService<DbProviderServices>("Microsoft.Data.SqlClient");

            providerServices.Should().NotBeNull();
            providerServices.GetType().Name.Should().Be("SqlProviderServices");
        }

        [TestMethod]
        public void GetServices_returns_single_item_for_registered_provider()
        {
            var services = DependencyResolver.Instance.GetServices(typeof(DbProviderServices), "Microsoft.Data.SqlClient");

            services.Should().NotBeNull();
            var serviceList = new System.Collections.Generic.List<object>(services);
            serviceList.Should().ContainSingle();
            serviceList[0].Should().NotBeNull();
        }

        [TestMethod]
        public void GetServices_returns_empty_for_unknown_type()
        {
            var services = DependencyResolver.Instance.GetServices(typeof(object), null);

            services.Should().NotBeNull();
            services.Should().BeEmpty();
        }

        [TestMethod]
        public void GetServices_returns_single_item_for_pluralization_service()
        {
            var services = DependencyResolver.Instance.GetServices(typeof(IPluralizationService), null);

            services.Should().NotBeNull();
            var serviceList = new System.Collections.Generic.List<object>(services);
            serviceList.Should().ContainSingle();
            serviceList[0].Should().BeAssignableTo<IPluralizationService>();
        }

        [TestMethod]
        public void GetService_returns_null_for_DbProviderServices_with_null_key()
        {
            // When key is null, should return null (not throw)
            var result = DependencyResolver.GetService<DbProviderServices>(null);
            result.Should().BeNull();
        }
    }
}
