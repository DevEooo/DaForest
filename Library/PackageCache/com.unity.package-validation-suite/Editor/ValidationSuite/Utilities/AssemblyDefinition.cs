namespace UnityEditor.PackageManager.ValidationSuite
{
    internal class AssemblyDefinition
    {
        public string name = "";
        public string[] references = new string[0];
        public string[] optionalUnityReferences = new string[0];
        public string[] includePlatforms = new string[0];
        public string[] excludePlatforms = new string[0];
        public string[] precompiledReferences = new string[0];
        public string[] defineConstraints = new string[0];
    }
}
