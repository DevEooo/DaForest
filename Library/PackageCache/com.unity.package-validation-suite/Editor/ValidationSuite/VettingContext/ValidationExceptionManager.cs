using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace UnityEditor.PackageManager.ValidationSuite
{
    /// <summary>
    /// Class that manages the validation exceptions for this package.
    /// </summary>
    public class ValidationExceptionManager
    {
        private const string ValidationExceptionsFileName = "ValidationExceptions.json";

        private const string TestExceptionPrefix = "__**Test-";

        private Dictionary<string, ValidationException> errorExceptionDictionary;
        private Dictionary<string, ValidationException> warningExceptionDictionary;

        /// <summary>
        /// Will return true if the package contains 1 or more validation error exceptions
        /// </summary>
        public bool HasErrorExceptions
        {
            get { return errorExceptionDictionary.Any(); }
        }

        /// <summary>
        /// Will return true if the package contains 1 or more validation warning exceptions
        /// </summary>
        public bool HasWarningExceptions
        {
            get { return warningExceptionDictionary.Any(); }
        }

        /// <summary>
        /// Will return true if the package contains 1 or more validation exceptions
        /// </summary>
        public bool HasExceptions => HasErrorExceptions || HasWarningExceptions;

        internal IEnumerable<ValidationException> ErrorExceptionsList
        {
            get { return errorExceptionDictionary.Values; }
        }

        internal IEnumerable<ValidationException> WarningExceptionsList
        {
            get { return warningExceptionDictionary.Values; }
        }

        /// <summary>
        /// Constructor for the Validation Exception Manager
        /// </summary>
        /// <param name="packagePath">Path that contains the exception file.</param>
        public ValidationExceptionManager(string packagePath)
        {
            errorExceptionDictionary = new Dictionary<string, ValidationException>();
            warningExceptionDictionary = new Dictionary<string, ValidationException>();
            var filePath = Path.Combine(packagePath, ValidationExceptionsFileName);

            if (LongPathUtils.File.Exists(filePath))
            {
                var exceptions = Utilities.GetDataFromJson<ValidationExceptions>(filePath);

                // If there is an exception error specified, let's use it.
                // If there isn't one specified, this is a test level exception, all errors for that test should be flagged as exceptions.
                if (exceptions.ErrorExceptions != null)
                    errorExceptionDictionary = exceptions.ErrorExceptions.ToDictionary(ex => string.IsNullOrWhiteSpace(ex.ExceptionMessage) ? (TestExceptionPrefix + ex.ValidationTest) : ex.ExceptionMessage);

                // If there is an exception warning specified, let's use it.
                // If there isn't one specified, this is a test level exception, all warning for that test should be flagged as exceptions.
                if (exceptions.WarningExceptions != null)
                    warningExceptionDictionary = exceptions.WarningExceptions.ToDictionary(ex => string.IsNullOrWhiteSpace(ex.ExceptionMessage) ? (TestExceptionPrefix + ex.ValidationTest) : ex.ExceptionMessage);

                // Handle backwards compatibility for old format (renamed fields)
                // Sadly JsonUtility.FromJson doesn't support the FormerlySerializedAs attribute (case 1119033)
                MergeWithOldFormat(filePath);
            }
        }

        void MergeWithOldFormat(string filePath)
        {
            var oldExceptions = Utilities.GetDataFromJson<OldValidationExceptions>(filePath);
            if (oldExceptions.Exceptions != null)
            {
                foreach (var oldException in oldExceptions.Exceptions)
                {
                    var errorException = new ValidationException
                    {
                        ValidationTest = oldException.ValidationTest,
                        ExceptionMessage = oldException.ExceptionError,
                        PackageVersion = oldException.PackageVersion,
                    };

                    var key = string.IsNullOrWhiteSpace(errorException.ExceptionMessage) ? (TestExceptionPrefix + errorException.ValidationTest) : errorException.ExceptionMessage;
                    errorExceptionDictionary.Add(key, errorException);
                }
            }
        }

        // Helper classes for ValidationExceptions.json format backwards compatibility
#pragma warning disable 0649
        [Serializable]
        class OldValidationException
        {
            public string ValidationTest;
            public string ExceptionError;
            public string PackageVersion;
        }

        [Serializable]
        class OldValidationExceptions
        {
            public OldValidationException[] Exceptions;
        }
#pragma warning restore 0649

        /// <summary>
        /// Tests whether the requested error is part of the validation exception list.
        /// </summary>
        /// <param name="validationTest">Validation test display name</param>
        /// <param name="validationMessage">Error string, verbatim</param>
        /// <param name="packageVersion">Version of the package this exception is for.</param>
        /// <returns>True if the error is part of the validation exception list.</returns>
        public bool IsErrorException(string validationTest, string validationMessage, string packageVersion)
        {
            ValidationException validationException;
            if (errorExceptionDictionary.TryGetValue(validationMessage, out validationException))
            {
                if (validationException.ValidationTest == validationTest &&
                    validationException.PackageVersion == packageVersion)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests whether the requested warning is part of the validation exception list.
        /// </summary>
        /// <param name="validationTest">Validation test display name</param>
        /// <param name="validationMessage">Warning string, verbatim</param>
        /// <param name="packageVersion">Version of the package this exception is for.</param>
        /// <returns>True if the warning is part of the validation exception list.</returns>
        public bool IsWarningException(string validationTest, string validationMessage, string packageVersion)
        {
            ValidationException validationException;
            if (warningExceptionDictionary.TryGetValue(validationMessage, out validationException))
            {
                if (validationException.ValidationTest == validationTest &&
                    validationException.PackageVersion == packageVersion)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests whether a test class has been exceptioned completely with respect to errors.
        /// </summary>
        /// <param name="validationTest">Validation test display name</param>
        /// <param name="packageVersion">Version of the package this exception is for.</param>
        /// <returns>True if the error is part of the validation exception list.</returns>
        public bool IsErrorException(string validationTest, string packageVersion)
        {
            ValidationException validationException;
            if (errorExceptionDictionary.TryGetValue(TestExceptionPrefix + validationTest, out validationException))
            {
                if (validationException.PackageVersion == packageVersion)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tests whether a test class has been exceptioned completely with respect to warnings.
        /// </summary>
        /// <param name="validationTest">Validation test display name</param>
        /// <param name="packageVersion">Version of the package this exception is for.</param>
        /// <returns>True if the warning is part of the validation exception list.</returns>
        public bool IsWarningException(string validationTest, string packageVersion)
        {
            ValidationException validationException;
            if (warningExceptionDictionary.TryGetValue(TestExceptionPrefix + validationTest, out validationException))
            {
                if (validationException.PackageVersion == packageVersion)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks the validity of the validation exception list.
        /// </summary>
        /// <param name="packageVersion">Package version used for comparison.</param>
        /// <returns> Returns a list of issues found with the exception list.  That list will be empty when no issues are found.</returns>
        public IEnumerable<string> CheckValidationExceptions(string packageVersion)
        {
            var formatString = "The following {0} was tagged as an exception to validation, but for a different version of the package. " +
                $"Please consider getting this exception fixed and removed from \"{ValidationExceptionsFileName}\" before updating its package version field.\r\n" +
                "    \"{1}\" - \"{2}\"";

            List<string> issuesList = new List<string>();
            foreach (var validationException in ErrorExceptionsList)
            {
                // Validate that all error exceptions that were used had the right package version.
                if (validationException.PackageVersion != packageVersion)
                {
                    issuesList.Add(string.Format(formatString, "error", validationException.ValidationTest, validationException.ExceptionMessage));
                }
            }
            foreach (var validationException in WarningExceptionsList)
            {
                // Validate that all warning exceptions that were used had the right package version.
                if (validationException.PackageVersion != packageVersion)
                {
                    issuesList.Add(string.Format(formatString, "warning", validationException.ValidationTest, validationException.ExceptionMessage));
                }
            }

            return issuesList;
        }
    }
}
