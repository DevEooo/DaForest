using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;
using UnityEngine.Assertions;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class FolderStructureConventionsUS0063 : BaseStandardChecker
    {
        public override string StandardCode => "US-0063";

        public override StandardVersion Version => new StandardVersion(1, 1, 1);

        static readonly string k_AssetsPath = Path.Combine("ProjectData~", "Assets");

        const int k_MaxFolderDepth = 10;

        private string m_AssetsRootFolder;


        public void Check(string packagePath)
        {
            //Attempt to navigate to the Assets folder location as though the validation is performed on a packed template
            m_AssetsRootFolder = Path.Combine(packagePath, k_AssetsPath);
            if (!LongPathUtils.Directory.Exists(m_AssetsRootFolder))
                m_AssetsRootFolder = packagePath;

            var emptyDirectories = new List<string>();
            var highFolderDepthDirectories = new List<string>();
            var items = Utilities.GetDirectoryAndFilesIn(m_AssetsRootFolder);
            foreach (var item in items.Where(x => x.Type == Utilities.DirectoryItemType.Directory))
            {
                if (item.ChildCount == 0)
                    emptyDirectories.Add(item.Path);

                if (item.Depth > k_MaxFolderDepth)
                    highFolderDepthDirectories.Add(item.Path);
            }

            Utilities.HandleWarnings(emptyDirectories,
                "Empty folders are not allowed for template packages, please remove the folders listed below.",
                "The following directory is empty: ",
                AddWarning, AddInformation);

            Utilities.HandleWarnings(highFolderDepthDirectories,
                $"Depth of folder hierarchy exceeds {k_MaxFolderDepth} for some paths.",
                "The folder depth at the following path exceeds the maximum: ",
                AddWarning, AddInformation);
        }
    }
}
