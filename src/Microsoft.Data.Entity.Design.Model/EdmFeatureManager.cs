// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the MIT license.  See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using Microsoft.Data.Entity.Design.VersioningFacade;

namespace Microsoft.Data.Entity.Design.Model
{
    /// <summary>
    ///     Manages feature state for EDM features.
    ///     All features are always enabled for EF6 (Version3).
    ///     Version checks are kept for backward compatibility with old EDMX files.
    /// </summary>
    internal static class EdmFeatureManager
    {
        /// <summary>
        ///     Returns the FeatureState for FunctionImports returning a ComplexType feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetFunctionImportReturningComplexTypeFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3); kept for backward compatibility with old files
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Whether enum feature is supported in the targeted schema version.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetEnumTypeFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Returns the FeatureState for the EnumTypes feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetEnumTypeFeatureState(EFArtifact artifact)
        {
            Debug.Assert(artifact != null, "artifact != null");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Returns the FeatureState for the 'Get Column Information' functionality for FunctionImports.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetFunctionImportColumnInformationFeatureState(EFArtifact artifact)
        {
            Debug.Assert(artifact != null, "artifact != null");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Return the FeatureState for the Function Import mapping feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetFunctionImportMappingFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Returns the FeatureState for the exposed foreign keys in the conceptual model feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetForeignKeysInModelFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Return the FeatureState for the GenerateUpdateViews feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetGenerateUpdateViewsFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Returns the FeatureState for the EntityContainers' TypeAccess attribute feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetEntityContainerTypeAccessFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Return the FeatureState for the LazyLoadingEnabled feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetLazyLoadingFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Return the FeatureState for the composable function imports feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetComposableFunctionImportFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }

        /// <summary>
        ///     Return the FeatureState for the UseStrongSpatialTypes feature.
        ///     Always enabled for EF6.
        /// </summary>
        internal static FeatureState GetUseStrongSpatialTypesFeatureState(Version schemaVersion)
        {
            Debug.Assert(EntityFrameworkVersion.IsValidVersion(schemaVersion), "Invalid schema version.");

            // Always enabled for EF6 (Version3)
            return FeatureState.VisibleAndEnabled;
        }
    }
}
