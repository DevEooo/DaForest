using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Semver;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class ChangelogExistsAndContainValidEntriesCheckUS0039 : BaseStandardChecker
    {
        public override string StandardCode => "US-0039";
        public override StandardVersion Version => new StandardVersion(1, 0, 1);

        public void Check(string packagePath, string packageVersion, ValidationType validationType)
        {
            var changeLogPath = Path.Combine(packagePath, Utilities.ChangeLogFilename);
            if (!LongPathUtils.File.Exists(changeLogPath))
            {
                AddError(string.Format("Cannot find changelog at '{0}'. Please create a '{1}' file and '{1}.meta'. {2}", changeLogPath, Utilities.ChangeLogFilename, ErrorDocumentation.GetLinkMessage(ErrorTypes.CannotFindChangelog)));
                return;
            }

            SemVersion packageJsonVersion;
            if (!SemVersion.TryParse(packageVersion, out packageJsonVersion))
            {
                AddError(string.Format("Version format '{0}' is not valid in '{1}'. Please correct the version format. {2}", packageVersion, packagePath, ErrorDocumentation.GetLinkMessage(ErrorTypes.VersionFormatIsNotValid)));
                return;
            }
            // We must strip the -build<commit> off the prerelease
            var buildInfoIndex = packageJsonVersion.Prerelease.IndexOf("build");
            if (buildInfoIndex > 0)
            {
                var cleanPrerelease = packageJsonVersion.Prerelease.Substring(0, buildInfoIndex - 1);
                packageJsonVersion = packageJsonVersion.Change(null, null, null, cleanPrerelease, null);
            }
            var packageJsonVersionNoPrelease = packageJsonVersion.Change(null, null, null, "", null);

            // We are basically searching for a string ## [Version] - YYYY-MM-DD or ## [Unreleased]
            var changeLogLineRegex = @"## \[(?<version>.*)]( - (?<date>.*))?";

            var textChangeLog = File.ReadAllLines(changeLogPath);

            // We match each line individually so we won't end up with line ending characters etc in the match
            List<Match> matches = new List<Match>();
            foreach (var line in textChangeLog)
            {
                var lineMatches = Regex.Matches(line, changeLogLineRegex);
                foreach (var match in lineMatches.Cast<Match>())
                {
                    matches.Add(match);
                }
            }
            if (matches.Count == 0)
            {
                AddError(string.Format("No valid changelog entries were found. The changelog needs to follow the https://keepachangelog.org specifications (`## [x.y.z] - YYYY-MM-DD` or `## [Unreleased]`). {0}", ErrorDocumentation.GetLinkMessage(ErrorTypes.NoValidChangelogEntriesWereFound)));
                return;
            }

            int foundIndex = -1;
            int currentIndex = 1;
            Match found = null;
            foreach (Match match in matches)
            {
                SemVersion versionInChangelog = null;
                string versionString = match.Groups["version"].ToString();
                if (versionString == "Unreleased")
                {
                    if (found == null)
                    {
                        // In case of duplicated fields etc we keep track of the first entry to so we dont just pile more errors on top
                        found = match;
                        foundIndex = currentIndex;
                    }

                    if (currentIndex != 1)
                    {
                        AddWarningWithLine(string.Format("Unreleased section has to be the first section in the Changelog but it was number {0}. Please move it to the top or remove duplicate entries. {1}", currentIndex, ErrorDocumentation.GetLinkMessage(ErrorTypes.UnreleasedSectionFirst)), match.ToString());
                    }

                    if (validationType == ValidationType.CI)
                    {
                        AddWarningWithLine(
                            string.Format(
                                "The package has an [unreleased] section in the changelog in {0}. This is accepted in CI, and internal publishing,"
                                + " but is not accepted when sharing externally with clients. Please curate your unreleased section to reflect the package version before promoting your package to production. {1}",
                                changeLogPath,
                                ErrorDocumentation.GetLinkMessage(ErrorTypes.UnreleasedNotAllowedInPromoting)
                                ),
                            match.ToString()
                        );
                    }

                    if (validationType == ValidationType.Promotion)
                    {
                        AddErrorWithLine(
                            string.Format(
                                "The package has an [unreleased] section in the changelog in {0}. This is accepted in CI, and internal publishing,"
                                + " but is not accepted when sharing externally with clients. Please curate your unreleased section to reflect the package version before promoting your package to production. {1}",
                                changeLogPath,
                                ErrorDocumentation.GetLinkMessage(ErrorTypes.UnreleasedNotAllowedInPromoting)
                                ),
                            match.ToString()
                        );
                    }

                    currentIndex++;
                    continue;
                }

                if (!SemVersion.TryParse(versionString, out versionInChangelog, true))
                {
                    AddErrorWithDeprecationFallback(string.Format("Version format '{0}' is not valid in '{1}'. Please correct the version format. {2}", match.Groups["version"].ToString(), changeLogPath, ErrorDocumentation.GetLinkMessage(ErrorTypes.VersionFormatIsNotValid)), match.ToString(), foundIndex, currentIndex);
                    currentIndex++;
                    continue;
                }

                if (found == null && (versionInChangelog == packageJsonVersion || versionInChangelog == packageJsonVersionNoPrelease))
                {
                    // In case of duplicated fields etc we keep track of the first entry to so we dont just pile more errors on top
                    found = match;
                    foundIndex = currentIndex;
                }
                DateTime date;
                string dateFormat = "yyyy-MM-dd";
                string[] dateWarningFormats = { "yyyy-MM-d", "yyyy-M-dd", "yyyy-M-d" };
                Group dateGroup = match.Groups["date"];
                string dateToCheck = null;
                if (!string.IsNullOrEmpty(dateGroup.Value))
                {
                    dateToCheck = dateGroup.ToString();
                }

                if (string.IsNullOrEmpty(dateGroup.Value))
                {
                    AddErrorWithDeprecationFallback(string.Format(
                        "Date field is missing. A date is required in each version section (except for '## [Unreleased]') in '{0}'. Expecting expecting ISO 8601 date format 'YYYY-MM-DD'. Update the date to one of the supported values. {1}",
                        changeLogPath,
                        ErrorDocumentation.GetLinkMessage(ErrorTypes.ChangelogDateIsNotValid)), match.ToString(), foundIndex, currentIndex);
                }
                else if (!DateTime.TryParseExact(dateToCheck,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out date))
                {
                    string errorString = string.Format(
                        "Date '{0}' does not follow ISO 8601 in '{1}'. Expecting format 'YYYY-MM-DD'. Update the date to one of the supported values. {2}",
                        dateToCheck, changeLogPath,
                        ErrorDocumentation.GetLinkMessage(ErrorTypes.ChangelogDateIsNotValid));
                    if (DateTime.TryParseExact(dateToCheck,
                        dateWarningFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out date))
                    {
                        AddWarningWithLine(errorString, match.ToString());
                    }
                    else
                    {
                        AddErrorWithDeprecationFallback(errorString, match.ToString(), foundIndex, currentIndex);
                    }
                }

                currentIndex++;
            }

            if (found == null)
            {
                var expected = string.Format("`## [{0}] - YYYY-MM-DD` or `## [{1}] - YYYY-MM-DD`", packageJsonVersion.ToString(), packageJsonVersionNoPrelease.ToString());
                AddError(string.Format("No changelog entry for version `{0}` was found in '{2}'. Please add or fix a section so you have a {1} section. {3}", packageJsonVersion.ToString(), expected, changeLogPath, ErrorDocumentation.GetLinkMessage(ErrorTypes.NoChangelogEntryForVersionFound)));
            }
            else if (foundIndex != 1)
            {
                AddErrorWithLine(string.Format("Found changelog entry but it was not the first entry in '{1}' (it was entry #{0}). Please rearrange your changelog with the most recent section at the top. {2}", foundIndex, changeLogPath, ErrorDocumentation.GetLinkMessage(ErrorTypes.FoundChangelogWrongPosition)), found.ToString());
            }
        }

        private void AddWarningWithLine(string message, string lineMatch)
        {
            AddWarning(string.Format("For '{0}': {1}", lineMatch, message));
        }

        private void AddErrorWithLine(string message, string lineMatch)
        {
            AddError(string.Format("For '{0}': {1}", lineMatch, message));
        }

        private void AddErrorWithDeprecationFallback(string errorString, string line, int foundIndex, int currentIndex)
        {
            // Previous implementation stopped parsing the file after it found the first valid section
            // This one continues parsing and all new things needs to be warnings so we dont block suddenly
            if (foundIndex > 0 && foundIndex != currentIndex)
            {
                AddWarningWithLine(errorString, line);
            }
            else
            {
                AddErrorWithLine(errorString, line);
            }
        }
    }
}
