using System;
using System.IO;
using System.Linq;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards
{
    class AssemblyDefinitionCorrectlyNamedUS0038 : BaseStandardChecker
    {
        const string AssemblyFileDefinitionExtension = "*.asmdef";
        const string AssemblyFileDefinitionReferenceExtension = "*.asmref";
        const string CSharpScriptExtension = "*.cs";

        public override string StandardCode => "US-0038";
        public override StandardVersion Version => new StandardVersion(1, 0, 0);

        bool FindValueInArray(string[] array, string value)
        {
            var foundValue = false;
            for (int i = 0; i < array.Length && !foundValue; ++i)
            {
                foundValue = array[i] == value;
            }

            return foundValue;
        }

        void CheckAssemblyDefinitionContent(string assemblyDefinitionPath, string packagePath)
        {
            var simplifiedPath = assemblyDefinitionPath.Replace(packagePath, "{Package-Root}");
            var isInEditorFolder = assemblyDefinitionPath.IndexOf(Path.DirectorySeparatorChar + "Editor" + Path.DirectorySeparatorChar) >= 0;
            var isInTestFolder = assemblyDefinitionPath.IndexOf(Path.DirectorySeparatorChar + "Tests" + Path.DirectorySeparatorChar) >= 0;

            try
            {
                var assemblyDefinitionData = Utilities.GetDataFromJson<AssemblyDefinition>(assemblyDefinitionPath);
                var editorInIncludePlatforms = FindValueInArray(assemblyDefinitionData.includePlatforms, "Editor");

                var isTestAssembly = FindValueInArray(assemblyDefinitionData.optionalUnityReferences, "TestAssemblies") || FindValueInArray(assemblyDefinitionData.precompiledReferences, "nunit.framework.dll");

                // Assemblies in the Editor folder should not have any other platforms defined
                if (!isTestAssembly && isInEditorFolder && assemblyDefinitionData.includePlatforms.Length > 1)
                {
                    AddError($"For editor assemblies, only 'Editor' should be present in 'includePlatform' in: [{simplifiedPath}]");
                }

                // Assemblies in the Editor folder must have Editor marked as platform
                if (!isTestAssembly && isInEditorFolder && !editorInIncludePlatforms)
                {
                    AddError($"For editor assemblies, 'Editor' should be present in the includePlatform section in: [{simplifiedPath}]");
                }

                // Assemblies in the test folder must only be Test assemblies
                if (!isTestAssembly && isInTestFolder)
                {
                    AddError($"Assembly {simplifiedPath} is not a test assembly and should not be present in the Tests folder of your package");
                }
            }
            catch (Exception e)
            {
                AddError($"Can't read assembly definition {simplifiedPath}: {e.Message}");
            }
        }

        public void Check(string packageName, string packagePath)
        {
            var isValidationSuite = packageName == "com.unity.package-validation-suite";

            // filter out `ApiValidationTestAssemblies` folder as the content of the folder is for testing only.
            Func<string, bool> filterTestAssemblies = f => !(isValidationSuite && f.IndexOf("ApiValidationTestAssemblies") >= 0);

            var asmDefinitionFiles = LongPathUtils.Directory.GetFiles(packagePath, AssemblyFileDefinitionExtension, SearchOption.AllDirectories).Where(filterTestAssemblies);
            var asmDefinitionReferencefFiles = LongPathUtils.Directory.GetFiles(packagePath, AssemblyFileDefinitionReferenceExtension, SearchOption.AllDirectories).Where(filterTestAssemblies);

            // check the existence of valid asmdef file if there are c# scripts in the Editor or Tests folder
            var foldersToCheck = new string[] { "Editor", "Tests" };
            foreach (var folder in foldersToCheck)
            {
                var folderPath = Path.Combine(packagePath, folder);
                if (!LongPathUtils.Directory.Exists(folderPath))
                    continue;

                var foldersWithAsmDefinitionFile = asmDefinitionFiles.Where(f => f.IndexOf(folderPath) >= 0).Select(f => Path.GetDirectoryName(f));
                var foldersWithAsmDefinitionReferenceFile = asmDefinitionReferencefFiles.Where(f => f.IndexOf(folderPath) >= 0).Select(f => Path.GetDirectoryName(f));
                var csFiles = LongPathUtils.Directory.GetFiles(folderPath, CSharpScriptExtension, SearchOption.AllDirectories).Where(filterTestAssemblies);
                foreach (var csFile in csFiles)
                {
                    // check if the cs file is not in any folder that has asmdef file
                    if (foldersWithAsmDefinitionFile.All(f => csFile.IndexOf(f) < 0) &&
                        foldersWithAsmDefinitionReferenceFile.All(f => csFile.IndexOf(f) < 0))
                    {
                        AddError("C# script found in \"" + folder + "\" folder, but no corresponding asmdef or asmref file: " + csFile);
                    }
                }
            }

            foreach (var asmdef in asmDefinitionFiles)
                CheckAssemblyDefinitionContent(asmdef, packagePath);
        }
    }
}
