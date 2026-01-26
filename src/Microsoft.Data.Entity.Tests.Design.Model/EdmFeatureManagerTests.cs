// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.Model
{
    using System;
    using Microsoft.Data.Entity.Design.Base.Context;
    using Microsoft.Data.Entity.Design.Model;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.Data.Tools.XmlDesignerBase.Model;
    using Moq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class EdmFeatureManagerTests
    {
        [TestMethod]
        public void FunctionImportReturningComplexType_disabled_only_for_v1_schema_version()
        {
            EdmFeatureManager.GetFunctionImportReturningComplexTypeFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetFunctionImportReturningComplexTypeFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleAndEnabled);

            EdmFeatureManager.GetFunctionImportReturningComplexTypeFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void EnumTypes_enabled_only_for_v3_schema_version()
        {
            EdmFeatureManager.GetEnumTypeFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetEnumTypeFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetEnumTypeFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void EnumTypes_enabled_only_for_v3_artifacts()
        {
            IsFeatureEnabledForArtifact(EntityFrameworkVersion.Version1, EdmFeatureManager.GetEnumTypeFeatureState)
                .Should().Be(FeatureState.VisibleButDisabled);

            IsFeatureEnabledForArtifact(EntityFrameworkVersion.Version2, EdmFeatureManager.GetEnumTypeFeatureState)
                .Should().Be(FeatureState.VisibleButDisabled);

            IsFeatureEnabledForArtifact(EntityFrameworkVersion.Version3, EdmFeatureManager.GetEnumTypeFeatureState)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void FunctionImportColumnInformation_always_enabled()
        {
            foreach (var targetSchemaVersion in EntityFrameworkVersion.GetAllVersions())
            {
                IsFeatureEnabledForArtifact(
                    targetSchemaVersion,
                    EdmFeatureManager.GetFunctionImportColumnInformationFeatureState)
                    .Should().Be(FeatureState.VisibleAndEnabled);
            }
        }

        private static FeatureState IsFeatureEnabledForArtifact(Version schemaVersion, Func<EFArtifact, FeatureState> funcToTest)
        {
            using (var editingContext = new EditingContext())
            {
                var modelManager = new Mock<ModelManager>(null, null).Object;
                var modelProvider = new Mock<XmlModelProvider>().Object;

                var entityDesignArtifactMock =
                    new Mock<EntityDesignArtifact>(modelManager, new Uri("urn:dummy"), modelProvider);

                editingContext.SetEFArtifactService(new EFArtifactService(entityDesignArtifactMock.Object));

                entityDesignArtifactMock.Setup(a => a.SchemaVersion).Returns(schemaVersion);
                entityDesignArtifactMock.Setup(a => a.EditingContext).Returns(editingContext);

                return funcToTest(entityDesignArtifactMock.Object);
            }
        }

        [TestMethod]
        public void FunctionImportMapping_disabled_only_for_v1_schema_version()
        {
            EdmFeatureManager.GetFunctionImportMappingFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetFunctionImportMappingFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleAndEnabled);

            EdmFeatureManager.GetFunctionImportMappingFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void ForeignKeys_disabled_only_for_v1_schema_version()
        {
            EdmFeatureManager.GetForeignKeysInModelFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetForeignKeysInModelFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleAndEnabled);

            EdmFeatureManager.GetForeignKeysInModelFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void UpdateViews_disabled_only_for_v1_schema_version()
        {
            EdmFeatureManager.GetGenerateUpdateViewsFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetGenerateUpdateViewsFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleAndEnabled);

            EdmFeatureManager.GetGenerateUpdateViewsFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void EntityContainerTypeAccess_disabled_only_for_v1_schema_version()
        {
            EdmFeatureManager.GetEntityContainerTypeAccessFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetEntityContainerTypeAccessFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleAndEnabled);

            EdmFeatureManager.GetEntityContainerTypeAccessFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void LazyLoading_disabled_only_for_v1_schema_version()
        {
            EdmFeatureManager.GetLazyLoadingFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetLazyLoadingFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleAndEnabled);

            EdmFeatureManager.GetLazyLoadingFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void ComposableFunctionImport_enabled_only_for_v3_schema_version()
        {
            EdmFeatureManager.GetComposableFunctionImportFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetComposableFunctionImportFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetComposableFunctionImportFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }

        [TestMethod]
        public void SpatialTypes_enabled_only_for_v3_schema_version()
        {
            EdmFeatureManager.GetUseStrongSpatialTypesFeatureState(EntityFrameworkVersion.Version1)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetUseStrongSpatialTypesFeatureState(EntityFrameworkVersion.Version2)
                .Should().Be(FeatureState.VisibleButDisabled);

            EdmFeatureManager.GetUseStrongSpatialTypesFeatureState(EntityFrameworkVersion.Version3)
                .Should().Be(FeatureState.VisibleAndEnabled);
        }
    }
}
