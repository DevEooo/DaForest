using System;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    class DependencyVersionsCorrectlyIndicatedUS0084 : BaseStandardChecker
    {
        public override string StandardCode => "US-0084";
        public override StandardVersion Version => new StandardVersion(1, 0, 0);

        public void Check(ManifestData manifestData)
        {
            if (manifestData.PackageType != PackageType.FeatureSet)
                return;

            foreach (var dependency in manifestData.dependencies)
            {
                if (dependency.Value != "default")
                {
                    AddError($@"In package.json for a feature, dependency ""{dependency.Key}"" : ""{dependency.Value}"" needs to be set to ""default""");
                }
            }
        }
    }
}
