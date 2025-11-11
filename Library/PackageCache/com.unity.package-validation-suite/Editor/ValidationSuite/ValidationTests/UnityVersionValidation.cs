using Semver;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class UnityVersionValidation : BaseValidation
    {
        string m_UnityVersion;

        // Move code that validates that development is happening on the right version based on the package.json
        public UnityVersionValidation()
        {
            TestName = "Unity Version Validation";
            TestDescription = "Validate that the package was developed on the right version of Unity.";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.CI };
        }

        // This method is called synchronously during initialization,
        // and allows a test to interact with APIs, which need to run from the main thread.
        public override void Setup()
        {
            m_UnityVersion = UnityEngine.Application.unityVersion;
        }

        protected override void Run()
        {
            TestState = TestState.Succeeded;

            // Check Unity Version, make sure it's valid given current version of Unity
            var packageUnityVersion = Context.ProjectPackageInfo.unity;
            if (!string.IsNullOrEmpty(packageUnityVersion) &&
                !UnityVersionSatisfiesPackageUnityVersion(m_UnityVersion, packageUnityVersion))
            {
                AddError($"In package.json, \"unity\" is pointing to a version higher ({packageUnityVersion}) than the editor you are currently using ({m_UnityVersion}). " +
                    $"Validation needs to happen on a version of the editor that is supported by the package.");
            }
        }

        internal static bool UnityVersionSatisfiesPackageUnityVersion(string unityVersionString, string packageUnityVersionString)
        {
            var unityVersion = UnityVersion.Parse(unityVersionString);
            var packageUnityVersion = UnityVersion.Parse(packageUnityVersionString);

            // We only care about major and minor version
            var truncatedUnityVersion = new SemVersion(unityVersion.Major, unityVersion.Minor);
            var truncatedPackageUnityVersion = new SemVersion(packageUnityVersion.Major, packageUnityVersion.Minor);

            return truncatedUnityVersion >= truncatedPackageUnityVersion;
        }
    }
}
