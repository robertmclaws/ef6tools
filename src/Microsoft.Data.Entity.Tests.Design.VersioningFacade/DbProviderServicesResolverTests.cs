// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using SystemDataCommon = System.Data.Common;

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade
{
    using System;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Core.Common.CommandTrees;
    using System.Globalization;
    using Microsoft.Data.Entity.Design.VersioningFacade.LegacyProviderWrapper;
    using Moq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;
    using Microsoft.Data.Entity.Design.VersioningFacade;

    [TestClass]
    public class DbProviderServicesResolverTests
    {
        private const string SqlClientInvariantName = "System.Data.SqlClient";

        [TestMethod]
        public void Legacy_provider_services_resolved_by_default()
        {
            var resolver = new DbProviderServicesResolver();
            var providerServices = resolver.GetService(typeof(DbProviderServices), SqlClientInvariantName);

            providerServices.Should().NotBeNull();
            providerServices.Should().BeOfType<LegacyDbProviderServicesWrapper>();
        }

        [TestMethod]
        public void Can_register_unregister_provider()
        {
            var resolver = new DbProviderServicesResolver();
            resolver.Register(Utils.SqlProviderServicesType, SqlClientInvariantName);

            // Verify the provider is resolved - either directly as SqlProviderServices or via wrapper
            var registeredService = resolver.GetService(typeof(DbProviderServices), SqlClientInvariantName);
            registeredService.Should().NotBeNull();
            registeredService.GetType().Name.Should().Be("SqlProviderServices");

            resolver.Unregister(SqlClientInvariantName);

            resolver.GetService(typeof(DbProviderServices), SqlClientInvariantName)
                .Should().BeOfType<LegacyDbProviderServicesWrapper>();
        }

        [TestMethod]
        public void Unregistering_not_registered_provider_does_not_throw()
        {
            var resolver = new DbProviderServicesResolver();
            resolver.Unregister(SqlClientInvariantName);

            resolver.GetService(typeof(DbProviderServices), SqlClientInvariantName)
                .Should().BeOfType<LegacyDbProviderServicesWrapper>();
        }

        [TestMethod]
        public void Registering_registered_provider_replaces_provider()
        {
            var mockProviderServices = new Mock<DbProviderServices>();

            var resolver = new DbProviderServicesResolver();
            resolver.Register(mockProviderServices.Object.GetType(), SqlClientInvariantName);
            resolver.Register(Utils.SqlProviderServicesType, SqlClientInvariantName);

            // Verify the provider is resolved - should be SqlProviderServices after replacement
            var service = resolver.GetService(typeof(DbProviderServices), SqlClientInvariantName);
            service.Should().NotBeNull();
            service.GetType().Name.Should().Be("SqlProviderServices");

            resolver.Unregister(SqlClientInvariantName);

            resolver.GetService(typeof(DbProviderServices), SqlClientInvariantName)
                .Should().BeOfType<LegacyDbProviderServicesWrapper>();
        }

        [TestMethod]
        public void Resolving_provider_without_static_Instance_field_or_property_falls_back_to_legacy_resolver()
        {
            var mockProviderServices = new Mock<DbProviderServices>();

            var resolver = new DbProviderServicesResolver();
            resolver.Register(mockProviderServices.Object.GetType(), "fakeProvider");

            // When provider has no Instance member, resolver falls back to LegacyDbProviderServicesResolver
            // which throws because "fakeProvider" is not registered with ADO.NET
            Action act = () => resolver.GetService(typeof(DbProviderServices), "fakeProvider");
            act.Should().Throw<InvalidOperationException>()
                .Where(e => e.Message.Contains("fakeProvider") && e.Message.Contains("not registered"));
        }

        private class ProviderFake : DbProviderServices
        {
            public static object Instance
            {
                get { return new object(); }
            }

            #region Not Implemented

            protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
            {
                throw new NotImplementedException();
            }

            protected override string GetDbProviderManifestToken(SystemDataCommon.DbConnection connection)
            {
                throw new NotImplementedException();
            }

            protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [TestMethod]
        public void Resolving_provider_whose_Instance_returns_non_DbProviderServices_falls_back_to_legacy_resolver()
        {
            var resolver = new DbProviderServicesResolver();
            resolver.Register(typeof(ProviderFake), "fakeProvider");

            // When Instance returns non-DbProviderServices, resolver falls back to LegacyDbProviderServicesResolver
            // which throws because "fakeProvider" is not registered with ADO.NET
            Action act = () => resolver.GetService(typeof(DbProviderServices), "fakeProvider");
            act.Should().Throw<InvalidOperationException>()
                .Where(e => e.Message.Contains("fakeProvider") && e.Message.Contains("not registered"));
        }

        [TestMethod]
        public void Resolving_non_DbProviderServices_type_returns_null()
        {
            new DbProviderServicesResolver().GetService(typeof(object), "abc").Should().BeNull();
        }

        [TestMethod]
        public void Resolving_without_invariant_name_type_returns_null()
        {
            new DbProviderServicesResolver().GetService(typeof(DbProviderServices), null).Should().BeNull();
            new DbProviderServicesResolver().GetService(typeof(DbProviderServices), new object()).Should().BeNull();
        }
    }
}
