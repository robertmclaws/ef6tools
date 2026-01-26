// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.CodeGeneration
{
    using Microsoft.Data.Entity.Design.CodeGeneration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class KeyPropertyConfigurationTests
    {
        [TestMethod]
        public void GetAttributeBody_returns_body()
        {
            var configuration = new KeyPropertyConfiguration();
            var code = new CSharpCodeHelper();

            configuration.GetAttributeBody(code).Should().Be("Key");
        }
    }
}
