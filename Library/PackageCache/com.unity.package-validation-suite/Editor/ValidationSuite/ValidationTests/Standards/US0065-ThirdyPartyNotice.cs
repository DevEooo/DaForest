using System.IO;
using System.Linq;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class ThirdyPartyNoticeUS0065 : BaseStandardChecker
    {
        public override string StandardCode => "US-0065";

        public override StandardVersion Version => new StandardVersion(1, 1, 3);

        public void Check(string packagePath)
        {
            var thirdPartyNoticeFilePath = Path.Combine(packagePath, Utilities.ThirdPartyNoticeFile);
            // Check that the 3rd party notice file is not empty, and does not come from the starter kit.
            if (LongPathUtils.File.Exists(thirdPartyNoticeFilePath))
            {
                CheckThirdPartyNoticeContent(thirdPartyNoticeFilePath);

                // TODO: Signal to the vetting report that the package contains a 3rd party notice
            }
            else
            {
                // TODO: check that the code doesn't have any copyright headers if the 3rd party notice file is empty.
                CheckForCopyrightMaterial();
            }
        }

        private void CheckThirdPartyNoticeContent(string filePath)
        {
            // if the file exists, make sure its not empty.
            var licenseContent = File.ReadAllLines(filePath);
            if (!licenseContent.Any())
            {
                AddError("A 3rd Party Notice file exists in the package, but it is empty.  If it isn't required, delete it, otherwise, follow this model to fill it out: https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit/blob/master/Third%20Party%20Notices.md");
                return;
            }

            int numberOf3rdParties = 0;
            bool lookForLicenseType = false;
            for (int i = 0; i < licenseContent.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(licenseContent[i]))
                {
                    continue;
                }

                if (licenseContent[i].StartsWith("License Type:"))
                {
                    if (!lookForLicenseType)
                    {
                        AddError("Invalid 3rd Party Notice File.  Found License Type line without a previous Component Name line. Please follow the model outlined here: https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit/blob/master/Third%20Party%20Notices.md");
                        return;
                    }

                    lookForLicenseType = false;
                }

                if (licenseContent[i].StartsWith("Component Name:"))
                {
                    numberOf3rdParties++;

                    if (lookForLicenseType)
                    {
                        AddError("Invalid 3rd Party Notice File.  Found Component Name line, but no License Type line that follows. Please follow the model outlined here: https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit/blob/master/Third%20Party%20Notices.md");
                        return;
                    }

                    lookForLicenseType = true;
                }
            }

            if (lookForLicenseType)
            {
                AddError("Invalid 3rd Party Notice File.  Found Component Name line, but no License Type line that follows. Please follow the model outlined here: https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit/blob/master/Third%20Party%20Notices.md");
            }

            if (numberOf3rdParties == 0)
            {
                AddError("Invalid 3rd Party Notice File.  Found no valid entries. Please follow the model outlined here: https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit/blob/master/Third%20Party%20Notices.md");
            }
        }

        void CheckForCopyrightMaterial()
        {
        }
    }
}
