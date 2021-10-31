using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;
using SolastaModHelpers;
using UnityEngine.AddressableAssets;

namespace SolastaExtraContent
{
    public class Druid
    {
        static public ConditionDefinition inside_spirit_area_condition;
        static public EffectProxyDefinition spirit_proxy;
        static public FeatureDefinitionFeatureSet spirits;
        static public FeatureDefinitionFeatureSet spirit_summoner;
        static public NewFeatureDefinitions.AddExtraConditionToTargetOnConditionApplication guardian_spirits;

        public static void create()
        {
            createCircleOfSpirits();
        }


        static void createCircleOfSpirits()
        {
            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&DruidSubclassCircleOfSpiritsDescription",
                    "Subclass/&DruidSubclassCircleOfSpiritsTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.SorcerousChildRift.GuiPresentation.SpriteReference)
                    .Build();

            createSpirits();
            createSpiritSummoner();
            createGuardianSpirits();
            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("DruidSubclassCircleOfSpirits", "75856caa-9d96-4d59-9b6e-64b0f9256511")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(spirits, 2)
                                                                                            .AddFeatureAtLevel(spirit_summoner, 6)
                                                                                            .AddFeatureAtLevel(guardian_spirits, 10)
                                                                                            .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceDruidCircle.Subclasses.Add(definition.Name);
        }


        static void createSpirits()
        {
            inside_spirit_area_condition = Helpers.ConditionBuilder.createCondition("DruidSubclassCircleOfSpiritsInsideSpiritAreaCondition",
                                                                                    "",
                                                                                    Common.common_no_title,
                                                                                    Common.common_no_title,
                                                                                    Common.common_no_icon,
                                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed
                                                                                    );
            spirit_proxy = Helpers.CopyFeatureBuilder<EffectProxyDefinition>.createFeatureCopy("DruidSubclassCircleOfSpiritsSpiritProxy",
                                                                                               "",
                                                                                               "Feature/&DruidSubclassCircleOfSpiritProxyTitle",
                                                                                               "",
                                                                                               null,
                                                                                               DatabaseHelper.EffectProxyDefinitions.ProxySilence
                                                                                               );
            //inside_spirit_area_condition.silentWhenAdded = true;
            //inside_spirit_area_condition.silentWhenRemoved = true;
            //inside_spirit_area_condition.guiPresentation.hidden = true;

            //protection spirit +1 AC, Saves and advantage on concentration checks
            var ac_bonus = DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHeraldOfBattle;
            var saves_bonus = DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityHeraldOfBattle;
            var concentration_advantage = Helpers.CopyFeatureBuilder<FeatureDefinitionMagicAffinity>.createFeatureCopy("DruidSubclassCircleOfSpiritsProtectionConcentrationBonus",
                                                                                                                "",
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_title,
                                                                                                                null,
                                                                                                                DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.overConcentrationThreshold = 0;
                                                                                                                }
                                                                                                                );

            //hunt spirit
            //+1 attack / damage, advantage on perception checks
            var attack_damage_bonus = DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierHeraldOfBattle;
            var perception_advantage = Helpers.AbilityCheckAffinityBuilder.createSkillCheckAffinity("DruidSubclassCircleOfSpiritsHuntPerceptionBonus",
                                                                                                    "",
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_icon,
                                                                                                    RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                                                                                                    0,
                                                                                                    RuleDefinitions.DieType.D1,
                                                                                                    Helpers.Skills.Perception
                                                                                                    );

            //healing spirit
            //targets regain max hitpoints when healed, advantage on death saves
            var max_healing = DatabaseHelper.FeatureDefinitionHealingModifiers.HealingModifierBeaconOfHope;
            var death_saves_advantage = DatabaseHelper.FeatureDefinitionDeathSavingThrowAffinitys.DeathSavingThrowAffinityBeaconOfHope;

            var protection_spirit = createSpiritPower("DruidSubclassCircleOfSpiritsProtectionSpirit",
                                                      "Feature/&DruidSubclassCircleOfSpiritsProtectionSpiritTitle",
                                                      "Feature/&DruidSubclassCircleOfSpiritsProtectionSpiritDescription",
                                                      DatabaseHelper.FeatureDefinitionPowers.PowerPaladinAuraOfCourage.guiPresentation.spriteReference,
                                                      ac_bonus,
                                                      saves_bonus,
                                                      concentration_advantage
                                                      );

