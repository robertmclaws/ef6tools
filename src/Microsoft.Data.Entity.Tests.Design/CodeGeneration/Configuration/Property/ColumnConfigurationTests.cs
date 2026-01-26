// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.CodeGeneration
{
    using Microsoft.Data.Entity.Design.CodeGeneration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class ColumnConfigurationTests
    {
        [TestMethod]
        public void GetAttributeBody_returns_body_when_name()
        {
            var configuration = new ColumnConfiguration { Name = "Id" };
            var code = new CSharpCodeHelper();

            configuration.GetAttributeBody(code).Should().Be("Column(\"Id\")");
        }

        [TestMethod]
        public void GetAttributeBody_returns_body_when_order()
        {
            var configuration = new ColumnConfiguration { Order = 0 };
            var code = new CSharpCodeHelper();

            configuration.GetAttributeBody(code).Should().Be("Column(Order = 0)");
        }

        [TestMethod]
        public void GetAttributeBody_returns_body_when_type()
        {
            var configuration = new ColumnConfiguration { TypeName = "int" };
            var code = new CSharpCodeHelper();

            configuration.GetAttributeBody(code).Should().Be("Column(TypeName = \"int\")");
        }

        [TestMethod]
        public void GetAttributeBody_returns_body_when_order_and_type()
        {
            var configuration = new ColumnConfiguration { Order = 0, TypeName = "int" };
            var code = new CSharpCodeHelper();

            configuration.GetAttributeBody(code).Should().Be("Column(Order = 0, TypeName = \"int\")");
        }

        [TestMethod]
        public void GetAttributeBody_returns_body_when_order_and_type_and_vb()
        {
            var configuration = new ColumnConfiguration { Order = 0, TypeName = "int" };
            var code = new VBCodeHelper();

            configuration.GetAttributeBody(code).Should().Be("Column(Order:=0, TypeName:=\"int\")");
        }

        [TestMethod]
        public void GetAttributeBody_returns_body_when_all()
        {
            var configuration = new ColumnConfiguration { Name = "Id", Order = 0, TypeName = "int" };
            var code = new CSharpCodeHelper();

            configuration.GetAttributeBody(code).Should().Be("Column(\"Id\", Order = 0, TypeName = \"int\")");
        }

        [TestMethod]
        public void GetMethodChain_returns_body_when_name()
        {
            var configuration = new ColumnConfiguration { Name = "Id" };
            var code = new CSharpCodeHelper();

            configuration.GetMethodChain(code).Should().Be(".HasColumnName(\"Id\")");
        }

        [TestMethod]
        public void GetMethodChain_returns_body_when_order()
        {
            var configuration = new ColumnConfiguration { Order = 0 };
            var code = new CSharpCodeHelper();

            configuration.GetMethodChain(code).Should().Be(".HasColumnOrder(0)");
        }

        [TestMethod]
        public void GetMethodChain_returns_body_when_type()
        {
            var configuration = new ColumnConfiguration { TypeName = "int" };
            var code = new CSharpCodeHelper();

            configuration.GetMethodChain(code).Should().Be(".HasColumnType(\"int\")");
        }

        [TestMethod]
        public void GetMethodChain_returns_body_when_all()
        {
            var configuration = new ColumnConfiguration { Name = "Id", Order = 0, TypeName = "int" };
            var code = new CSharpCodeHelper();

            configuration.GetMethodChain(code).Should().Be(".HasColumnName(\"Id\").HasColumnOrder(0).HasColumnType(\"int\")");
        }
    }
}
