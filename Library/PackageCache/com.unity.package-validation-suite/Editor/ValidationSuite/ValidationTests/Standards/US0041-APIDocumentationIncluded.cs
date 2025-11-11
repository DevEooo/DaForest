using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    class APIDocumentationIncludedUS0041 : BaseStandardChecker
    {
        public override string StandardCode => "US-0041";
        public override StandardVersion Version => new StandardVersion(2, 1, 1);

        public void Check(string packagePath, AssemblyInfo[] assemblyInfo, ValidationAssemblyInformation validationAssemblyInformation)
        {
            var monopath = Utilities.GetMonoPath();
            var exePath = Path.GetFullPath("packages/com.unity.package-validation-suite/Bin~/FindMissingDocs/FindMissingDocs.exe");

            List<string> excludePaths = new List<string>();
            excludePaths.AddRange(LongPathUtils.Directory.GetDirectories(packagePath, "*~", SearchOption.AllDirectories));
            excludePaths.AddRange(LongPathUtils.Directory.GetDirectories(packagePath, ".*", SearchOption.AllDirectories));
            excludePaths.AddRange(LongPathUtils.Directory.GetDirectories(packagePath, "Tests", SearchOption.AllDirectories));
            foreach (var assembly in assemblyInfo)
            {
                //exclude sources from test assemblies explicitly. Do not exclude entire directories, as there may be nested public asmdefs
                if (validationAssemblyInformation.IsTestAssembly(assembly) && assembly.assemblyKind == AssemblyInfo.AssemblyKind.Asmdef)
                    excludePaths.AddRange(assembly.assembly.sourceFiles);
            }
            string responseFileParameter = string.Empty;
            string responseFilePath = null;
            if (excludePaths.Count > 0)
            {
                responseFilePath = Path.GetTempFileName();
                var excludedPathsParameter = $@"--excluded-paths=""{string.Join(",", excludePaths.Select(s => Path.GetFullPath(s)))}""";
                File.WriteAllText(responseFilePath, excludedPathsParameter);
                responseFileParameter = $@"--response-file=""{responseFilePath}""";
            }

            var filterYamlParameter = "";
            var filterYamlPath = Path.Combine(packagePath, "Documentation~", "filter.yml");
            if (Utilities.FileExists(filterYamlPath))
            {
                filterYamlParameter = $@"--path-to-filter-yaml=""{filterYamlPath}""";
            }

            var startInfo = new ProcessStartInfo(monopath, $@"""{exePath}"" --root-path=""{packagePath}"" {filterYamlParameter} {responseFileParameter}")
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var process = Process.Start(startInfo);

            var stdout = new ProcessOutputStreamReader(process, process.StandardOutput);
            var stderr = new ProcessOutputStreamReader(process, process.StandardError);
            process.WaitForExit();
            var stdoutLines = stdout.GetOutput();
            var stderrLines = stderr.GetOutput();
            if (process.ExitCode != 0)
            {
                // If FindMissingDocs fails and returns a non-zero exit code (like an unhandled exception) it means that
                // we couldn't validate the XmdDocValidation because the result is inconclusive. For that reason, we
                // should add it as an error to be addressed by the developer. If there's any bug with the tool itself
                // then that will need to be addressed in the XmlDoc repo and rebuild the binaries from PVS.
                AddError($"FindMissingDocs.exe returned {process.ExitCode}, a non-zero exit code and XmlDocValidation test is inconclusive.");
            }
            if (stderrLines.Length > 0)
            {
                AddWarning($"Internal Error running FindMissingDocs. Output:\n{string.Join("\n", stderrLines)}");
                return;
            }

            if (stdoutLines.Length > 0)
            {
                var errorMessage = FormatErrorMessage(stdoutLines);
                AddWarning(errorMessage);

                //// JonH: Enable errors in non-preview packages once the check has been put through its paces and the change is coordinated with RM and PM
                // if (Context.ProjectPackageInfo.IsPreview)
                //     Warning(errorMessage);
                // else
                // {
                //     TestState = TestState.Failed;
                //     Error(errorMessage);
                // }
            }

            if (responseFilePath != null)
                File.Delete(responseFilePath);
        }

        public static string FormatErrorMessage(IEnumerable<string> expectedMessages)
        {
            return $@"The following APIs are missing documentation: {string.Join(Environment.NewLine, expectedMessages)}";
        }
    }
}
