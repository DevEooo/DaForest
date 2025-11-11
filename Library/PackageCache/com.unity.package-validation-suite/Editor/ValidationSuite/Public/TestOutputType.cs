using System;

namespace UnityEditor.PackageManager.ValidationSuite
{
    /// <summary>
    /// Type of output data returns by a validation suite test.
    /// </summary>
    public enum TestOutputType
    {
        /// <summary>The test output string is simply informational.</summary>
        Information,

        /// <summary>The test output string is a warning</summary>
        Warning,

        /// <summary>The test output string is a warning, but was part of the exception list</summary>
        WarningMarkedWithException,

        /// <summary>The test output string is an error</summary>
        Error,

        /// <summary>The test output string is an error, but was part of the exception list</summary>
        ErrorMarkedWithException
    }
}
