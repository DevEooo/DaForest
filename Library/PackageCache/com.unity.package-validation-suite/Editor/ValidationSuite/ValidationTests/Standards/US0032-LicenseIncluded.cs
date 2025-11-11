using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class LicenseIncludedUS0032 : BaseStandardChecker
    {
        public override string StandardCode => "US-0032";

        public override StandardVersion Version => new StandardVersion(1, 2, 1);

        public void Check(string path, ValidationType validationType, string packageName, string packageDisplayName)
        {
            var licenseFilePath = Path.Combine(path, Utilities.LicenseFile);

            // Check that the package has a license.md file.  All packages should have one.
            if (LongPathUtils.File.Exists(licenseFilePath))
            {
                // TODO: If the license file exists, check that the copyright year is setup properly.
                CheckLicenseContent(licenseFilePath, packageName, packageDisplayName);
            }
            else if (validationType == ValidationType.VerifiedSet)
            {
                AddWarning(string.Format("Every package must have a LICENSE.md file. {0}", ErrorDocumentation.GetLinkMessage(ErrorTypes.LicenseFileMissing)));
            }
            else
            {
                AddError(string.Format("Every package must have a LICENSE.md file. {0}", ErrorDocumentation.GetLinkMessage(ErrorTypes.LicenseFileMissing)));
            }
        }

        private void CheckLicenseContent(string licenseFilePath, string packageName, string packageDisplayName)
        {
            // if the file exists, make sure its not empty.
            var licenseContent = File.ReadAllLines(licenseFilePath);
            if (!licenseContent.Any())
            {
                AddError("A LICENSE.md file exists in the package, but it is empty.  All packages need a valid license");
                return;
            }

            // check that the license is valid.  We expect the first line to look like this:
            var escapedName = Regex.Escape(packageName);
            var escapedDisplayName = Regex.Escape(packageDisplayName);
            var expectedLicenseHeader = $"^({escapedName}|{escapedDisplayName}) copyright \u00a9 \\d+ \\S(.*\\S)?$";
            if (!Regex.IsMatch(licenseContent[0], expectedLicenseHeader, RegexOptions.IgnoreCase))
            {
                // TODO: Make this an error at some point soon.
                var message = string.Format("A LICENSE.md file exists in the package, but is in the wrong format.  " +
                    "Ensure the copyright year is set properly, otherwise, please check the package starter kit's license file as reference.  " +
                    "https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit/blob/master/LICENSE.md  " +
                    "It was `{0}` but was expecting it to match regex `{1}`",
                    licenseContent[0], expectedLicenseHeader);
                AddWarning(message);
            }
        }
    }
}
