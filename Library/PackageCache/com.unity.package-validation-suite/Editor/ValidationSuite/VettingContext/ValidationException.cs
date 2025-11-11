using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEditor.PackageManager.ValidationSuite
{
    [Serializable]
    class ValidationException
    {
        /// <summary>
        /// Name of validaiton test in which the exception is requested.
        /// </summary>
        public string ValidationTest;

        /// <summary>
        /// Error for which the exception is requested.
        /// </summary>
        public string ExceptionMessage;

        /// <summary>
        /// Package Version
        /// </summary>
        public string PackageVersion;
    }

    [Serializable]
    class ValidationExceptions
    {
        public ValidationException[] ErrorExceptions;
        public ValidationException[] WarningExceptions;
    }
}
