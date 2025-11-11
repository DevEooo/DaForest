using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class ChangeLogValidation : BaseValidation
    {
        readonly ChangelogExistsAndContainValidEntriesCheckUS0039 changelogExistsAndContainValidEntriesCheckUS0039 = new ChangelogExistsAndContainValidEntriesCheckUS0039();

        internal override List<IStandardChecker> ImplementedStandardsList =>
            new List<IStandardChecker>
        {
            changelogExistsAndContainValidEntriesCheckUS0039,
        };

        public ChangeLogValidation()
        {
            TestName = "ChangeLog Validation";
            TestDescription = "Validate Changelog contains entry for given package.";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
            SupportedPackageTypes = new[] { PackageType.Template, PackageType.Tooling, PackageType.FeatureSet };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;

            changelogExistsAndContainValidEntriesCheckUS0039.Check(Context.ProjectPackageInfo.path, Context.ProjectPackageInfo.version, Context.ValidationType);
        }
    }
}
