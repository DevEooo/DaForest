using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class AssemblyDefinitionValidation : BaseValidation
    {
        readonly AssemblyDefinitionCorrectlyNamedUS0038 assemblyDefinitionCorrectlyNamedUs0038 = new AssemblyDefinitionCorrectlyNamedUS0038();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            assemblyDefinitionCorrectlyNamedUs0038,
        };

        public AssemblyDefinitionValidation()
        {
            TestName = "Assembly Definition Validation";
            TestDescription = "Validate Presence and Contents of Assembly Definition Files.";
            TestCategory = TestCategory.ContentScan;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;
            var packageName = Context.PublishPackageInfo.name;
            var packagePath = Context.PublishPackageInfo.path;
            var manifestFilePath = Path.Combine(packagePath, Utilities.PackageJsonFilename);

            if (!LongPathUtils.File.Exists(manifestFilePath))
            {
                AddError("Can't find manifest: " + manifestFilePath);
                return;
            }

            assemblyDefinitionCorrectlyNamedUs0038.Check(packageName, packagePath);
        }
    }
}
