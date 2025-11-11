using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite
{
    public class TextReport
    {
        public string FilePath { get; set; }

        private const string exceptionSectionPlaceholder = "\nExceptions section\n";

        readonly string m_PackageVersion;

        public TextReport(string packageId)
        {
            FilePath = ReportPath(packageId);
        }

        internal TextReport(string packageId, string packageVersion)
            : this(packageId)
        {
            m_PackageVersion = packageVersion;
        }

        internal void Initialize(VettingContext context)
        {
            var packageInfo = context.ProjectPackageInfo;
            Write(
                string.Format("Validation Suite Results for package \"{0}\"\n", packageInfo.name) +
                string.Format(" - Path: {0}\n", packageInfo.path) +
                string.Format(" - Version: {0}\n", packageInfo.version) +
                string.Format(" - Type: {0}\n", context.PackageType) +
                string.Format(" - Context: {0}\n", context.ValidationType) +
                string.Format(" - Lifecycle: {0}\n", packageInfo.lifecycle) +
                string.Format(" - Test Time: {0}\n", DateTime.Now) +
                string.Format(" - Tested with {0} version: {1}\n", context.VSuiteInfo.name, context.VSuiteInfo.version)
            );

            if (context.ProjectPackageInfo.dependencies.Any())
            {
                Append("\nPACKAGE DEPENDENCIES:\n");
                Append("--------------------\n");
                foreach (var dependencies in context.ProjectPackageInfo.dependencies)
                {
                    Append(string.Format("    - {0}@{1}\n", dependencies.Key, dependencies.Value));
                }
            }

            Append(exceptionSectionPlaceholder);

            Append("\nVALIDATION RESULTS:\n");
            Append("------------------\n");
        }

        public void Clear()
        {
            if (LongPathUtils.File.Exists(FilePath))
                File.Delete(FilePath);
        }

        public void Write(string text)
        {
            File.WriteAllText(FilePath, text);
        }

        public void Append(string text)
        {
            File.AppendAllText(FilePath, text);
        }

        public void Replace(string text, string replacement)
        {
            string str = File.ReadAllText(FilePath);
            str = str.Replace(text, replacement);
            File.WriteAllText(FilePath, str);
        }

        public void GenerateReport(ValidationSuite suite)
        {
            SaveTestResult(suite, TestState.Failed);
            SaveTestResult(suite, TestState.Succeeded);
            SaveTestResult(suite, TestState.NotRun);
            SaveTestResult(suite, TestState.NotImplementedYet);
            PrintExceptions(suite.context); // make sure to print this at the end, when all exceptions were verified
        }

        void PrintExceptions(VettingContext context)
        {
            string str = "";
            if (context.ValidationExceptionManager.HasExceptions)
            {
                str = "\n\n***************************************\n";
                str += "PACKAGE CONTAINS VALIDATION EXCEPTIONS!\n";
                var issuesList = context.ValidationExceptionManager.CheckValidationExceptions(context.PublishPackageInfo.version);
                foreach (var issue in issuesList)
                {
                    str += "\n- Issue: " + issue + "\n";
                }
                str += "***************************************\n";
            }

            Replace(exceptionSectionPlaceholder, str);
        }

        void SaveTestResult(ValidationSuite suite, TestState testState)
        {
            if (suite.ValidationTests == null)
            {
                return;
            }

            foreach (var testResult in suite.ValidationTests.Where(t => t.TestState == testState))
            {
                Append(string.Format("\n{0} - \"{1}\"\n    ", testResult.TestState, testResult.TestName));
                foreach (var testOutput in testResult.TestOutput)
                {
                    Append(testOutput.ToString());
                    Append("\n\n    ");

                    // If test caused a failure show how to except it
                    if (m_PackageVersion != null && testState == TestState.Failed &&
                        testResult.CanUseValidationExceptions && testOutput.Type == TestOutputType.Error)
                    {
                        var validationExceptions = new ValidationExceptions
                        {
                            ErrorExceptions = new[]
                            {
                                new ValidationException
                                {
                                    ValidationTest = testResult.ValidationTest.TestName,
                                    ExceptionMessage = testOutput.Output,
                                    PackageVersion = m_PackageVersion,
                                },
                            },
                        };

                        Append("The above error can be excepted with the following ValidationExceptions.json contents:\n");
                        Append(JsonUtility.ToJson(validationExceptions, true));
                        Append("\n\n    ");
                    }
                }
            }
        }

        public static string ReportPath(string packageId)
        {
            return Path.Combine(ValidationSuiteReport.ResultsPath, packageId + ".txt");
        }

        public static bool ReportExists(string packageId)
        {
            return LongPathUtils.File.Exists(ReportPath(packageId));
        }
    }
}
