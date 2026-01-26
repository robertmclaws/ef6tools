// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade
{
    using System;
    using System.Data.Common;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Moq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class LegacyProviderServicesUtilsTests
    {
        [TestMethod]
        public void CanGetDbProviderServices_returns_true_if_DbProviderServices_returned_from_service_provider()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(p => p.GetService(typeof(DbProviderServices)))
                .Returns(new Mock<DbProviderServices>());

            LegacyDbProviderServicesUtils.CanGetDbProviderServices(mockServiceProvider.Object).Should().BeTrue();
        }

        [TestMethod]
        public void CanGetDbProviderServices_returns_false_if_DbProviderServices_not_returned_from_service_provider()
        {
            LegacyDbProviderServicesUtils.CanGetDbProviderServices(new Mock<IServiceProvider>().Object).Should().BeFalse();
        }

        [TestMethod]
        public void CanGetDbProviderServices_returns_false_if_service_provider_throws()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider
                .Setup(p => p.GetService(typeof(DbProviderServices)))
                .Throws<InvalidOperationException>();

            LegacyDbProviderServicesUtils.CanGetDbProviderServices(mockServiceProvider.Object).Should().BeFalse();
        }
    }
}
