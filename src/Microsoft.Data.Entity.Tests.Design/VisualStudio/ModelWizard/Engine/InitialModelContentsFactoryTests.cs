// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

namespace Microsoft.Data.Entity.Tests.Design.VisualStudio.ModelWizard.Engine
{
    using FluentAssertions;
    using Microsoft.Data.Entity.Design.VersioningFacade;
    using Microsoft.Data.Entity.Design.VisualStudio;
    using Microsoft.Data.Entity.Design.VisualStudio.ModelWizard.Engine;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class InitialModelContentsFactoryTests
    {
        [TestMethod]
        public void GetInitialModelContents_returns_contents()
        {
            foreach (var targetSchemaVersion in EntityFrameworkVersion.GetAllVersions())
            {
                new InitialModelContentsFactory().GetInitialModelContents(targetSchemaVersion)
                    .Should().Be(EdmUtils.CreateEdmxString(targetSchemaVersion, string.Empty, string.Empty, string.Empty));
            }
        }
    }
}
