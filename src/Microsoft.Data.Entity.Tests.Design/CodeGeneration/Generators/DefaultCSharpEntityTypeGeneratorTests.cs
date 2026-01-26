// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.CodeGeneration
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Data.Entity.Design.CodeGeneration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DefaultCSharpEntityTypeGeneratorTests : GeneratorTestBase
    {
        [TestMethod]
        public void Generate_returns_code()
        {
            var generator = new DefaultCSharpEntityTypeGenerator();
            var result = generator.Generate(
                Model.ConceptualModel.Container.EntitySets.First(),
                Model,
                "WebApplication1.Models");

            result.Should().Be(
                @"namespace WebApplication1.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Entity
    {
        public int Id { get; set; }
    }
}
");
        }
    }
}
