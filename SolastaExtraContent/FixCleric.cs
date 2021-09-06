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

namespace SolastaExtraContent
{
    public class FixCleric
    {
        static public FeatureDefinitionAdditionalDamage potent_spellcasting;

        static internal void run()
        {
            createPotentSpellcasting();
            fixInsight();
            fixLaw();
            fixOblivion();
            fixSun();
            //fixElemental();


        }


        static void fixSun()
        {


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
                                                                                                                        }
                                                                                                                        );
            feature_set.featureSet.Add(proxy_power);
            feature_set.featureSet.Add(apply_proxy_power);
            DatabaseHelper.CharacterSubclassDefinitions.DomainSun.featureUnlocks.RemoveAll(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunIndomitableLight);
            DatabaseHelper.CharacterSubclassDefinitions.DomainSun.featureUnlocks.Find(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun).featureDefinition = feature_set;
        }




        static void fixOblivion()
        {
            var heral_of_pain = DatabaseHelper.FeatureDefinitionPowers.PowerDomainOblivionHeraldOfPain;
            heral_of_pain.effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Damage).applyLevel  = EffectForm.LevelApplianceType.Add;
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
