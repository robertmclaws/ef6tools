// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Data.Entity.Design.Model;
using Microsoft.Data.Entity.Design.Model.Entity;
using Microsoft.Data.Entity.Design.VersioningFacade;
using Microsoft.Data.Entity.Design.VersioningFacade.LegacyCodegen;
using Microsoft.Data.Tools.XmlDesignerBase.Model;
using Moq;
using Moq.Protected;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Data.Entity.Tests.Design.Model
{
    [TestClass]
    public class LegacyCodeGenerationDriverTests
    {
        [TestMethod]
        public void GenerateCode_creates_code_generator_and_generates_code()
        {
            var modelManager = new Mock<ModelManager>(null, null).Object;

            Mock<EntityDesignArtifact> mockArtifact =
                new Mock<EntityDesignArtifact>(
                    modelManager, new Uri("http://tempuri"), new Mock<XmlModelProvider>().Object);

            XElement modelElement = new XElement("{http://schemas.microsoft.com/ado/2009/11/edm}Schema");

            using (var conceptualModel = new Mock<ConceptualEntityModel>(mockArtifact.Object, modelElement).Object)
            {
                mockArtifact.Setup(a => a.ConceptualModel).Returns(conceptualModel);
                mockArtifact.Setup(a => a.Uri).Returns(new Uri("http://tempuri"));

                Mock<CodeGeneratorBase> mockCodeGen = new Mock<CodeGeneratorBase>();
                Mock<LegacyCodeGenerationDriver> mockCodeGenDriver = new Mock<LegacyCodeGenerationDriver>(
                    LanguageOption.GenerateVBCode, EntityFrameworkVersion.Version3) { CallBase = true };

                mockCodeGenDriver
                    .Protected()
                    .Setup<CodeGeneratorBase>(
                        "CreateCodeGenerator", ItExpr.IsAny<LanguageOption>(),
                        ItExpr.IsAny<Version>())
                    .Returns(mockCodeGen.Object);

                mockCodeGenDriver.Object.GenerateCode(mockArtifact.Object, null, new StringWriter());

                mockCodeGenDriver
                    .Protected()
                    .Verify<CodeGeneratorBase>(
                        "CreateCodeGenerator", Times.Once(),
                        ItExpr.IsAny<LanguageOption>(), ItExpr.IsAny<Version>());

                mockCodeGen.Verify(g => g.AddNamespaceMapping(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                mockCodeGen.Verify(g => g.GenerateCode(It.IsAny<XmlReader>(), It.IsAny<TextWriter>()), Times.Once());
            }
        }

        [TestMethod]
        public void GenerateCode_adds_default_namespace_if_not_null()
        {
            var modelManager = new Mock<ModelManager>(null, null).Object;

            Mock<EntityDesignArtifact> mockArtifact =
                new Mock<EntityDesignArtifact>(
                    modelManager, new Uri("http://tempuri"), new Mock<XmlModelProvider>().Object);

            XElement modelElement = new XElement("{http://schemas.microsoft.com/ado/2009/11/edm}Schema");

            using (var conceptualModel = new Mock<ConceptualEntityModel>(mockArtifact.Object, modelElement).Object)
            {
                mockArtifact.Setup(a => a.ConceptualModel).Returns(conceptualModel);
                mockArtifact.Setup(a => a.Uri).Returns(new Uri("http://tempuri"));

                Mock<CodeGeneratorBase> mockCodeGen = new Mock<CodeGeneratorBase>();
                Mock<LegacyCodeGenerationDriver> mockCodeGenDriver = new Mock<LegacyCodeGenerationDriver>(
                    LanguageOption.GenerateVBCode, EntityFrameworkVersion.Version3) { CallBase = true };

                mockCodeGenDriver
                    .Protected()
                    .Setup<CodeGeneratorBase>(
                        "CreateCodeGenerator", ItExpr.IsAny<LanguageOption>(),
                        ItExpr.IsAny<Version>())
                    .Returns(mockCodeGen.Object);

                mockCodeGenDriver.Object.GenerateCode(mockArtifact.Object, null, new StringWriter());
                mockCodeGen.Verify(g => g.AddNamespaceMapping(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

                mockCodeGenDriver.Object.GenerateCode(mockArtifact.Object, string.Empty, new StringWriter());
                mockCodeGen.Verify(g => g.AddNamespaceMapping(It.IsAny<string>(), string.Empty), Times.Once());

                conceptualModel.Namespace.Dispose();
            }
        }
    }
}
