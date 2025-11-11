using System;
using System.Collections.Generic;
using System.IO;
using Semver;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class ReleaseValidation : BaseValidation
    {
        static readonly string k_DocsFilePath = "release_validation_error.html";

        public ReleaseValidation()
        {
            TestName = "Release Validation";
            TestDescription = "Check if this release is allowed to be published, relative to existing versions of this package in the registry.";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopmentInternal };
        }

        protected override void Run()
        {
            TestState = TestState.Succeeded;

            if (!SemVersion.TryParse(Context.PublishPackageInfo.version, out var thisVersion, true))
            {
                AddError("Failed to parse package version \"{0}\"", Context.PublishPackageInfo.version);
                return;
            }

            var lastFullReleaseVersion = SemVersion.Parse(Context.PreviousPackageInfo != null ? Context.PreviousPackageInfo.version : "0.0.0");

            // if previous version's major is greater than 0, then all is good
            // if previous version's major is 0 and trying to release between [0, 1].y.z, then all is good
            if (lastFullReleaseVersion.Major >= 1 || thisVersion.Major - lastFullReleaseVersion.Major <= 1)
            {
                return;
            }

            var message = "Invalid major version " + thisVersion + " when publishing to production registry.";
            if (lastFullReleaseVersion == "0.0.0")
            {
                message += $" There has never been a full release of this package. The major should be 0 or 1.";
            }
            else
            {
                message += "The next release cannot be more than 1 major above the latest full release (" +
                    lastFullReleaseVersion + ").";
            }

            AddWarning(message + " {0}", ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "invalid-major-release"));
        }
    }
}
