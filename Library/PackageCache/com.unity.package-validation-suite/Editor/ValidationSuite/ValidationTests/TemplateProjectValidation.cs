using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.ValidationSuite.Utils;
using UnityEngine;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;

namespace UnityEditor.PackageManager.ValidationSuite.ValidationTests
{
    /*
     * TODO: the description of this standard does not strictly match its implementation
     * To be precise, the current check is not checking the dependencies of the templates. It checks if the project setting "Enable Preview Packages" or "Enable Pre release packages" are enabled or not.
     * One of the side effects is that the template cannot have dependencies on preview o pre release packages, but it also means that a project created from a template will by default not allow the user to add an pre-release package as well.
     */

    internal class PackageManagerSettingsValidation
    {
        public bool m_EnablePreviewPackages;
        public bool m_EnablePreReleasePackages;
    }

    internal class TemplateProjectValidation : BaseValidation
    {
        private readonly string[] _allowedFields =
        {
            "dependencies"
        };

        internal static readonly string k_DocsFilePath = "template_project_validation_errors.html";

        readonly TemplateUsesOnlyReleasedPackagesUS0188 templateUsesOnlyReleasedPackagesUs0188 = new TemplateUsesOnlyReleasedPackagesUS0188();
        readonly AssetFolderNamingUS0061 assetFolderNamingUS0061 = new AssetFolderNamingUS0061();
        readonly FolderStructureConventionsUS0063 folderStructureConventionsUs0063 = new FolderStructureConventionsUS0063();
        readonly AssetNamingUS0057 assetNamingUS0057 = new AssetNamingUS0057();

        internal override List<IStandardChecker> ImplementedStandardsList => new List<IStandardChecker>
        {
            templateUsesOnlyReleasedPackagesUs0188,
            assetFolderNamingUS0061,
            folderStructureConventionsUs0063,
            assetNamingUS0057
        };

        public TemplateProjectValidation()
        {
            TestName = "Template Project Manifest and Assets Validation";
            TestDescription = "Validate that the project manifest and assets included in a template package follow standards";
            TestCategory = TestCategory.DataValidation;
            SupportedPackageTypes = new[] { PackageType.Template };
        }

        protected override void Run()
        {
            TestState = TestState.Succeeded;

            ValidateProjectManifest();
            templateUsesOnlyReleasedPackagesUs0188.Check(Context.ProjectInfo.PackageManagerSettingsValidation);
            assetFolderNamingUS0061.Check(Context.PublishPackageInfo.path);
            folderStructureConventionsUs0063.Check(Context.PublishPackageInfo.path);
            assetNamingUS0057.Check(Context.PublishPackageInfo.path);
        }

        // Generate a standard error message for project manifest field checks. This is also used during tests
        internal string CreateFieldErrorMessage(string fieldName)
        {
            string docsLink = ErrorDocumentation.GetLinkMessage(k_DocsFilePath,
                "The-{fieldName}-field-in-the-project-manifest-is-not-a-valid-field-for-template-packages");

            return $"The `{fieldName}` field in the project manifest is not a valid field for template packages. Please remove this field from {Context.ProjectInfo.ManifestPath}. {docsLink}";
        }

        private void ValidateProjectManifest()
        {
            foreach (string fieldName in Context.ProjectInfo.ProjectManifestKeys)
            {
                if (_allowedFields.Contains(fieldName))
                    continue;

                AddError(CreateFieldErrorMessage(fieldName));
            }
        }
    }
}
