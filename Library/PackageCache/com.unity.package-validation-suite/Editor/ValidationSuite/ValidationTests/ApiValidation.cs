using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Semver;
using Unity.APIComparison.Framework.Changes;
using Unity.APIComparison.Framework.Collectors;
using UnityEditor.Compilation;
using UnityEditor.PackageManager.ValidationSuite.Utils;
using UnityEngine;

using AssemblyResolutionException = Mono.Cecil.AssemblyResolutionException;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    internal class ApiValidation : BaseAssemblyValidation
    {
        public ApiValidation()
        {
            TestName = "API Validation";
            TestDescription = "Checks public API for style and changest that conflict with Semantic Versioning.";
            TestCategory = TestCategory.ApiValidation;
            SupportedValidations = new[] { ValidationType.CI, ValidationType.LocalDevelopmentInternal, ValidationType.Promotion };
            SupportedPackageTypes = new[] { PackageType.Tooling };
        }

        public ApiValidation(ValidationAssemblyInformation validationAssemblyInformation)
            : base(validationAssemblyInformation)
        {
        }

        private AssemblyInfo[] GetAndCheckAsmdefs()
        {
            var relevantAssemblyInfo = GetRelevantAssemblyInfo();

            if (Context.ProjectPackageInfo.LifecyclePhase != LifecyclePhase.Preview && Context.ProjectPackageInfo.LifecyclePhase != LifecyclePhase.Experimental)
            {
                var previousAssemblyDefinitions = GetAssemblyDefinitionDataInFolder(Context.PreviousPackageInfo.path);
                var versionChangeType = Context.VersionChangeType;

                foreach (var assemblyInfo in relevantAssemblyInfo)
                {
                    //assembly is in the package
                    var previousAssemblyDefinition = previousAssemblyDefinitions.FirstOrDefault(ad => DoAssembliesMatch(ad, assemblyInfo.assemblyDefinition));
                    if (previousAssemblyDefinition == null)
                        continue; //new asmdefs are fine

                    var excludePlatformsDiff = string.Format("Was:\"{0}\" Now:\"{1}\"",
                        string.Join(", ", previousAssemblyDefinition.excludePlatforms),
                        string.Join(", ", assemblyInfo.assemblyDefinition.excludePlatforms));
                    if (previousAssemblyDefinition.excludePlatforms.Any(p => !assemblyInfo.assemblyDefinition.excludePlatforms.Contains(p)) &&
                        versionChangeType == VersionChangeType.Patch)
                        AddError("Removing from excludePlatfoms requires a new minor or major version. " + excludePlatformsDiff);
                    else if (assemblyInfo.assemblyDefinition.excludePlatforms.Any(p =>
                        !previousAssemblyDefinition.excludePlatforms.Contains(p)) &&
                             (versionChangeType == VersionChangeType.Patch || versionChangeType == VersionChangeType.Minor))
                        AddError("Adding to excludePlatforms requires a new major version. " + excludePlatformsDiff);

                    var includePlatformsDiff = string.Format("Was:\"{0}\" Now:\"{1}\"",
                        string.Join(", ", previousAssemblyDefinition.includePlatforms),
                        string.Join(", ", assemblyInfo.assemblyDefinition.includePlatforms));
                    if (previousAssemblyDefinition.includePlatforms.Any(p => !assemblyInfo.assemblyDefinition.includePlatforms.Contains(p)) &&
                        (versionChangeType == VersionChangeType.Patch || versionChangeType == VersionChangeType.Minor))
                        AddError("Removing from includePlatfoms requires a new major version. " + includePlatformsDiff);
                    else if (assemblyInfo.assemblyDefinition.includePlatforms.Any(p => !previousAssemblyDefinition.includePlatforms.Contains(p)))
                    {
                        if (previousAssemblyDefinition.includePlatforms.Length == 0 &&
                            (versionChangeType == VersionChangeType.Minor || versionChangeType == VersionChangeType.Patch))
                            AddError("Adding the first entry in includePlatforms requires a new major version. " + includePlatformsDiff);
                        else if (versionChangeType == VersionChangeType.Patch)
                            AddError("Adding to includePlatforms requires a new minor or major version. " + includePlatformsDiff);
                    }
                }
            }

            return relevantAssemblyInfo;
        }

        private bool DoAssembliesMatch(AssemblyDefinition assemblyDefinition1, AssemblyDefinition assemblyDefinition2)
        {
            return validationAssemblyInformation.GetAssemblyName(assemblyDefinition1, true).Equals(validationAssemblyInformation.GetAssemblyName(assemblyDefinition2, false));
        }

        private AssemblyDefinition[] GetAssemblyDefinitionDataInFolder(string directory)
        {
            return LongPathUtils.Directory.GetFiles(directory, "*.asmdef", SearchOption.AllDirectories)
                .Select(Utilities.GetDataFromJson<AssemblyDefinition>).ToArray();
        }

        protected override void Run(AssemblyInfo[] info)
        {
            TestState = TestState.Succeeded;
            var packagePath = Context.ProjectPackageInfo.path;
            var files = new HashSet<string>(LongPathUtils.Directory.GetFiles(packagePath, "*", SearchOption.AllDirectories).Select(Path.GetFullPath));

            //does it compile?
            if (EditorUtility.scriptCompilationFailed)
            {
                AddError("Compilation failed. Please fix any compilation errors.");
                return;
            }

            if (EditorApplication.isCompiling)
            {
                AddError("Compilation in progress. Please wait for compilation to finish.");
                return;
            }

            if (Context.PreviousPackageInfo == null)
            {
                AddInformation("No previous package version. Skipping Semantic Versioning checks.");
                TestState = TestState.NotRun;
                return;
            }

            //does it have asmdefs for all scripts?
            var assemblies = GetAndCheckAsmdefs();

            CheckApiDiff(assemblies);

            //Run analyzers
        }

        [Serializable]
        class AssemblyChange
        {
            public string assemblyName;
            public List<string> additions = new List<string>();
            public List<string> breakingChanges = new List<string>();

            public AssemblyChange(string assemblyName)
            {
                this.assemblyName = assemblyName;
            }
        }

#if UNITY_2019_1_OR_NEWER
        [Serializable]
        class ApiDiff
        {
            public List<string> missingAssemblies = new List<string>();
            public List<string> newAssemblies = new List<string>();
            public List<AssemblyChange> assemblyChanges = new List<AssemblyChange>();
            public int breakingChanges;
            public int additions;
            public int removedAssemblyCount;
        }
#endif

        private void CheckApiDiff(AssemblyInfo[] assemblyInfo)
        {
#if UNITY_2018_1_OR_NEWER && !UNITY_2019_1_OR_NEWER
            TestState = TestState.NotRun;
            AddInformation("Api breaking changes validation only available on Unity 2019.1 or newer.");
            return;
#else
            var diff = new ApiDiff();
            var assembliesForPackage = assemblyInfo.Where(a => !validationAssemblyInformation.IsTestAssembly(a)).ToArray();
            if (Context.PreviousPackageBinaryDirectory == null)
            {
                TestState = TestState.NotRun;
                AddInformation("Previous package binaries must be present on artifactory to do API diff.");
                return;
            }

            var oldAssemblyPaths = LongPathUtils.Directory.GetFiles(Context.PreviousPackageBinaryDirectory, "*.dll");

            //Build diff
            foreach (var info in assembliesForPackage)
            {
                var assemblyDefinition = info.assemblyDefinition;
                var oldAssemblyPath = oldAssemblyPaths.FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == assemblyDefinition.name);

                if (info.assembly != null)
                {
                    var extraSearchFolder = Path.GetDirectoryName(typeof(System.ObsoleteAttribute).Assembly.Location);
                    var assemblySearchFolder = new[]
                    {
                        extraSearchFolder, // System assemblies folder
                        Path.Combine(EditorApplication.applicationContentsPath, "Managed"), // Main Unity assemblies folder.
                        Path.Combine(EditorApplication.applicationContentsPath, "Managed/UnityEngine"), // Module assemblies folder.
                        Path.GetDirectoryName(info.assembly.outputPath), // TODO: This is not correct. We need to keep all dependencies for the previous binaries. For now, use the same folder as the current version when resolving dependencies.
                        Context.ProjectPackageInfo.path // make sure to add the package folder as well, because it may contain .dll files
                    };

                    const string logsDirectory = "Logs";
                    if (!LongPathUtils.Directory.Exists(logsDirectory))
                        Directory.CreateDirectory(logsDirectory);

                    File.WriteAllText($"{logsDirectory}/ApiValidationParameters.txt", $"previous: {oldAssemblyPath}\ncurrent: {info.assembly.outputPath}\nsearch path: {string.Join("\n", assemblySearchFolder)}");
                    var apiChangesAssemblyInfo = new APIChangesCollector.AssemblyInfo()
                    {
                        BaseAssemblyPath = oldAssemblyPath,
                        BaseAssemblyExtraSearchFolders = assemblySearchFolder,
                        CurrentAssemblyPath = info.assembly.outputPath,
                        CurrentExtraSearchFolders = assemblySearchFolder
                    };

                    List<IAPIChange> entityChanges;
                    try
                    {
                        entityChanges = APIChangesCollector.Collect(apiChangesAssemblyInfo)
                            .SelectMany(c => c.Changes).ToList();
                    }
                    catch (AssemblyResolutionException exception)
                    {
                        if (exception.AssemblyReference.Name == "UnityEditor.CoreModule" ||
                            exception.AssemblyReference.Name == "UnityEngine.CoreModule")
                        {
                            AddError(
                                "Failed comparing against assemblies of previously promoted version of package. \n" +
                                "This is most likely because the assemblies that were compared against were built with a different version of Unity. \n" +
                                "If you are certain that there are no API changes warranting bumping the package version then you can add an exception for this error:\n" +
                                ErrorDocumentation.GetLinkMessage("validation_exceptions.html", ""));
                            AddInformation($"APIChangesCollector.Collect threw exception:\n{exception}");
                            return;
                        }

                        throw;
                    }

                    var assemblyChange = new AssemblyChange(info.assembly.name)
                    {
                        additions = entityChanges.Where(c => c.IsAdd()).Select(c => c.ToString()).ToList(),
                        // Among all attribute changes, only the Obsolete attribute should be considered a breaking change
                        breakingChanges = entityChanges.Where(c => !c.IsAdd() && !((c.GetType()).Equals(typeof(AttributeChange)))).Select(c => c.ToString()).ToList()
                    };

                    if (entityChanges.Count > 0)
                        diff.assemblyChanges.Add(assemblyChange);
                }

                if (oldAssemblyPath == null)
                    diff.newAssemblies.Add(assemblyDefinition.name);
            }

            foreach (var oldAssemblyPath in oldAssemblyPaths)
            {
                var oldAssemblyName = Path.GetFileNameWithoutExtension(oldAssemblyPath);
                if (assembliesForPackage.All(a => a.assemblyDefinition.name != oldAssemblyName))
                    diff.missingAssemblies.Add(oldAssemblyName);
            }

            //separate changes
            diff.additions = diff.assemblyChanges.Sum(v => v.additions.Count);
            diff.removedAssemblyCount = diff.missingAssemblies.Count;
            diff.breakingChanges = diff.assemblyChanges.Sum(v => v.breakingChanges.Count);

            AddInformation("Tested against version {0}", Context.PreviousPackageInfo.Id);
            AddInformation("API Diff - Breaking changes: {0} Additions: {1} Missing Assemblies: {2}",
                diff.breakingChanges,
                diff.additions,
                diff.removedAssemblyCount);

            if (diff.breakingChanges > 0 || diff.additions > 0)
            {
                TestOutput.AddRange(diff.assemblyChanges.Select(c => new ValidationTestOutput() { Type = TestOutputType.Information, Output = JsonUtility.ToJson(c, true) }));
            }

            string json = JsonUtility.ToJson(diff, true);
            Directory.CreateDirectory(ValidationSuiteReport.ResultsPath);
            File.WriteAllText(Path.Combine(ValidationSuiteReport.ResultsPath, "ApiValidationReport.json"), json);

            //Figure out type of version change (patch, minor, major)
            //Error if changes are not allowed
            var changeType = Context.VersionChangeType;

            if (changeType == VersionChangeType.Unknown)
                return;

            if (Context.ProjectPackageInfo.LifecyclePhase == LifecyclePhase.Preview || Context.ProjectPackageInfo.LifecyclePhase == LifecyclePhase.Experimental)
                ExperimentalPackageValidateApiDiffs(diff, changeType);
            else
                ReleasePackageValidateApiDiffs(diff, changeType);
#endif
        }

