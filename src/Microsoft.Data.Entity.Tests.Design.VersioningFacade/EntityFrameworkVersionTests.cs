// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade
{
    using System;
    using System.Linq;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class EntityFrameworkVersionTests
    {
        [TestMethod]
        public void GetAllVersions_returns_all_declared_versions()
        {
            var declaredVersions =
                typeof(EntityFrameworkVersion)
                    .GetFields()
                    .Where(f => f.FieldType == typeof(Version))
                    .Select(f => f.GetValue(null))
                    .OrderByDescending(v => v);

            EntityFrameworkVersion.GetAllVersions().SequenceEqual(declaredVersions).Should().BeTrue();
        }

        [TestMethod]
        public void IsValidVersion_returns_true_for_valid_versions()
        {
            EntityFrameworkVersion.IsValidVersion(new Version(1, 0, 0, 0)).Should().BeTrue();
            EntityFrameworkVersion.IsValidVersion(new Version(2, 0, 0, 0)).Should().BeTrue();
            EntityFrameworkVersion.IsValidVersion(new Version(3, 0, 0, 0)).Should().BeTrue();
        }

        [TestMethod]
        public void IsValidVersion_returns_false_for_invalid_versions()
        {
            EntityFrameworkVersion.IsValidVersion(null).Should().BeFalse();
            EntityFrameworkVersion.IsValidVersion(new Version(0, 0, 0, 0)).Should().BeFalse();
            EntityFrameworkVersion.IsValidVersion(new Version(4, 0, 0, 0)).Should().BeFalse();
            EntityFrameworkVersion.IsValidVersion(new Version(3, 0)).Should().BeFalse();
            EntityFrameworkVersion.IsValidVersion(new Version(2, 0, 0)).Should().BeFalse();
        }

        [TestMethod]
        public void DoubleToVersion_returns_valid_version_for_known_double_versions()
        {
            EntityFrameworkVersion.DoubleToVersion(3.0).Should().Be(EntityFrameworkVersion.Version3);
            EntityFrameworkVersion.DoubleToVersion(2.0).Should().Be(EntityFrameworkVersion.Version2);
            EntityFrameworkVersion.DoubleToVersion(1.0).Should().Be(EntityFrameworkVersion.Version1);
        }

        [TestMethod]
        public void VersionToDouble_returns_valid_version_for_known_double_versions()
        {
            EntityFrameworkVersion.VersionToDouble(EntityFrameworkVersion.Version3).Should().Be(3.0);
            EntityFrameworkVersion.VersionToDouble(EntityFrameworkVersion.Version2).Should().Be(2.0);
            EntityFrameworkVersion.VersionToDouble(EntityFrameworkVersion.Version1).Should().Be(1.0);
        }

        [TestMethod]
        public void Latest_EF_version_is_really_latest()
        {
            EntityFrameworkVersion.Latest.Should().Be(
                EntityFrameworkVersion.GetAllVersions().OrderByDescending(v => v).First());
        }
    }
}
