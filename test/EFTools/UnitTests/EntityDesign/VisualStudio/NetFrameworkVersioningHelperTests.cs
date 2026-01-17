// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.VisualStudio
{
    using UnitTests.TestHelpers;
    using Xunit;

    public class NetFrameworkVersioningHelperTests
    {
        [Fact]
        public void TargetNetFrameworkVersion_returns_targeted_version_from_valid_net_framework_moniker()
        {
            var mockMonikerHelper = new MockDTE(".NETFramework,Version=v4.0,Profile=Client");

            Assert.Equal(
                NetFrameworkVersioningHelper.NetFrameworkVersion4,
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void TargetNetFrameworkVersion_returns_null_for_null_target_net_framework_moniker()
        {
            var mockMonikerHelper = new MockDTE(null);

            Assert.Null(
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void TargetNetFrameworkVersion_returns_null_for_empty_target_net_framework_moniker()
        {
            var mockMonikerHelper = new MockDTE(string.Empty);

            Assert.Null(
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void TargetNetFrameworkVersion_returns_null_for_non_string_target_net_framework_moniker()
        {
            var mockMonikerHelper = new MockDTE(new object());

            Assert.Null(
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void TargetNetFrameworkVersion_returns_null_for_invalid_target_framework_moniker()
        {
            var mockMonikerHelper = new MockDTE("abc");

            Assert.Null(
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void TargetNetFrameworkVersion_returns_null_for_misc_project()
        {
            const string vsMiscFilesProjectUniqueName = "<MiscFiles>";

            var mockMonikerHelper = new MockDTE("abc", vsMiscFilesProjectUniqueName);

            Assert.Null(
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void TargetNetFrameworkVersion_returns_null_for_non_NetFramework_project()
        {
            var mockMonikerHelper = new MockDTE("Xbox,Version=v4.0");

            Assert.Null(
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        // Tests for modern .NET support (Phase 1)

        [Fact]
        public void TargetNetFrameworkVersion_returns_null_for_modern_dotnet_project()
        {
            // .NET 8 project should return null from TargetNetFrameworkVersion
            var mockMonikerHelper = new MockDTE(".NET,Version=v8.0");

            Assert.Null(
                NetFrameworkVersioningHelper.TargetNetFrameworkVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsModernDotNetProject_returns_true_for_net8()
        {
            var mockMonikerHelper = new MockDTE(".NET,Version=v8.0");

            Assert.True(
                NetFrameworkVersioningHelper.IsModernDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsModernDotNetProject_returns_true_for_net9()
        {
            var mockMonikerHelper = new MockDTE(".NET,Version=v9.0");

            Assert.True(
                NetFrameworkVersioningHelper.IsModernDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsModernDotNetProject_returns_true_for_net10()
        {
            var mockMonikerHelper = new MockDTE(".NET,Version=v10.0");

            Assert.True(
                NetFrameworkVersioningHelper.IsModernDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsModernDotNetProject_returns_true_for_netcoreapp31()
        {
            var mockMonikerHelper = new MockDTE(".NETCoreApp,Version=v3.1");

            Assert.True(
                NetFrameworkVersioningHelper.IsModernDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsModernDotNetProject_returns_false_for_net_framework()
        {
            var mockMonikerHelper = new MockDTE(".NETFramework,Version=v4.8");

            Assert.False(
                NetFrameworkVersioningHelper.IsModernDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsModernDotNetProject_returns_false_for_null_moniker()
        {
            var mockMonikerHelper = new MockDTE(null);

            Assert.False(
                NetFrameworkVersioningHelper.IsModernDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void TargetRuntimeVersion_returns_version_for_net8()
        {
            var mockMonikerHelper = new MockDTE(".NET,Version=v8.0");

            var version = NetFrameworkVersioningHelper.TargetRuntimeVersion(
                mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider);

            Assert.NotNull(version);
            Assert.Equal(8, version.Major);
            Assert.Equal(0, version.Minor);
        }

        [Fact]
        public void TargetRuntimeVersion_returns_version_for_net_framework()
        {
            var mockMonikerHelper = new MockDTE(".NETFramework,Version=v4.8");

            var version = NetFrameworkVersioningHelper.TargetRuntimeVersion(
                mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider);

            Assert.NotNull(version);
            Assert.Equal(4, version.Major);
            Assert.Equal(8, version.Minor);
        }

        [Fact]
        public void TargetRuntimeVersion_returns_null_for_unsupported_framework()
        {
            var mockMonikerHelper = new MockDTE("Xbox,Version=v4.0");

            Assert.Null(
                NetFrameworkVersioningHelper.TargetRuntimeVersion(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsSupportedDotNetProject_returns_true_for_net8()
        {
            var mockMonikerHelper = new MockDTE(".NET,Version=v8.0");

            Assert.True(
                NetFrameworkVersioningHelper.IsSupportedDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsSupportedDotNetProject_returns_true_for_net10()
        {
            var mockMonikerHelper = new MockDTE(".NET,Version=v10.0");

            Assert.True(
                NetFrameworkVersioningHelper.IsSupportedDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsSupportedDotNetProject_returns_true_for_net_framework_48()
        {
            var mockMonikerHelper = new MockDTE(".NETFramework,Version=v4.8");

            Assert.True(
                NetFrameworkVersioningHelper.IsSupportedDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsSupportedDotNetProject_returns_true_for_net_framework_35()
        {
            var mockMonikerHelper = new MockDTE(".NETFramework,Version=v3.5");

            Assert.True(
                NetFrameworkVersioningHelper.IsSupportedDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsSupportedDotNetProject_returns_false_for_net_framework_20()
        {
            var mockMonikerHelper = new MockDTE(".NETFramework,Version=v2.0");

            Assert.False(
                NetFrameworkVersioningHelper.IsSupportedDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }

        [Fact]
        public void IsSupportedDotNetProject_returns_false_for_unsupported_framework()
        {
            var mockMonikerHelper = new MockDTE("Xbox,Version=v4.0");

            Assert.False(
                NetFrameworkVersioningHelper.IsSupportedDotNetProject(
                    mockMonikerHelper.Project, mockMonikerHelper.ServiceProvider));
        }
    }
}
