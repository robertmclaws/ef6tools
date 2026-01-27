// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using Microsoft.Data.Entity.Design.VersioningFacade;
using Microsoft.Data.Entity.Design.VersioningFacade.LegacyCodegen;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace Microsoft.Data.Entity.Tests.Design.VersioningFacade.LegacyCodegen
{
    [TestClass]
    public class CodeGeneratorBaseTests
    {
        [TestMethod]
        public void Create_returns_EntityCodeGenerator_for_Version3()
        {
            CodeGeneratorBase.Create(LanguageOption.GenerateCSharpCode, EntityFrameworkVersion.Version3)
                .Should().BeOfType<EntityCodeGenerator>();
        }
    }
}
