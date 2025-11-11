using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class MetaFilesValidation : BaseValidation
    {
        PackageContainsMetafileUS0112 packageContainsMetafileUs0112 = new PackageContainsMetafileUS0112();
        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            packageContainsMetafileUs0112
        };

        public MetaFilesValidation()
        {
            TestName = "Meta Files Validation";
            TestDescription = "Validate that metafiles are present for all package files.";
            TestCategory = TestCategory.ContentScan;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Structure, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;
            //check if each file/folder has its .meta counter-part
            packageContainsMetafileUs0112.Check(Context.PublishPackageInfo.path);
        }
    }
}
