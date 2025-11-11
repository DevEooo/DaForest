using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class LicenseValidation : BaseValidation
    {
        LicenseIncludedUS0032 licenseIncludedUs0032 = new LicenseIncludedUS0032();
        ThirdyPartyNoticeUS0065 thirdyPartyNoticeUS0065 = new ThirdyPartyNoticeUS0065();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            licenseIncludedUs0032,
            thirdyPartyNoticeUS0065
        };

        public LicenseValidation()
        {
            TestName = "License Validation";
            TestDescription = "Verify that licensing information is present, and filled out.";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;
            licenseIncludedUs0032.Check(Context.PublishPackageInfo.path, Context.ValidationType, Context.PublishPackageInfo.name, Context.PublishPackageInfo.displayName);
            thirdyPartyNoticeUS0065.Check(Context.PublishPackageInfo.path);
        }
    }
}
