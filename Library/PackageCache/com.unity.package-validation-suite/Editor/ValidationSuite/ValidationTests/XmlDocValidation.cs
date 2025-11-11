using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class XmlDocValidation : BaseAssemblyValidation
    {
        readonly APIDocumentationIncludedUS0041 apiDocumentationIncludedUs0041 = new APIDocumentationIncludedUS0041();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            apiDocumentationIncludedUs0041,
        };

        public XmlDocValidation()
        {
            Init();
        }

        public XmlDocValidation(ValidationAssemblyInformation validationAssemblyInformation)
            : base(validationAssemblyInformation)
        {
            Init();
        }

        private void Init()
        {
            TestName = "Xmldoc Validation";
            TestDescription = "Checks public API to ensure xmldocs exist.";
            TestCategory = TestCategory.ApiValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion };
        }

        protected override void Run(AssemblyInfo[] info)
        {
            TestState = TestState.Succeeded;

            apiDocumentationIncludedUs0041.Check(Context.ProjectPackageInfo.path, info, validationAssemblyInformation);
        }
    }
}
