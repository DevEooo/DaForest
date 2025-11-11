using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class AssetsValidation : BaseValidation
    {
        private readonly AssetsDoNotUseJpgUS0110 assetsDoNotUseJpgUs0110 = new AssetsDoNotUseJpgUS0110();
        internal override List<IStandardChecker> ImplementedStandardsList
        {
            get
            {
                return new List<IStandardChecker>()
                {
                    assetsDoNotUseJpgUs0110
                };
            }
        }

        public AssetsValidation()
        {
            TestName = "Assets Validation";
            TestDescription = "Make sure assets included with the package meet Unity standards.";
            TestCategory = TestCategory.ContentScan;
        }

        protected override void Run()
        {
            TestState = TestState.Succeeded;

            // Validate that all images our users will interact with meet quality standards
            assetsDoNotUseJpgUs0110.Check(Context.PublishPackageInfo.path);
        }
    }
}
