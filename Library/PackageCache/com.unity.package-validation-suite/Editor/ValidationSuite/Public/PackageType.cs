using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEditor.PackageManager.ValidationSuite
{
    /// <summary>
    /// The type of packages supported in Validation.
    /// </summary>
    public enum PackageType
    {
        /// <summary>Package containing editor and runtime code to be used within the Unity Editor.</summary>
        Tooling,

        /// <summary>Package containing a Unity Template</summary>
        Template,

        /// <summary>Package defining a FeatureSet</summary>
        FeatureSet,
    }

    internal static class PackageTypeParser
    {
        internal static PackageType Parse(string type)
        {
            switch (type.ToLowerInvariant())
            {
                case "template":
                    return PackageType.Template;
                case "feature":
                    return PackageType.FeatureSet;
                default:
                    return PackageType.Tooling;
            }
        }
    }

    internal static class PackageTypePolicies
    {
        //Controls if the packages needs to be packed, and then unpacked to be able to run tests against it.
        //Useful when locally validating packages, to make sure we validate against what is packed
        internal static bool NeedsLocalPublishing(this PackageType type)
        {
            switch (type)
            {
                case PackageType.Template:
                case PackageType.FeatureSet: //TODO: not sure what do decide for FeatureSets
                    return false;
                default:
                    return true;
            }
        }
    }
}
