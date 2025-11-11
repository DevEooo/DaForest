using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal interface IValidationTestResult
    {
        // The test associated to this result
        IValidationTest ValidationTest { get; }

        TestState TestState { get; }

        List<ValidationTestOutput> TestOutput { get; }

        DateTime StartTime { get; }

        DateTime EndTime { get; }
    }
}
