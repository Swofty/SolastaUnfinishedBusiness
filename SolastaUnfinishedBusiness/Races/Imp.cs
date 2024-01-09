﻿using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using TA;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Races;

internal static class RaceImpBuilder
{
    private const string Name = "Imp";
    private const ActionDefinitions.Id ImpishWrathToggle = (ActionDefinitions.Id)ExtraActionId.ImpishWrathToggle;

    internal static CharacterRaceDefinition RaceImp { get; } = BuildImp();

    [NotNull]
    private static CharacterRaceDefinition BuildImp()
    {
        var raceImp = CharacterRaceDefinitionBuilder
            .Create(CharacterRaceDefinitions.Tiefling, $"Race{Name}")
            .SetOrUpdateGuiPresentation(Category.Race)
            .SetSizeDefinition(CharacterSizeDefinitions.Small)
            .SetFeaturesAtLevel(1,
                FeatureDefinitionMoveModes.MoveModeMove6,
                FeatureDefinitionSenses.SenseNormalVision,
                FeatureDefinitionSenses.SenseDarkvision,
                FlexibleRacesContext.FeatureSetLanguageCommonPlusOne)
            .AddToDB();

        RacesContext.RaceScaleMap[raceImp] = 7f / 9.4f;

        raceImp.subRaces = [BuildImpInfernal(raceImp), BuildImpForest(raceImp)];
        return raceImp;
    }

    #region Infernal Imp

    private static CharacterRaceDefinition BuildImpInfernal(CharacterRaceDefinition raceImp)
    {
        const string NAME = "ImpInfernal";

        var spriteReference = Sprites.GetSprite(NAME, Resources.ImpInfernal, 1024, 512);

        var featureSetImpInfernalFiendishResistance = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}FiendishResistance")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance)
            .AddToDB();

