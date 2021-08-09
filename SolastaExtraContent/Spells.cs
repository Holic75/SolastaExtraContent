﻿using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModHelpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AddressableAssets;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;

namespace SolastaExtraContent
{
    public class Spells
    {
        static public NewFeatureDefinitions.ReactionOnDamageSpell hellish_rebuke;
        static public SpellDefinition call_lightning;
        static public SpellDefinition heat_metal;

        static public SpellDefinition polymorph;

        internal static void create()
        {
            createHellishRebuke();
            createCallLightning();
            createHeatMetal();
            //createPolymorph();
        }



        static void createPolymorph()
        {
            var title_string = "Spell/&PolymorphTitle";
            var description_string = "Spell/&PolymorphDescription";
            var sprite = DatabaseHelper.SpellDefinitions.IdentifyCreatures.GuiPresentation.spriteReference;
            DatabaseHelper.MonsterDefinitions.ConjuredOneBeastTiger_Drake.SetFullyControlledWhenAllied(true);

            var wolf = Common.createPolymoprhUnit(DatabaseHelper.MonsterDefinitions.Wolf, "PolymorphTestWolf", "", "", "");
            var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.Polymorph>.createFeature("PolymorphTestFeature",
                                                                                                 "",
                                                                                                 title_string,
                                                                                                 description_string,
                                                                                                 null,
                                                                                                 a =>
                                                                                                 {
                                                                                                     a.monster = wolf;
                                                                                                     a.transferFeatures = true;
                                                                                                     a.statsToTransfer = new string[] { Helpers.Stats.Charisma, Helpers.Stats.Intelligence, Helpers.Stats.Wisdom };
                                                                                                     a.allowSpellcasting = false;
                                                                                                 });
            
            var condition = Helpers.ConditionBuilder.createCondition("PolymorphCondition",
                                                                    "",
                                                                    title_string,
                                                                    Common.common_no_title,
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionBarkskin,
                                                                    feature
                                                                    );

            
            condition.conditionTags.Clear();
            condition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Barkskin.EffectDescription);
            effect.EffectForms.Clear();
            effect.RangeType = RuleDefinitions.RangeType.Self;
            effect.TargetSide = RuleDefinitions.Side.Ally;
            effect.targetType = RuleDefinitions.TargetType.Self;
            effect.durationType = RuleDefinitions.DurationType.Minute;

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect.EffectForms.Add(effect_form);
            
