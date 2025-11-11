using System.Collections.Generic;
using System.Text;

namespace UnityEditor.PackageManager.ValidationSuite
{
    public enum ErrorTypes
    {
        CannotFindChangelog,
        VersionFormatIsNotValid,
        NoValidChangelogEntriesWereFound,
        ChangelogDateIsNotValid,
        NoChangelogEntryForVersionFound,
        FoundChangelogWrongPosition,
        UnreleasedSectionFirst,
        UnreleasedNotAllowedInPromoting,
        LicenseFileMissing,
        InvalidLifecycleV2
    }

    public static class ErrorDocumentation
    {
        class ErrorLink
        {
            public string FilePath { get; set; }
            public string FileSection { get; set; }
        }

        static Dictionary<ErrorTypes, ErrorLink> _links = new Dictionary<ErrorTypes, ErrorLink>
        {
            {ErrorTypes.CannotFindChangelog, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "cannot-find-changelog"}},
            {ErrorTypes.VersionFormatIsNotValid, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "version-format-is-not-valid"}},
            {ErrorTypes.NoValidChangelogEntriesWereFound, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "no-valid-changelog-entries-were-found"}},
            {ErrorTypes.ChangelogDateIsNotValid, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "date-is-not-valid"}},
            {ErrorTypes.NoChangelogEntryForVersionFound, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "no-changelog-entry-for-version-was-found"}},
            {ErrorTypes.FoundChangelogWrongPosition, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "found-changelog-correct-entry-but-it-was-not-the-first-entry"}},
            {ErrorTypes.UnreleasedSectionFirst, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "unreleased-section-has-to-be-the-first-section-in-the-changelog"}},
            {ErrorTypes.UnreleasedNotAllowedInPromoting, new ErrorLink { FilePath = "changelog_validation_error.html", FileSection = "unreleased-section-in-the-changelog-is-not-allowed-while-promoting"}},
            {ErrorTypes.LicenseFileMissing, new ErrorLink { FilePath = "license_validation_error.html", FileSection = "every-package-must-have-a-license-file"}},
            {ErrorTypes.InvalidLifecycleV2, new ErrorLink { FilePath = "lifecycle_validation_error.html", FileSection = "lifecycle-v2-tagging-rules"}},
        };

        public static string GetLinkMessage(ErrorTypes error)
        {
            ErrorLink errorLink;
            if (!_links.TryGetValue(error, out errorLink))
            {
                return "Further documentation for this error is unavailable. Please report this since we always want to have further documentation on all errors";
            }

            return GetLinkMessage(errorLink.FilePath, errorLink.FileSection);
        }

        public static string GetLinkMessage(string filePath, string fileSection)
        {
            var sb = new StringBuilder();
            sb.Append("Read more about this error and potential solutions at https://docs.unity3d.com/Packages/com.unity.package-validation-suite@latest/index.html?preview=1&subfolder=/manual/");
            sb.Append(filePath);
            sb.Append("#");
            sb.Append(fileSection);
            return sb.ToString();
        }
    }
}
