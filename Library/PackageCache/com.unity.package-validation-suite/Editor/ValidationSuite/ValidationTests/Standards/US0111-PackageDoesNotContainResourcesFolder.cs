using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class PackageDoesNotContainResourcesFolderUS0111 : BaseStandardChecker
    {
        public override string StandardCode => "US-0111";
        public override StandardVersion Version => new StandardVersion(1, 0, 0);

        public void Check(string packagePath)
        {
            List<string> problematicDirectoryList = new List<string>();

            ScanForResourcesDir(problematicDirectoryList, packagePath);

            if (problematicDirectoryList.Any())
            {
                AddWarning("The Resources Directory should not be used in packages.  For more guidance, please visit https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity6.html");
                problematicDirectoryList.ForEach(s => AddInformation("Problematic directory: /" + s));
            }
        }

        private void ScanForResourcesDir(List<string> problematicDirectoryList, string path)
        {
            var directories = LongPathUtils.Directory.GetDirectories(path);
            if (!directories.Any())
                return;

            foreach (var directory in directories)
            {
                ScanForResourcesDir(problematicDirectoryList, directory);
            }

            var resourcePath = Path.Combine(path, "Resources");
            if (LongPathUtils.Directory.Exists(resourcePath))
            {
                problematicDirectoryList.Add(resourcePath.Replace("\\", "/"));
            }
        }
    }
}
