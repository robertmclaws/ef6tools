// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System.Xml.Schema;

namespace Microsoft.Data.Entity.Design.Model
{
    /// <summary>
    ///     Simple class to use to count the number of schema validation errors
    /// </summary>
    internal class SchemaValidationErrorCollector
    {
        private int _errorCount;

        internal int ErrorCount
        {
            get { return _errorCount; }
        }

        internal void ValidationCallBack(object sender, ValidationEventArgs e)
        {
            ++_errorCount;
        }
    }
}
