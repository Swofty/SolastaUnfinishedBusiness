﻿using System.Diagnostics;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using UnityEngine;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterRaceDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSavingThrowAffinitys;
using Resources = SolastaUnfinishedBusiness.Properties.Resources;

namespace SolastaUnfinishedBusiness.Races;

internal static class ObsidianDwarfSubraceBuilder
{
    internal static CharacterRaceDefinition SubraceObsidianDwarf { get; } = BuildObsidianDwarf();

    [NotNull]
    private static CharacterRaceDefinition BuildObsidianDwarf()
    {
        var obsidianDwarfSpriteReference = Sprites.GetSprite("ObsidianDwarf", Resources.ObsidianDwarf, 1024, 512);

        var attributeModifierObsidianDwarfStrengthAbilityScoreIncrease = FeatureDefinitionAttributeModifierBuilder
            .Create("AttributeModifierObsidianDwarfStrengthAbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Strength, 1)
            .AddToDB();

        var damageAffinityObsidianDwarfFireResistance = FeatureDefinitionDamageAffinityBuilder
            .Create("DamageAffinityObsidianDwarfFireResistance")
            .SetGuiPresentation(Category.Feature)
            .SetDamageAffinityType(DamageAffinityType.Resistance)
            .SetDamageType(DamageTypeFire)
            .AddToDB();

        var proficiencyObsidianDwarfLanguages = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyObsidianDwarfLanguages")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Language, "Language_Common", "Language_Dwarvish", "Language_Giant")
            .AddToDB();

        var obsidianDwarfRacePresentation = Dwarf.RacePresentation.DeepCopy();

        obsidianDwarfRacePresentation.preferedSkinColors = new RangedInt(52, 57);
        obsidianDwarfRacePresentation.preferedHairColors = new RangedInt(25, 29);

        obsidianDwarfRacePresentation.femaleNameOptions = DwarfHill.RacePresentation.FemaleNameOptions;
        obsidianDwarfRacePresentation.maleNameOptions = DwarfHill.RacePresentation.MaleNameOptions;

        var raceObsidianDwarf = CharacterRaceDefinitionBuilder
            .Create(DwarfHill, "RaceObsidianDwarf")
            .SetGuiPresentation(Category.Race, obsidianDwarfSpriteReference)
            .SetRacePresentation(obsidianDwarfRacePresentation)
            .SetFeaturesAtLevel(1,
                attributeModifierObsidianDwarfStrengthAbilityScoreIncrease,
                damageAffinityObsidianDwarfFireResistance,
                proficiencyObsidianDwarfLanguages)
            .AddToDB();

        raceObsidianDwarf.subRaces.Clear();
        Dwarf.SubRaces.Add(raceObsidianDwarf);

        return raceObsidianDwarf;
    }
}
