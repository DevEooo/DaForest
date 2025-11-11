using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;
using UnityEngine.Assertions;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class AssetFolderNamingUS0061 : BaseStandardChecker
    {
        public override string StandardCode => "US-0061";

        public override StandardVersion Version => new StandardVersion(1, 0, 1);

        private static readonly Regex k_NamingConventionRegex = new Regex("^[A-Z]+[A-Za-z0-9_-]*$");

        static readonly string k_AssetsPath = Path.Combine("ProjectData~", "Assets");

        private string m_AssetsRootFolder;

        public void Check(string packagePath)
        {
            //Attempt to navigate to the Assets folder location as though the validation is performed on a packed template
            m_AssetsRootFolder = Path.Combine(packagePath, k_AssetsPath);
            if (!LongPathUtils.Directory.Exists(m_AssetsRootFolder))
                m_AssetsRootFolder = packagePath;

            var items = Utilities.GetDirectoryAndFilesIn(m_AssetsRootFolder);
            var incorrectlyNamedDirectories = items.Where(item => item.Type == Utilities.DirectoryItemType.Directory &&
                !DirectoryIsExempt(Path.GetFileName(item.Path)) &&
                !k_NamingConventionRegex.IsMatch(Path.GetFileName(item.Path))
                ).Select(item => item.Path).ToList();


            Utilities.HandleWarnings(incorrectlyNamedDirectories,
                "Some folders do not adhere to naming conventions.",
                "The following directory does not match the naming convention: ",
                AddWarning, AddInformation);
        }

        //Skip directories that are exempt from the standards around naming conventions
        bool DirectoryIsExempt(string targetName)
        {
            //The exceptions are based on the Hidden Folders section: https://docs.unity3d.com/Manual/SpecialFolders.html
            return targetName.StartsWith(".") || targetName.EndsWith("~");
        }
    }
}
