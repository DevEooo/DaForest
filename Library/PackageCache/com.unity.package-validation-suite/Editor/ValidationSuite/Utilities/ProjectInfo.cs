using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests;
using UnityEditorInternal;
using UnityEngine.Profiling;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal class ProjectInfo
    {
        public string ManifestPath = "";
        public string PackageManagerSettingsPath = "";
        // Used for template package validation
        public string[] ProjectManifestKeys = new string[0];
        public PackageManagerSettingsValidation PackageManagerSettingsValidation = new PackageManagerSettingsValidation()
        {
            m_EnablePreviewPackages = false,
            m_EnablePreReleasePackages = false
        };

        public ProjectInfo() { }

        /// <summary>
        /// Contains project information that is relevant for Template testing. During testing, the used manifest path
        /// and package manager settings path will be defined by the existence of those files within the package's
        /// ProjectData~ folder
        /// </summary>
        /// <param name="context"></param>
        public ProjectInfo(VettingContext context)
        {
            string packedManifestPath =
                Path.Combine(context.PublishPackageInfo.path, "ProjectData~", "Packages", "manifest.json");
            string packmanSettingsPath = Path.Combine(context.PublishPackageInfo.path, "ProjectData~",
                "ProjectSettings", "PackageManagerSettings.asset");

            ManifestPath = LongPathUtils.File.Exists(packedManifestPath)
                ? packedManifestPath
                : Path.GetFullPath("Packages/manifest.json");

            PackageManagerSettingsPath = LongPathUtils.File.Exists(packmanSettingsPath)
                ? packmanSettingsPath
                : Path.GetFullPath("ProjectSettings/PackageManagerSettings.asset");

            SetProjectManifestKeys();
            SetPackageManagerSettings();
        }

        internal void SetProjectManifestKeys()
        {
            Profiler.BeginSample("GetProjectManifestKeys");

            if (!LongPathUtils.File.Exists(ManifestPath))
                throw new FileNotFoundException($"A project manifest file could not be found in {ManifestPath}");

            var contents = File.ReadAllText(ManifestPath);

            ProjectManifestKeys = ParseFirstLevelKeys(contents);

            Profiler.EndSample();
        }

        internal void SetPackageManagerSettings()
        {
            if (!LongPathUtils.File.Exists(PackageManagerSettingsPath))
                return; // If no settings are saved then we just do nothing

            // This might fail in the future if the PackageManagerSettings.asset file is saved in a binary format
            // by unity, which is not happening right now.
            // If it DOES happen, we can implement a usage of the bundled tool: Binary2Text to get a string out of
            // the binarized file.
            var contents = File.ReadAllText(PackageManagerSettingsPath);

            PackageManagerSettingsValidation = new PackageManagerSettingsValidation()
            {
                m_EnablePreviewPackages = Regex.IsMatch(contents, "m_EnablePreviewPackages: 1"),
                m_EnablePreReleasePackages = Regex.IsMatch(contents, "m_EnablePreReleasePackages: 1")
            };
        }

        /// <summary>
        /// Retrieve first level keys for a given json string
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private static string[] ParseFirstLevelKeys(string json)
        {
            string minified = new Regex("[\\s]").Replace(json, "");
            var regex = new Regex("\"(\\w*)\":({[^}]*}|\\[[^]]*\\]|\"[^\"]*\"|true|false|\\d*)");

            if (!regex.IsMatch(minified)) // json is not a dictionary
                return new string[0];

            List<string> results = new List<string>();
            var matches = regex.Matches(minified).OfType<Match>().Select(m => m.Groups[1].Value).ToArray();

            return matches;
        }
    }
}
