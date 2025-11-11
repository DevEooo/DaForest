using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests;
using UnityEngine;
using UnityEngine.Profiling;

namespace UnityEditor.PackageManager.ValidationSuite
{
    // Attribute for methods to be called before any tests are run, prototype is "void MyMethod(VettingContext)"
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidationSuiteSetup : Attribute { }

    // Attribute for methods to be called after all tests are run, prototype is "void MyMethod(VettingContext)"
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidationSuiteTeardown : Attribute { }

    // Delegate called after every test to provide immediate feedback on single test results.
    internal delegate void SingleTestCompletedDelegate(IValidationTestResult testResult);

    // Delegate called after the test run completed, whether it succeeded, failed or got canceled.
    internal delegate void AllTestsCompletedDelegate(ValidationSuite suite, TestState testRunState);

    /// <summary>
    /// The validation suite allows you to validate a package while in development
    /// </summary>
    [InitializeOnLoad]
    public class ValidationSuite
    {
        // List of validation tests
        private IEnumerable<BaseValidation> validationTests;

        // Delegate called after every test to provide immediate feedback on single test results.
        private SingleTestCompletedDelegate singleTestCompletionDelegate;

        // Delegate called after the test run completed, whether it succeeded, failed or got canceled.
        private AllTestsCompletedDelegate allTestsCompletedDelegate;

        // Vetting context
        internal readonly VettingContext context;
        private readonly ValidationSuiteReport report;

        internal TestState testSuiteState;

        internal DateTime StartTime;

        internal DateTime EndTime;

        public ValidationSuite()
        {
        }

        internal ValidationSuite(SingleTestCompletedDelegate singleTestCompletionDelegate,
                                 AllTestsCompletedDelegate allTestsCompletedDelegate,
                                 VettingContext context,
                                 ValidationSuiteReport report)
        {
            this.singleTestCompletionDelegate += singleTestCompletionDelegate;
            this.allTestsCompletedDelegate += allTestsCompletedDelegate;
            this.context = context;
            this.report = report;
            testSuiteState = TestState.NotRun;

            BuildTestSuite();
        }

        internal IEnumerable<BaseValidation> ValidationTests
        {
            get { return validationTests.Where(test => (test.SupportedValidations.Contains(context.ValidationType) && test.SupportedPackageTypes.Contains(context.PackageType))); }
            set { validationTests = value; }
        }

        internal IEnumerable<IValidationTestResult> ValidationTestResults
        {
            get { return validationTests.Cast<IValidationTestResult>(); }
        }

        /// <summary>
        /// Validate a package for the given validation context.
        /// </summary>
        /// <param name="packageId">Package Id in the format of [package name]@[package version].</param>
        /// <param name="validationType">The type of validation to assess.</param>
        /// <returns>True if the validation successfully completed.</returns>
        public static bool ValidatePackage(string packageId, ValidationType validationType)
        {
            if (string.IsNullOrEmpty(packageId))
                throw new ArgumentNullException(packageId);

            var packageIdParts = packageId.Split('@');

            if (packageIdParts.Length != 2)
                throw new ArgumentException("Malformed package Id " + packageId);

            return ValidatePackage(packageIdParts[0], packageIdParts[1], validationType);
        }

        /// <summary>
        /// Validate a package for the given validation context.
        /// </summary>
        /// <param name="packageName">The name of the package to validate.</param>
        /// <param name="packageVersion">The version of the package to validate.</param>
        /// <param name="validationType">The type of validation to assess.</param>
        /// <returns>True if the validation successfully completed.</returns>
        public static bool ValidatePackage(string packageName, string packageVersion, ValidationType validationType)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentNullException(packageName);

            if (string.IsNullOrEmpty(packageVersion))
                throw new ArgumentNullException(packageVersion);

            var packageId = Utilities.CreatePackageId(packageName, packageVersion);
            var packagePath = FindPackagePath(packageName);
            var report = new ValidationSuiteReport(packageId, packageName, packageVersion, packagePath);

            if (string.IsNullOrEmpty(packagePath))
            {
                report.OutputErrorReport(string.Format("Unable to find package \"{0}\" on disk.", packageName));
                return false;
            }

            // publish locally for embedded and local packages
            var context = VettingContext.CreatePackmanContext(packageId, validationType);
            return ValidatePackage(context, out report);
        }

        public static bool ValidatePackage(string packageName, string packageVersion, string[] packageIdsForPromotion)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentNullException(packageName);

            if (string.IsNullOrEmpty(packageVersion))
                throw new ArgumentNullException(packageVersion);

            var packageId = Utilities.CreatePackageId(packageName, packageVersion);
            var packagePath = FindPackagePath(packageName);
            var report = new ValidationSuiteReport(packageId, packageName, packageVersion, packagePath);

            if (string.IsNullOrEmpty(packagePath))
            {
                report.OutputErrorReport(string.Format("Unable to find package \"{0}\" on disk.", packageName));
                return false;
            }

