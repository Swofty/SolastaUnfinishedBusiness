﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Models;
using SolastaMulticlass.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.GameUi.CharacterInspection
{
    [HarmonyPatch(typeof(CharacterInformationPanel), "EnumerateFeatures")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInformationPanel_EnumerateFeatures
    {
        private static bool TryFindChoiceFeature(
            CharacterInformationPanel panel,
            FeatureDefinition subFeature,
            out FeatureDefinition choiceFeature)
        {
            choiceFeature = null;

            foreach (var featureUnlock in panel.InspectedCharacter.MainClassDefinition.FeatureUnlocks)
            {
                var featureDefinition = featureUnlock.FeatureDefinition;

                if (featureDefinition is CustomFeatureDefinitionSet set && set.AllFeatures.Contains(subFeature))
                {
                    choiceFeature = featureDefinition;

                    return true;
                }
                else if (featureDefinition is FeatureDefinitionFeatureSet definitionFeatureSet
                    && definitionFeatureSet.Mode == FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion
                    && definitionFeatureSet.FeatureSet.Contains(subFeature))
                {
                    choiceFeature = featureDefinition;

                    return true;
                }
            }

            return false;
        }

        internal static bool Prefix(
            CharacterInformationPanel __instance,
            RectTransform table,
            List<FeatureUnlockByLevel> features,
            string insufficientLevelFormat,
            TooltipDefinitions.AnchorMode tooltipAnchorMode,
            GameObject ___featurePrefab)
        {
            if (!Main.Settings.EnableEnhancedCharacterInspection)
            {
                return true;
            }

            while (table.childCount < features.Count)
            {
                Gui.GetPrefabFromPool(___featurePrefab, table);
            }

            int index = 0;

            foreach (var feature in features)
            {
                var child = table.GetChild(index);

                child.gameObject.SetActive(true);

                var label = child.GetComponent<GuiLabel>();
                var noLevel = feature.Level == 0;
                var title = feature.FeatureDefinition.FormatTitle();

                label.Text = title + (!noLevel ? $" ({feature.Level})" : string.Empty);
                Gui.HexaKeyToColor(noLevel ? Gui.ColorAlmostWhite : Gui.ColorNegative, out var color);
                label.TMP_Text.color = color;

                var tooltip = child.GetComponent<GuiTooltip>();
                var provider = new CustomTooltipProvider(feature.FeatureDefinition, null);

                if (feature.FeatureDefinition is FeatureDefinitionPower)
                {
                    GuiPowerDefinition guiPowerDefinition = ServiceRepository.GetService<IGuiWrapperService>()
                        .GetGuiPowerDefinition(feature.FeatureDefinition.Name);
                    tooltip.Content = guiPowerDefinition.Description;

                    provider.SetSubtitle(guiPowerDefinition.Title);
                }
                else if (TryFindChoiceFeature(__instance, feature.FeatureDefinition, out var choiceFeature))
                {
                    label.Text = Gui.Format("{1} ({0})", choiceFeature.FormatTitle(), 
                        feature.FeatureDefinition.FormatTitle());
                    tooltip.Content = feature.FeatureDefinition.FormatDescription();

                    provider.SetSubtitle(choiceFeature.GuiPresentation.Title);
                }
                else
                {
                    tooltip.Content = feature.FeatureDefinition.FormatDescription();
                }

                tooltip.TooltipClass = "FeatDefinition";
                tooltip.DataProvider = provider;
                tooltip.Context = __instance.InspectedCharacter?.RulesetCharacter;
                tooltip.AnchorMode = tooltipAnchorMode;

                if (!noLevel)
                {
                    var levelRequirement = Gui.Format(insufficientLevelFormat, feature.Level.ToString());

                    provider.SetPrerequisites(levelRequirement);
                }
                else
                {
                    provider.SetPrerequisites("");
                }

                ++index;
            }

            for (int count = features.Count; count < table.childCount; ++count)
            {
                table.GetChild(count).gameObject.SetActive(false);
            }

            return false;
        }
    }

    // Switch positions of Class and Background descriptions, and switch description and features list in Class panel
    [HarmonyPatch(typeof(CharacterInformationPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterInformationPanel_Bind
    {
        private static Transform ClassSelector { get; set; } = null;

        private static void LoadClassSelector(RectTransform backGroup, RectTransform classGroup)
        {
            var voice = backGroup.FindChildRecursive("Voice");

            ClassSelector = Object.Instantiate(voice, classGroup.transform);
            ClassSelector.FindChildRecursive("PlayAudio").gameObject.SetActive(false);
            ClassSelector.FindChildRecursive("HeaderGroup").gameObject.SetActive(false);
        }

        internal static void Postfix(CharacterInformationPanel __instance)
        {
            if (!Main.Settings.EnableEnhancedCharacterInspection)
            {
                return;
            }

            var backGroup = __instance.transform.Find("BackgroundGroup")?.GetComponent<RectTransform>();
            var classGroup = __instance.transform.Find("ClassGroup")?.GetComponent<RectTransform>();
             
            if (classGroup != null && backGroup != null)
            {
                backGroup.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 32, 662);
                backGroup.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 32, 458);
                
                classGroup.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 32, 662);
                classGroup.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 32, 856);

                //this is actualyy top-right one
                var child = backGroup.Find("OrnamentBottomRight")?.GetComponent<RectTransform>();

                if (child != null)
                {
                    child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 5, 50);
                }
                
                child = backGroup.Find("BackgroundImageMask")?.GetComponent<RectTransform>();

                if (child != null)
                {
                    child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 218);
                }
                
                child = backGroup.Find("BackgroundDescriptionGroup")?.GetComponent<RectTransform>();

                if (child != null)
                {
                    child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 65, 175);
                }
                
                child = classGroup.Find("ClassFeaturesGroup")?.GetComponent<RectTransform>();

                if (child != null)
                {
                    child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 20, 642);
                    child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 260, 590);
                    child.sizeDelta = new Vector2(child.sizeDelta.x, child.sizeDelta.y - 100);
                }
                
                child = classGroup.Find("ClassDescriptionGroup")?.GetComponent<RectTransform>();

                if (child != null)
                {
                    child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 355);
                    child.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 270);
                }

                classGroup.FindChildRecursive("OrnamentBottomLeft")?.gameObject.SetActive(false);

                //
                // setup class buttons for MC scenarios
                //

                var hero = Global.InspectedHero;
                var classesTitles = hero.ClassesAndLevels.Select(x => x.Key.FormatTitle()).ToList();

                if (hero == null || hero.ClassesAndLevels.Count == 1)
                {
                    if (ClassSelector != null)
                    {
                        ClassSelector.gameObject.SetActive(false);
                    }

                    InspectionPanelContext.SelectedClassIndex = 0;

                    return;
                }

                if (ClassSelector == null)
                {
                    LoadClassSelector(backGroup, classGroup);
                }

                var labelsGroup = ClassSelector.FindChildRecursive("LabelsGroup");
                var classesCount = classesTitles.Count;

                for (var i = 0; i < classesCount; i++)
                {
                    var childToggle = labelsGroup.GetChild(i);
                    var labelChoiceToggle = childToggle.GetComponent<LabelChoiceToggle>();
                    var uiToggle = childToggle.GetComponent<Toggle>();

                    labelChoiceToggle.Bind(i, classesTitles[i], (x) =>
                    {
                        var screen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();

                        if (uiToggle.isOn)
                        {
                            InspectionPanelContext.SelectedClassIndex = x;
                            __instance.RefreshNow();

                            for (var i = 0; i < classesCount; ++i)
                            {
                                if (i != x)
                                {
                                    labelsGroup.GetChild(i).GetComponent<LabelChoiceToggle>().Refresh(false, true);
                                }
                            }
                        }
                    });
                }

                for (var i = classesCount; i < 3; i++)
                {
                    labelsGroup.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
