using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class FolderStructureValidation : BaseValidation
    {
        readonly PackageDoesNotContainResourcesFolderUS0111 packageDoesNotContainResourcesFolderUs0111 = new PackageDoesNotContainResourcesFolderUS0111();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            packageDoesNotContainResourcesFolderUs0111
        };

        public FolderStructureValidation()
        {
            TestName = "Folder Structure Validation";
            TestDescription = "Verify that the folder structure meets expectations.";
            TestCategory = TestCategory.ContentScan;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            TestState = TestState.Succeeded;
            packageDoesNotContainResourcesFolderUs0111.Check(Context.PublishPackageInfo.path);
        }
    }
}