            // publish locally for embedded and local packages
            var context = VettingContext.CreatePackmanContext(packageId, ValidationType.Promotion);
            context.packageIdsForPromotion = packageIdsForPromotion;
            return ValidatePackage(context, out report);
        }

        public static bool ValidatePackage(VettingContext context, out ValidationSuiteReport report)
        {
            Profiler.BeginSample("ValidatePackage");

            report = new ValidationSuiteReport(context.ProjectPackageInfo.Id, context.ProjectPackageInfo.name, context.ProjectPackageInfo.version, context.ProjectPackageInfo.path);

            try
            {
                // publish locally for embedded and local packages
                var testSuite = new ValidationSuite(SingleTestCompletedDelegate, AllTestsCompletedDelegate, context, report);

                report.Initialize(testSuite.context);
                testSuite.RunSync();
                Profiler.EndSample();
                return testSuite.testSuiteState == TestState.Succeeded;
            }
            catch (Exception e)
            {
                report.OutputErrorReport(string.Format("Test Setup Error: \"{0}\"\r\n", e));
                Profiler.EndSample();
                return false;
            }
        }

#if UNITY_2020_1_OR_NEWER
        [Obsolete("Providing validationType to ValidatePackage has been deprecated, please set validationType in VettingContext (UnityUpgradable) -> !1", false)]
