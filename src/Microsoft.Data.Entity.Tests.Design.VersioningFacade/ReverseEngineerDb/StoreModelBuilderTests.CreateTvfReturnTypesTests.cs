// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade.ReverseEngineerDb
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Data.Entity.Design.VersioningFacade.ReverseEngineerDb.SchemaDiscovery;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    public partial class StoreModelBuilderTests
    {
        [TestMethod]
        public void CreateTvfReturnTypes_creates_row_types_for_valid_input_rows()
        {
            var columns =
                new List<TableDetailsRow>
                    {
                        CreateRow(table: "rowtype", columnName: "Id", dataType: "int"),
                        CreateRow(table: "rowtype", columnName: "Name", dataType: "nvarchar(max)")
                    };

            var rowTypes = CreateStoreModelBuilder().CreateTvfReturnTypes(columns);

            rowTypes.Should().NotBeNull();
            rowTypes.Count.Should().Be(1);
            rowTypes.Single().Value.Properties.Select(p => p.Name).Should().BeEquivalentTo(new[] { "Id", "Name" });
            rowTypes.Single().Value.MetadataProperties.Any(p => p.Name == "EdmSchemaErrors").Should().BeFalse();
        }

        [TestMethod]
        public void CreateTvfReturnTypes_creates_multiple_row_types_for_multiple_valid_definitons()
        {
            var columns =
                new List<TableDetailsRow>
                    {
                        CreateRow(table: "rowtype", columnName: "Id", dataType: "int"),
                        CreateRow(table: "rowtype", columnName: "Name", dataType: "nvarchar(max)"),
                        CreateRow(table: "rowtype1", columnName: "Name", dataType: "nvarchar(max)")
                    };

            var rowTypes = CreateStoreModelBuilder().CreateTvfReturnTypes(columns);

            rowTypes.Should().NotBeNull();
            rowTypes.Count.Should().Be(2);
        }

        [TestMethod]
        public void CreateTvfReturnTypes_creates_row_types_with_errors_for_invalidtype()
        {
            var columns =
                new List<TableDetailsRow>
                    {
                        CreateRow(table: "rowtype", columnName: "Id", dataType: "foo"),
                        CreateRow(table: "rowtype", columnName: "Name", dataType: "nvarchar(max)"),
                    };

            var rowTypes = CreateStoreModelBuilder().CreateTvfReturnTypes(columns);

            rowTypes.Should().NotBeNull();
            rowTypes.Count.Should().Be(1);
            rowTypes.Single().Value.MetadataProperties.Any(p => p.Name == "EdmSchemaErrors").Should().BeTrue();
        }
    }
}
