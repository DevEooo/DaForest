using System;
using System.Collections.Generic;
using System.Linq;
using Semver;
using UnityEditor.PackageManager.ValidationSuite.Utils;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class LifecycleValidation : BaseValidation
    {
        readonly CorrectPackageVersionTagsUS0017 correctPackageVersionTagsUS0017 = new CorrectPackageVersionTagsUS0017();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            correctPackageVersionTagsUS0017
        };

        internal static readonly string k_DocsFilePath = "lifecycle_validation_error.html";

        public LifecycleValidation()
        {
            TestName = "Package Lifecycle Validation";
            TestDescription = "Validate that the package respects the lifecycle transition guidelines.";
            TestCategory = TestCategory.DataValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopment, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion, ValidationType.VerifiedSet };
        }

        protected override void Run()
        {
            TestState = TestState.Succeeded;

            if (Context.PublishPackageInfo.lifecycle == 1.0)
            {
                ValidateVersion(Context.PublishPackageInfo, LifecycleV1VersionValidator);
            }
            else
            {
                ValidateVersion(Context.PublishPackageInfo, correctPackageVersionTagsUS0017.Check);

                // We don't check this on the verified set since it relies on previous package information and reaching the internet
                if (Context.PublishPackageInfo.LifecyclePhase == LifecyclePhase.PreRelease &&
                    Context.ValidationType != ValidationType.VerifiedSet)
                    PreReleaseChecks(Context.ProjectPackageInfo);
            }

            /*
             * TODO: this test is producing false failures, we need to rework it.
             * Disabling so we don't block people trying to get into trunk, and to workaround they are adding full exemption to their packages
             * We have guards further in the trunk katana process that will guard against packages depending on other packages outside the correct set.
             * We have identified 2 challenges for making this check work in package CI:
             *  - A new package version will never be part of the EditorManifest, so we need to tell PVS to pretend that this package will be RC
             *  - Same as above, but for a set of related/dependent packages (think monorepos, or project context)
            */
            //ValidateDependenciesLifecyclePhase(Context.ProjectPackageInfo.dependencies);
        }

        private void ValidateVersion(ManifestData manifestData, Action<SemVersion, VersionTag> lifecycleVersionValidator)
        {
            // Check package version, make sure it's a valid SemVer string.
            SemVersion packageVersionNumber;
            if (!SemVersion.TryParse(manifestData.version, out packageVersionNumber))
            {
                AddError("In package.json, \"version\" needs to be a valid \"Semver\". {0}", ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "version-needs-to-be-a-valid-semver"));
                return;
            }

            VersionTag versionTag;

            try
            {
                versionTag = VersionTag.Parse(packageVersionNumber.Prerelease);
            }
            catch (ArgumentException e)
            {
                AddError("In package.json, \"version\" doesn't follow our lifecycle rules. {0}. {1}", e.Message, ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "version-is-invalid-tag-must-follow-lifecycle-rules"));
                return;
            }

            lifecycleVersionValidator(packageVersionNumber, versionTag);
            ValidateVersionAbilityToPromote(packageVersionNumber, versionTag, manifestData);
        }

        private void LifecycleV1VersionValidator(SemVersion packageVersionNumber, VersionTag versionTag)
        {
            if (Context.IsCore && (!versionTag.IsEmpty() || packageVersionNumber.Major < 1))
            {
                AddError("Core packages cannot be preview. " + ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "core-packages-cannot-be-preview"));
                return;
            }

            if (packageVersionNumber.Major < 1 && (versionTag.IsEmpty() || versionTag.Tag != "preview"))
            {
                AddError("In package.json, \"version\" < 1, please tag the package as " + packageVersionNumber.VersionOnly() + "-preview. " + ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "version-1-please-tag-the-package-as-xxx-preview"));
                return;
            }

            // The only pre-release tag we support is -preview
            if (!versionTag.IsEmpty() && !(versionTag.Tag == "preview" && versionTag.Feature == "" &&
                                           versionTag.Iteration <= 999999))
            {
                AddError(
                    "In package.json, \"version\": the only pre-release filter supported is \"-preview.[num < 999999]\". " + ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "version-the-only-pre-release-filter-supported-is--preview-num-999999"));
            }
        }

        private void ValidateDependenciesLifecyclePhase(Dictionary<string, string> dependencies)
        {
            // No dependencies, exit early
            if (!dependencies.Any()) return;

            // Extract the current track, since otherwise we'd be potentially parsing the version
            // multiple times
            var currentTrack = PackageLifecyclePhase.GetLifecyclePhaseOrRelation(Context.ProjectPackageInfo.version, Context.ProjectPackageInfo.name, Context);

            var supportedVersions = PackageLifecyclePhase.GetPhaseSupportedVersions(currentTrack);

            // Check each dependency against supported versions
            foreach (var dependency in dependencies)
            {
                // Skip invalid dependencies from this check
                SemVersion depVersion;
                if (!SemVersion.TryParse(dependency.Value, out depVersion)) continue;

                LifecyclePhase dependencyTrack = PackageLifecyclePhase.GetLifecyclePhaseOrRelation(dependency.Value.ToLower(), dependency.Key.ToLower(), Context);
                var depId = Utilities.CreatePackageId(dependency.Key, dependency.Value);
                if (!supportedVersions.HasFlag(dependencyTrack))
                    AddError($"Package {Context.ProjectPackageInfo.Id} depends on package {depId} which is in an invalid track for release purposes. {currentTrack} versions can only depend on {supportedVersions.ToString()} versions. {ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "package-depends-on-a-package-which-is-in-an-invalid-track-for-release-purposes")}");
            }
        }

        /**
         * Can't promote if it's a release or RC
         * Can promote -preview, -pre (unless it is the first one, but that is verified somewhere else) and -exp
         * Error in Promotion
         * Only warn when in CI
         */
        private void ValidateVersionAbilityToPromote(SemVersion packageVersionNumber, VersionTag versionTag, ManifestData manifestData)
        {
            // Make this check only in promotion, to avoid network calls
            if (Context.PackageVersionExistsOnProduction)
            {
                AddPromotionConditionalError("Version " + Context.ProjectPackageInfo.version + " of this package already exists in production.");
            }

            var message = String.Empty;
            if (PackageLifecyclePhase.IsReleasedVersion(packageVersionNumber, versionTag) ||
                PackageLifecyclePhase.IsRCForThisEditor(manifestData.name, Context))
            {
                message = $"Automated promotion of Release or Release Candidate packages is not allowed. Release Management are the only ones that can promote Release and Release Candidate packages to production, if you need this to happen, please go to #devs-pkg-promotion. {ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "a-release-package-must-be-manually-promoted-by-release-management")}";
            }
            else
            {
                // We send a message if this is the first version of the package being promoted
                if (!Context.PackageExistsOnProduction)
                    message = $"{Context.PublishPackageInfo.name} has never been promoted to production before. Please contact Release Management through slack in #devs-pkg-promotion to promote the first version of your package before trying to use this automated pipeline. {ErrorDocumentation.GetLinkMessage(k_DocsFilePath, "the-very-first-version-of-a-package-must-be-promoted-by-release-management")}";
            }

            if (message != String.Empty)
                AddPromotionConditionalError(message);
        }

        private void PreReleaseChecks(ManifestData currentManifest)
        {
            string errorMsg = String.Empty;
            if (Context.AllVersions != null)
            {
                SemVersion pkgVersion = SemVersion.Parse(currentManifest.version);
                // Get all versions that match x.y.z (so, any exp, preview, prerelease, etc).
                List<SemVersion> relatedVersions = pkgVersion.GetRelatedVersions(Context.AllVersions);

                // Get only the related previous versions
                SemVersion prevVersion = relatedVersions.Where(v => v <= pkgVersion).ToList().LastOrDefault();

                if (prevVersion != null)
                {
                    if (!PackageLifecyclePhase.IsPreReleaseVersion(prevVersion, VersionTag.Parse(prevVersion.Prerelease)))
                    {
                        errorMsg = string.Format(
                            "The previous version of this package ({0}) is not a Pre-Release. By Lifecycle V2 rules, a Pre-Release package can only be promoted automatically to production when the previous version is also a Pre-Release version. {1}",
                            prevVersion.ToString(),
                            ErrorDocumentation.GetLinkMessage(k_DocsFilePath,
                                "previous-version-of-this-package-is-not-a-pre-release-version"));
                    }
                    else
                    {
                        SemVersion lastPreVersion = relatedVersions.Where(v =>
                        {
                            VersionTag t = VersionTag.Parse(v.Prerelease);
                            return PackageLifecyclePhase.IsPreReleaseVersion(v, t);
                        }).ToList().LastOrDefault();

                        if (lastPreVersion != null)
                        {
                            VersionTag lastTag = VersionTag.Parse(lastPreVersion.Prerelease);
                            VersionTag pkgTag = VersionTag.Parse(pkgVersion.Prerelease);
                            if (pkgTag.Iteration <= lastTag.Iteration)
                            {
                                errorMsg = string.Format(
                                    "This package iteration ({0}) must be higher than the highest published iteration ({1}). Please update your package version to {2} {3}",
                                    pkgTag.Iteration,
                                    lastTag.Iteration,
                                    string.Format("{0}-{1}.{2}", pkgVersion.VersionOnly(), pkgTag.Tag,
                                        lastTag.Iteration + 1),
                                    ErrorDocumentation.GetLinkMessage(k_DocsFilePath,
                                        "this-package-iteration-(x)-must-be-higher-than-the-highest-published-iteration-(y)"));
                            }
                        }
                    }
                }
                else
                {
                    errorMsg = string.Format(
                        "There is no previous Pre-Release version of this package available. By Lifecycle V2 rules, the first Pre-Release iteration of a new version needs to be approved and promoted by Release Management. Please contact Release Management to promote your package. {0}",
                        ErrorDocumentation.GetLinkMessage(k_DocsFilePath,
                            "previous-version-of-this-package-is-not-a-pre-release-version"));
                }
            }

            if (errorMsg != String.Empty)
                AddPromotionConditionalError(errorMsg);
        }
    }
}
