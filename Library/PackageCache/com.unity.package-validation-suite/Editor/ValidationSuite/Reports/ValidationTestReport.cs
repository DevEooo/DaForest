using System;
using System.Collections.Generic;

namespace UnityEditor.PackageManager.ValidationSuite
{
    /// <summary>
    /// The result of a single test for the validation suite.
    /// </summary>
    [Serializable]
    public class ValidationTestReport
    {
        /// <summary>The name of the test.</summary>
        public string TestName;
        /// <summary>The description for the test.</summary>
        public string TestDescription;
        /// <summary>The test result as a string.</summary>
        public string TestResult;
        /// <summary>The test result.</summary>
        public TestState TestState;
        /// <summary>The list of individual output string for this tests. This gives the description of everything that went wrong when running the test.</summary>
        public ValidationTestOutput[] TestOutput;
        /// <summary>The start time that the test was run.</summary>
        public string StartTime;
        /// <summary>The end time when the test completed.</summary>
        public string EndTime;
        /// <summary>How long it took to execute the test.</summary>
        public int Elapsed;
    }

    /// <summary>
    /// Test output details.
    /// </summary>
    [Serializable]
    public class ValidationTestOutput
    {
        /// <summary>
        /// Type of output
        /// </summary>
        public TestOutputType Type;

        /// <summary>
        /// Output string
        /// </summary>
        public string Output;

        public override string ToString()
        {
            return Type + ": " + Output;
        }
    }
}
