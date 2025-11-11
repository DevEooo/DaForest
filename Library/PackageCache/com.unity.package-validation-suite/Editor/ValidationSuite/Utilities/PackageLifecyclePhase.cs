using System;
using System.Reflection;
using Semver;

namespace UnityEditor.PackageManager.ValidationSuite.Utils
{
    [Flags]
    internal enum LifecyclePhase
    {
        InvalidVersionTag = 0,
        Preview = 2,
        Experimental = 4,
        PreRelease = 8,
        ReleaseCandidate = 16,
        Release = 32
    }

    internal static class PackageLifecyclePhase
    {
        internal static bool IsRCForThisEditor(string packageName, VettingContext context)
        {
            // RC is a joined state between a package and an editor version (package a is a candidate for editor x)
            // we can detect RCs if the package has a prerelease tag, but is added to the editor
            // I can get this info only in 2021.1+
            // packageInfo.unityLifecycle.version = version field directly found in the editor manifest
            // use reflection to get the packageInfo.unityLifecycle.version field
#if UNITY_2021_1_OR_NEWER
            var packageInfo = context.GetPackageInfo(packageName);
            if (packageInfo != null)
            {
                var unityLifecycle = typeof(PackageInfo)
                    .GetField("m_UnityLifecycle", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(packageInfo);
                var unityLifecycleVersion = Type.GetType("UnityEditor.PackageManager.UnityLifecycle.UnityLifecycleInfo, UnityEditor.CoreModule, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")
                    .GetField("m_Version", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(unityLifecycle);
                return packageInfo.version == (string)unityLifecycleVersion && packageInfo.version.Contains("-");
            }

            return false;
#else
            return false;
#endif
        }

        internal static bool IsPreReleaseVersion(SemVersion version, VersionTag tag)
        {
            return tag.Tag == "pre" && version.Major > 0;
        }

        internal static bool IsExperimentalVersion(SemVersion version, VersionTag tag)
        {
            return (tag.Tag == "exp" && version.Major > 0) || version.Major == 0;
        }

        internal static bool IsReleasedVersion(SemVersion version, VersionTag tag)
        {
            return string.IsNullOrEmpty(tag.Tag) && version.Major >= 1;
        }

        internal static bool IsPreviewVersion(SemVersion version, VersionTag tag)
        {
#if UNITY_2021_1_OR_NEWER
            return version.Prerelease.Contains("preview");
#else
            return version.Prerelease.Contains("preview") || version.Major == 0;
#endif
        }

        /**
         * Use this to know which package phase or which relationship it has for a given editor. Release Candidate (RC) is not a phase. It is a status that is applied on top of the existing phase, meaning that the package is being stabilised towards release.
         * However, if you need to know only the phase of the package, then use GetLifecyclePhase
         */
        internal static LifecyclePhase GetLifecyclePhaseOrRelation(string version, string packageName, VettingContext context)
        {
            if (IsRCForThisEditor(packageName, context)) return LifecyclePhase.ReleaseCandidate;
            return GetLifecyclePhase(version);
        }

        /**
         * Use this to know with package phase the version infers
         * However, if you need to know if this package is a release candidate, then use GetLifecyclePhaseOrRelation instead
         */
        internal static LifecyclePhase GetLifecyclePhase(string version)
        {
            SemVersion semVer = SemVersion.Parse(version);
            try
            {
                VersionTag pre = VersionTag.Parse(semVer.Prerelease);

                if (IsPreviewVersion(semVer, pre)) return LifecyclePhase.Preview;
                if (IsReleasedVersion(semVer, pre)) return LifecyclePhase.Release;
                if (IsExperimentalVersion(semVer, pre)) return LifecyclePhase.Experimental;
                if (IsPreReleaseVersion(semVer, pre)) return LifecyclePhase.PreRelease;
                return LifecyclePhase.InvalidVersionTag;
            }
            catch (ArgumentException e)
            {
                if (e.Message.Contains("invalid version tag"))
                    return LifecyclePhase.InvalidVersionTag;
                throw e;
            }
        }

        internal static LifecyclePhase GetPhaseSupportedVersions(LifecyclePhase phase)
        {
            var supportedVersions = phase;
            // Set extra supported versions for other tracks
            switch (phase)
            {
                case LifecyclePhase.Release:
                    supportedVersions = LifecyclePhase.Release;
                    break;
                case LifecyclePhase.ReleaseCandidate:
                    supportedVersions = LifecyclePhase.Release | LifecyclePhase.ReleaseCandidate;
                    break;
                case LifecyclePhase.PreRelease:
                    supportedVersions = LifecyclePhase.PreRelease | LifecyclePhase.ReleaseCandidate | LifecyclePhase.Release;
                    break;
                case LifecyclePhase.Experimental:
                    supportedVersions = LifecyclePhase.Preview | LifecyclePhase.Experimental | LifecyclePhase.PreRelease | LifecyclePhase.ReleaseCandidate | LifecyclePhase.Release;
                    break;
                case LifecyclePhase.Preview:
                    supportedVersions = LifecyclePhase.Preview | LifecyclePhase.Experimental | LifecyclePhase.PreRelease | LifecyclePhase.ReleaseCandidate | LifecyclePhase.Release;
                    break;
            }

            return supportedVersions;
        }
    }
}