            var hunt_spirit = createSpiritPower("DruidSubclassCircleOfSpiritsHuntSpirit",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHuntSpiritTitle",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHuntSpiritDescription",
                                                  DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandVolcanicAura.guiPresentation.SpriteReference,
                                                  attack_damage_bonus,
                                                  perception_advantage
                                                  );

            var healing_spirit = createSpiritPower("DruidSubclassCircleOfSpiritsHealingSpirit",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHealingSpiritTitle",
                                                  "Feature/&DruidSubclassCircleOfSpiritsHealingSpiritDescription",
                                                  DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarAuraTruth.guiPresentation.SpriteReference,
                                                  max_healing,
                                                  death_saves_advantage
                                                  );


            var spirit_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.TerminateEffectsOnPowerUse>.createFeature("DruidSubclassCircleOfSpiritsSpiritWatcher",
                                                                                                                        "",
                                                                                                                        Common.common_no_title,
                                                                                                                        Common.common_no_title,
                                                                                                                        Common.common_no_icon,
                                                                                                                        a =>
                                                                                                                        {
                                                                                                                            a.powers = new List<FeatureDefinitionPower> { hunt_spirit, healing_spirit, protection_spirit };
                                                                                                                            a.effectsToTerminate = a.powers.Cast<FeatureDefinition>().ToList();
                                                                                                                        }
                                                                                                                        );
            spirits = Helpers.FeatureSetBuilder.createFeatureSet("DruidSubclassCircleOfSpiritsSpiritsFeatureSet",
                                                                "",
                                                                "Feature/&DruidSubclassCircleOfSpiritsSpiritsFeatureSetTitle",
                                                                "Feature/&DruidSubclassCircleOfSpiritsSpiritsFeatureSetDescription",
                                                                true,
                                                                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                false,
                                                                healing_spirit,
                                                                hunt_spirit,
                                                                protection_spirit,
                                                                spirit_watcher
                                                                );
        }


