using System;
using System.Collections.Generic;
using UnityEditorInternal;

namespace UnityEditor.PackageManager.ValidationSuite.UI
{
    public class ValidationTypeDropdown
    {
        public const string StructureLabelText = "Structure";
        public const string AssetStoreLabelText = "Against Asset Store standards";
        public const string UnityCandidatesStandardsLabelText = "Against Unity candidates standards";
        public const string UnityProductionStandardsLabelText = "Against Unity production standards";

        public static List<string> ToList()
        {
            var listOfChoices = new List<string>();

            foreach (var type in (ValidationType[])Enum.GetValues(typeof(ValidationType)))
            {
                string choiceText;
                switch (type)
                {
                    case ValidationType.Structure:
                        choiceText = StructureLabelText;
                        break;
                    case ValidationType.AssetStore:
                        choiceText = AssetStoreLabelText;
                        break;
                    case ValidationType.CI:
                        choiceText = UnityCandidatesStandardsLabelText;
                        break;
                    case ValidationType.Promotion:
                        choiceText = UnityProductionStandardsLabelText;
                        break;
                    default:
                        choiceText = null;
                        break;
                }

                if (choiceText != null) listOfChoices.Add(choiceText);
            }

            return listOfChoices;
        }

        public static ValidationType ValidationTypeFromDropdown(string popupFieldValue, PackageSource packageSource)
        {
            switch (popupFieldValue)
            {
                case StructureLabelText:
                    return ValidationType.Structure;
                case AssetStoreLabelText:
                    return ValidationType.AssetStore;
                case UnityCandidatesStandardsLabelText:
                    return ValidationType.CI;
                case UnityProductionStandardsLabelText:
                    return packageSource == PackageSource.Registry ? ValidationType.Promotion : ValidationType.LocalDevelopmentInternal;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
