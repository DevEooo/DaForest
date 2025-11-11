using System.IO;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class PrimedTemplateLibraryUS0114 : BaseStandardChecker
    {
        public override string StandardCode => "US-0114";

        public override StandardVersion Version => new StandardVersion(1, 0, 0);

        static readonly string k_DocsFilePath = "primed_library_validation_error.html";
        static readonly string k_LibraryPath = Path.Combine("ProjectData~", "Library");
        static readonly string[] k_PrimedLibraryPaths =
        {
            "ArtifactDB",
            "Artifacts",
            "SourceAssetDB",
        };

        public void Check(string path)
        {
            // Check that Library directory of template contains primed paths
            foreach (var primedLibraryPath in k_PrimedLibraryPaths)
            {
                var packageRelativePath = Path.Combine(k_LibraryPath, primedLibraryPath);
                var fullPath = Path.Combine(path, packageRelativePath);

                if (!(LongPathUtils.File.Exists(fullPath) || LongPathUtils.Directory.Exists(fullPath)))
                {
                    var documentationLink = ErrorDocumentation.GetLinkMessage(
                        k_DocsFilePath, "template-is-missing-primed-library-path");
                    AddError($"Template is missing primed library path at {packageRelativePath}. " +
                        $"It should have been added automatically in the CI packing process. {documentationLink}");
                }
            }
        }
    }
}