        static FeatureDefinitionPower createSpiritPower(string name, string title, string description, AssetReferenceSprite power_sprite, params FeatureDefinition[] features)
        {
            var condition = Helpers.ConditionBuilder.createCondition(name + "AreaCondition",
                                                                    "",
                                                                    title.Replace("Feature/&", "Rules/&Condition"),
                                                                    description.Replace("Feature/&", "Rules/&Condition"),
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed,
                                                                    features
                                                                    );
            condition.parentCondition = inside_spirit_area_condition;
            condition.allowMultipleInstances = false;

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Silence.effectDescription);
            effect.rangeParameter = 12;
            effect.durationParameter = 1;
            effect.effectForms.Clear();
            var condition_form = new EffectForm();
            condition_form.formType = EffectForm.EffectFormType.Condition;
            condition_form.conditionForm = new ConditionForm();
            condition_form.createdByCharacter = true;
            condition_form.conditionForm.conditionDefinition = condition;
            effect.effectForms.Add(condition_form);
            var summon_form = new EffectForm();
            summon_form.formType = EffectForm.EffectFormType.Summon;
            summon_form.summonForm = new SummonForm();
            summon_form.createdByCharacter = true;
            summon_form.summonForm.summonType = SummonForm.Type.EffectProxy;
            summon_form.summonForm.effectProxyDefinitionName = spirit_proxy.name;
            summon_form.summonForm.number = 1;
            effect.effectForms.Add(summon_form);
            effect.targetSide = RuleDefinitions.Side.Ally;
            effect.canBePlacedOnCharacter = true;

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.LinkedPower>.createPower(name + "Power",
                                                                                                   "",
                                                                                                   title,
                                                                                                   description,
                                                                                                   power_sprite,
                                                                                                   effect,
                                                                                                   RuleDefinitions.ActivationTime.BonusAction,
                                                                                                   2,
                                                                                                   RuleDefinitions.UsesDetermination.Fixed,
                                                                                                   RuleDefinitions.RechargeRate.ShortRest);
            power.linkedPower = DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape;
            return power;
        }


        static void createSpiritSummoner()
        {
            var title = "Feature/&DruidSubclassCircleOfSpiritsSpiritSummonerTitle";
            var description = "Feature/&DruidSubclassCircleOfSpiritsSpiritSummonerDescription";


            var magic_tag = Helpers.FeatureBuilder<NewFeatureDefinitions.AddAttackTagIfHasFeature>.createFeature("DruidSubclassCircleOfSpiritsSpiritSummonerMagicAttacks",
                                                                                                                  "",
                                                                                                                  Common.common_no_title,
                                                                                                                  Common.common_no_title,
                                                                                                                  Common.common_no_icon,
                                                                                                                  a =>
                                                                                                                  {
                                                                                                                      a.requiredFeature = null;
                                                                                                                      a.tag = "Magical";
                                                                                                                  }
                                                                                                                  );

            var extra_hp = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseMonsterHitPointsToTargetOnConditionApplication>
                                                                                .createFeature("DruidSubclassCircleOfSpiritsExtraHP",
                                                                                               "",
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_icon,
                                                                                               a =>
                                                                                               {
                                                                                                   a.requiredCondition = DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature;
                                                                                                   a.hdMultiplier = 2;
                                                                                               }
                                                                                               );

            var condition = Helpers.ConditionBuilder.createCondition("DruidSubclassCircleOfSpiritsSpiritSummonerCondition",
                                                                    "",
                                                                    title,
                                                                    "Rules/&DruidSubclassCircleOfSpiritsSpiritSummonerConditionDescription",
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance.guiPresentation.spriteReference,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed,
                                                                    magic_tag
                                                                    );

            var apply_condition = Helpers.FeatureBuilder<NewFeatureDefinitions.AddExtraConditionToTargetOnConditionApplication>
                                                                                .createFeature("DruidSubclassCircleOfSpiritsApplySpiritSummonerCondition",
                                                                                               "",
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_icon,
                                                                                               a =>
                                                                                               {
                                                                                                   a.requiredCondition = DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature;
                                                                                                   a.extraCondition = condition;
                                                                                               }
                                                                                               );

            spirit_summoner = Helpers.FeatureSetBuilder.createFeatureSet("DruidSubclassCircleOfSpiritsSpiritSummonerFeatureSet",
                                                                            "",
                                                                            title,
                                                                            description,
                                                                            false,
                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                            false,
                                                                            apply_condition,
                                                                            extra_hp
                                                                            );
        }


        static void createGuardianSpirits()
        {
            var title = "Feature/&DruidSubclassCircleOfSpiritsGuardianSpiritsTitle";
            var description = "Feature/&DruidSubclassCircleOfSpiritsGuardianSpiritsDescription";

            var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.HealAtTurnEndIfHasConditionBasedOnCasterLevel>
                                                                    .createFeature("DruidSubclassCircleOfSpiritsGuardianSpiritsFeature",
                                                                                   "",
                                                                                   Common.common_no_title,
                                                                                   Common.common_no_title,
                                                                                   Common.common_no_icon,
                                                                                   a =>
                                                                                   {
                                                                                       a.casterCondition = inside_spirit_area_condition;
                                                                                       a.allowParentConditions = true;
                                                                                       a.characterClass = DatabaseHelper.CharacterClassDefinitions.Druid;
                                                                                       a.levelHealing = new List<(int, int)>
                                                                                       {
                                                                                           (9, 0),
                                                                                           (11, 5),
                                                                                           (13, 6),
                                                                                           (15, 7),
                                                                                           (17, 8),
                                                                                           (19, 9),
                                                                                           (20, 10)
                                                                                       };
                                                                                   }
                                                                                   );



            var condition = Helpers.ConditionBuilder.createCondition("DruidSubclassCircleOfSpiritsGuardianSpiritsCondition",
                                                                    "",
                                                                    title,
                                                                    "Rules/&DruidSubclassCircleOfSpiritsGuardianSpiritsConditionDescription",
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBarkskin.guiPresentation.spriteReference,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBlessed,
                                                                    feature
                                                                    );

            guardian_spirits = Helpers.FeatureBuilder<NewFeatureDefinitions.AddExtraConditionToTargetOnConditionApplication>
                                                                                .createFeature("DruidSubclassCircleOfSpiritsGuardianSpiritsApplyCondition",
                                                                                               "",
                                                                                               title,
                                                                                               description,
                                                                                               Common.common_no_icon,
                                                                                               a =>
                                                                                               {
                                                                                                   a.requiredCondition = DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature;
                                                                                                   a.extraCondition = condition;
                                                                                               }
                                                                                               );
        }
    }
}
