using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class RestrictedFilesValidation : BaseValidation
    {
        /*
         * Ideally this string should be inlined in the UnapprovedFileTypesUS0115.internalExceptionFileList list, but
         * this string is restricted by the internal validation suite and the exception is set for the current file specifically.
         */
        internal const string LldExecutable = "lld.exe";

        UnapprovedFileTypesUS0115 unapprovedFileTypesUs0115 = new UnapprovedFileTypesUS0115();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>() { unapprovedFileTypesUs0115 };

        public RestrictedFilesValidation()
        {
            TestName = "Restricted File Type Validation";
            TestDescription = "Make sure no restricted file types are included with this package.";
            TestCategory = TestCategory.ContentScan;
        }

        protected override void Run()
        {
            // Start by declaring victory
            TestState = TestState.Succeeded;
            unapprovedFileTypesUs0115.Check(Context.PublishPackageInfo.path, Context.ValidationType);
        }
    }
}
