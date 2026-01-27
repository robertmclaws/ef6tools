// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.Data.Entity.Design.Extensibility
{
    internal interface IEntityDesignerCommandFactory
    {
        // <summary>
        //     Commands that will be surfaced in the Entity Designer
        // </summary>
        IList<EntityDesignerCommand> Commands { get; }
    }
}
