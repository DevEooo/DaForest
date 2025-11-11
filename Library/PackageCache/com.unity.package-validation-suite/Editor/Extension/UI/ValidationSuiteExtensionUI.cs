using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests.Standards;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace UnityEditor.PackageManager.ValidationSuite.UI
{
#if UNITY_2019_1_OR_NEWER
    internal class ValidationSuiteExtensionUI : VisualElement
    {
        private const string PackagePath = "Packages/com.unity.package-validation-suite/";
        private const string ResourcesPath = PackagePath + "Editor/Resources/";
        private const string TemplatePath = ResourcesPath + "Templates/ValidationSuiteTools.uxml";
        private const string DarkStylePath = ResourcesPath + "Styles/Dark.uss";
        private const string LightStylePath = ResourcesPath + "Styles/Light.uss";

        private VisualElement root;

        private PopupField<string> validationPopupField;

        private List<string> _validationChoices = ValidationTypeDropdown.ToList();
        public List<string> ValidationChoices
        {
            get => _validationChoices;
            private set => _validationChoices = value;
        }

        private PackageNamingConventionUS0006 _namingConventionUs0006 = new PackageNamingConventionUS0006();
        private PackageInfo CurrentPackageinfo { get; set; }
        private string PackageId { get; set; }

        public static ValidationSuiteExtensionUI CreateUI()
        {
            var asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TemplatePath);
            return asset == null ? null : new ValidationSuiteExtensionUI(asset);
        }

        private ValidationSuiteExtensionUI(VisualTreeAsset asset)
        {
            root = asset.CloneTree();
            string path = EditorGUIUtility.isProSkin ? DarkStylePath : LightStylePath;
            var styleSheet = EditorGUIUtility.Load(path) as StyleSheet;
            root.styleSheets.Add(styleSheet);
            Add(root);

            validationPopupField = new PopupField<string>("", ValidationChoices, 0);
            root.Q<VisualElement>("ValidationTypeDropdown").Add(validationPopupField);

            ValidateButton.clickable.clicked += Validate;
            ViewResultsButton.clickable.clicked += ViewResults;
            ViewDiffButton.clickable.clicked += ViewDiffs;
        }

        static bool PackageAvailable(PackageInfo packageInfo)
        {
#if UNITY_2019_2_OR_NEWER
            var installedPackageInfo = PackageInfo.FindForAssetPath($"Packages/{packageInfo.name}");
            return installedPackageInfo != null && installedPackageInfo.version == packageInfo.version;
#else
            return packageInfo.status == PackageStatus.Available || packageInfo.status == PackageStatus.Error;
#endif
        }

        public static bool SourceSupported(PackageInfo info)
        {
            PackageSource source = info.source;
#if UNITY_2019_3_OR_NEWER
            // Tarball is available here only, so check if its a tarball and return true
            if (source == PackageSource.LocalTarball) return true;
#endif

            return source == PackageSource.Embedded
                || source == PackageSource.Local
                || source == PackageSource.Registry
                || source == PackageSource.Git
                || (info.source == PackageSource.BuiltIn && info.type != "module");
        }

        public void OnPackageSelectionChange(PackageInfo packageInfo)
        {
            if (root == null)
                return;

            var showValidationUI = packageInfo != null && PackageAvailable(packageInfo) && SourceSupported(packageInfo);
            UIUtils.SetElementDisplay(this, showValidationUI);
            if (!showValidationUI)
                return;

            CurrentPackageinfo = packageInfo;
            PackageId = CurrentPackageinfo.name + "@" + CurrentPackageinfo.version;
            ValidationResults.text = string.Empty;

            AddRemoveUnitySpecificValidations(NamePrefixEligibleForUnityStandardsOptions(CurrentPackageinfo.name));

            validationPopupField.value = NamePrefixEligibleForUnityStandardsOptions(CurrentPackageinfo.name) ? ValidationTypeDropdown.UnityProductionStandardsLabelText : ValidationTypeDropdown.StructureLabelText;

            UIUtils.SetElementDisplay(ViewResultsButton, ValidationSuiteReport.ReportExists(PackageId));
            UIUtils.SetElementDisplay(ViewDiffButton, ValidationSuiteReport.DiffsReportExists(PackageId));

            root.style.backgroundColor = Color.gray;
        }

        public void AddRemoveUnitySpecificValidations(bool showUnityStandards)
        {
            ValidationChoices.Clear();

            ValidationChoices.Add(ValidationTypeDropdown.StructureLabelText);
            ValidationChoices.Add(ValidationTypeDropdown.AssetStoreLabelText);

            if (showUnityStandards)
            {
                ValidationChoices.Add(ValidationTypeDropdown.UnityCandidatesStandardsLabelText);
                ValidationChoices.Add(ValidationTypeDropdown.UnityProductionStandardsLabelText);
            }
        }

        private void Validate()
        {
            if (root == null)
                return;

            if (Utilities.NetworkNotReachable)
            {
                EditorUtility.DisplayDialog("", "Validation suite requires network access and cannot be used offline.", "Ok");
                return;
            }

            var validationType = ValidationTypeDropdown.ValidationTypeFromDropdown(validationPopupField.value, CurrentPackageinfo.source);

            var results = ValidationSuite.ValidatePackage(PackageId, validationType);
            var report = ValidationSuiteReport.GetReport(PackageId);

            UIUtils.SetElementDisplay(ViewResultsButton, ValidationSuiteReport.ReportExists(PackageId));
            UIUtils.SetElementDisplay(ViewDiffButton, ValidationSuiteReport.DiffsReportExists(PackageId));

            if (!results)
            {
                ValidationResults.text = "Failed";
                root.style.backgroundColor = Color.red;
            }
            else if (report != null && report.Tests.Any(t => t.TestOutput.Any(o => o.Type == TestOutputType.Warning)))
            {
                ValidationResults.text = "Warnings";
                root.style.backgroundColor = Color.yellow;
            }
            else
            {
                ValidationResults.text = "Success";
                root.style.backgroundColor = Color.green;
            }
        }

        private void ViewResults()
        {
            var filePath = TextReport.ReportPath(PackageId);
            try
            {
                try
                {
                    var targetFile = Directory.GetCurrentDirectory() + "/" + filePath;
                    if (!LongPathUtils.File.Exists(targetFile))
                        throw new Exception("Validation Result not found!");

                    Process.Start(targetFile);
                }
                catch (Exception)
                {
                    var data = File.ReadAllText(filePath);
                    EditorUtility.DisplayDialog("Validation Results", data, "Ok");
                }
            }
            catch (Exception)
            {
                EditorUtility.DisplayDialog("Validation Results", "Results are missing", "Ok");
            }
        }

        private void ViewDiffs()
        {
            if (ValidationSuiteReport.DiffsReportExists(PackageId))
            {
                Application.OpenURL("file://" + Path.GetFullPath(ValidationSuiteReport.DiffsReportPath(PackageId)));
            }
        }

        public bool NamePrefixEligibleForUnityStandardsOptions(string packageName)
        {
            return _namingConventionUs0006.GetPackageNamePrefixList()
                .Any(packageName.StartsWith);
        }

        internal Label ValidationResults { get { return root.Q<Label>("validationResults");} }

        internal Button ValidateButton { get { return root.Q<Button>("validateButton"); } }

        internal Button ViewResultsButton { get { return root.Q<Button>("viewResults"); } }

        internal Button ViewDiffButton { get { return root.Q<Button>("viewdiff"); } }
    }
#endif
}
