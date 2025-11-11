using System.IO;
using System.Linq;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class UserManualDocumentationIncludedUS0040 : BaseStandardChecker
    {
        public override string StandardCode => "US-0040";

        public override StandardVersion Version => new StandardVersion(2, 0, 1);

        public void Check(string packagePath)
        {
            // Check for a documentation directory.
            string[] rootDirs = LongPathUtils.Directory.GetDirectories(packagePath);
            var wrongNameDocsDir = rootDirs.FirstOrDefault(d =>
            {
                var path = Path.GetFileName(d).ToLower();
                return path == ".documentation" || path == "documentation";
            });
            var docsDir = rootDirs.FirstOrDefault(d =>
            {
                var path = Path.GetFileName(d).ToLower();
                return path == ".documentation~" || path == "documentation~";
            });

            string[] allDocsDir = LongPathUtils.Directory.GetDirectories(packagePath, "*Documentation*");
            if (allDocsDir.Length > 1)
            {
                AddError("You have multiple documentation folders. Please keep only the one named \"Documentation~\".");
                return;
            }
            else if (!string.IsNullOrEmpty(wrongNameDocsDir))
            {
                AddError("Please rename your \"Documentation\" folder to \"Documentation~\" so that its files are ignored by the asset database.");
                return;
            }
            else if (string.IsNullOrEmpty(docsDir))
            {
                AddError("Your package must contain a \"Documentation~\" folder at the root, which holds your package's documentation.");
                return;
            }

            var defaultFiles = LongPathUtils.Directory.GetFiles(docsDir, "your-package-name.md");
            // Check the default file is not there anymore
            if (defaultFiles.Length > 0)
            {
                AddError("File \"your-package-name.md\" found in \"Documentation~\" directory, which comes from the package template.  Please take the time to work on your documentation.");
            }

            var docFiles = LongPathUtils.Directory.GetFiles(docsDir, "*.md");
            // Check for at least 1 md file in that directory.
            if (docFiles.Length == 0)
            {
                AddError("Your package must contain a \"Documentation~\" folder, with at least one \"*.md\" file in order for documentation to properly get built.");
            }
            // Check that documentation files (except the default file) have at least 10 characters in them
            else if (docFiles.Length > defaultFiles.Length)
            {
                foreach (string filePath in docFiles)
                {
                    if (filePath.IndexOf("your-package-name.md") == -1)
                    {
                        long fileLength = new FileInfo(filePath).Length;
                        if (fileLength < 10)
                        {
                            AddError("Your documentation file " + filePath + " should contain at least 10 characters.");
                        }
                    }
                }
            }
        }
    }
}
