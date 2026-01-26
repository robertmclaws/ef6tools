// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade.ReverseEngineerDb
{
    using System.Collections.Generic;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.Data.Entity.Design.VersioningFacade.ReverseEngineerDb;
    using Microsoft.Data.Entity.Design.VersioningFacade.ReverseEngineerDb.SchemaDiscovery;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    public partial class StoreModelBuilderTests
    {
        [TestMethod]
        public void GetFunctionParameterType_returns_PrimitiveType_for_valid_parameter_type()
        {
            var errors = new List<EdmSchemaError>();

            var type = CreateStoreModelBuilder()
                .GetFunctionParameterType(
                    CreateFunctionDetailsRow(paramTypeName: "smallint"), 1, errors);

            type.Should().NotBeNull();
            type.PrimitiveTypeKind.Should().Be(PrimitiveTypeKind.Int16);
        }

        [TestMethod]
        public void GetFunctionParameterType_returns_error_for_null_parameter_type_name()
        {
            var errors = new List<EdmSchemaError>();

            var type = CreateStoreModelBuilder()
                .GetFunctionParameterType(
                    CreateFunctionDetailsRow(functionName: "function", paramName: "param", paramTypeName: null),
                    1, errors);

            type.Should().BeNull();
            errors.Count.Should().Be(1);
            var error = errors.Single();

            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.UnsupportedType);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedFunctionParameterDataType,
                    "function", "param", 1, "null"));
        }

        [TestMethod]
        public void GetFunctionParameterType_returns_error_for_invalid_parameter_type()
        {
            var errors = new List<EdmSchemaError>();

            var type = CreateStoreModelBuilder()
                .GetFunctionParameterType(
                    CreateFunctionDetailsRow(functionName: "function", paramName: "param", paramTypeName: "foo-type"),
                    1, errors);

            type.Should().BeNull();
            errors.Count.Should().Be(1);
            var error = errors.Single();

            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.UnsupportedType);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedFunctionParameterDataType,
                    "function", "param", 1, "foo-type"));
        }

        [TestMethod]
        public void GetFunctionParameterType_returns_error_for_unsupported_type_for_schema_version()
        {
            var errors = new List<EdmSchemaError>();

            var type = CreateStoreModelBuilder(targetEntityFrameworkVersion: EntityFrameworkVersion.Version2)
                .GetFunctionParameterType(
                    CreateFunctionDetailsRow(functionName: "function", paramName: "param", paramTypeName: "geography"),
                    1, errors);

            type.Should().BeNull();
            errors.Count.Should().Be(1);
            var error = errors.Single();

            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.UnsupportedType);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedFunctionParameterDataTypeForTarget,
                    "function", "param", 1, "geography"));
        }

        [TestMethod]
        public void CreateFunctionParameter_returns_null_for_invalid_type_name()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder()
                .CreateFunctionParameter(
                    CreateFunctionDetailsRow(functionName: "function", paramName: "param", paramTypeName: null),
                    new UniqueIdentifierService(),
                    1, errors);

            parameter.Should().BeNull();
            errors.Count.Should().Be(1);
            var error = errors.Single();

            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.UnsupportedType);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedFunctionParameterDataType,
                    "function", "param", 1, "null"));
        }

        [TestMethod]
        public void CreateFunctionParameter_returns_null_for_invalid_parameter_direction()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder()
                .CreateFunctionParameter(
                    CreateFunctionDetailsRow(
                        functionName: "function", paramName: "param", paramTypeName: "smallint", parameterDirection: "foo"),
                    new UniqueIdentifierService(),
                    1, errors);

            parameter.Should().BeNull();
            errors.Count.Should().Be(1);
            var error = errors.Single();

            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.ParameterDirectionNotValid);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.ParameterDirectionNotValid,
                    "function", "param", "foo"));
        }

        [TestMethod]
        public void CreateFunctionParameter_returns_parameter_for_valid_function_details_row()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder()
                .CreateFunctionParameter(
                    CreateFunctionDetailsRow(
                        functionName: "function", paramName: "param", paramTypeName: "smallint", parameterDirection: "INOUT"),
                    new UniqueIdentifierService(),
                    1, errors);

            parameter.Should().NotBeNull();
            errors.Should().BeEmpty();
            parameter.Name.Should().Be("param");
            parameter.TypeUsage.EdmType.Name.Should().Be("smallint");
            parameter.Mode.Should().Be(ParameterMode.InOut);
        }

        [TestMethod]
        public void CreateFunctionParameter_applies_ECMA_name_conversion_for_parameter_name()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder()
                .CreateFunctionParameter(
                    CreateFunctionDetailsRow(
                        functionName: "function", paramName: "p@r@m", paramTypeName: "smallint", parameterDirection: "INOUT"),
                    new UniqueIdentifierService(),
                    1, errors);

            parameter.Should().NotBeNull();
            errors.Should().BeEmpty();
            parameter.Name.Should().Be("p_r_m");
            parameter.TypeUsage.EdmType.Name.Should().Be("smallint");
            parameter.Mode.Should().Be(ParameterMode.InOut);
        }

        [TestMethod]
        public void CreateFunctionParameter_applies_ECMA_name_conversion_and_uniquifies_parameter_name()
        {
            var errors = new List<EdmSchemaError>();
            var uniquifiedIdentifierService = new UniqueIdentifierService();
            uniquifiedIdentifierService.AdjustIdentifier("p_r_m");

            var parameter = CreateStoreModelBuilder()
                .CreateFunctionParameter(
                    CreateFunctionDetailsRow(
                        functionName: "function", paramName: "p@r@m", paramTypeName: "smallint", parameterDirection: "INOUT"),
                    uniquifiedIdentifierService,
                    1, errors);

            parameter.Should().NotBeNull();
            errors.Should().BeEmpty();
            parameter.Name.Should().Be("p_r_m1");
            parameter.TypeUsage.EdmType.Name.Should().Be("smallint");
            parameter.Mode.Should().Be(ParameterMode.InOut);
        }

        [TestMethod]
        public void CreateFunctionParameters_creates_parameters_for_all_valid_rows()
        {
            var functionDetailsRows =
                new[]
                    {
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param",
                            paramTypeName: "smallint", parameterDirection: "IN"),
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param2",
                            paramTypeName: "int", parameterDirection: "INOUT"),
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param3",
                            paramTypeName: "geometry", parameterDirection: "OUT")
                    };

            var errors = new List<EdmSchemaError>();
            var parameters = CreateStoreModelBuilder()
                .CreateFunctionParameters(functionDetailsRows, errors).ToArray();

            parameters.Length.Should().Be(3);
            parameters.Select(p => p.Name).Should().BeEquivalentTo(new[] { "param", "param2", "param3" });
            parameters.Select(p => p.TypeName).Should().BeEquivalentTo(new[] { "smallint", "int", "geometry" });
            parameters.Select(p => p.Mode).Should().BeEquivalentTo(
                new[] { ParameterMode.In, ParameterMode.InOut, ParameterMode.Out, });

            errors.Should().BeEmpty();
        }

        [TestMethod]
        public void CreateFunctionParameters_uniquifies_parameter_names()
        {
            var functionDetailsRows =
                new[]
                    {
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param",
                            paramTypeName: "smallint", parameterDirection: "IN"),
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param",
                            paramTypeName: "geometry", parameterDirection: "OUT")
                    };

            var errors = new List<EdmSchemaError>();
            var parameters = CreateStoreModelBuilder()
                .CreateFunctionParameters(functionDetailsRows, errors).ToArray();

            parameters.Length.Should().Be(2);
            parameters.Select(p => p.Name).Should().BeEquivalentTo(new[] { "param", "param1" });
            errors.Should().BeEmpty();
        }

        [TestMethod]
        public void CreateFunctionParameters_does_not_return_parameters_for_invalid_rows()
        {
            var functionDetailsRows =
                new[]
                    {
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param",
                            paramTypeName: "smallint", parameterDirection: "IN"),
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param2",
                            paramTypeName: "foo", parameterDirection: "INOUT"),
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param3",
                            paramTypeName: "geometry", parameterDirection: "OUT")
                    };

            var errors = new List<EdmSchemaError>();
            var parameters = CreateStoreModelBuilder()
                .CreateFunctionParameters(functionDetailsRows, errors).ToArray();

            parameters.Length.Should().Be(2);
            parameters.Any(p => p == null).Should().BeFalse();

            errors.Count.Should().Be(1);
            var error = errors.Single();
            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.UnsupportedType);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedFunctionParameterDataType,
                    "function", "param2", 1, "foo"));
        }

        [TestMethod]
        public void CreateReturnParameter_creates_return_parameter_for_scalar_function()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder()
                .CreateReturnParameter(
                    CreateFunctionDetailsRow(functionName: "function", returnTypeName: "int"),
                    new Dictionary<string, RowType>(),
                    errors);

            parameter.Should().NotBeNull();
            parameter.Name.Should().Be("ReturnType");
            parameter.TypeName.Should().Be("int");
            errors.Should().BeEmpty();
        }

        [TestMethod]
        public void CreateReturnParameter_returns_error_if_return_type_not_valid()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder()
                .CreateReturnParameter(
                    CreateFunctionDetailsRow(functionName: "function", returnTypeName: "foo"),
                    new Dictionary<string, RowType>(),
                    errors);

            parameter.Should().BeNull();
            errors.Count.Should().Be(1);

            var error = errors.Single();
            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.UnsupportedType);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedFunctionReturnDataType,
                    "function", "foo"));
        }

        [TestMethod]
        public void CreateReturnParameter_returns_error_if_return_type_not_valid_for_schema_version()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder(targetEntityFrameworkVersion: EntityFrameworkVersion.Version1)
                .CreateReturnParameter(
                    CreateFunctionDetailsRow(functionName: "function", returnTypeName: "geometry"),
                    new Dictionary<string, RowType>(),
                    errors);

            parameter.Should().BeNull();
            errors.Count.Should().Be(1);

            var error = errors.Single();
            error.Severity.Should().Be(EdmSchemaErrorSeverity.Warning);
            error.ErrorCode.Should().Be((int)ModelBuilderErrorCode.UnsupportedType);
            error.Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedFunctionReturnDataTypeForTarget,
                    "function", "geometry"));
        }

        [TestMethod]
        public void CreateReturnParameter_returns_null_for_stored_proc()
        {
            var errors = new List<EdmSchemaError>();

            var parameter = CreateStoreModelBuilder()
                .CreateReturnParameter(
                    CreateFunctionDetailsRow(functionName: "function", returnTypeName: null, isTvf: false),
                    new Dictionary<string, RowType>(),
                    errors);

            parameter.Should().BeNull();
            errors.Count.Should().Be(0);
        }

        [TestMethod]
        public void CreateFunction_returns_TVF_with_errors_if_return_rowtype_for_TVF_is_invalid_and_copies_errors_from_invalid_RowType()
        {
            var tvfReturnTypeDetailsRow =
                new List<TableDetailsRow>
                    {
                        CreateRow(
                            catalog: "myDb", schema: "dbo", table: "function", columnName: "Id",
                            dataType: "foo")
                    };

            var functionDetailsRow =
                CreateFunctionDetailsRow(catalog: "myDb", schema: "dbo", functionName: "function", isTvf: true);

            var errors = new List<EdmSchemaError>();
            var storeModelBuilder = CreateStoreModelBuilder();
            var tvfReturnTypes = storeModelBuilder.CreateTvfReturnTypes(tvfReturnTypeDetailsRow);
            var parameter = storeModelBuilder.CreateReturnParameter(functionDetailsRow, tvfReturnTypes, errors);

            parameter.Should().BeNull();

            errors.Count.Should().Be(2);
            errors[0].Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.TableReferencedByTvfWasNotFound,
                    "myDb.dbo.function"));

            errors[1].Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.UnsupportedDataType,
                    "foo", "myDb.dbo.function", "Id"));
        }

        [TestMethod]
        public void CreateReturnParameter_returns_null_if_return_type_for_TVF_not_found()
        {
            var functionDetailsRows =
                CreateFunctionDetailsRow(
                    catalog: "myDb", schema: "dbo",
                    functionName: "function", isTvf: true, paramName: "param",
                    paramTypeName: "smallint", parameterDirection: "IN");

            var errors = new List<EdmSchemaError>();
            var parameter =
                CreateStoreModelBuilder()
                    .CreateReturnParameter(functionDetailsRows, new Dictionary<string, RowType>(), errors);

            parameter.Should().BeNull();

            errors.Count.Should().Be(1);
            errors.Single().Message.Should().Be(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources_VersioningFacade.TableReferencedByTvfWasNotFound,
                    "myDb.dbo.function"));

            errors.Single().ErrorCode.Should().Be((int)ModelBuilderErrorCode.MissingTvfReturnTable);
        }

        [TestMethod]
        public void CreateFunction_does_not_create_function_for_TVF_if_TVF_not_supported_by_schema_version()
        {
            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(isTvf: true)
                    };

            CreateStoreModelBuilder(targetEntityFrameworkVersion: EntityFrameworkVersion.Version2)
                .CreateFunction(functionDetailsRows, new Dictionary<string, RowType>()).Should().BeNull();
        }

        [TestMethod]
        public void CreateFunction_creates_scalar_function_for_valid_function_details_rows()
        {
            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(
                            catalog: "myDb", schema: "dbo",
                            functionName: "function", returnTypeName: "int",
                            isAggregate: true, isComposable: true, isBuiltIn: true,
                            isNiladic: true, isTvf: false, paramName: "param",
                            paramTypeName: "smallint", parameterDirection: "IN"),
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param2",
                            paramTypeName: "int", parameterDirection: "INOUT"),
                        CreateFunctionDetailsRow(
                            functionName: "function", paramName: "param3",
                            paramTypeName: "geometry", parameterDirection: "OUT")
                    };

            var function = CreateStoreModelBuilder()
                .CreateFunction(functionDetailsRows, new Dictionary<string, RowType>());

            function.Should().NotBeNull();
            function.FullName.Should().Be("myModel.function");
            function.StoreFunctionNameAttribute.Should().BeNull();
            function.Parameters.Count.Should().Be(3);
            function.ReturnParameter.TypeName.Should().Be("int");
            function.AggregateAttribute.Should().BeTrue();
            function.IsComposableAttribute.Should().BeTrue();
            function.BuiltInAttribute.Should().BeTrue();
            function.NiladicFunctionAttribute.Should().BeTrue();
            function.MetadataProperties.Any(p => p.Name == "EdmSchemaErrors").Should().BeFalse();
        }

        [TestMethod]
        public void CreateFunction_creates_TVF_function_for_valid_function_details_rows()
        {
            var tvfReturnTypeDetailsRow =
                new List<TableDetailsRow>
                    {
                        CreateRow(catalog: "myDb", schema: "dbo", table: "function", columnName: "Id", dataType: "int"),
                        CreateRow(catalog: "myDb", schema: "dbo", table: "function", columnName: "Name", dataType: "nvarchar(max)")
                    };

            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(
                            catalog: "myDb", schema: "dbo",
                            functionName: "function", isTvf: true, paramName: "param",
                            paramTypeName: "smallint", parameterDirection: "IN"),
                    };

            var storeModelBuilder = CreateStoreModelBuilder();
            var tvfReturnTypes = storeModelBuilder.CreateTvfReturnTypes(tvfReturnTypeDetailsRow);
            var function = storeModelBuilder.CreateFunction(functionDetailsRows, tvfReturnTypes);

            function.Should().NotBeNull();
            function.FullName.Should().Be("myModel.function");
            function.ReturnParameter.TypeUsage.EdmType.Should().BeSameAs(tvfReturnTypes.Values.Single().GetCollectionType());
            function.MetadataProperties.Any(p => p.Name == "EdmSchemaErrors").Should().BeFalse();
        }

        [TestMethod]
        public void CreateFunction_creates_EdmSchemaErrors_metadata_property_for_invalid_functions()
        {
            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(functionName: "function", isTvf: true),
                    };

            var function =
                CreateStoreModelBuilder()
                    .CreateFunction(functionDetailsRows, new Dictionary<string, RowType>());

            function.MetadataProperties.Any(p => p.Name == "EdmSchemaErrors").Should().BeTrue();
        }

        [TestMethod]
        public void CreateFunction_creates_stored_procedure_for_valid_function_details_rows()
        {
            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(
                            catalog: "myDb", schema: "dbo",
                            functionName: "function", isTvf: false, paramName: "param",
                            paramTypeName: "smallint", parameterDirection: "IN"),
                    };

            var function = CreateStoreModelBuilder()
                .CreateFunction(functionDetailsRows, new Dictionary<string, RowType>());

            function.Should().NotBeNull();
            function.FullName.Should().Be("myModel.function");
            function.StoreFunctionNameAttribute.Should().BeNull();
            function.Parameters.Count.Should().Be(1);
            function.ReturnParameter.Should().BeNull();
            function.MetadataProperties.Any(p => p.Name == "EdmSchemaErrors").Should().BeFalse();
        }

        [TestMethod]
        public void Can_CreateFunction_without_parameters()
        {
            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(
                            catalog: "myDb", schema: "dbo",
                            functionName: "function", isTvf: false, paramName: null),
                    };

            var function = CreateStoreModelBuilder()
                .CreateFunction(functionDetailsRows, new Dictionary<string, RowType>());

            function.Should().NotBeNull();
            function.FullName.Should().Be("myModel.function");
            function.StoreFunctionNameAttribute.Should().BeNull();
            function.Parameters.Count.Should().Be(0);
            function.ReturnParameter.Should().BeNull();
            function.MetadataProperties.Any(p => p.Name == "EdmSchemaErrors").Should().BeFalse();
        }

        [TestMethod]
        public void CreateFunction_applies_ECMA_name_conversion_and_uniquifies_parameter_name()
        {
            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(functionName: "#@$&!", isTvf: false, paramName: null),
                    };

            var storeModelBuilder = CreateStoreModelBuilder();
            var function = storeModelBuilder.CreateFunction(functionDetailsRows, new Dictionary<string, RowType>());

            function.Should().NotBeNull();
            function.FullName.Should().Be("myModel.f_____");

            function = storeModelBuilder.CreateFunction(functionDetailsRows, new Dictionary<string, RowType>());

            function.Should().NotBeNull();
            function.FullName.Should().Be("myModel.f_____1");
        }

        [TestMethod]
        public void CreateFunction_sets_StoreFunctionName_if_the_original_name_was_changed()
        {
            var functionDetailsRows =
                new List<FunctionDetailsRowView>
                    {
                        CreateFunctionDetailsRow(functionName: "#@$&!", isTvf: false, paramName: null),
                    };

            var function = CreateStoreModelBuilder()
                .CreateFunction(functionDetailsRows, new Dictionary<string, RowType>());

            function.Should().NotBeNull();
            function.FullName.Should().Be("myModel.f_____");
            function.StoreFunctionNameAttribute.Should().Be("#@$&!");
        }
    }
}
