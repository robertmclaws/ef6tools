// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.CodeGeneration
{
    using FluentAssertions;
    using Microsoft.Data.Entity.Design.CodeGeneration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultCSharpContextGeneratorTests : GeneratorTestBase
    {
        [TestMethod]
        public void Generate_returns_code()
        {
            var generator = new DefaultCSharpContextGenerator();
            var result = generator.Generate(Model, "WebApplication1.Models", "MyContext", "MyContextConnString");

            result.Should().Be(
                @"namespace WebApplication1.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MyContext : DbContext
    {
        public MyContext()
            : base(""name=MyContextConnString"")
        {
        }

        public virtual DbSet<Entity> Entities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
");
        }
    }
}
