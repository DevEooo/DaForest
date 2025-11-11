using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class LocalDocumentationValidation : BaseValidation
    {
        UserManualDocumentationIncludedUS0040 userManualDocumentationIncludedUS0040 = new UserManualDocumentationIncludedUS0040();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            userManualDocumentationIncludedUS0040
        };

        public LocalDocumentationValidation()
        {
            TestName = "Documentation Validation";
            TestDescription = "Make sure the package has local documentation.";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            TestState = TestState.Succeeded;
            userManualDocumentationIncludedUS0040.Check(Context.ProjectPackageInfo.path);
        }
    }
}
