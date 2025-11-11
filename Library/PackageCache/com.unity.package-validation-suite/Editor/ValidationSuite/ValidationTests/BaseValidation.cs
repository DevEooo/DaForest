using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    public abstract class BaseValidation : IValidationTest, IValidationTestResult
    {
        public ValidationType[] SupportedValidations { get; set; }

        public PackageType[] SupportedPackageTypes { get; set; }

        public ValidationSuite Suite { get; set; }

        public string TestName { get; protected set; }

        public string TestDescription { get; protected set; }

        // Category mostly used for sorting tests, or grouping in UI.
        public TestCategory TestCategory { get; protected set; }

        public IValidationTest ValidationTest { get { return this; } }

        public TestState TestState { get; set; }

        public List<ValidationTestOutput> TestOutput { get; set; }

        public List<VettingReportEntry> VettingEntries { get; set; }

        public DateTime StartTime { get; private set; }

        public DateTime EndTime { get; private set; }

        public VettingContext Context { get; set; }

        public bool ShouldRun { get; set; }

        public bool CanUseValidationExceptions { get; set; }

        public bool CanUseCompleteTestExceptions { get; set; }

        //TODO: make this abstract so it is mandatory for child classes to define it
        //during refactor is easier to have a default impl
        internal virtual List<IStandardChecker> ImplementedStandardsList
        {
            get
            {
                return new List<IStandardChecker>();
            }
        }

        protected BaseValidation()
        {
            TestState = TestState.NotRun;
            TestOutput = new List<ValidationTestOutput>();
            ShouldRun = true;
            CanUseValidationExceptions = true;
            CanUseCompleteTestExceptions = true;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            SupportedValidations = new[] { ValidationType.AssetStore, ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
            SupportedPackageTypes = new[] { PackageType.Tooling, PackageType.Template };
        }

        // This method is called synchronously during initialization,
        // and allows a test to interact with APIs, which need to run from the main thread.
        public virtual void Setup()
        {
        }

        public void RunTest()
        {
            ActivityLogger.Log("Starting validation test \"{0}\"", TestName);
            StartTime = DateTime.Now;
            try
            {
                Run();
                ConvertStandardsIssuesToTestOutput();
            }
            catch (Exception e)
            {
                if (AllTestErrorsExcepted())
                {
                    TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.ErrorMarkedWithException, Output = e.Message + e.StackTrace });
                }
                else
                {
                    throw;
                }
            }

            EndTime = DateTime.Now;
            var elapsedTime = EndTime - StartTime;
            ActivityLogger.Log("Finished validation test \"{0}\" in {1}ms", TestName, elapsedTime.TotalMilliseconds);
        }

        // This needs to be implemented for every test
        protected abstract void Run();

        protected void ConvertStandardsIssuesToTestOutput()
        {
            //TODO: figure out a better way to pass the checks output to the pipeline
            foreach (var standard in ImplementedStandardsList)
            {
                foreach (var issue in standard.IssuesFound)
                {
                    switch (issue.Type)
                    {
                        case StandardIssueType.Error:
                            AddError(issue.Message);
                            break;
                        case StandardIssueType.Warning:
                            AddWarning(issue.Message);
                            break;
                        case StandardIssueType.Info:
                            AddInformation(issue.Message);
                            break;
                        default:
                            throw new Exception("unknown issue type");
                    }
                }
            }
        }

        public void AddError(string message, params object[] args)
        {
            AddError(string.Format(message, args));
        }

        public void AddWarning(string message, params object[] args)
        {
            AddWarning(string.Format(message, args));
        }

        public void AddInformation(string message, params object[] args)
        {
            AddInformation(string.Format(message, args));
        }

        public void AddError(string message)
        {
            // let's check for Validation Exceptions.
            // #1 - Specific error exception
            if (CanUseValidationExceptions && Context.ValidationExceptionManager.IsErrorException(TestName, message, Context.PublishPackageInfo.version))
            {
                TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.ErrorMarkedWithException, Output = message });
            }
            // #2 - All test errors excepted
            else if (AllTestErrorsExcepted())
            {
                TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.ErrorMarkedWithException, Output = message });
            }
            else
            {
                TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.Error, Output = message });
                TestState = TestState.Failed;
            }
        }

        public void AddWarning(string message)
        {
            // let's check for Validation Exceptions.
            // #1 - Specific warning exception
            if (Context.ValidationExceptionManager.IsWarningException(TestName, message, Context.PublishPackageInfo.version))
            {
                TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.WarningMarkedWithException, Output = message });
            }
            // #2 - All test warnings excepted
            else if (AllTestWarningsExcepted())
            {
                TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.WarningMarkedWithException, Output = message });
            }
            else
            {
                TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.Warning, Output = message });
            }
        }

        protected void AddInformation(string message)
        {
            TestOutput.Add(new ValidationTestOutput() { Type = TestOutputType.Information, Output = message });
        }

        protected void AddVettingEntry(VettingReportEntryType type, string entry)
        {
            VettingEntries.Add(new VettingReportEntry() { Type = type, Entry = entry });
        }

        protected void AddPromotionConditionalError(string message)
        {
            if (Context.ValidationType == ValidationType.Promotion)
                AddError(message);
            else
                AddWarning(message);
        }

        protected void AddUnityAuthoredConditionalError(ManifestData manifest, string message)
        {
            if (manifest.IsAuthoredByUnity())
                AddError(message);
            else
                AddWarning(message);
        }

        private bool AllTestErrorsExcepted()
        {
            return CanUseCompleteTestExceptions &&
                Context.ValidationExceptionManager.IsErrorException(TestName, Context.PublishPackageInfo.version);
        }

        private bool AllTestWarningsExcepted()
        {
            return Context.ValidationExceptionManager.IsWarningException(TestName, Context.PublishPackageInfo.version);
        }
    }
}
