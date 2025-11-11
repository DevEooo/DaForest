using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite
{
    public class ValidationSuiteReport
    {
        public static readonly string ResultsPath = Path.Combine("Library", "ValidationSuiteResults");

        private readonly string jsonReportPath;
        TextReport TextReport { get; set; }
        VettingReport VettingReport { get; set; }

        public ValidationSuiteReportData ReportData { get; set; }

        public ValidationSuiteReport()
        { }

        public ValidationSuiteReport(string packageId, string packageName, string packageVersion, string packagePath)
        {
            jsonReportPath = Path.Combine(ResultsPath, packageId + ".json");

            if (!LongPathUtils.Directory.Exists(ResultsPath))
                Directory.CreateDirectory(ResultsPath);

#if !UNITY_PACKAGE_MANAGER_DEVELOP_EXISTS
            TextReport = new TextReport(packageId, packageVersion);
#endif
            TextReport?.Clear();

            if (LongPathUtils.File.Exists(jsonReportPath))
                File.Delete(jsonReportPath);
        }

        internal void Initialize(VettingContext context)
        {
            TextReport?.Initialize(context);
        }

        private ValidationTestReport[] BuildReport(ValidationSuite suite)
        {
            var testReports = new ValidationTestReport[suite.ValidationTests.Count()];
            var i = 0;
            foreach (var validationTest in suite.ValidationTests)
            {
                testReports[i] = new ValidationTestReport();
                testReports[i].TestName = validationTest.TestName;
                testReports[i].TestDescription = validationTest.TestDescription;
                testReports[i].TestResult = validationTest.TestState.ToString();
                testReports[i].TestState = validationTest.TestState;
                testReports[i].TestOutput = validationTest.TestOutput.ToArray();
                testReports[i].StartTime = validationTest.StartTime.ToString();
                testReports[i].EndTime = validationTest.EndTime.ToString();
                var span = validationTest.EndTime - validationTest.StartTime;
                testReports[i].Elapsed = span.TotalMilliseconds > 1 ? (int)(span.TotalMilliseconds) : 1;
                i++;
            }

            return testReports;
        }

        public static string DiffsReportPath(string packageId)
        {
            return Path.Combine(ResultsPath, packageId + ".delta");
        }

        public static bool ReportExists(string packageId)
        {
            return TextReport.ReportExists(packageId);
        }

        public static string GetJsonReportPath(string packageId)
        {
            return Path.Combine(ResultsPath, packageId + ".json");
        }

        public static bool JsonReportExists(string packageId)
        {
            return LongPathUtils.File.Exists(GetJsonReportPath(packageId));
        }

        public static bool DiffsReportExists(string packageId)
        {
            var deltaReportPath = Path.Combine(ResultsPath, packageId + ".delta");
            return LongPathUtils.File.Exists(deltaReportPath);
        }

        public static ValidationSuiteReportData GetReport(string packageId)
        {
            if (!JsonReportExists(packageId))
                return null;

            return Utilities.GetDataFromJson<ValidationSuiteReportData>(GetJsonReportPath(packageId));
        }

        public void OutputErrorReport(string error)
        {
            TextReport?.Append(error);
            ActivityLogger.Log(error);
        }

        public void GenerateVettingReport(ValidationSuite suite)
        {
            VettingReport?.GenerateReport(suite);
        }

        public void GenerateTextReport(ValidationSuite suite)
        {
            TextReport?.GenerateReport(suite);
        }

        public void GenerateJsonReport(ValidationSuite suite)
        {
            var testLists = BuildReport(suite);
            var span = suite.EndTime - suite.StartTime;

            ReportData = new ValidationSuiteReportData
            {
                Type = suite.context.ValidationType,
                TestResult = suite.testSuiteState,
                StartTime = suite.StartTime.ToString(),
                EndTime = suite.EndTime.ToString(),
                Elapsed = span.TotalMilliseconds > 1 ? (int)(span.TotalMilliseconds) : 1,
                Tests = testLists.ToList()
            };

            File.WriteAllText(jsonReportPath, JsonUtility.ToJson(ReportData));
        }
    }
}
