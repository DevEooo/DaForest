using Semver;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class SemVerCheckUS0005 : BaseStandardChecker
    {
        public override string StandardCode
        {
            get { return "US-0005"; }
        }
        public override StandardVersion Version => new StandardVersion(1, 0, 2);

        public void Check(string v)
        {
            SemVersion version;
            // Check package version, make sure it's a valid SemVer string.
            if (!SemVersion.TryParse(v, out version))
            {
                AddError($"In package.json, \"version\" needs to be a valid \"Semver\". {ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "version-needs-to-be-a-valid-semver")}");
            }
        }
    }
}
