using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class SamplesValidation : BaseValidation
    {
        SamplesUS0116 samplesUs0116 = new SamplesUS0116();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker> { samplesUs0116 };

        public SamplesValidation()
        {
            TestName = "Samples Validation";
            TestDescription = "Verify that samples meet expectation, if the package has samples.";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
            SupportedPackageTypes = new[] { PackageType.Tooling, PackageType.Template, PackageType.FeatureSet };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;

            var samplesDirInfo = SamplesUtilities.GetSampleDirectoriesInfo(Context.PublishPackageInfo.path);
            //TODO: this is an implementation of Condition USC-0026, are we modeling Conditions as well?
            if (!samplesDirInfo.SamplesDirExists && !samplesDirInfo.SamplesTildeDirExists && Context.PublishPackageInfo.samples.Count == 0)
            {
                AddInformation("No samples found. Skipping Samples Validation.");
                TestState = TestState.NotRun;
                return;
            }

            samplesUs0116.Check(Context.PublishPackageInfo.path, Context.PublishPackageInfo.samples, Context.ValidationType);
        }
    }
}
