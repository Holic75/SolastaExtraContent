using SolastaModApi;
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
        public static NewFeatureDefinitions.SpellWithRestrictions shillelagh;
        public static SpellDefinition frostbite;
        //public static SpellDefinition produce_flame;
        public static SpellDefinition thunder_strike;
        public static SpellDefinition air_blast;
        public static SpellDefinition burst_of_radiance;
        public static SpellDefinition acid_claws;

        internal static void create()
        {
            createSunlightBlade();
            createViciousMockery();
            createShillelagh();
            createFrostbite();
            createAirBlast();
            createThunderstrike();
            createBurstOfRadiance();
            createAcidClaws();
        }


        static void createAcidClaws()
        {
            var title_string = "Spell/&AcidClawsTitle";
            var description_string = "Spell/&AcidClawsDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("AcidClawsCantripImage",
                                                    $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/AcidCLaws.png",
                                                    128, 128);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.VampiricTouchIntelligence.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.durationParameter = 1;
            effect.hasSavingThrow = false;
            effect.durationType = RuleDefinitions.DurationType.Instantaneous;
            effect.rangeType = RuleDefinitions.RangeType.MeleeHit;
            effect.targetType = RuleDefinitions.TargetType.Individuals;
            effect.targetSide = RuleDefinitions.Side.Enemy;


            var effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 1;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D10;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Necrotic;
            effect_form.hasSavingThrow = false;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.None;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.CasterLevelTable;
            //effect.effectParticleParameters.casterParticleReference = DatabaseHelper.SpellDefinitions.EnhanceAbilityBullsStrength.effectDescription.effectParticleParameters.casterParticleReference;

            acid_claws = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("AcidClawsSpell",
                                                                                "",
                                                                                title_string,
                                                                                description_string,
                                                                                sprite,
                                                                                effect,
                                                                                RuleDefinitions.ActivationTime.Action,
                                                                                0,
                                                                                false,
                                                                                false,
                                                                                true,
                                                                                Helpers.SpellSchools.Necromancy
                                                                                );
            acid_claws.materialComponentType = RuleDefinitions.MaterialComponentType.None;
        }


        static void createThunderstrike()
        {
            var title_string = "Spell/&ThunderStrikeTitle";
            var description_string = "Spell/&ThunderStrikeDescription";
            var sprite = DatabaseHelper.SpellDefinitions.Shield.guiPresentation.spriteReference;

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Shatter.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 0;
            effect.targetParameter = 2;
            effect.targetParameter2 = 2;
            effect.durationType = RuleDefinitions.DurationType.Instantaneous;
            effect.savingThrowAbility = Helpers.Stats.Constitution;
            effect.rangeType = RuleDefinitions.RangeType.Self;
            effect.targetType = RuleDefinitions.TargetType.Sphere;
            effect.targetSide = RuleDefinitions.Side.All;
            effect.targetExcludeCaster = true;


            var effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 1;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D6;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Thundering;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.CasterLevelTable;

            thunder_strike = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("ThunderStrikeSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       0,
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       Helpers.SpellSchools.Evocation
                                                                                       );
            thunder_strike.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            DatabaseHelper.SpellListDefinitions.SpellListWizard.spellsByLevel[0].spells.Add(thunder_strike);
            DatabaseHelper.SpellListDefinitions.SpellListSorcerer.spellsByLevel[0].spells.Add(thunder_strike);
        }


        static void createBurstOfRadiance()
        {
            var title_string = "Spell/&BurstOfRadianceTitle";
            var description_string = "Spell/&BurstOfRadianceDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("BurstOfRadianceCantripImage",
                                                    $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/BurstOfRadiance.png",
                                                    128, 128);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerFireOspreyBlast.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 0;
            effect.targetParameter = 2;
            effect.targetParameter = 2;
            effect.targetParameter2 = 2;
            effect.durationType = RuleDefinitions.DurationType.Instantaneous;
            effect.savingThrowAbility = Helpers.Stats.Constitution;
            effect.rangeType = RuleDefinitions.RangeType.Self;
            effect.targetType = RuleDefinitions.TargetType.Sphere;
            effect.targetSide = RuleDefinitions.Side.Enemy;
            effect.targetExcludeCaster = true;
            effect.restrictedCreatureFamilies = new List<string>();
            effect.effectParticleParameters.casterParticleReference = DatabaseHelper.SpellDefinitions.BurningHands.effectDescription.effectParticleParameters.casterParticleReference;

            var effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 1;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D6;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Radiant;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.CasterLevelTable;

            burst_of_radiance = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("BurstOfRadianceSpell",
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
                                                                                       Helpers.SpellSchools.Evocation
                                                                                       );
            burst_of_radiance.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            DatabaseHelper.SpellListDefinitions.SpellListCleric.spellsByLevel[0].spells.Add(burst_of_radiance);
            
        }


        static void createAirBlast()
        {
            var title_string = "Spell/&AirBlastTitle";
            var description_string = "Spell/&AirBlastDescription";
            var sprite = DatabaseHelper.SpellDefinitions.GustOfWind.guiPresentation.spriteReference;

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.ShadowDagger.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 6;
            effect.durationType = RuleDefinitions.DurationType.Instantaneous;
            effect.savingThrowAbility = Helpers.Stats.Strength;
            effect.rangeType = RuleDefinitions.RangeType.Distance;
            effect.targetType = RuleDefinitions.TargetType.Individuals;
            effect.targetSide = RuleDefinitions.Side.Enemy;


            var effect_form = new EffectForm();
            effect_form.motionForm = new MotionForm();
            effect_form.FormType = EffectForm.EffectFormType.Motion;
            effect_form.motionForm.distance = 1;
            effect_form.motionForm.type = MotionForm.MotionType.PushFromOrigin;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 1;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D6;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Bludgeoning;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.CasterLevelTable;

            air_blast = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("AirBlastSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       0,
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       Helpers.SpellSchools.Transmutation
                                                                                       );
            air_blast.materialComponentType = RuleDefinitions.MaterialComponentType.None;
            DatabaseHelper.SpellListDefinitions.SpellListWizard.spellsByLevel[0].spells.Add(air_blast);
            DatabaseHelper.SpellListDefinitions.SpellListSorcerer.spellsByLevel[0].spells.Add(air_blast);
        }


        static void createFrostbite()
        {
            var title_string = "Spell/&IceStrikeTitle";
            var description_string = "Spell/&IceStrikeDescription";
            var sprite = DatabaseHelper.SpellDefinitions.SacredFlame_B.guiPresentation.spriteReference;

            var disadvantage = Helpers.FeatureBuilder<NewFeatureDefinitions.DisadvantageOnWeaponAttack>.createFeature("IceStrikeFeature",
                                                                                                             "",
                                                                                                             Common.common_no_title,
                                                                                                             Common.common_no_title,
                                                                                                             Common.common_no_icon                                                                                                         
                                                                                                             );
            var condition = Helpers.ConditionBuilder.createConditionWithInterruptions("IceStrikeCondition",
                                                                                      "",
                                                                                      title_string,
                                                                                      "Rules/&ConditionWeaponAttackDisadvantageDescription",
                                                                                      null,
                                                                                      DatabaseHelper.ConditionDefinitions.ConditionCursedByBestowCurseAttackRoll,
                                                                                      new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks },
                                                                                      disadvantage
                                                                                      );
            condition.conditionTags.Clear();
            condition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(condition);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.RayOfFrost.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 12;
            effect.hasSavingThrow = true;
            effect.durationType = RuleDefinitions.DurationType.Round;
            effect.savingThrowAbility = Helpers.Stats.Constitution;
            effect.rangeType = RuleDefinitions.RangeType.Distance;
            effect.targetType = RuleDefinitions.TargetType.Individuals;
            effect.targetSide = RuleDefinitions.Side.Enemy;


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
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D6;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Cold;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.CasterLevelTable;

            frostbite = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("IceStrikeSpell",
                                                                                "",
                                                                                title_string,
                                                                                description_string,
                                                                                sprite,
                                                                                effect,
                                                                                RuleDefinitions.ActivationTime.Action,
                                                                                0,
                                                                                false,
                                                                                true,
                                                                                true,
                                                                                Helpers.SpellSchools.Evocation
                                                                                );
            frostbite.materialComponentType = RuleDefinitions.MaterialComponentType.None;
            DatabaseHelper.SpellListDefinitions.SpellListWizard.spellsByLevel[0].spells.Add(frostbite);
            DatabaseHelper.SpellListDefinitions.SpellListSorcerer.spellsByLevel[0].spells.Add(frostbite);
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
                                                                                             Common.common_no_title,
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
                                                                    title_string,
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionMagicalWeapon,
                                                                    caster_stat_feature,
                                                                    damage_feature,
                                                                    tag_feature
                                                                    );
            condition.conditionTags.Clear();
            condition.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
            condition.conditionType = RuleDefinitions.ConditionType.Beneficial;
            condition.possessive = true;
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

            shillelagh = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithRestrictions>
                                                                        .createSpell("ShillelaghSpell",
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
                                                                                      "Rules/&ConditionAttackDisadvantageDescription",
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
                                                                                        false,
                                                                                        true,
                                                                                        Helpers.SpellSchools.Evocation
                                                                                        );
            sunlight_blade.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            sunlight_blade.levelEffectList = effects.Skip(1).ToList();
            sunlight_blade.minCustomEffectLevel = 5;
            DatabaseHelper.SpellListDefinitions.SpellListWizard.spellsByLevel[0].spells.Add(sunlight_blade);
            DatabaseHelper.SpellListDefinitions.SpellListSorcerer.spellsByLevel[0].spells.Add(sunlight_blade);
        }
    }
}
