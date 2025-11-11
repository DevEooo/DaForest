namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class TemplateUsesOnlyReleasedPackagesUS0188 : BaseStandardChecker
    {
        public override string StandardCode => "US-0188";

        public override StandardVersion Version => new StandardVersion(1, 0, 2);

        public void Check(PackageManagerSettingsValidation packageManagerSettingsValidation)
        {
            ValidatePackageManagerSettings(packageManagerSettingsValidation);
        }

        /// <summary>
        /// For templates, we want to make sure that Show Preview Packages or Show Pre-Release packages
        /// are not enabled.
        /// For <=2019.4 this is saved on the profile preferences and is not bound to be
        /// pre-enabled. For >2019.4 this is saved in ProjectSettings/PackageManagerSettings.asset
        /// </summary>
        private void ValidatePackageManagerSettings(PackageManagerSettingsValidation packageManagerSettingsValidation)
        {
            if (packageManagerSettingsValidation == null)
                return;

            //TODO: the standard says that the template cannot depend on prerelease packages, this tests something else, that one of the side effects is that.
            if (packageManagerSettingsValidation.m_EnablePreviewPackages)
                AddError($"Preview packages are not allowed to be enabled on template packages. Please disable the `Enable Preview Packages` option in ProjectSettings > PackageManager > Advanced Settings. {ErrorDocumentation.GetLinkMessage(TemplateProjectValidation.k_DocsFilePath, "preview|prerelease-packages-are-not-allowed-to-be-enabled-on-template-packages")}");

            if (packageManagerSettingsValidation.m_EnablePreReleasePackages)
                AddError($"PreRelease packages are not allowed to be enabled on template packages. Please disable the `Enable PreRelease Packages` option in ProjectSettings > PackageManager > Advanced Settings. {ErrorDocumentation.GetLinkMessage(TemplateProjectValidation.k_DocsFilePath, "preview|prerelease-packages-are-not-allowed-to-be-enabled-on-template-packages")}");
        }
    }
}
