// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Design.Model.Entity
{
    using System;
    using System.Data.Entity.SqlServer;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.Data.Tools.XmlDesignerBase.Model;
    using Moq;
    using Xunit;

    public class StorageEntityModelTests
    {
        [Fact(Skip = "Updated binary has updated types")]
        public void StoreTypeNameToStoreTypeMap_returns_type_map()
        {
            var ssdl =
                XElement.Parse(
                    "<Schema Namespace=\"Model.Store\" Provider=\"System.Data.SqlClient\" ProviderManifestToken=\"2008\" Alias=\"Self\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm/ssdl\" />");

            using (var storageModel = new StorageEntityModel(null, ssdl))
            {
                var typeMap = storageModel.StoreTypeNameToStoreTypeMap;

                Assert.Equal(
                    SqlProviderServices.Instance.GetProviderManifest("2008").GetStoreTypes().Select(t => t.Name),
                    typeMap.Keys);

                Assert.False(typeMap.Any(t => t.Key != t.Value.Name));
            }
        }

        [Fact]
        public void XNamespace_returns_element_namespace_if_element_not_null()
        {
            var element = new XElement("{urn:tempuri}element");
            var modelManager = new Mock<ModelManager>(null, null).Object;
            var modelProvider = new Mock<XmlModelProvider>().Object;
            var entityDesignArtifactMock = new Mock<EntityDesignArtifact>(modelManager, new Uri("urn:dummy"), modelProvider);
            entityDesignArtifactMock.Setup(a => a.SchemaVersion).Returns(EntityFrameworkVersion.Version3);

            using (var storageModel = new StorageEntityModel(entityDesignArtifactMock.Object, element))
            {
                Assert.Same(element.Name.Namespace, storageModel.XNamespace);
            }
        }

        [Fact]
        public void XNamespace_returns_root_namespace_if_element_null()
        {
            var tmpElement = new XElement("{http://schemas.microsoft.com/ado/2009/11/edm/ssdl}Schema");

            var modelManager = new Mock<ModelManager>(null, null).Object;
            var modelProvider = new Mock<XmlModelProvider>().Object;
            var enityDesignArtifiact =
                new Mock<EntityDesignArtifact>(modelManager, new Uri("urn:dummy"), modelProvider)
                    {
                        CallBase = true
                    }.Object;

            enityDesignArtifiact.SetXObject(
                XDocument.Parse("<Edmx xmlns=\"http://schemas.microsoft.com/ado/2009/11/edmx\" />"));

            using (var storageModel = new StorageEntityModel(enityDesignArtifiact, tmpElement))
            {
                storageModel.SetXObject(null);
                Assert.Equal("http://schemas.microsoft.com/ado/2009/11/edm/ssdl", storageModel.XNamespace);

                // resetting the element is required for clean up
                storageModel.SetXObject(tmpElement);
            }
        }

        [Fact]
        public void GetStoragePrimitiveType_returns_type_name_for_valid_type()
        {
            var ssdl =
                XElement.Parse(
                    "<Schema Namespace=\"Model.Store\" Provider=\"System.Data.SqlClient\" ProviderManifestToken=\"2008\" Alias=\"Self\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm/ssdl\" />");

            using (var storageModel = new StorageEntityModel(null, ssdl))
            {
                Assert.Equal(
                    "tinyint",
                    storageModel.GetStoragePrimitiveType("tinyint").Name);
            }
        }

        [Fact]
        public void GetStoragePrimitiveType_returns_null_for_unknown_type()
        {
            var ssdl =
                XElement.Parse(
                    "<Schema Namespace=\"Model.Store\" Provider=\"System.Data.SqlClient\" ProviderManifestToken=\"2008\" Alias=\"Self\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm/ssdl\" />");

            using (var storageModel = new StorageEntityModel(null, ssdl))
            {
                Assert.Null(storageModel.GetStoragePrimitiveType("foo"));
            }
        }

        [Fact]
        public void StoreTypeNameToStoreTypeMap_works_with_MicrosoftDataSqlClient()
        {
            // This test verifies that the provider resolution works correctly for Microsoft.Data.SqlClient
            // The StoreTypeNameToStoreTypeMap property triggers DependencyResolver.GetService<DbProviderServices>(Provider.Value)
            // which should return SqlProviderServices (not LegacyDbProviderServicesWrapper)
            var ssdl =
                XElement.Parse(
                    "<Schema Namespace=\"Model.Store\" Provider=\"Microsoft.Data.SqlClient\" ProviderManifestToken=\"2008\" Alias=\"Self\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm/ssdl\" />");

            using (var storageModel = new StorageEntityModel(null, ssdl))
            {
                // This should NOT throw - if Microsoft.Data.SqlClient isn't properly registered,
                // it would try to use LegacyDbProviderServicesWrapper which would fail
                var typeMap = storageModel.StoreTypeNameToStoreTypeMap;

                Assert.NotNull(typeMap);
                Assert.True(typeMap.Count > 0);

                // Verify we get the expected SQL Server types
                Assert.True(typeMap.ContainsKey("int"));
                Assert.True(typeMap.ContainsKey("varchar"));
                Assert.True(typeMap.ContainsKey("nvarchar"));
                Assert.True(typeMap.ContainsKey("datetime"));
            }
        }

        [Fact]
        public void GetStoragePrimitiveType_works_with_MicrosoftDataSqlClient()
        {
            var ssdl =
                XElement.Parse(
                    "<Schema Namespace=\"Model.Store\" Provider=\"Microsoft.Data.SqlClient\" ProviderManifestToken=\"2008\" Alias=\"Self\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm/ssdl\" />");

            using (var storageModel = new StorageEntityModel(null, ssdl))
            {
                // This exercises the full code path: Provider.Value -> DependencyResolver -> SqlProviderServices
                Assert.Equal("int", storageModel.GetStoragePrimitiveType("int").Name);
                Assert.Equal("nvarchar", storageModel.GetStoragePrimitiveType("nvarchar").Name);
            }
        }

        [Fact]
        public void MicrosoftDataSqlClient_and_SystemDataSqlClient_return_same_types()
        {
            var ssdlMds =
                XElement.Parse(
                    "<Schema Namespace=\"Model.Store\" Provider=\"Microsoft.Data.SqlClient\" ProviderManifestToken=\"2008\" Alias=\"Self\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm/ssdl\" />");
            var ssdlSds =
                XElement.Parse(
                    "<Schema Namespace=\"Model.Store\" Provider=\"System.Data.SqlClient\" ProviderManifestToken=\"2008\" Alias=\"Self\" xmlns=\"http://schemas.microsoft.com/ado/2009/11/edm/ssdl\" />");

            using (var mdsModel = new StorageEntityModel(null, ssdlMds))
            using (var sdsModel = new StorageEntityModel(null, ssdlSds))
            {
                // Both providers should return the same store types since they both use SqlProviderServices
                var mdsTypes = mdsModel.StoreTypeNameToStoreTypeMap;
                var sdsTypes = sdsModel.StoreTypeNameToStoreTypeMap;

                Assert.Equal(sdsTypes.Count, mdsTypes.Count);
                foreach (var typeName in sdsTypes.Keys)
                {
                    Assert.True(mdsTypes.ContainsKey(typeName), $"Microsoft.Data.SqlClient missing type: {typeName}");
                }
            }
        }
    }
}
