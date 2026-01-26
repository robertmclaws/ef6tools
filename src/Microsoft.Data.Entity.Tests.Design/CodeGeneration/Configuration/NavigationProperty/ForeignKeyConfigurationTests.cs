// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.CodeGeneration
{
    using System.Data.Entity.Core.Metadata.Edm;
    using Microsoft.Data.Entity.Design.CodeGeneration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class ForeignKeyConfigurationTests
    {
        [TestMethod, Ignore("Different API Visiblity between official dll and locally built")]
        public void GetMethodChain_returns_chain_when_one_property()
        {
            //var configuration = new ForeignKeyConfiguration { Properties = { new EdmProperty("EntityId") } };
            //var code = new CSharpCodeHelper();

            //configuration.GetMethodChain(code).Should().Be(".HasForeignKey(e => e.EntityId)");
        }

        [TestMethod, Ignore("Different API Visiblity between official dll and locally built")]
        public void GetMethodChain_returns_chain_when_more_than_one_property()
        {
            //var configuration = new ForeignKeyConfiguration
            //    {
            //        Properties = { new EdmProperty("EntityId1"), new EdmProperty("EntityId2") }
            //    };
            //var code = new CSharpCodeHelper();

            //configuration.GetMethodChain(code).Should().Be(".HasForeignKey(e => new { e.EntityId1, e.EntityId2 })");
        }
    }
}
