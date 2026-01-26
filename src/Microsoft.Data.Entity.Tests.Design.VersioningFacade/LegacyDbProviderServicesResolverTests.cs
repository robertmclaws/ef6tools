// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade
{
    using System;
    using System.Data.Entity.Core.Common;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.Data.Entity.Design.VersioningFacade.LegacyProviderWrapper;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class LegacyDbProviderServicesResolverTests
    {
        [TestMethod]
        public void LegacyDbProviderServicesResolver_creates_wrapper_for_legacy_providers()
        {
            new LegacyDbProviderServicesResolver().GetService(typeof(DbProviderServices), "System.Data.SqlClient")
                .Should().BeOfType<LegacyDbProviderServicesWrapper>();
        }

        [TestMethod]
        public void DefaultDbProviderServicesResolver_returns_null_for_unknown_type()
        {
            new LegacyDbProviderServicesResolver().GetService(typeof(Object), "System.Data.SqlClient")
                .Should().BeNull();
        }

        [TestMethod]
        public void DefaultDbProviderServicesResolver_returns_null_for_non_string_key()
        {
            new LegacyDbProviderServicesResolver().GetService(typeof(DbProviderServices), new object())
                .Should().BeNull();
        }

        [TestMethod]
        public void LegacyDbProviderServicesResolver_handles_MicrosoftDataSqlClient_via_SystemDataSqlClient()
        {
            // Microsoft.Data.SqlClient is handled by redirecting to System.Data.SqlClient
            // because Microsoft.Data.SqlClient doesn't support the legacy EF6 provider model.
            // This allows EDMX files that reference Microsoft.Data.SqlClient to work correctly.
            new LegacyDbProviderServicesResolver().GetService(typeof(DbProviderServices), "Microsoft.Data.SqlClient")
                .Should().BeOfType<LegacyDbProviderServicesWrapper>();
        }

        [TestMethod]
        public void LegacyDbProviderServicesResolver_handles_MicrosoftDataSqlClient_case_insensitive()
        {
            new LegacyDbProviderServicesResolver().GetService(typeof(DbProviderServices), "microsoft.data.sqlclient")
                .Should().BeOfType<LegacyDbProviderServicesWrapper>();
            new LegacyDbProviderServicesResolver().GetService(typeof(DbProviderServices), "MICROSOFT.DATA.SQLCLIENT")
                .Should().BeOfType<LegacyDbProviderServicesWrapper>();
        }
    }
}
