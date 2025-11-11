using System;
using System.Collections.Generic;

namespace UnityEditor.PackageManager.ValidationSuite
{
    /// <summary>
    /// The validation suite report.
    ///
    /// This contains all the information regarding every test that the validation suite has run.
    /// </summary>
    [Serializable]
    public class ValidationSuiteReportData
    {
        /// <summary>The type of validation that was run for the report.</summary>
        public ValidationType Type;

        /// <summary>The validation result for the report.</summary>
        public TestState TestResult;

        /// <summary>The start time when the validation report was ran.</summary>
        public string StartTime;

        /// <summary>The end time when the validation report was ran.</summary>
        public string EndTime;

        /// <summary>How long the validation report took to run.</summary>
        public int Elapsed;

        /// <summary>The list of individual tests that are included in the report.</summary>
        public List<ValidationTestReport> Tests;
    }
}
