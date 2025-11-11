using System.IO;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal static class SamplesUtilities
    {
        public const string SamplesDirName = "Samples";
        public const string SamplesTildeDirName = "Samples~";

        internal struct SampleDirInfo
        {
            public bool SamplesDirExists;
            public bool SamplesTildeDirExists;
        }

        public static SampleDirInfo GetSampleDirectoriesInfo(string packagePath)
        {
            var samplesDirExists = LongPathUtils.Directory.Exists(Path.Combine(packagePath, SamplesDirName));
            var samplesTildeDirExists = LongPathUtils.Directory.Exists(Path.Combine(packagePath, SamplesTildeDirName));
            return new SampleDirInfo()
            {
                SamplesDirExists = samplesDirExists,
                SamplesTildeDirExists = samplesTildeDirExists
            };
        }
    }
}
