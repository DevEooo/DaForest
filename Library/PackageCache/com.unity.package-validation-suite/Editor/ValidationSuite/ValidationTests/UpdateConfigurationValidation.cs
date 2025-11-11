using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
#if UNITY_2019_1_OR_NEWER
    internal class UpdateConfigurationValidation : BaseAssemblyValidation
    {
        readonly PackageIncludesApiUpdaterScriptsUS0117 packageIncludesApiUpdaterScriptsUs0117 = new PackageIncludesApiUpdaterScriptsUS0117();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>() {packageIncludesApiUpdaterScriptsUs0117};

        public UpdateConfigurationValidation()
        {
            TestName = "API Updater Configuration Validation";
            TestDescription = "Checks the validity of script updater code included with the package.";
            TestCategory = TestCategory.ApiValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion };
            SupportedPackageTypes = new[] { PackageType.Tooling };
        }

        protected override bool IncludePrecompiledAssemblies => true;
        protected override void Run(AssemblyInfo[] info)
        {
            TestState = TestState.Succeeded;
            if (Context.ProjectPackageInfo?.name == "com.unity.package-validation-suite")
            {
                AddInformation("PackageValidationSuite update configurations tested by editor tests.");
                return;
            }

            packageIncludesApiUpdaterScriptsUs0117.Check(info, Context.ProjectPackageInfo.path, Context.ProjectPackageInfo?.name);
        }
    }
#endif
}
