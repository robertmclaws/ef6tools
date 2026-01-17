// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.Model
{
    using Xunit;

    public class ProviderNamesTests
    {
        [Fact]
        public void IsSqlServerProvider_returns_true_for_System_Data_SqlClient()
        {
            Assert.True(ProviderNames.IsSqlServerProvider("System.Data.SqlClient"));
        }

        [Fact]
        public void IsSqlServerProvider_returns_true_for_Microsoft_Data_SqlClient()
        {
            Assert.True(ProviderNames.IsSqlServerProvider("Microsoft.Data.SqlClient"));
        }

        [Fact]
        public void IsSqlServerProvider_returns_true_case_insensitive()
        {
            Assert.True(ProviderNames.IsSqlServerProvider("SYSTEM.DATA.SQLCLIENT"));
            Assert.True(ProviderNames.IsSqlServerProvider("microsoft.data.sqlclient"));
            Assert.True(ProviderNames.IsSqlServerProvider("System.Data.SQLCLIENT"));
        }

        [Fact]
        public void IsSqlServerProvider_returns_false_for_other_providers()
        {
            Assert.False(ProviderNames.IsSqlServerProvider("System.Data.SqlServerCe.4.0"));
            Assert.False(ProviderNames.IsSqlServerProvider("Oracle.ManagedDataAccess.Client"));
            Assert.False(ProviderNames.IsSqlServerProvider("Npgsql"));
            Assert.False(ProviderNames.IsSqlServerProvider("MySql.Data.MySqlClient"));
        }

        [Fact]
        public void IsSqlServerProvider_returns_false_for_null()
        {
            Assert.False(ProviderNames.IsSqlServerProvider(null));
        }

        [Fact]
        public void IsSqlServerProvider_returns_false_for_empty_string()
        {
            Assert.False(ProviderNames.IsSqlServerProvider(string.Empty));
        }

        [Fact]
        public void IsSqlServerProvider_returns_false_for_partial_match()
        {
            // Should not match partial provider names
            Assert.False(ProviderNames.IsSqlServerProvider("System.Data.SqlClient.Extended"));
            Assert.False(ProviderNames.IsSqlServerProvider("My.System.Data.SqlClient"));
        }

        [Fact]
        public void Constants_have_correct_values()
        {
            Assert.Equal("System.Data.SqlClient", ProviderNames.SystemDataSqlClient);
            Assert.Equal("Microsoft.Data.SqlClient", ProviderNames.MicrosoftDataSqlClient);
        }
    }
}