#if UNITY_2019_1_OR_NEWER

        private void ExperimentalPackageValidateApiDiffs(ApiDiff diff, VersionChangeType changeType)
        {
            if (diff.breakingChanges > 0 && changeType == VersionChangeType.Patch)
                AddError("For Experimental or Preview Packages, breaking changes require a new minor version.");

            if (changeType == VersionChangeType.Patch)
            {
                foreach (var assembly in diff.missingAssemblies)
                {
                    AddError("Assembly \"{0}\" no longer exists or is no longer included in build. For Experimental or Preview Packages, this change requires a new minor version.", assembly);
                }
            }
        }

        private void ReleasePackageValidateApiDiffs(ApiDiff diff, VersionChangeType changeType)
        {
            if (diff.breakingChanges > 0 && changeType != VersionChangeType.Major)
                AddError("Breaking changes require a new major version.");
            if (diff.additions > 0 && changeType == VersionChangeType.Patch)
                AddError("Additions require a new minor or major version.");
            if (changeType != VersionChangeType.Major)
            {
                foreach (var assembly in diff.missingAssemblies)
                {
                    AddError("Assembly \"{0}\" no longer exists or is no longer included in build. This change requires a new major version.", assembly);
                }
            }
            if (changeType == VersionChangeType.Patch)
            {
                foreach (var assembly in diff.newAssemblies)
                {
                    AddError("New assembly \"{0}\" may only be added in a new minor or major version.", assembly);
                }
            }
        }

#endif
    }
}
