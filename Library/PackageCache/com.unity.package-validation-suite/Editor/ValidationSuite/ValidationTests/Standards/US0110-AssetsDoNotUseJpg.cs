using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class AssetsDoNotUseJpgUS0110 : BaseStandardChecker
    {
        public override string StandardCode => "US-0110";
        public override StandardVersion Version => new StandardVersion(1, 0, 0);

        public void Check(string packagePath)
        {
            foreach (var fileType in restrictedImageFileList)
            {
                List<string> matchingFiles = new List<string>();
                Utilities.RecursiveDirectorySearch(packagePath, fileType, ref matchingFiles);

                if (matchingFiles.Any())
                {
                    foreach (var file in matchingFiles)
                    {
                        var cleanRelativePath = Utilities.GetOSAgnosticPath(Utilities.GetPathFromRoot(file, packagePath));

                        if (!FileInExceptionPath(cleanRelativePath))
                        {
                            AddError(cleanRelativePath + " cannot be included in a package. All images we share with users must use the png format. This is an expectation shared with asset store packages as well, and will help us provide high quality images for our users.");
                        }
                    }
                }
            }
        }

        private bool FileInExceptionPath(string filePath)
        {
            // break up path into parts.
            var pathParts = filePath.ToLower().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var pathPart in pathParts)
            {
                if (imageQualityExceptionPaths.Contains(pathPart))
                {
                    // If any part of the path is in the exception list, return immediately.
                    return true;
                }
            }

            return false;
        }

        private readonly string[] restrictedImageFileList =
        {
            "*.jpg",
            "*.jpeg",
        };

        private readonly HashSet<string> imageQualityExceptionPaths = new HashSet<string>
        {
            "documentation~",
            "tests",
        };
    }
}
