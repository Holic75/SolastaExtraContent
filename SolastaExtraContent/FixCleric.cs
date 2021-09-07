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
    public class FixCleric
    {
        static public FeatureDefinitionAdditionalDamage potent_spellcasting;
        static public CharacterSubclassDefinition elemental_domain;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellsKnown bonus_elemental_cantrip;
        static public FeatureDefinitionFeatureSet primal_harmony;
        static public FeatureDefinitionFeatureSet wrath_of_the_elements;
        static public FeatureDefinitionPower wind_channel;
        static public FeatureDefinitionPower cold_channel;
        static public FeatureDefinitionFeatureSet thunder_channel;


        static internal void run()
        {
            createPotentSpellcasting();
            fixInsight();
            fixLaw();
            fixOblivion();
            fixSun();
            createWindChannel();
            createColdChannel();
            createThunderChannel();
            fixElemental();
        }


        static void createWindChannel()
        {
            var wind_effect = new EffectDescription();
            wind_effect.Copy(DatabaseHelper.SpellDefinitions.GustOfWind.effectDescription);
            wind_effect.targetSide = RuleDefinitions.Side.Enemy;
            wind_effect.durationType = RuleDefinitions.DurationType.Instantaneous;
            wind_effect.hasSavingThrow = true;
            wind_effect.savingThrowAbility = Helpers.Stats.Strength;
            wind_effect.difficultyClassComputation = RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
            wind_effect.savingThrowDifficultyAbility = Helpers.Stats.Wisdom;
            wind_effect.effectForms.Clear();

            var damage_form = new EffectForm();
            damage_form.createdByCharacter = true;
            damage_form.formType = EffectForm.EffectFormType.Damage;
            damage_form.applyLevel = EffectForm.LevelApplianceType.Add;
            damage_form.levelMultiplier = 1;
            damage_form.hasSavingThrow = true;
            damage_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            damage_form.damageForm = new DamageForm();
            damage_form.damageForm.damageType = Helpers.DamageTypes.Bludgeoning;
            damage_form.damageForm.dieType = RuleDefinitions.DieType.D8;
            damage_form.damageForm.diceNumber = 2;
            wind_effect.effectForms.Add(damage_form);

            var motion_form = new EffectForm();
            motion_form.createdByCharacter = true;
            motion_form.formType = EffectForm.EffectFormType.Motion;
            motion_form.hasSavingThrow = true;
            motion_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            motion_form.motionForm = new MotionForm();
            motion_form.motionForm.type = MotionForm.MotionType.PushFromOrigin;
            motion_form.motionForm.distance = 1;
            wind_effect.effectForms.Add(motion_form);

            motion_form = new EffectForm();
            motion_form.createdByCharacter = true;
            motion_form.formType = EffectForm.EffectFormType.Motion;
            motion_form.hasSavingThrow = true;
            motion_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            motion_form.motionForm = new MotionForm();
            motion_form.motionForm.type = MotionForm.MotionType.FallProne;
            wind_effect.effectForms.Add(motion_form);

            wind_channel = Helpers.GenericPowerBuilder<FeatureDefinitionPower>.createPower("DomainElementalCallUponWind",
                                                                                           "",
                                                                                           "Feature/&DomainElementalCallUponWindTitle",
                                                                                           "Feature/&DomainElementalCallUponWindDescription",
                                                                                           DatabaseHelper.FeatureDefinitionPowers.PowerDefilerMistyFormEscape.guiPresentation.spriteReference,
                                                                                           wind_effect,
                                                                                           RuleDefinitions.ActivationTime.Action,
                                                                                           1,
                                                                                           RuleDefinitions.UsesDetermination.Fixed,
                                                                                           RuleDefinitions.RechargeRate.ChannelDivinity,
                                                                                           ability: Helpers.Stats.Wisdom);
        }


        static void createThunderChannel()
        {
            var thunder_effect = new EffectDescription();
            thunder_effect.Copy(DatabaseHelper.SpellDefinitions.Shatter.effectDescription);
            thunder_effect.targetSide = RuleDefinitions.Side.Enemy;
            thunder_effect.durationType = RuleDefinitions.DurationType.Instantaneous;
            thunder_effect.hasSavingThrow = true;
            thunder_effect.savingThrowAbility = Helpers.Stats.Constitution;
            thunder_effect.difficultyClassComputation = RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
            thunder_effect.savingThrowDifficultyAbility = Helpers.Stats.Wisdom;
            thunder_effect.targetExcludeCaster = true;
            thunder_effect.targetType = RuleDefinitions.TargetType.Sphere;
            thunder_effect.rangeParameter = 0;
            thunder_effect.targetParameter = 2;
            thunder_effect.targetParameter2 = 2;
            thunder_effect.effectAdvancement.Clear();
            thunder_effect.effectForms.Clear();

            var damage_form = new EffectForm();
            damage_form.createdByCharacter = true;
            damage_form.formType = EffectForm.EffectFormType.Damage;
            damage_form.applyLevel = EffectForm.LevelApplianceType.Add;
            damage_form.levelMultiplier = 1;
            damage_form.hasSavingThrow = true;
            damage_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            damage_form.damageForm = new DamageForm();
            damage_form.damageForm.damageType = Helpers.DamageTypes.Thundering;
            damage_form.damageForm.dieType = RuleDefinitions.DieType.D8;
            damage_form.damageForm.diceNumber = 2;
            thunder_effect.effectForms.Add(damage_form);

            var thunder_channel_damage  = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>.createPower("DomainElementalCallUponThunderDamage",
                                                                                                   "",
                                                                                                   "Feature/&DomainElementalCallUponThunderTitle",
                                                                                                   "Feature/&DomainElementalCallUponThunderDescription",
                                                                                                   DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.guiPresentation.spriteReference,
                                                                                                   thunder_effect,
                                                                                                   RuleDefinitions.ActivationTime.NoCost,
                                                                                                   1,
                                                                                                   RuleDefinitions.UsesDetermination.Fixed,
                                                                                                   RuleDefinitions.RechargeRate.AtWill,
                                                                                                   ability: Helpers.Stats.Wisdom);

            var thunder_channel_power = Helpers.GenericPowerBuilder<FeatureDefinitionPower>.createPower("DomainElementalCallUponThunder",
                                                                                       "",
                                                                                       "Feature/&DomainElementalCallUponThunderTitle",
                                                                                       "Feature/&DomainElementalCallUponThunderDescription",
                                                                                       DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.guiPresentation.spriteReference,
                                                                                       DatabaseHelper.SpellDefinitions.MistyStep.effectDescription,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       1,
                                                                                       RuleDefinitions.UsesDetermination.Fixed,
                                                                                       RuleDefinitions.RechargeRate.ChannelDivinity,
                                                                                       ability: Helpers.Stats.Wisdom);

            var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyPowerAfterPowerUseToCaster>.createFeature("DomainElementalCallUponThunderFeature",
                                                                                                                      "",
                                                                                                                      Common.common_no_title,
                                                                                                                      Common.common_no_title,
                                                                                                                      Common.common_no_icon,
                                                                                                                      a =>
                                                                                                                      {
                                                                                                                          a.usedPower = thunder_channel_power;
                                                                                                                          a.power = thunder_channel_power;
                                                                                                                      }
                                                                                                                      );

            thunder_channel = Helpers.FeatureSetBuilder.createFeatureSet("DomainElementalCallUponThunderFeatureSet",
                                                                         "",
                                                                         thunder_channel_power.guiPresentation.title,
                                                                         thunder_channel_power.guiPresentation.description,
                                                                         false,
                                                                         FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                         false,
                                                                         thunder_channel_power,
                                                                         thunder_channel_damage,
                                                                         feature
                                                                         );
        }

        static void createColdChannel()
        {
            cold_channel = Helpers.CopyFeatureBuilder<FeatureDefinitionPower>.createFeatureCopy("DomainElementalCallUponCold",
                                                                                                "",
                                                                                               "Feature/&DomainElementalCallUponColdTitle",
                                                                                               "Feature/&DomainElementalCallUponColdDescription",
                                                                                               DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalIceLance.guiPresentation.spriteReference,
                                                                                               DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold,
                                                                                               a =>
                                                                                               {
                                                                                                   a.fixedUsesPerRecharge = 1;
                                                                                                   a.rechargeRate = RuleDefinitions.RechargeRate.ChannelDivinity;
                                                                                               }
                                                                                               );
        }


        static void fixElemental()
        {
            var gui_presentation = new GuiPresentationBuilder(
                                            "Subclass/&DomainElementalDescription",
                                            "Subclass/&DomainElementalTitle")
                                            .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionShockArcanist.GuiPresentation.SpriteReference)
                                            .Build();

            var cantrips = new List<SpellDefinition> {  DatabaseHelper.SpellDefinitions.FireBolt,
                                                        DatabaseHelper.SpellDefinitions.ShockingGrasp,
                                                        Cantrips.frostbite,
                                                        DatabaseHelper.SpellDefinitions.RayOfFrost};


            var spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("DomainElementalBonusCantripSpelllist",
                                                                           "",
                                                                           Common.common_no_title,
                                                                           cantrips
                                                                           );

            bonus_elemental_cantrip = Helpers.ExtraSpellSelectionBuilder.createExtraCantripSelection("DomainElementalBonusCantrip",
                                                                                                    "",
                                                                                                    "Feature/&DomainElementalBonusCantripTitle",
                                                                                                    "Feature/&DomainElementalBonusCantripDescription",
                                                                                                    DatabaseHelper.CharacterClassDefinitions.Cleric,
                                                                                                    1,
                                                                                                    1,
                                                                                                    spelllist
                                                                                                    );

            var primal_harmony = Helpers.FeatureSetBuilder.createFeatureSet("DomainElementalPrimalHarmony",
                                                                        "",
                                                                        "Feature/&DomainElementalPrimalHarmonyTitle",
                                                                        "Feature/&DomainElementalPrimalHarmonyDescription",
                                                                        false,
                                                                        FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                        false);

            var damages = new List<(string damage_type, AssetReferenceSprite sprite)> {(Helpers.DamageTypes.Fire, DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandVolcanicAura.guiPresentation.spriteReference),
                                                                                       (Helpers.DamageTypes.Cold, DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarAuraTruth.guiPresentation.spriteReference),
                                                                                       (Helpers.DamageTypes.Lightning, DatabaseHelper.FeatureDefinitionPowers.PowerPaladinAuraOfProtection.guiPresentation.spriteReference)};




            foreach (var d in damages)
            {
                var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ModifySpellDamageType>.createFeature($"DomainElementalHeraldOfTheElements{d.damage_type}Feature",
                                                                                                                "",
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_icon,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.newDamageType = d.damage_type;
                                                                                                                }
                                                                                                                );

                var condition = Helpers.ConditionBuilder.createCondition($"DomainElementalHeraldOfTheElements{d.damage_type}Condition",
                                                                         "",
                                                                         $"Feature/&DomainElementalDiscipleOfTheElements{d.damage_type}Title",
                                                                         $"Feature/&DomainElementalDiscipleOfTheElements{d.damage_type}Description",
                                                                         null,
                                                                         DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneFury,
                                                                         feature
                                                                         );

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneFury.effectDescription);
                effect.durationParameter = 1;
                effect.durationType = RuleDefinitions.DurationType.Round;
                effect.endOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
                effect.effectForms.Clear();

                var form = new EffectForm();
                form.createdByCharacter = true;
                form.formType = EffectForm.EffectFormType.Condition;
                form.conditionForm = new ConditionForm();
                form.conditionForm.conditionDefinition = condition;
                form.conditionForm.operation = ConditionForm.ConditionOperation.Add;
                effect.effectForms.Add(form);

                var power = Helpers.GenericPowerBuilder<FeatureDefinitionPower>.createPower($"DomainElementalHeraldOfTheElements{d.damage_type}Power",
                                                                                            "",
                                                                                            $"Feature/&DomainElementalDiscipleOfTheElements{d.damage_type}Title",
                                                                                            $"Feature/&DomainElementalDiscipleOfTheElements{d.damage_type}Description",
                                                                                            d.sprite,
                                                                                            effect,
                                                                                            RuleDefinitions.ActivationTime.BonusAction,
                                                                                            1,
                                                                                            RuleDefinitions.UsesDetermination.Fixed,
                                                                                            RuleDefinitions.RechargeRate.AtWill
                                                                                            );
                primal_harmony.featureSet.Add(power);
            }

            var herald_of_the_elements = Helpers.FeatureSetBuilder.createFeatureSet("DomainElementalHeraldOfTheElements",
                                                                        "",
                                                                        "Feature/&DomainElementalDiscipleOfTheElementsTitle",
                                                                        "Feature/&DomainElementalHeraldOfTheElementsDescription",
                                                                        false,
                                                                        FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                        false);

            var herald_powers = new List<FeatureDefinitionPower>{DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsFire,
                                                                 DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsCold,
                                                                 DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning,
                                                                };

            for (int i = 0; i < herald_powers.Count; i++)
            {
                var gui = herald_powers[i].effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Condition).conditionForm.conditionDefinition.guiPresentation;
                var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.LinkedPower>.createPower("DomainElemental" + herald_powers[i],
                                                                                                       "",
                                                                                                       gui.title,
                                                                                                       "Feature/&DomainElementalHeraldOfTheElementsDescription",
                                                                                                       herald_powers[i].guiPresentation.spriteReference,
                                                                                                       herald_powers[i].effectDescription,
                                                                                                       herald_powers[i].activationTime,
                                                                                                       1,
                                                                                                       RuleDefinitions.UsesDetermination.Fixed,
                                                                                                       RuleDefinitions.RechargeRate.ShortRest);
                herald_of_the_elements.featureSet.Add(power);
                if (i >= 1)
                {
                    power.linkedPower = (herald_of_the_elements.featureSet[0] as FeatureDefinitionPower);
                }
            }

            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainElemental.autoPreparedSpellsGroups[3].spellsList[1] = DatabaseHelper.SpellDefinitions.Stoneskin;

            CharacterSubclassDefinition elemental_domain = new CharacterSubclassDefinitionBuilder("DomainElemental", "90dddcb0-bb0d-46a5-ba92-83bdbaec21fd")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainElemental, 1)
                                                                                            .AddFeatureAtLevel(bonus_elemental_cantrip, 1)
                                                                                            .AddFeatureAtLevel(primal_harmony, 1)
                                                                                            .AddFeatureAtLevel(cold_channel, 2)
                                                                                            .AddFeatureAtLevel(thunder_channel, 2)
                                                                                            .AddFeatureAtLevel(wind_channel, 2)
                                                                                            .AddFeatureAtLevel(herald_of_the_elements, 6)
                                                                                            .AddFeatureAtLevel(potent_spellcasting, 8)
                                                                                            .AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionPowers.PowerClericDivineInterventionWizard, 10)
                                                                                            .AddToDB();
            var arun = DatabaseHelper.DeityDefinitions.Arun;
            arun.subclasses.Add(elemental_domain.Name);
            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceClericDivineDomains.subclasses.Add(elemental_domain.Name);
            arun.subclasses.Remove(DatabaseHelper.CharacterSubclassDefinitions.DomainElementalFire.Name);
            arun.subclasses.Remove(DatabaseHelper.CharacterSubclassDefinitions.DomainElementalCold.Name);
            arun.subclasses.Remove(DatabaseHelper.CharacterSubclassDefinitions.DomainElementalLighting.Name);
        }


        static void fixSun()
        {
            DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainSun.bonusCantrips[0] = DatabaseHelper.SpellDefinitions.SacredFlame;
            DatabaseHelper.FeatureDefinitionBonusCantripss.BonusCantripsDomainSun.guiPresentation.description = DatabaseHelper.SpellDefinitions.SacredFlame.guiPresentation.title;

            var power = DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight;
            power.rechargeRate = RuleDefinitions.RechargeRate.ChannelDivinity;
            power.usesDetermination = RuleDefinitions.UsesDetermination.Fixed;
            power.guiPresentation.title = DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun.guiPresentation.title;
            var feature_set = Helpers.FeatureSetBuilder.createFeatureSet("HeraldOfTheSunFeatureSet",
                                                                         "",
                                                                         DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.title,
                                                                         DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight.GuiPresentation.description,
                                                                         false,
                                                                         FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                         false,
                                                                         power);

            var effect = new EffectDescription();
            effect.Copy(power.EffectDescription);
            effect.recurrentEffect = RuleDefinitions.RecurrentEffect.No;
            effect.savingThrowAbility = Helpers.Stats.Constitution;
            effect.hasSavingThrow = true;
            effect.savingThrowDifficultyAbility = Helpers.Stats.Wisdom;
            effect.difficultyClassComputation = RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
            effect.targetSide = RuleDefinitions.Side.Enemy;
            effect.effectForms.Clear();
            var blind_effect = DatabaseHelper.SpellDefinitions.Blindness.effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Condition);
            blind_effect.canSaveToCancel = true;
            effect.effectForms.Add(blind_effect);

            var proxy_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>.createPower("HeraldOfTheSunProxyPower",
                                                                                                          "",
                                                                                                          power.GuiPresentation.title,
                                                                                                          power.guiPresentation.description,
                                                                                                          power.guiPresentation.spriteReference,
                                                                                                          effect,
                                                                                                          RuleDefinitions.ActivationTime.NoCost,
                                                                                                          1,
                                                                                                          RuleDefinitions.UsesDetermination.Fixed,
                                                                                                          RuleDefinitions.RechargeRate.AtWill,
                                                                                                          ability: Helpers.Stats.Wisdom,
                                                                                                          show_casting: false);
            proxy_power.guiPresentation.hidden = true;
            var apply_proxy_power = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyPowerOnProxySummon>.createFeature("HeralOfTheSunApplyProxyPowerFeature",
                                                                                                                        "",
                                                                                                                        Common.common_no_title,
                                                                                                                        Common.common_no_title,
                                                                                                                        Common.common_no_icon,
                                                                                                                        a =>
                                                                                                                        {
                                                                                                                            a.power = proxy_power;
                                                                                                                            a.proxy = DatabaseHelper.EffectProxyDefinitions.ProxyIndomitableLight;
                                                                                                                        }
                                                                                                                        );
            feature_set.featureSet.Add(proxy_power);
            feature_set.featureSet.Add(apply_proxy_power);
            DatabaseHelper.CharacterSubclassDefinitions.DomainSun.featureUnlocks.RemoveAll(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight);
            DatabaseHelper.CharacterSubclassDefinitions.DomainSun.featureUnlocks.Find(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun).featureDefinition = feature_set;
            DatabaseHelper.CharacterSubclassDefinitions.DomainSun.featureUnlocks.Find(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike).featureDefinition = potent_spellcasting;
        }




        static void fixOblivion()
        {
            var heral_of_pain = DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionHeraldOfPain;
            heral_of_pain.effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Damage).applyLevel  = EffectForm.LevelApplianceType.Add;
            heral_of_pain.effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Damage).levelMultiplier = 1;
            heral_of_pain.effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Damage).savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            heral_of_pain.guiPresentation.description = "Feature/&PowerDomainOblivionHeraldOfPainLevelDescription";

            DatabaseHelper.CharacterSubclassDefinitions.DomainOblivion.featureUnlocks.Find(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate).featureDefinition = potent_spellcasting;

            DatabaseHelper.CharacterSubclassDefinitions.DomainOblivion.featureUnlocks
                .Find(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainOblivionStrikeOblivion).featureDefinition = DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate;
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate.rechargeRate = RuleDefinitions.RechargeRate.LongRest;
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate.usesAbilityScoreName = Helpers.Stats.Wisdom;
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate.fixedUsesPerRecharge = 0;
            DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionMarkOfFate.usesDetermination = RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed;
        }


        static void fixLaw()
        {
            DatabaseHelper.CharacterSubclassDefinitions.DomainLaw.featureUnlocks.RemoveAll(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawForceOfLaw);
            
            var force_strike = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("AdditionalDamageDomainLawForceStrikeOblivion",
                                                                                                               "",
                                                                                                               "Feature/&AdditionalDamageDomainLifeDivineStrikeTitle",
                                                                                                               "Feature/&AdditionalDamageDomainLawForceStrikeOblivionDescription",
                                                                                                               null,
                                                                                                               DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainOblivionStrikeOblivion,
                                                                                                               a =>
                                                                                                               {
                                                                                                                   a.damageDieType = RuleDefinitions.DieType.D8;
                                                                                                                   a.specificDamageType = Helpers.DamageTypes.Force;
                                                                                                                   a.notificationTag = "DivineStrike";
                                                                                                               }
                                                                                                               );
            DatabaseHelper.CharacterSubclassDefinitions.DomainLaw.featureUnlocks.Find(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawAnathema).featureDefinition = force_strike;
        }


        static void fixInsight()
        {
            DatabaseHelper.CharacterSubclassDefinitions.DomainInsight.featureUnlocks.Add(new FeatureUnlockByLevel(potent_spellcasting, 8));
            DatabaseHelper.CharacterSubclassDefinitions.DomainInsight.featureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                                                                                        {
                                                                                            return a.Level - b.Level;
                                                                                        }
                                                                                        );
        }



        static void createPotentSpellcasting()
        {
            var title_string = "Feature/&PotentSpellcastingCantirpTitle";
            var description_string = "Feature/&PotentSpellcastingCantirpDescription";

            potent_spellcasting = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("PotentSpellcastingCantirpExtraDamage",
                                                                                                                "",
                                                                                                                title_string,
                                                                                                                description_string,
                                                                                                                null,
                                                                                                                DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageSorcererDraconicElementalAffinity,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.triggerCondition = (RuleDefinitions.AdditionalDamageTriggerCondition)SolastaModHelpers.ExtendedEnums.AdditionalDamageTriggerCondition.CantripDamage;
                                                                                                                    a.additionalDamageType = RuleDefinitions.AdditionalDamageType.SameAsBaseDamage;
                                                                                                                    a.damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.SpellcastingBonus;
                                                                                                                    a.notificationTag = "PotentSpellcasting";
                                                                                                                }
                                                                                                                );
        }




    }
}
