﻿using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;

namespace SolastaCommunityExpansion.Multiclass.CustomDefinitions
{
    internal static class SkillProficiencyPointPoolSkills
    {
        internal static readonly FeatureDefinitionPointPool PointPoolBardSkillPointsMulticlass = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolBardSkillPointsMulticlass", "a69b2527569b4893abe57ad1f80e97ed")
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&BardSkillsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Arcana,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.History,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Medecine,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.Religion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth,
                SkillDefinitions.Survival
            )
            .AddToDB();

        internal static readonly FeatureDefinitionPointPool PointPoolRangerSkillPointsMulticlass = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRangerSkillPointsMulticlass", "096e4e01b52b490e807cf8d458845aa5")
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&RangerSkillsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Athletics,
                SkillDefinitions.Insight,
                SkillDefinitions.Investigation,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival,
                SkillDefinitions.Stealth
            )
            .AddToDB();

        internal static readonly FeatureDefinitionPointPool PointPoolRogueSkillPointsMulticlass = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRogueSkillPointsMulticlass", "451259da8c5c41f4b1b363f00b01be4e")
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&RogueSkillPointsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth
            )
            .AddToDB();
    }
}
