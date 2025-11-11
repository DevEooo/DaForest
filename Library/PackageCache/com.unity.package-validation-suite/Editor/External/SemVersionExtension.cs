using System.Collections.Generic;
using System.Linq;

namespace Semver
{
    internal static class SemVersionExtension
    {
        public static string VersionOnly(this SemVersion version)
        {
            return "" + version.Major + "." + version.Minor + "." + version.Patch;
        }

        public static string ShortVersion(this SemVersion version)
        {
            var versionStr = "" + version.Major + "." + version.Minor;
            if (!string.IsNullOrEmpty(version.Prerelease))
                versionStr += "-" + version.Prerelease;
            if (!string.IsNullOrEmpty(version.Build))
                versionStr += "+" + version.Build;
            return versionStr;
        }

        public static List<SemVersion> GetRelatedVersions(this SemVersion version, string[] versions)
        {
            // Convert and sort the list of versions
            List<SemVersion> sortedVersions = new List<SemVersion>();
            for (int i = 0; i < versions.Length; i++)
            {
                SemVersion semVer = SemVersion.Parse(versions[i]);
                if (semVer.VersionOnly() == version.VersionOnly())
                    sortedVersions.Add(semVer);
            }

            sortedVersions.Sort();
            return sortedVersions;
        }
    }
}
