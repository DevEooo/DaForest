using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;
using UnityEngine.Assertions;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class AssetNamingUS0057 : BaseStandardChecker
    {
        public override string StandardCode => "US-0057";

        public override StandardVersion Version => new StandardVersion(1, 0, 1);

        private static readonly Regex k_NamingConventionRegex = new Regex("^[A-Z]+[A-Za-z0-9_-]*$");

        /// <summary>
        /// This list provides extensions that should be ignored during naming convention validation.
        /// It also includes common double extensions, since C# methods do not return the full extension for assets that have multiple extensions.
        /// </summary>
        private static readonly List<string> k_ExemptedFileSuffixes = new List<string>
        {
            ".md.txt",
            ".tmp",
            ".meta",
            ".asmdef"
        };

        static readonly string k_AssetsPath = Path.Combine("ProjectData~", "Assets");

        private string m_AssetsRootFolder;

        public void Check(string packagePath)
        {
            //Attempt to navigate to the Assets folder location as though the validation is performed on a packed template
            m_AssetsRootFolder = Path.Combine(packagePath, k_AssetsPath);
            if (!LongPathUtils.Directory.Exists(m_AssetsRootFolder))
                m_AssetsRootFolder = packagePath;

            var items = Utilities.GetDirectoryAndFilesIn(m_AssetsRootFolder);
            var incorrectlyNamedFiles = items.Where(item =>
            {
                if (item.Type != Utilities.DirectoryItemType.File)
                {
                    return false;
                }

                var fileName = Path.GetFileName(item.Path);

                if (!FileIsExempt(fileName) && !k_NamingConventionRegex.IsMatch(Path.GetFileNameWithoutExtension(fileName)))
                {
                    return true;
                }
                return false;
            }).Select(item => item.Path).ToList();

            Utilities.HandleWarnings(incorrectlyNamedFiles,
                "Some assets do not adhere to naming conventions.",
                "The following asset does not match the naming convention: ",
                AddWarning, AddInformation);
        }

        bool FileIsExempt(string targetName)
        {
            string fullName = targetName;

            if (!targetName.StartsWith(".") && Path.HasExtension(targetName))
            {
                targetName = Path.GetFileNameWithoutExtension(targetName);
            }

            bool hasExemptedFileSuffix = k_ExemptedFileSuffixes.FindIndex(element => fullName.EndsWith(element)) >= 0;

            return targetName == "cvs" || targetName.StartsWith(".") || targetName.EndsWith("~") || hasExemptedFileSuffix;
        }
    }
}
