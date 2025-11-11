using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class PackageNamingConventionUS0006 : BaseStandardChecker
    {
        public override string StandardCode
        {
            get { return "US-0006"; }
        }
        public override StandardVersion Version => new StandardVersion(1, 0, 1);

        private string[] PackageNamePrefixList = { "com.unity.", "com.autodesk.", "com.havok.", "com.ptc." }; //TODO: only 'com.unity' is mentioned in the standard
        private const string UpmRegex = @"^[a-z0-9][a-z0-9-._]{0,213}$";
        private const string UpmDisplayRegex = @"^[a-zA-Z0-9 ]+$";
        private static readonly int MaxDisplayNameLength = 50;


        public void Check(string packageName, string displayName)
        {
            // Check the package Name, which needs to start with one of the approved company names.
            // This should probably be executed only in internal development, CI and Promotion contexts
            if (!PackageNamePrefixList.Any(namePrefix => (packageName.StartsWith(namePrefix) && packageName.Length > namePrefix.Length)))
            {
                AddError(String.Format("In package.json, \"name\" needs to start with one of these approved company names: {0}. {1}", string.Join(", ", PackageNamePrefixList), ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "name-needs-to-start-with-one-of-these-approved-company-names")));
            }

            // There cannot be any capital letters in package names.
            if (packageName.ToLower(CultureInfo.InvariantCulture) != packageName)
            {
                AddError(String.Format("In package.json, \"name\" cannot contain capital letters. {0}", ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "name-cannot-contain-capital-letters")));
            }

            // Check name against our regex.
            Match match = Regex.Match(packageName, UpmRegex);
            if (!match.Success)
            {
                AddError(String.Format("In package.json, \"name\" is not a valid name. {0}", ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "name-is-not-a-valid-name")));
            }

            // Package name cannot end with .framework, .plugin or .bundle.
            String[] strings = { ".framework", ".bundle", ".plugin" };
            foreach (var value in strings)
            {
                if (packageName.EndsWith(value))
                {
                    AddError(String.Format("In package.json, \"name\" cannot end with .plugin, .bundle or .framework. {0}", ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "name-cannot-end-with")));
                }
            }

            if (string.IsNullOrEmpty(displayName)) //TODO: the standard only mentions that the package has 50 or less, it doesn't mention that this is required.
            {
                AddError(String.Format("In package.json, \"displayName\" must be set. {0}", ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "displayName-must-be-set")));
            }
            else if (displayName.Length > MaxDisplayNameLength)
            {
                AddError(String.Format("In package.json, \"displayName\" is too long. Max Length = {0}. Current Length = {1}. {2}", MaxDisplayNameLength, displayName.Length, ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "displayName-is-too-long")));
            }
            else if (!Regex.Match(displayName, UpmDisplayRegex).Success) //TODO: this is not part of the standard
            {
                AddError(String.Format("In package.json, \"displayName\" cannot have any special characters. {0}", ErrorDocumentation.GetLinkMessage(ManifestValidation.k_DocsFilePath, "displayName-cannot-have-any-special-characters")));
            }
        }

        public string[] GetPackageNamePrefixList()
        {
            return PackageNamePrefixList;
        }
    }
}