            polymorph = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithRestrictions>.createSpell("PolymorphTestSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       wolf.GuiPresentation.spriteReference,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       1,
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       Helpers.SpellSchools.Transmutation
                                                                                       );
            polymorph.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizard, polymorph);
        }


        static void createHeatMetal()
        {
            var title_string = "Spell/&HeatMetalTitle";
            var description_string = "Spell/&HeatMetalDescription";
            var title_string_condition = "Rules/&HeatMetalTargetConditionTitle";
            var description_string_condition = "Rules/&HeatMetalTargetConditionDescription";
            var title_string_condition2 = "Rules/&HeatMetalTargetCondition2Title";

            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("HeatMetalSpellImage",
                                                    $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/HeatMetal.png",
                                                    128, 128);


            var condition = Helpers.ConditionBuilder.createCondition("HeatMetalCasterCondition",
                                                                    "",
                                                                    title_string,
                                                                    description_string,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked.guiPresentation.SpriteReference,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked
                                                                    );
            condition.conditionType = RuleDefinitions.ConditionType.Beneficial;

            var condition_cooldown = Helpers.ConditionBuilder.createCondition("HeatMetalCasterCooldownCondition",
                                                                        "",
                                                                        Common.common_no_title,
                                                                        Common.common_no_title,
                                                                        Common.common_no_icon,
                                                                        DatabaseHelper.ConditionDefinitions.ConditionDummy
                                                                        );
            condition_cooldown.specialDuration = true;
            condition_cooldown.durationParameter = 1;
            condition_cooldown.durationType = RuleDefinitions.DurationType.Round;
            condition_cooldown.specialInterruptions = new List<RuleDefinitions.ConditionInterruption> { RuleDefinitions.ConditionInterruption.AnyBattleTurnEnd };

            var condition_target = Helpers.ConditionBuilder.createCondition("HeatMetalTargetCondition",
                                                        "",
                                                        title_string_condition,
                                                        description_string_condition,
                                                        DatabaseHelper.ConditionDefinitions.ConditionOnFire.guiPresentation.SpriteReference,
                                                        DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked
                                                        );
            condition_target.conditionType = RuleDefinitions.ConditionType.Detrimental;
            condition_target.allowMultipleInstances = true;
            condition_target.terminateWhenRemoved = true;

            var attack_disadvantage = Helpers.CopyFeatureBuilder<FeatureDefinitionCombatAffinity>.createFeatureCopy("HeatMetalTooHotAttackRollsFeature",
                                                                                                 "",
                                                                                                 Common.common_no_title,
                                                                                                 Common.common_no_title,
                                                                                                 Common.common_no_icon,
                                                                                                 DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityChilledByTouch,
                                                                                                 a =>
                                                                                                 {
                                                                                                     a.myselfFamilyRestrictions = new List<string>();
                                                                                                     a.situationalContext = RuleDefinitions.SituationalContext.None;
                                                                                                 }
                                                                                                 );

            var checks_disadvantage = Helpers.AbilityCheckAffinityBuilder.createAbilityCheckAffinity("HeatMetalTooHotAbilityChecksFeature",
                                                                                                 "",
                                                                                                 Common.common_no_title,
                                                                                                 Common.common_no_title,
                                                                                                 Common.common_no_icon,
                                                                                                 RuleDefinitions.CharacterAbilityCheckAffinity.Disadvantage,
                                                                                                 0,
                                                                                                 RuleDefinitions.DieType.D1,
                                                                                                 Helpers.Stats.getAllStats().ToArray()
                                                                                                 );

            var condition_target2 = Helpers.ConditionBuilder.createCondition("HeatMetalTargetCondition2",
                                            "",
                                            title_string_condition2,
                                            Common.common_no_title,
                                            DatabaseHelper.ConditionDefinitions.ConditionOnFire.guiPresentation.SpriteReference,
                                            DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked,
                                            attack_disadvantage,
                                            checks_disadvantage
                                            );
            condition_target2.specialDuration = true;
            condition_target2.durationParameter = 1;
            condition_target2.turnOccurence = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            condition_target2.durationType = RuleDefinitions.DurationType.Round;
            condition_target2.conditionType = RuleDefinitions.ConditionType.Detrimental;


            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsFire.effectDescription);
            effect.rangeParameter = 12;
            effect.rangeType = RuleDefinitions.RangeType.Distance;
            effect.targetType = RuleDefinitions.TargetType.Individuals;
            effect.targetParameter = 1;
            effect.targetParameter2 = 1;
            effect.savingThrowAbility = Helpers.Stats.Constitution;
            effect.EffectForms.Clear();
            effect.targetSide = RuleDefinitions.Side.Enemy;
            effect.durationType = RuleDefinitions.DurationType.Minute;
            effect.durationParameter = 1;
            effect.createdByCharacter = true;

            var effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 2;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D8;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Fire;
            effect_form.hasSavingThrow = false;
            effect_form.createdByCharacter = true;
            effect_form.levelMultiplier = 1;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;
            effect_form.createdByCharacter = true;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition_target;
            effect_form.createdByCharacter = true;
            effect.EffectForms.Add(effect_form);


            effect_form = new EffectForm();
            effect_form.formType = EffectForm.EffectFormType.LightSource;
            effect_form.lightSourceForm = DatabaseHelper.SpellDefinitions.Shine.effectDescription.effectForms[0].lightSourceForm;
            effect_form.createdByCharacter = true;
            effect.EffectForms.Add(effect_form);

            var effect_form2 = new EffectForm();
            effect_form2.ConditionForm = new ConditionForm();
            effect_form2.FormType = EffectForm.EffectFormType.Condition;
            effect_form2.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form2.ConditionForm.ConditionDefinition = condition_target2;
            effect_form2.createdByCharacter = true;
            effect_form2.hasSavingThrow = true;
            effect_form2.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            //effect_form2.canSaveToCancel = true;
            effect.EffectForms.Add(effect_form2);


            effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect_form.conditionForm.forceOnSelf = true;
            effect_form.conditionForm.applyToSelf = true;
            effect_form.createdByCharacter = true;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition_cooldown;
            effect_form.conditionForm.forceOnSelf = true;
            effect_form.conditionForm.applyToSelf = true;
            effect_form.createdByCharacter = true;
            effect.EffectForms.Add(effect_form);

            effect.SetEffectAdvancement(DatabaseHelper.SpellDefinitions.Shatter.effectDescription.effectAdvancement);
            effect.targetFilteringTag = (RuleDefinitions.TargetFilteringTag)SolastaModHelpers.ExtendedEnums.ExtraTargetFilteringTag.MetalArmor;

            heat_metal = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithRestrictions>.createSpell("HeatMetalSpell",
                                                                                                                "",
                                                                                                                title_string,
                                                                                                                description_string,
                                                                                                                sprite,
                                                                                                                effect,
                                                                                                                RuleDefinitions.ActivationTime.Action,
                                                                                                                2,
                                                                                                                true,
                                                                                                                true,
                                                                                                                true,
                                                                                                                Helpers.SpellSchools.Transmutation
                                                                                                                );
            heat_metal.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;


            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithContextFromCondition>
                                                            .createPower("HeatMetalPower",
                                                                          "",
                                                                          heat_metal.GuiPresentation.title,
                                                                          heat_metal.GuiPresentation.description,
                                                                          heat_metal.GuiPresentation.spriteReference,
                                                                          heat_metal.effectDescription,
                                                                          RuleDefinitions.ActivationTime.BonusAction,
                                                                          1,
                                                                          RuleDefinitions.UsesDetermination.Fixed,
                                                                          RuleDefinitions.RechargeRate.AtWill
                                                                          );
            power.restrictions.Add(new NewFeatureDefinitions.HasConditionRestriction(condition));
            power.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(condition_cooldown));
            power.minCustomEffectLevel = 3;
            for (int i = 2; i < 10; i++)
            {
                var eff = new EffectDescription();
                eff.Copy(power.effectDescription);
                eff.effectForms.Clear();
                eff.createdByCharacter = true;

                var damage = new EffectForm();
                damage.damageForm = new DamageForm();
                damage.FormType = EffectForm.EffectFormType.Damage;
                damage.damageForm.diceNumber = i;
                damage.DamageForm.dieType = RuleDefinitions.DieType.D8;
                damage.damageForm.damageType = Helpers.DamageTypes.Fire;
                damage.hasSavingThrow = true;
                damage.createdByCharacter = true;
                damage.levelMultiplier = 1;
                damage.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
                eff.effectForms.Add(damage);
                eff.effectForms.Add(effect_form2);
                eff.immuneCreatureFamilies = new List<string> {Helpers.Misc.createImmuneIfHasNoConditionFamily(condition_target) };
                eff.savingThrowDifficultyAbility = Helpers.Misc.createContextDeterminedAttribute(condition);
                if (i == 2)
                {
                    power.effectDescription = eff;
                }
                else
                {
                    power.levelEffectList.Add((i, eff));
                }
            }
            power.condition = condition;
            
            var condition_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.GrantPowerOnConditionApplication>.createFeature("HeatMetalGrantPowerFeature",
                                                                                                                                 "",
                                                                                                                                 Common.common_no_title,
                                                                                                                                 Common.common_no_title,
                                                                                                                                 Common.common_no_icon,
                                                                                                                                 a =>
                                                                                                                                 {
                                                                                                                                     a.condition = condition;
                                                                                                                                     a.power = power;
                                                                                                                                 }
                                                                                                                                 );
            condition.features.Add(condition_feature);
            NewFeatureDefinitions.Polymorph.transferablePowers.Add(power);

            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, heat_metal);
        }


        static void createCallLightning()
        {
            var title_string = "Spell/&CallLightningTitle";
            var description_string = "Spell/&CallLightningDescription";

            var sprite = DatabaseHelper.SpellDefinitions.Shatter.guiPresentation.spriteReference;


            var condition = Helpers.ConditionBuilder.createCondition("CallLightningCondition",
                                                                    "",
                                                                    title_string,
                                                                    description_string,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked.guiPresentation.SpriteReference,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked
                                                                    );
            condition.conditionType = RuleDefinitions.ConditionType.Beneficial;

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalLightningBlade.effectDescription);
            effect.rangeParameter = 24;
            effect.rangeType = RuleDefinitions.RangeType.Distance;
            effect.targetType = RuleDefinitions.TargetType.Cylinder;
            effect.targetParameter = 1;
            effect.targetParameter2 = 1;
            effect.savingThrowAbility = Helpers.Stats.Dexterity;
            effect.EffectForms.Clear();
            effect.targetSide = RuleDefinitions.Side.All;
            effect.durationType = RuleDefinitions.DurationType.Minute;
            effect.durationParameter = 10;
            effect.createdByCharacter = true;

            var effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 3;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D10;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Lightning;
            effect_form.hasSavingThrow = true;
            effect_form.createdByCharacter = true;
            effect_form.levelMultiplier = 1;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            effect_form.createdByCharacter = true;
            effect.EffectForms.Add(effect_form);


            effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect_form.conditionForm.forceOnSelf = true;
            effect_form.conditionForm.applyToSelf = true;
            effect_form.createdByCharacter = true;


            effect.EffectForms.Add(effect_form);

            effect.SetEffectAdvancement(DatabaseHelper.SpellDefinitions.Shatter.effectDescription.effectAdvancement);


            call_lightning = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithRestrictions>.createSpell("CallLightningSpell",
                                                                                                                   "",
                                                                                                                   title_string,
                                                                                                                   description_string,
                                                                                                                   sprite,
                                                                                                                   effect,
                                                                                                                   RuleDefinitions.ActivationTime.Action,
                                                                                                                   3,
                                                                                                                   true,
                                                                                                                   true,
                                                                                                                   true,
                                                                                                                   Helpers.SpellSchools.Conjuration
                                                                                                                   );
            call_lightning.materialComponentType = RuleDefinitions.MaterialComponentType.None;


            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithContextFromCondition>
                                                            .createPower("CallLightingPower",
                                                                          "",
                                                                          call_lightning.GuiPresentation.title,
                                                                          call_lightning.GuiPresentation.description,
                                                                          call_lightning.GuiPresentation.spriteReference,
                                                                          call_lightning.effectDescription,
                                                                          RuleDefinitions.ActivationTime.Action,
                                                                          1,
                                                                          RuleDefinitions.UsesDetermination.Fixed,
                                                                          RuleDefinitions.RechargeRate.AtWill
                                                                          );
            power.restrictions.Add(new NewFeatureDefinitions.HasConditionRestriction(condition));
            power.minCustomEffectLevel = 4;
            for (int i = 3; i < 10; i++)
            {
                var eff = new EffectDescription();
                eff.Copy(power.effectDescription);
                eff.effectForms.Clear();
                eff.createdByCharacter = true;

                var damage = new EffectForm();
                damage.damageForm = new DamageForm();
                damage.FormType = EffectForm.EffectFormType.Damage;
                damage.damageForm.diceNumber = 3;
                damage.DamageForm.dieType = RuleDefinitions.DieType.D10;
                damage.damageForm.damageType = Helpers.DamageTypes.Lightning;
                damage.hasSavingThrow = true;
                damage.createdByCharacter = true;
                damage.levelMultiplier = 1;
                damage.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
                eff.effectForms.Add(damage);
                eff.savingThrowDifficultyAbility = Helpers.Misc.createContextDeterminedAttribute(condition);
                if (i == 3)
                {
                    power.effectDescription = eff;
                }
                else
                {
                    power.levelEffectList.Add((i, eff));
                }
            }
            power.condition = condition;

            var condition_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.GrantPowerOnConditionApplication>.createFeature("CallLightingGrantPowerFeature",
                                                                                                                                 "",
                                                                                                                                 Common.common_no_title,
                                                                                                                                 Common.common_no_title,
                                                                                                                                 Common.common_no_icon,
                                                                                                                                 a =>
                                                                                                                                 {
                                                                                                                                     a.condition = condition;
                                                                                                                                     a.power = power;
                                                                                                                                 }
                                                                                                                                 );
            condition.features.Add(condition_feature);
            NewFeatureDefinitions.Polymorph.transferablePowers.Add(power);

            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, call_lightning);
        }


        static void createHellishRebuke()
        {
            var title_string = "Spell/&HellishRebukeTitle";
            var description_string = "Spell/&HellishRebukeDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("HellishRebukeCantripImage",
                                                    $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/HellishRebuke.png",
                                                    128, 128);

  
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.SacredFlame.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 12;
            effect.ignoreCover = false;


            var effect_form = new EffectForm();

            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 2;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D10;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Fire;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;

            hellish_rebuke = Helpers.GenericSpellBuilder<NewFeatureDefinitions.ReactionOnDamageSpell>.createSpell("HellishRebukeSpell",
                                                                                                                   "",
                                                                                                                   title_string,
                                                                                                                   description_string,
                                                                                                                   sprite,
                                                                                                                   effect,
                                                                                                                   RuleDefinitions.ActivationTime.Reaction,
                                                                                                                   1,
                                                                                                                   false,
                                                                                                                   true,
                                                                                                                   true,
                                                                                                                   Helpers.SpellSchools.Evocation
                                                                                                                   );
            hellish_rebuke.materialComponentType = RuleDefinitions.MaterialComponentType.None;
        }
    }
}
