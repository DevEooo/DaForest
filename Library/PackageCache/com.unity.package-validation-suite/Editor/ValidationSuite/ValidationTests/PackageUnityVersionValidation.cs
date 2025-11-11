using System;
using System.Security.Policy;
using System.Text.RegularExpressions;
using Semver;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    class PackageUnityVersionValidation : BaseValidation
    {
        private static readonly string k_DocsFilePath = "package_unity_version_validation_error.html";

        const string k_DocsMoreStrictSection =
            "the-unity-version-requirement-is-more-strict-than-in-the-previous-version-of-the-package";
        const string k_DocsLessStrictSection =
            "the-unity-version-requirement-is-less-strict-than-in-the-previous-version-of-the-package";

        public PackageUnityVersionValidation()
        {
            TestName = "Package Unity Version Validation";
            TestDescription = "Validate Unity and package version relationship";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment,
                                           ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;

            ValidateUnityVersionBump(Context.PreviousPackageInfo, Context.ProjectPackageInfo);
        }

        // Require a minor bump of the package version when the minimum Unity version requirement becomes more strict.
        // This is to ensure that we have patch versions available for the previous version of the package.
        void ValidateUnityVersionBump(ManifestData previousManifest, ManifestData currentManifest)
        {
            // This is the first version of the package.
            if (previousManifest == null)
                return;

            // Minimum Unity version requirement did not change.
            var currentPackageUnityVersion = GetPackageUnityVersion(currentManifest);
            var previousPackageUnityVersion = GetPackageUnityVersion(previousManifest);
            if (currentPackageUnityVersion == previousPackageUnityVersion)
                return;

            // Minimum Unity version requirement became less strict.
            if (currentPackageUnityVersion < previousPackageUnityVersion)
            {
                AddWarning(
                    "The Unity version requirement is less strict than in the previous version of the package. " +
                    "Please confirm that this change is deliberate and intentional. " +
                    ErrorDocumentation.GetLinkMessage(k_DocsFilePath, k_DocsLessStrictSection));
                return;
            }

            // Major or minor version of package was bumped.
            var previousPackageVersion = SemVersion.Parse(previousManifest.version);
            var currentPackageVersion = SemVersion.Parse(currentManifest.version);
            if (currentPackageVersion.Major > previousPackageVersion.Major ||
                currentPackageVersion.Minor > previousPackageVersion.Minor)
                return;

            AddUnityAuthoredConditionalError(currentManifest,
                "The Unity version requirement is more strict than in the previous version of the package. " +
                "Increment the minor version of the package to leave patch versions available for previous version. " +
                ErrorDocumentation.GetLinkMessage(k_DocsFilePath, k_DocsMoreStrictSection));
        }

        static SemVersion GetPackageUnityVersion(ManifestData manifest)
        {
            return string.IsNullOrEmpty(manifest.unity) ? new SemVersion(0) : UnityVersion.Parse(manifest.unity);
        }
    }
}
