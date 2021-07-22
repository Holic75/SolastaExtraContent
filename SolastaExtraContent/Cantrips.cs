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
using static FeatureDefinitionSavingThrowAffinity;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;

namespace SolastaExtraContent
{
    public class Cantrips
    {
        public static NewFeatureDefinitions.SpellFollowedByMeleeAttack sunlight_blade;
        public static SpellDefinition vicious_mockery;
        public static NewFeatureDefinitions.SpellWithRestricitons shillelagh;

        internal static void create()
        {
            createSunlightBlade();
            createViciousMockery();
            createShillelagh();
        }


        static void createShillelagh()
        {
            var title_string = "Spell/&ShillelaghTitle";
            var description_string = "Spell/&ShillelaghDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("ShillelaghCantripImage",
                                                    $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/Shillelagh.png",
                                                    128, 128);
            var feature = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("ShillelaghWeaponFeature",
                                                                                             "",
                                                                                             title_string,
                                                                                             description_string,
                                                                                             sprite);

            var caster_stat_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ReplaceWeaponAbilityScoreWithHighestStatIfWeaponHasFeature>
                                                  .createFeature("ShillelaghAbilityScoreFeature",
                                                                 "",
                                                                 Common.common_no_title,
                                                                 Common.common_no_title,
                                                                 Common.common_no_icon,
                                                                 a =>
                                                                 {
                                                                     a.abilityScores = new List<string> { Helpers.Stats.Charisma, Helpers.Stats.Intelligence, Helpers.Stats.Wisdom };
                                                                     a.weaponFeature = feature;
                                                                 }
                                                                 );

            var damage_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.OverwriteDamageOnWeaponWithFeature>
                                                              .createFeature("ShillelaghDamageFeature",
                                                                             "",
                                                                             Common.common_no_title,
                                                                             Common.common_no_title,
                                                                             Common.common_no_icon,
                                                                             a =>
                                                                             {
                                                                                 a.numDice = 1;
                                                                                 a.dieType = RuleDefinitions.DieType.D8;
                                                                                 a.weaponFeature = feature;
                                                                             }
                                                                             );


            var tag_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.AddAttackTagonWeaponWithFeature>
                                                  .createFeature("ShillelaghTagFeature",
                                                                 "",
                                                                 Common.common_no_title,
                                                                 Common.common_no_title,
                                                                 Common.common_no_icon,
                                                                 a =>
                                                                 {
                                                                     a.tag = "Magical";
                                                                     a.weaponFeature = feature;
                                                                 }
                                                                 );

            var condition = Helpers.ConditionBuilder.createCondition("ShillelaghCondition",
                                                                    "",
                                                                    title_string,
                                                                    Common.common_no_title,
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionMagicalWeapon,
                                                                    caster_stat_feature,
                                                                    damage_feature,
                                                                    tag_feature
                                                                    );
            condition.conditionTags.Clear();
            condition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
           
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 1;
            effect.durationParameter = 1;
            effect.itemSelectionType = ActionDefinitions.ItemSelectionType.Weapon;
            effect.targetType = RuleDefinitions.TargetType.Self;
            effect.rangeType = RuleDefinitions.RangeType.Self;
            
            effect.durationType = RuleDefinitions.DurationType.Minute;           

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect_form.ConditionForm.applyToSelf = true;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.itemPropertyForm = new ItemPropertyForm();
            effect_form.FormType = EffectForm.EffectFormType.ItemProperty;
            effect_form.itemPropertyForm.featureBySlotLevel = new List<FeatureUnlockByLevel> { new FeatureUnlockByLevel(feature, 0) };
            effect.EffectForms.Add(effect_form);
            effect.effectAdvancement.Clear();

            shillelagh = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithRestricitons>.createSpell("ShillelaghSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.BonusAction,
                                                                                       0,
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       Helpers.SpellSchools.Transmutation
                                                                                       );
            shillelagh.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            shillelagh.uniqueInstance = true;
            var allowed_weapons = new List<string> { Helpers.WeaponProficiencies.Club, Helpers.WeaponProficiencies.QuarterStaff };
            shillelagh.restrictions = new List<NewFeatureDefinitions.IRestriction> { new NewFeatureDefinitions.SpecificWeaponInMainHandRestriction(allowed_weapons) };
        }


        static void createViciousMockery()
        {
            var title_string = "Spell/&ViciousMockeryTitle";
            var description_string = "Spell/&ViciousMockeryDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("ViciousMockeryCantripImage",
                                                    $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/ViciousMockery.png",
                                                    128, 128);

            var disadvantage = Helpers.CopyFeatureBuilder<FeatureDefinitionCombatAffinity>.createFeatureCopy("ViciousMockeryFeature",
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
            var condition = Helpers.ConditionBuilder.createConditionWithInterruptions("ViciousMockeryCondition",
                                                                                      "",
                                                                                      title_string,
                                                                                      Common.common_no_title,
                                                                                      null,
                                                                                      DatabaseHelper.ConditionDefinitions.ConditionCursedByBestowCurseAttackRoll,
                                                                                      new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks },
                                                                                      disadvantage
                                                                                      );
            condition.conditionTags.Clear();
            condition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(condition);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.HideousLaughter.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 12;
            effect.durationType = RuleDefinitions.DurationType.Round;
            

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 1;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D4;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Psychic;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.CasterLevelTable;

