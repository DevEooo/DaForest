using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    class PrimedLibraryValidation : BaseValidation
    {
        PrimedTemplateLibraryUS0114 primedTemplateLibraryUs0114 = new PrimedTemplateLibraryUS0114();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker> { primedTemplateLibraryUs0114 };

        public PrimedLibraryValidation()
        {
            TestName = "Primed Library Validation";
            TestDescription = "Validate that the Library directory of a template contains primed paths";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.Promotion };
            SupportedPackageTypes = new[] { PackageType.Template };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;
            primedTemplateLibraryUs0114.Check(Context.PublishPackageInfo.path);
        }
    }
}
