using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class PathLengthValidation : BaseValidation
    {
        private FilePathLengthUS0113 filePathLengthUs0113 = new FilePathLengthUS0113();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker> { filePathLengthUs0113 };

        public int MaxPathLength
        {
            get => filePathLengthUs0113.MaxPathLength;
            set => filePathLengthUs0113.MaxPathLength = value;
        }

        public PathLengthValidation()
        {
            TestName = "Path Length Validation";
            TestDescription = "Validate that all package files are below a maximum path threshold, to ensure that excessively long paths are not produced on Windows machines within user projects.";
            TestCategory = TestCategory.ContentScan;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;

            var rootPath = Context.PublishPackageInfo.path;
            if (!Path.IsPathRooted(Context.PublishPackageInfo.path))
                rootPath = Path.GetFullPath(rootPath);

            //check if each file/folder has a sufficiently short path relative to the base
            filePathLengthUs0113.Check(rootPath);
        }
    }
}