#endif
        public static bool ValidatePackage(VettingContext context, ValidationType validationType, out ValidationSuiteReport report) => ValidatePackage(context, out report);

        internal static void ValidateEmbeddedPackages(ValidationType validationType)
        {
            var packageIdList = new List<string>();
            var directories = LongPathUtils.Directory.GetDirectories("Packages/", "*", SearchOption.TopDirectoryOnly);
            foreach (var directory in directories)
            {
                ActivityLogger.Log("Starting package validation for " + directory);
                packageIdList.Add(VettingContext.GetManifest(directory).Id);
            }

            if (packageIdList.Any())
            {
                var success = ValidatePackages(packageIdList, validationType);
                ActivityLogger.Log("Package validation done and batchmode is set. Shutting down Editor");
                EditorApplication.Exit(success ? 0 : 1);
            }
            else
            {
                EditorApplication.Exit(1);
            }
        }

        internal static bool RunAssetStoreValidationSuite(string packageName, string packageVersion, string packagePath, string previousPackagePath = null)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentNullException(packageName);

            if (string.IsNullOrEmpty(packageVersion))
                throw new ArgumentNullException(packageVersion);

            if (string.IsNullOrEmpty(packagePath))
                throw new ArgumentNullException(packageName);

            var report = new ValidationSuiteReport(packageName + "@" + packageVersion, packageName, packageVersion, packagePath);

            try
            {
                var context = VettingContext.CreateAssetStoreContext(packageName, packageVersion, packagePath, previousPackagePath);
                var testSuite = new ValidationSuite(SingleTestCompletedDelegate, AllTestsCompletedDelegate, context, report);
                testSuite.RunSync();
                return testSuite.testSuiteState == TestState.Succeeded;
            }
            catch (Exception e)
            {
                report.OutputErrorReport(string.Format("\r\nTest Setup Error: \"{0}\"\r\n", e));
                return false;
            }
        }

        /// <summary>
        /// Get the validation suite report for the given package.
        /// </summary>
        /// <param name="packageName">Package name.</param>
        /// <param name="packageVersion">Package version.</param>
        /// <returns>The validation suite report as a string.</returns>
        public static string GetValidationSuiteReport(string packageName, string packageVersion)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentNullException(packageName);

            if (string.IsNullOrEmpty(packageVersion))
                throw new ArgumentNullException(packageVersion);

            var packageId = Utilities.CreatePackageId(packageName, packageVersion);
            return GetValidationSuiteReport(packageId);
        }

        /// <summary>
        /// Get the validation suite report for the given package id.
        /// </summary>
        /// <param name="packageId">Package Id in the format of [package name]@[package version].</param>
        /// <returns>The validation suite report as a string.</returns>
        public static string GetValidationSuiteReport(string packageId)
        {
            if (string.IsNullOrEmpty(packageId))
                throw new ArgumentNullException(packageId);

            return ValidationSuiteReport.ReportExists(packageId) ? File.ReadAllText(TextReport.ReportPath(packageId)) : null;
        }

        internal void RunSync()
        {
            Profiler.BeginSample("RunSync");
            foreach (var test in validationTests)
            {
                test.Context = context;
                test.Suite = this;

                Profiler.BeginSample(test.TestName + ".setup");
                test.Setup();
                Profiler.EndSample();
            }

            Run();

            Profiler.EndSample();
        }

        static bool ValidatePackages(IEnumerable<string> packageIds, ValidationType validationType)
        {
            var success = true;
            foreach (var packageId in packageIds)
            {
                var result = ValidatePackage(packageId, validationType);
                if (result)
                {
                    ActivityLogger.Log("Validation succeeded for " + packageId);
                }
                else
                {
                    success = false;
                    ActivityLogger.Log("Validation failed for " + packageId);
                }
            }

            return success;
        }

        void BuildTestSuite()
        {
            Profiler.BeginSample("BuildTestSuite");

            // Use reflection to discover all Validation Tests in the project with base type == BaseValidation.
            List<BaseValidation> testList = new List<BaseValidation>();
            Assembly[] currentDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in currentDomainAssemblies)
            {
                try
                {
                    testList.AddRange((from t in Utilities.GetTypesSafe(assembly)
                                       where typeof(BaseValidation).IsAssignableFrom(t) && t.GetConstructor(Type.EmptyTypes) != null && !t.IsAbstract
                                       select (BaseValidation)Activator.CreateInstance(t)).ToList());
                }
                catch (System.Reflection.ReflectionTypeLoadException)
                {
                    // There seems to be an isue with assembly.GetTypes throwing an exception.
                    // This quick fix is to allow validation suite to work without blocking anyone
                    // while the owner of this code is contacted.
                    continue;
                }
            }

            validationTests = testList;

            Profiler.EndSample();
        }

        // Call all static methods with a given attribute type passing them the vetting context
        void CallSuiteHandler(Type handlerAttributeType)
        {
            Profiler.BeginSample("CallSuiteHandler." + handlerAttributeType.Name);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type type in Utilities.GetTypesSafe(assembly))
                    {
                        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).Where(methodInfo => methodInfo.GetCustomAttributes(handlerAttributeType, false).Length > 0))
                        {
                            ParameterInfo[] methodParameters = method.GetParameters();
                            if ((method.ReturnType != typeof(void)) || (methodParameters.Length != 1) || (methodParameters[0].ParameterType != typeof(VettingContext)))
                                throw new InvalidOperationException("Method '" + type.Name + "." + method.Name + "' with attribute [" + handlerAttributeType.Name + "] has the incorrect prototype, it must be \"void XXX(VettingContext)\"");

                            Profiler.BeginSample("CallSuiteHandler." + type.Name + "." + method.Name);
                            method.Invoke(null, new object[] { context });
                            Profiler.EndSample();
                        }
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                }
            }

            Profiler.EndSample();
        }

        void Run()
        {
            Profiler.BeginSample("Run");

            testSuiteState = TestState.Succeeded;
            StartTime = DateTime.Now;
            testSuiteState = TestState.Running;

            // Let each suite know we are about to start running tests so they can do setup if necessary.
            CallSuiteHandler(typeof(ValidationSuiteSetup));

            // Run through tests
            foreach (var test in ValidationTests)
            {
                if (!test.ShouldRun)
                    continue;

                try
                {
                    Profiler.BeginSample(test.TestName + ".run");
                    test.RunTest();
                    Profiler.EndSample();

                    if (test.TestState == TestState.Failed)
                    {
                        testSuiteState = TestState.Failed;
                    }

                    // Signal single test results to caller.
                    singleTestCompletionDelegate(test);
                }
                catch (Exception ex)
                {
                    // if the test didn't behave, return an error.
                    testSuiteState = TestState.Failed;

                    // Change the test outcome.
                    test.AddError(ex.ToString());
                    singleTestCompletionDelegate(test);
                }
            }

            // Let each suite know we have finished running tests so they can do tidy up if necessary
            CallSuiteHandler(typeof(ValidationSuiteTeardown));

            EndTime = DateTime.Now;
            if (testSuiteState != TestState.Failed)
                testSuiteState = TestState.Succeeded;

            // when we're done, signal the main thread and all other interested
            allTestsCompletedDelegate(this, testSuiteState);

            Profiler.EndSample();
        }

        /// <summary>
        /// Find out if the validation suite report exists for the given package id.
        /// </summary>
        /// <param name="packageId">Package Id in the format of [package name]@[package version].</param>
        /// <returns>True if the validation suite report exists.</returns>
        public static bool ReportExists(string packageId)
        {
            return ValidationSuiteReport.ReportExists(packageId);
        }

        /// <summary>
        /// Find out if the validation suite report exists for the given package id.
        /// </summary>
        /// <param name="packageId">Package Id in the format of [package name]@[package version].</param>
        /// <returns>True if the validation suite report exists.</returns>
        public static bool JsonReportExists(string packageId)
        {
            return ValidationSuiteReport.JsonReportExists(packageId);
        }

        /// <summary>
        /// Get the validation suite report for the given package id.
        /// </summary>
        /// <param name="packageId">Package Id in the format of [package name]@[package version].</param>
        /// <returns>The validation suite report.</returns>
        public static ValidationSuiteReportData GetReport(string packageId)
        {
            return ValidationSuiteReport.GetReport(packageId);
        }

        static string FindPackagePath(string packageId)
        {
            var path = string.Format("Packages/{0}/package.json", packageId);
            var absolutePath = Path.GetFullPath(path);
            return !LongPathUtils.File.Exists(absolutePath) ? string.Empty : Directory.GetParent(absolutePath).FullName;
        }

        static void SingleTestCompletedDelegate(IValidationTestResult testResult)
        {
        }

        static void AllTestsCompletedDelegate(ValidationSuite suite, TestState testRunState)
        {
            suite.report.GenerateTextReport(suite);
            suite.report.GenerateJsonReport(suite);
            suite.report.GenerateVettingReport(suite);
        }
    }
}
