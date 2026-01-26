// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade
{
    using System;
    using System.Linq;
    using System.Xml;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FluentAssertions;

    [TestClass]
    public class SchemaManagerTests
    {
        private const string CsdlNsV1 = "http://schemas.microsoft.com/ado/2006/04/edm";
        private const string CsdlNsV2 = "http://schemas.microsoft.com/ado/2008/09/edm";
        private const string CsdlNsV3 = "http://schemas.microsoft.com/ado/2009/11/edm";

        private const string SsdlNsV1 = "http://schemas.microsoft.com/ado/2006/04/edm/ssdl";
        private const string SsdlNsV2 = "http://schemas.microsoft.com/ado/2009/02/edm/ssdl";
        private const string SsdlNsV3 = "http://schemas.microsoft.com/ado/2009/11/edm/ssdl";

        private const string MslNsV1 = "urn:schemas-microsoft-com:windows:storage:mapping:CS";
        private const string MslNsV2 = "http://schemas.microsoft.com/ado/2008/09/mapping/cs";
        private const string MslNsV3 = "http://schemas.microsoft.com/ado/2009/11/mapping/cs";

        private const string EdmxNsV1 = "http://schemas.microsoft.com/ado/2007/06/edmx";
        private const string EdmxNsV2 = "http://schemas.microsoft.com/ado/2008/10/edmx";
        private const string EdmxNsV3 = "http://schemas.microsoft.com/ado/2009/11/edmx";

        private const string EntityStoreSchemaGeneratorNs = "http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator";

        private const string CodeGenerationNs = "http://schemas.microsoft.com/ado/2006/04/codegeneration";

        private const string ProviderManifestNs = "http://schemas.microsoft.com/ado/2006/04/edm/providermanifest";

        private const string AnnotationNs = "http://schemas.microsoft.com/ado/2009/02/edm/annotation";

        [TestMethod]
        public void SchemaManager_GetCSDLNamespaceName_returns_correct_Csdl_namespaces_for_requested_version()
        {
            SchemaManager.GetCSDLNamespaceName(new Version(1, 0, 0, 0)).Should().Be(CsdlNsV1);
            SchemaManager.GetCSDLNamespaceName(new Version(2, 0, 0, 0)).Should().Be(CsdlNsV2);
            SchemaManager.GetCSDLNamespaceName(new Version(3, 0, 0, 0)).Should().Be(CsdlNsV3);
        }

        [TestMethod]
        public void SchemaManager_GetCSDLNamespaceName_returns_all_known_Csdl_namespaces()
        {
            var csdlNamespaces = SchemaManager.GetCSDLNamespaceNames();

            new[] { CsdlNsV1, CsdlNsV2, CsdlNsV3 }.SequenceEqual(csdlNamespaces).Should().BeTrue();
        }

        [TestMethod]
        public void SchemaManager_GetSSDLNamespaceName_returns_correct_Ssdl_namespaces_for_requested_version()
        {
            SchemaManager.GetSSDLNamespaceName(new Version(1, 0, 0, 0)).Should().Be(SsdlNsV1);
            SchemaManager.GetSSDLNamespaceName(new Version(2, 0, 0, 0)).Should().Be(SsdlNsV2);
            SchemaManager.GetSSDLNamespaceName(new Version(3, 0, 0, 0)).Should().Be(SsdlNsV3);
        }

        [TestMethod]
        public void SchemaManager_GetSSDLNamespaceName_returns_all_known_Ssdl_namespaces()
        {
            var csdlNamespaces = SchemaManager.GetSSDLNamespaceNames();

            new[] { SsdlNsV1, SsdlNsV2, SsdlNsV3 }.SequenceEqual(csdlNamespaces).Should().BeTrue();
        }

        [TestMethod]
        public void SchemaManager_GetMSLNamespaceName_returns_correct_Msl_namespaces_for_requested_version()
        {
            SchemaManager.GetMSLNamespaceName(new Version(1, 0, 0, 0)).Should().Be(MslNsV1);
            SchemaManager.GetMSLNamespaceName(new Version(2, 0, 0, 0)).Should().Be(MslNsV2);
            SchemaManager.GetMSLNamespaceName(new Version(3, 0, 0, 0)).Should().Be(MslNsV3);
        }

        [TestMethod]
        public void SchemaManager_GetMSLNamespaceName_returns_all_known_Msl_namespaces()
        {
            var csdlNamespaces = SchemaManager.GetMSLNamespaceNames();

            new[] { MslNsV1, MslNsV2, MslNsV3 }.SequenceEqual(csdlNamespaces).Should().BeTrue();
        }

        [TestMethod]
        public void SchemaManager_GetEDMXNamespaceName_returns_correct_Edmx_namespaces_for_requested_version()
        {
            SchemaManager.GetEDMXNamespaceName(new Version(1, 0, 0, 0)).Should().Be(EdmxNsV1);
            SchemaManager.GetEDMXNamespaceName(new Version(2, 0, 0, 0)).Should().Be(EdmxNsV2);
            SchemaManager.GetEDMXNamespaceName(new Version(3, 0, 0, 0)).Should().Be(EdmxNsV3);
        }

        [TestMethod]
        public void SchemaManager_GetEDMXNamespaceName_returns_all_known_Edmx_namespaces()
        {
            var csdlNamespaces = SchemaManager.GetEDMXNamespaceNames();

            new[] { EdmxNsV1, EdmxNsV2, EdmxNsV3 }.SequenceEqual(csdlNamespaces).Should().BeTrue();
        }

        [TestMethod]
        public void
            SchemaManager_GetEntityStoreSchemaGeneratorNamespaceName_returns_correct_EntityStoreSchemaGenerator_namespace()
        {
            SchemaManager.GetEntityStoreSchemaGeneratorNamespaceName().Should().Be(EntityStoreSchemaGeneratorNs);
        }

        [TestMethod]
        public void SchemaManager_GetCodeGenerationNamespaceName_returns_correct_CodeGeneration_namespace()
        {
            SchemaManager.GetCodeGenerationNamespaceName().Should().Be(CodeGenerationNs);
        }

        [TestMethod]
        public void SchemaManager_GetProviderManifestNamespaceName_returns_correct_ProviderManifest_namespace()
        {
            SchemaManager.GetProviderManifestNamespaceName().Should().Be(ProviderManifestNs);
        }

        [TestMethod]
        public void SchemaManager_GetAnnotationNamespaceName_returns_correct_Annotation_namespace()
        {
            SchemaManager.GetAnnotationNamespaceName().Should().Be(AnnotationNs);
        }

        [TestMethod]
        public void SchemaManager_GetAllNamespacesForVersion_returns_correct_namespaces_for_requested_version()
        {
            var namespacesV1 = new[] { EdmxNsV1, CsdlNsV1, SsdlNsV1, ProviderManifestNs, CodeGenerationNs, MslNsV1, AnnotationNs };
            var namespacesV2 = new[] { EdmxNsV2, CsdlNsV2, SsdlNsV2, ProviderManifestNs, CodeGenerationNs, MslNsV2, AnnotationNs };
            var namespacesV3 = new[] { EdmxNsV3, CsdlNsV3, SsdlNsV3, ProviderManifestNs, CodeGenerationNs, MslNsV3, AnnotationNs };

            namespacesV1.SequenceEqual(SchemaManager.GetAllNamespacesForVersion(new Version(1, 0, 0, 0))).Should().BeTrue();
            namespacesV2.SequenceEqual(SchemaManager.GetAllNamespacesForVersion(new Version(2, 0, 0, 0))).Should().BeTrue();
            namespacesV3.SequenceEqual(SchemaManager.GetAllNamespacesForVersion(new Version(3, 0, 0, 0))).Should().BeTrue();
        }

        [TestMethod]
        public void SchemaManager_GetSchemaVersion_returns_correct_version_for_namespace()
        {
            var v1 = new Version(1, 0, 0, 0);
            var v2 = new Version(2, 0, 0, 0);
            var v3 = new Version(3, 0, 0, 0);


            SchemaManager.GetSchemaVersion(CsdlNsV1).Should().Be(v1);
            SchemaManager.GetSchemaVersion(CsdlNsV2).Should().Be(v2);
            SchemaManager.GetSchemaVersion(CsdlNsV3).Should().Be(v3);

            SchemaManager.GetSchemaVersion(SsdlNsV1).Should().Be(v1);
            SchemaManager.GetSchemaVersion(SsdlNsV2).Should().Be(v2);
            SchemaManager.GetSchemaVersion(SsdlNsV3).Should().Be(v3);

            SchemaManager.GetSchemaVersion(MslNsV1).Should().Be(v1);
            SchemaManager.GetSchemaVersion(MslNsV2).Should().Be(v2);
            SchemaManager.GetSchemaVersion(MslNsV3).Should().Be(v3);

            SchemaManager.GetSchemaVersion(EdmxNsV1).Should().Be(v1);
            SchemaManager.GetSchemaVersion(EdmxNsV2).Should().Be(v2);
            SchemaManager.GetSchemaVersion(EdmxNsV3).Should().Be(v3);

            SchemaManager.GetSchemaVersion(null).Should().Be(v1);
            SchemaManager.GetSchemaVersion("abc").Should().Be(v1);
        }

        [TestMethod]
        public void SchemaManager_GetSchemaVersion_returns_null_for_unknown_namespace()
        {
            SchemaManager.GetSchemaVersion("http://tempuri.org").Should().Be(new Version(1, 0, 0, 0));
        }

        [TestMethod]
        public void SchemaManager_GetEdmxNamespaceManager_namespace_manager_with_correct_bindings()
        {
            for (var majorVersion = 1; majorVersion <= 3; majorVersion++)
            {
                var version = new Version(majorVersion, 0, 0, 0);

                var nsMgr = SchemaManager.GetEdmxNamespaceManager(new NameTable(), version);

                nsMgr.LookupNamespace("edmx").Should().Be(SchemaManager.GetEDMXNamespaceName(version));
                nsMgr.LookupNamespace("csdl").Should().Be(SchemaManager.GetCSDLNamespaceName(version));
                nsMgr.LookupNamespace("essg").Should().Be(SchemaManager.GetEntityStoreSchemaGeneratorNamespaceName());
                nsMgr.LookupNamespace("ssdl").Should().Be(SchemaManager.GetSSDLNamespaceName(version));
                nsMgr.LookupNamespace("msl").Should().Be(SchemaManager.GetMSLNamespaceName(version));
            }
        }
    }
}
