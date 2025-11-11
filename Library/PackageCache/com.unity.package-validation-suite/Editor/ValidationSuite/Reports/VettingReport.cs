using System.IO;

namespace UnityEditor.PackageManager.ValidationSuite
{
    public enum VettingReportEntryType
    {
        GeneralConcerns,

        LegalConcerns,

        SecurityConcerns,
    }

    public class VettingReportEntry
    {
        public VettingReportEntryType Type { get; set; }

        public string Entry { get; set; }
    }

    internal class VettingReport
    {
        internal static readonly string ResultsPath = Path.Combine("Library", "VettingReport");

        public VettingReport()
        {
        }

        public void GenerateReport(ValidationSuite suite)
        {
        }
    }
}

/*
- Is this the first version of the package.
- Were we able to evaluate APIs by fetching libraries.
- Does the package contain a 3rd Party Notice file.
- Does the oackage call out to a service.
- Does the package reference a package other than com.unity.
- Does the package contain dlls or exes
- Does the package contain new dlls
*/
