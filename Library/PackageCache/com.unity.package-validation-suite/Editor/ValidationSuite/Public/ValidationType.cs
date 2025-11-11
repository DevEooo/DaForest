using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEditor.PackageManager.ValidationSuite
{
    /// <summary>
    /// The type of validation used to validate a package.
    /// </summary>
    public enum ValidationType
    {
        /// <summary>Validation for local development.</summary>
        LocalDevelopment,
        /// <summary>Validation for local development internal to Unity.</summary>
        LocalDevelopmentInternal,
        /// <summary>Validation for verified packages.</summary>
        VerifiedSet,
        /// <summary>Validation for continuous integration.</summary>
        CI,
        /// <summary>Validation for package publishing.</summary>
        Publishing,
        /// <summary>Validation for package promotion.</summary>
        Promotion,
        /// <summary>Validation for the asset store.</summary>
        AssetStore,
        /// <summary>Running just tests marked for InternalTesting</summary>
        InternalTesting,
        /// <summary>Validation for the structure of the package</summary>
        Structure
    }
}
