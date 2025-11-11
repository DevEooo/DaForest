using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal class VersionTag
    {
        public string Tag { get; private set; }
        public string Feature { get; private set; }
        public int Iteration { get; private set; }

        public VersionTag(string tag = "", string feature = "", int iteration = 0)
        {
            this.Tag = tag;
            this.Feature = feature;
            this.Iteration = iteration;
        }

        public bool IsEmpty()
        {
            return this.Tag == "" && this.Feature == "" && this.Iteration == 0;
        }

        public static VersionTag Parse(string versionTag)
        {
            var prereleaseRegex = new Regex(@"^(?<tag>\w+)(\-(?<feature>[A-Za-z0-9-]+))?(\.(?<iteration>[1-9]\d*))?$");

            if (string.IsNullOrEmpty(versionTag)) return new VersionTag();

            var match = prereleaseRegex.Match(versionTag);
            if (!match.Success)
            {
                throw new ArgumentException(String.Format("\"{0}\" is an invalid version tag", versionTag));
            }

            var iterationMatch = match.Groups["iteration"];

            var iteration = 0;
            if (iterationMatch.Success)
            {
                iteration = int.Parse(iterationMatch.Value, CultureInfo.InvariantCulture);
            }

            var tag = match.Groups["tag"].Value;
            var feature = match.Groups["feature"].Value;

            return new VersionTag(tag, feature, iteration);
        }
    }
}