            vicious_mockery = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("ViciousMockerySpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       0,
                                                                                       false,
                                                                                       true,
                                                                                       false,
                                                                                       Helpers.SpellSchools.Enchantment
                                                                                       );
            vicious_mockery.materialComponentType = RuleDefinitions.MaterialComponentType.None;
        }


        static void createSunlightBlade()
        {
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("SunlightBladeCantripImage",
                                                                $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/SunlightBlade.png",
                                                                128, 128);

            var title_string = "Spell/&SunlightBladeTitle";
            var description_string = "Spell/&SunlightBladeDescription";
            var levels = new int[] { 4, 10, 16, 20 };
            List<(int, EffectDescription)> effects = new List<(int, EffectDescription)>();

            var highlighted_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("SunlightBladeHighlightedCondition",
                                                                                                  "",
                                                                                                  "",
                                                                                                  "",
                                                                                                  null,
                                                                                                  DatabaseHelper.ConditionDefinitions.ConditionHighlighted,
                                                                                                  new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacked },
                                                                                                  DatabaseHelper.ConditionDefinitions.ConditionHighlighted.Features[0]
                                                                                                  );

            var on_hit_effect = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnDamageDone>
                                                            .createFeature("SunlightBladeHighlightedOnHitEffect",
                                                                           "",
                                                                           Common.common_no_title,
                                                                           Common.common_no_title,
                                                                           Common.common_no_icon,
                                                                           a =>
                                                                           {
                                                                               a.durationType = RuleDefinitions.DurationType.Round;
                                                                               a.durationValue = 1;
                                                                               a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                                               a.condition = highlighted_condition;
                                                                               a.onlyWeapon = true;
                                                                           }
                                                                           );

            for (int i = 0; i < levels.Length; i++)
            {
                var lvl = levels[i];
                var extra_damage = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("SunlightBladeExtraDamage" + lvl.ToString(),
                                                                                                                   "",
                                                                                                                   Common.common_no_title,
                                                                                                                   Common.common_no_title,
                                                                                                                   Common.common_no_icon,
                                                                                                                   DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor,
                                                                                                                   a =>
                                                                                                                   {
                                                                                                                       a.damageAdvancement = RuleDefinitions.AdditionalDamageAdvancement.None;
                                                                                                                       a.specificDamageType = Helpers.DamageTypes.Radiant;
                                                                                                                       a.SetDamageDieType(RuleDefinitions.DieType.D8);
                                                                                                                       a.SetDamageDiceNumber(i);
                                                                                                                       a.SetNotificationTag("SunlightBlade");
                                                                                                                       
                                                                                                                   }
                                                                                                                   );
                extra_damage.SetAddLightSource(true);
                extra_damage.lightSourceForm = DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageBrandingSmite.LightSourceForm;
                var condition = Helpers.ConditionBuilder.createConditionWithInterruptions("SunlightBladeCasterCondition" + lvl.ToString(),
                                                                                          "",
                                                                                          title_string,
                                                                                          description_string,
                                                                                          null,
                                                                                          DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle,
                                                                                          new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks },
                                                                                          extra_damage,
                                                                                          on_hit_effect);

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.DivineFavor.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Touch);
                effect.SetRangeParameter(1);
                effect.DurationParameter = 1;
                effect.DurationType = RuleDefinitions.DurationType.Round;
                effect.EffectForms.Clear();
                effect.SetTargetType(RuleDefinitions.TargetType.Individuals);
                effect.SetTargetSide(RuleDefinitions.Side.Enemy);
                effect.SetTargetParameter(1);
                effect.SetTargetParameter2(1);

                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = condition;
                effect_form.conditionForm.applyToSelf = true;
                effect.EffectForms.Add(effect_form);

                effects.Add((lvl, effect));
            }
            sunlight_blade = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellFollowedByMeleeAttack>.createSpell("SunlightBladeSpell",
                                                                                        "",
                                                                                        title_string,
                                                                                        description_string,
                                                                                        sprite, //DatabaseHelper.SpellDefinitions.FlameBlade.GuiPresentation.SpriteReference,
                                                                                        effects[0].Item2,
                                                                                        RuleDefinitions.ActivationTime.Action,
                                                                                        0,
                                                                                        false,
                                                                                        true,
                                                                                        true,
                                                                                        Helpers.SpellSchools.Evocation
                                                                                        );
            sunlight_blade.materialComponentType = RuleDefinitions.MaterialComponentType.None;
            sunlight_blade.levelEffectList = effects.Skip(1).ToList();
            sunlight_blade.minCustomEffectLevel = 5;
            DatabaseHelper.SpellListDefinitions.SpellListWizard.spellsByLevel[0].spells.Add(sunlight_blade);
            DatabaseHelper.SpellListDefinitions.SpellListSorcerer.spellsByLevel[0].spells.Add(sunlight_blade);
        }
    }
}
