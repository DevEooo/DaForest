using System;
using System.Text.RegularExpressions;
using Semver;

namespace UnityEditor.PackageManager.ValidationSuite
{
    static class UnityVersion
    {
        const string k_Major = @"(?<major>\d{1,4})";
        const string k_Minor = @"(?<minor>[1-9])";
        const string k_Patch = @"(?<update>\d|[1-9]\d*)";
        const string k_Build = @"(?<buildtype>[abfp])(?<buildnumber>[1-9]\d*)(-.*)?";
        static readonly string k_UnityVersionPattern = $@"^{k_Major}\.{k_Minor}(\.{k_Patch}({k_Build})?)?$";

        // Converts a Unity version (2017.3, 2018.1.0a1, etc.) into a SemVersion instance.
        // Throws on invalid Unity version syntax.
        //
        // Ported from upm-core:
        // https://github.cds.internal.unity3d.com/unity/upm-core/blob/2a5c988/lib/private/utilities/unity.js
        internal static SemVersion Parse(string unityVersion)
        {
            var match = Regex.Match(unityVersion, k_UnityVersionPattern);
            if (!match.Success)
                throw new ArgumentException($"{nameof(unityVersion)} is not a valid Unity version: {unityVersion}");

            var major = int.Parse(match.Groups["major"].Value);
            var minor = int.Parse(match.Groups["minor"].Value);
            var patch = match.Groups["update"].Success ? int.Parse(match.Groups["update"].Value) : 0;
            var prerelease = match.Groups["buildtype"].Success ?
                $"{match.Groups["buildtype"].Value}.{match.Groups["buildnumber"].Value}" :
                "a.1";

            return new SemVersion(major, minor, patch, prerelease, string.Empty);
        }
    }
}
