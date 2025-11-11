using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite
{
    internal class CIUtils
    {
        internal const string UpmCIUtilsId = "upm-ci-utils@stable";

        internal static string GetCIUtilsScript()
        {
            var persistentDataPath = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "../../Unity"));
            var upmTemplateUtilsPath = Path.Combine(persistentDataPath, UpmCIUtilsId);
            var buildScript = Path.Combine(upmTemplateUtilsPath, "node_modules/upm-ci-utils/index.js");

            if (LongPathUtils.File.Exists(buildScript))
                return buildScript;

            if (!LongPathUtils.Directory.Exists(upmTemplateUtilsPath))
                Directory.CreateDirectory(upmTemplateUtilsPath);

            var launcher = new NodeLauncher();
            launcher.NpmLogLevel = "error";
            launcher.NpmRegistry = NodeLauncher.BintrayNpmRegistryUrl;
            launcher.WorkingDirectory = upmTemplateUtilsPath;
            launcher.NpmPrefix = ".";

            try
            {
                launcher.NpmInstall(UpmCIUtilsId);
            }
            catch (ApplicationException exception)
            {
                exception.Data["code"] = "installFailed";
                throw exception;
            }

            return LongPathUtils.File.Exists(buildScript) ? buildScript : string.Empty;
        }

        internal static List<string> _Pack(string command, string path, string destinationPath)
        {
            //Create a copy of the package on the temp folder so that it can be modified

            var launcher = new NodeLauncher();
            launcher.WorkingDirectory = path;
            launcher.Script = GetCIUtilsScript();
            launcher.Args = command + " pack --npm-path \"" + NodeLauncher.NpmScriptPath + "\"";
            launcher.Launch();

            List<string> packagePaths = new List<string>();

            var paths = LongPathUtils.Directory.GetFiles(Path.Combine(path, "upm-ci~", "packages"), "*tgz");

            foreach (var packagePath in paths)
            {
                //Copy the file to the destinationPath
                string packageName = Path.GetFileName(packagePath);
                string finalPackagePath = Path.Combine(destinationPath, packageName);

                if (LongPathUtils.File.Exists(finalPackagePath))
                {
                    File.Delete(finalPackagePath);
                }

                File.Move(packagePath, finalPackagePath);
                packagePaths.Add(finalPackagePath);
            }

            // TODO: Remove this part when we switch to Packman API pack function
            var packagesJsonPath = Path.Combine(path, "packages.json");
            if (LongPathUtils.File.Exists(packagesJsonPath))
            {
                File.Delete(packagesJsonPath);
            }

            //See if upm-ci~ exists and remove
            if (LongPathUtils.Directory.Exists(Path.Combine(path, "upm-ci~")))
            {
                Directory.Delete(Path.Combine(path, "upm-ci~"), true);
            }

            return packagePaths;
        }
    }
}
