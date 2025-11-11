using System.Collections.Generic;
using System.IO;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    internal class SamplesUS0116 : BaseStandardChecker
    {
        public override string StandardCode => "US-0116";

        public override StandardVersion Version => new StandardVersion(1, 0, 0);

        public void Check(string path, List<SampleData> samples, ValidationType validationType)
        {
            var samplesDirInfo = SamplesUtilities.GetSampleDirectoriesInfo(path);
            if (samplesDirInfo.SamplesDirExists && samplesDirInfo.SamplesTildeDirExists)
            {
                AddError("`Samples` and `Samples~` cannot both be present in the package.");
            }

            if ((validationType == ValidationType.Promotion || validationType == ValidationType.VerifiedSet) && samplesDirInfo.SamplesDirExists)
            {
                AddError("In a published package, the `Samples` needs to be renamed to `Samples~`. It should have been done automatically in the CI publishing process.");
            }

            var samplesDir = samplesDirInfo.SamplesDirExists ? SamplesUtilities.SamplesDirName : SamplesUtilities.SamplesTildeDirName;
            var matchingFiles = new List<string>();
            Utilities.RecursiveDirectorySearch(Path.Combine(path, samplesDir), ".sample.json", ref matchingFiles);

            if (matchingFiles.Count == 0)
            {
                AddError(samplesDir + " folder exists but no `.sample.json` files found in it." +
                    "A `.sample.json` file is required for a sample to be recognized." +
                    "Please refer to https://github.cds.internal.unity3d.com/unity/com.unity.package-starter-kit/blob/master/Samples/Example/.sample.json for more info.");
            }

            if (validationType == ValidationType.Promotion || validationType == ValidationType.VerifiedSet)
            {
                if (samples.Count != matchingFiles.Count)
                {
                    AddError("The number of samples in `package.json` does not match the number of `.sample.json` files found in `" + samplesDir + "`.");
                }

                foreach (var sample in samples)
                {
                    if (string.IsNullOrEmpty(sample.path))
                        AddError("Sample path must be set and non-empty in `package.json`.");
                    if (string.IsNullOrEmpty(sample.displayName))
                        AddError("Sample display name will be shown in the UI, and it must be set and non-empty in `package.json`.");
                    var samplePath = Path.Combine(path, sample.path);
                    var sampleJsonPath = Path.Combine(samplePath, ".sample.json");
                    if (!LongPathUtils.Directory.Exists(samplePath))
                    {
                        AddError("Sample path set in `package.json` does not exist: " + sample.path + ".");
                    }
                    else if (!LongPathUtils.File.Exists(sampleJsonPath))
                    {
                        AddError("Cannot find `.sample.json` file in the sample path: " + sample.path + ".");
                    }
                }
            }
        }
    }
}
