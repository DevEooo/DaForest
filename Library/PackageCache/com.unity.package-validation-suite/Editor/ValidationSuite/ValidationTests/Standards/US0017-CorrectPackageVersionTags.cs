using Semver;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class CorrectPackageVersionTagsUS0017 : BaseStandardChecker
    {
        public override string StandardCode => "US-0017";

        public override StandardVersion Version => new StandardVersion(1, 0, 2);

        public void Check(SemVersion packageVersionNumber, VersionTag versionTag)
        {
            if (versionTag.IsEmpty()) return;

            if (versionTag.Tag == "preview")
            {
                AddError("In package.json, \"version\" cannot be tagged \"preview\" in lifecycle v2, please use \"exp\". " + ErrorDocumentation.GetLinkMessage(ErrorTypes.InvalidLifecycleV2));
                return;
            }

            if (packageVersionNumber.Major < 1) //TODO: this is not captured by a standard
            {
                AddError("In package.json, \"version\" cannot be tagged \"" + packageVersionNumber.Prerelease + "\" while the major version less than 1. " + ErrorDocumentation.GetLinkMessage(ErrorTypes.InvalidLifecycleV2));
                return;
            }

            if (versionTag.Tag != "exp" && versionTag.Tag != "pre")
            {
                AddError("In package.json, \"version\" must be a valid tag. \"" + versionTag.Tag + "\" is invalid, try either \"pre\" or \"exp\". " + ErrorDocumentation.GetLinkMessage(ErrorTypes.InvalidLifecycleV2));
                return;
            }

            if (versionTag.Tag != "exp" && versionTag.Feature != "")
            {
                AddError("In package.json, \"version\" must be a valid tag. Custom tag \"" + versionTag.Feature + "\" only allowed with \"exp\". " + ErrorDocumentation.GetLinkMessage(ErrorTypes.InvalidLifecycleV2));
                return;
            }

            if (versionTag.Tag == "exp" && versionTag.Feature.Length > 10)
            {
                AddError("In package.json, \"version\" must be a valid tag. Custom tag \"" + versionTag.Feature + "\" is too long, must be 10 characters long or less. " + ErrorDocumentation.GetLinkMessage(ErrorTypes.InvalidLifecycleV2));
                return;
            }

            if (versionTag.Iteration < 1)
            {
                AddError("In package.json, \"version\" must be a valid tag. Iteration is required to be 1 or greater. " + ErrorDocumentation.GetLinkMessage(ErrorTypes.InvalidLifecycleV2));
                return;
            }
        }
    }
}