        var featureSetImpInfernalAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierDragonbornAbilityScoreIncreaseCha)
            .AddToDB();

        var castSpellImpInfernal = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellTiefling, $"CastSpell{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
            .SetSpellList(
                SpellListDefinitionBuilder
                    .Create($"SpellList{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .ClearSpells()
                    .SetSpellsAtLevel(0, SpellDefinitions.ViciousMockery)
                    .SetSpellsAtLevel(1, SpellDefinitions.Invisibility)
                    .FinalizeSpells(true, 1)
                    .AddToDB())
            .AddToDB();

        var raceImpInfernal = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{NAME}")
            .SetBaseHeight(42)
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpInfernalAbilityScoreIncrease,
                featureSetImpInfernalFiendishResistance,
                castSpellImpInfernal)
            .AddToDB();

        raceImpInfernal.racePresentation.preferedSkinColors = new RangedInt(15, 19);

        return raceImpInfernal;
    }

    #endregion

    #region Forest Imp

    private static CharacterRaceDefinition BuildImpForest(CharacterRaceDefinition raceImp)
    {
        const string NAME = "ImpForest";

        var spriteReference = Sprites.GetSprite(NAME, Resources.ImpForest, 1024, 512);

        var featureSetImpForestAbilityScoreIncrease = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}AbilityScoreIncrease")
            .SetGuiPresentation(Category.Feature)
            .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Union)
            .AddFeatureSet(
                FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                FeatureDefinitionAttributeModifiers.AttributeModifierHalfOrcAbilityScoreIncreaseCon)
            .AddToDB();

        var actionAffinityImpForestInnateCunning = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{NAME}InnateCunning")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(
                ActionDefinitions.Id.DisengageBonus,
                ActionDefinitions.Id.HideBonus)
            .AddToDB();

        var powerImpForestImpishWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}ImpishWrath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.Reaction)
            .DelegatedToAction()
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        var toggle = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "ImpishWrathToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ImpishWrathToggle)
            .SetActivatedPower(powerImpForestImpishWrath)
            .AddToDB();
        toggle.parameter = ActionDefinitions.ActionParameter.ActivatePower;

        var actionAffinityImpishWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityImpishWrathToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ImpishWrathToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(
                    ValidatorsCharacter.HasAvailablePowerUsage(powerImpForestImpishWrath)))
            .AddToDB();

        powerImpForestImpishWrath.AddCustomSubFeatures(
            new AttackBeforeHitConfirmedImpishWrath(powerImpForestImpishWrath, actionAffinityImpishWrathToggle));

        var featureSetImpForestImpishWrath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}ImpishWrath")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(powerImpForestImpishWrath, actionAffinityImpishWrathToggle)
            .AddToDB();

        featureSetImpForestImpishWrath.guiPresentation.title = powerImpForestImpishWrath.guiPresentation.title;
        featureSetImpForestImpishWrath.guiPresentation.description =
            powerImpForestImpishWrath.guiPresentation.description;

        var raceImpForest = CharacterRaceDefinitionBuilder
            .Create(raceImp, $"Race{NAME}")
            .SetGuiPresentation(Category.Race, spriteReference)
            .SetFeaturesAtLevel(1,
                featureSetImpForestAbilityScoreIncrease,
                actionAffinityImpForestInnateCunning,
                featureSetImpForestImpishWrath,
                FeatureDefinitionFeatureSets.FeatureSetElfFeyAncestry)
            .AddToDB();

        raceImpForest.racePresentation.preferedSkinColors = new RangedInt(28, 37);

        return raceImpForest;
    }

    private class AttackBeforeHitConfirmedImpishWrath(
        FeatureDefinitionPower powerPool,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionActionAffinity actionAffinityImpishWrathToggle)
        : IPhysicalAttackFinishedByMe, IMagicalAttackFinishedByMe
    {
        public IEnumerator OnMagicalAttackFinishedByMe(
            CharacterActionMagicEffect action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var rulesetEffect = action.actionParams.RulesetEffect;

            if (!rulesetEffect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            if (action.SaveOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess ||
                action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield return HandleImpishWrath(attacker,
                    defender,
                    [],
                    rulesetEffect.EffectDescription.FindFirstDamageForm()?.damageType);
            }
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (action.AttackRollOutcome != RollOutcome.Success &&
                action.AttackRollOutcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            yield return HandleImpishWrath(
                attacker,
                defender,
                attackerAttackMode.attackTags,
                attackerAttackMode.EffectDescription.FindFirstDamageForm()?.damageType);
        }

        private IEnumerator HandleImpishWrath(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<string> attackTags,
            string damageType = DamageTypeBludgeoning)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();

            // check action affinity for backward compatibility
            if (attacker.RulesetCharacter.HasAnyFeature(actionAffinityImpishWrathToggle) &&
                !attacker.RulesetCharacter.IsToggleEnabled(ImpishWrathToggle))
            {
                yield break;
            }

            if (implementationService == null
                || gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrUnconscious: false })
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetAttacker.CanUsePower(powerPool))
            {
                yield break;
            }

            // maybe add some toggle here similar to Paladin Smite

            var usablePower = UsablePowersProvider.Get(powerPool, rulesetAttacker);
            var bonusDamage = AttributeDefinitions.ComputeProficiencyBonus(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel));

            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter = Gui.Format("Reaction/&CustomReactionImpishWrathDescription",
                    bonusDamage.ToString(), rulesetDefender.Name),
                UsablePower = usablePower
            };

            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("ImpishWrath", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UsePower(usablePower);

            var damageForm = new DamageForm
            {
                DamageType = damageType, DieType = DieType.D1, DiceNumber = 0, BonusDamage = bonusDamage
            };

            var applyFormsParams = new RulesetImplementationDefinitions.ApplyFormsParams
            {
                sourceCharacter = rulesetAttacker,
                targetCharacter = rulesetDefender,
                position = defender.LocationPosition
            };

            RulesetActor.InflictDamage(
                bonusDamage,
                damageForm,
                damageType,
                applyFormsParams,
                rulesetDefender,
                false,
                rulesetAttacker.Guid,
                false,
                attackTags,
                new RollInfo(DieType.D1, [], bonusDamage),
                true,
                out _);
        }
    }

    #endregion
}
