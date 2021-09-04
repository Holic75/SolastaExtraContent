using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModHelpers;
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
        static public NewFeatureDefinitions.SpellWithSlotLevelDependentEffects flame_blade;
        static public SpellDefinition conjure_spirit_animal;
        static public SpellDefinition winter_blast;
        static public SpellDefinition spike_growth;
        static public SpellDefinition vulnerability_hex;
        static public SpellDefinition earth_tremor;

        static public SpellDefinition polymorph;

        internal static void create()
        {
            createHellishRebuke();
            createCallLightning();
            createHeatMetal();
            createFlameBlade();
            //createPolymorph();
            createConjureSpiritAnimal();
            createWinterBlast();

            var entangle_effect = DatabaseHelper.SpellDefinitions.Entangle.effectDescription;
            entangle_effect.targetType = RuleDefinitions.TargetType.Cylinder;
            entangle_effect.targetParameter = 3;
            entangle_effect.targetParameter2 = 1;
            entangle_effect.effectParticleParameters.zoneParticleReference = null;

            fixSurfaceSpell(DatabaseHelper.SpellDefinitions.Entangle);
            fixSurfaceSpell(DatabaseHelper.SpellDefinitions.Grease);
            fixSurfaceSpell(DatabaseHelper.SpellDefinitions.BlackTentacles);
            fixGiantInsects();
            fixConjureMinorElemental();

            createSpikeGrowth();
            createVulnerabilityHex();
            createEarthTremor();
        }

        static void fixConjureMinorElemental()
        {
           DatabaseHelper.SpellDefinitions.ConjureMinorElementalsTwo.guiPresentation.description = "Spell/&SolastaExtraContentConjureMinorElementalsTwoDescription";
           DatabaseHelper.SpellDefinitions.ConjureMinorElementalsFour.guiPresentation.description = "Spell/&SolastaExtraContentConjureMinorElementalsOneDescription";
           DatabaseHelper.SpellDefinitions.ConjureMinorElementalsOne.guiPresentation.description = "Spell/&SolastaExtraContentConjureMinorElementalsOne2Description";
           //DatabaseHelper.SpellDefinitions.ConjureMinorElementalsFour.effectDescription.effectForms.Find(f => f.formType == EffectForm.EffectFormType.Summon).summonForm.number = 1;
           DatabaseHelper.SpellDefinitions.ConjureMinorElementalsFour.effectDescription.targetParameter = 1;
            DatabaseHelper.SpellDefinitions.ConjureMinorElementalsFour.guiPresentation.title = DatabaseHelper.SpellDefinitions.ConjureMinorElementalsOne.guiPresentation.title;
        }



        static void fixGiantInsects()
        {
            //Deep spider is actually similar in stats to giant spider (even worse since it has light sensitivity), dnd spell allows to su
            var spell = DatabaseHelper.SpellDefinitions.GiantInsect;
            spell.effectDescription.effectForms.Find(f => f.formType == EffectForm.EffectFormType.Summon).summonForm.number = 3;
        }

        static void createEarthTremor()
        {
            var title_string = "Spell/&EarthTremorTitle";
            var description_string = "Spell/&EarthTremorDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("EarthTremorSpellImage",
                                         $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/EarthTremor.png",
                                         128, 128);

            var rubble_proxy = Helpers.CopyFeatureBuilder<EffectProxyDefinition>.createFeatureCopy("RubbleProxy",
                                                                                                   "",
                                                                                                   title_string,
                                                                                                   description_string,
                                                                                                   sprite,
                                                                                                   DatabaseHelper.EffectProxyDefinitions.ProxyGrease
                                                                                                   );

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Shatter.EffectDescription);
            effect.targetType = RuleDefinitions.TargetType.Cylinder;
            effect.rangeParameter = 24;
            effect.rangeType = RuleDefinitions.RangeType.Distance;
            effect.targetParameter = 2;
            effect.targetParameter2 = 1;
            effect.EffectForms.Clear();
            effect.targetExcludeCaster = false;
            effect.durationType = RuleDefinitions.DurationType.Minute;
            effect.durationParameter = 10;
            effect.hasSavingThrow = true;
            effect.savingThrowAbility = Helpers.Stats.Dexterity;
            effect.difficultyClassComputation = RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
            effect.effectParticleParameters.activeEffectSurfaceStartParticleReference = DatabaseHelper.SpellDefinitions.Grease.effectDescription.effectParticleParameters.activeEffectSurfaceStartParticleReference;
            effect.effectParticleParameters.activeEffectSurfaceParticleReference = DatabaseHelper.SpellDefinitions.Grease.effectDescription.effectParticleParameters.activeEffectSurfaceParticleReference;
            effect.effectParticleParameters.activeEffectSurfaceEndParticleReference = DatabaseHelper.SpellDefinitions.Grease.effectDescription.effectParticleParameters.activeEffectSurfaceEndParticleReference;

            var effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 3;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D12;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Bludgeoning;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            effect.EffectForms.Add(effect_form);


            effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.motionForm = new MotionForm();
            effect_form.FormType = EffectForm.EffectFormType.Motion;
            effect_form.MotionForm.type = MotionForm.MotionType.FallProne;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.summonForm = new SummonForm();
            effect_form.FormType = EffectForm.EffectFormType.Summon;
            effect_form.summonForm.summonType = SummonForm.Type.EffectProxy;
            effect_form.summonForm.effectProxyDefinitionName = rubble_proxy.name;
            effect.EffectForms.Add(effect_form);

            effect.EffectForms.Add(DatabaseHelper.SpellDefinitions.Grease.effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Topology));

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;

            earth_tremor = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("EarthTremorSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       3,
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       Helpers.SpellSchools.Transmutation
                                                                                       );
            earth_tremor.materialComponentType = RuleDefinitions.MaterialComponentType.None;
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, earth_tremor);
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizard, earth_tremor);
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListSorcerer, earth_tremor);
        }


        static void createVulnerabilityHex()
        {
            var title_string = "Spell/&VulnerabilityHexTitle";
            var description_string = "Spell/&VulnerabilityHexDescription";
            var sprite = DatabaseHelper.SpellDefinitions.HuntersMark.guiPresentation.spriteReference;

            var weapon_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("AdditionalDamageVulnerabilityHexWeapon",
                                                                                                         "",
                                                                                                         Common.common_no_title,
                                                                                                         Common.common_no_title,
                                                                                                         Common.common_no_icon,
                                                                                                         DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageHuntersMark,
                                                                                                         a =>
                                                                                                         {
                                                                                                             a.requiredTargetCondition = null;
                                                                                                             a.notificationTag = "VulnerabilityHex";
                                                                                                         }
                                                                                                         );
            
            var spells_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("AdditionalDamageVulnerabilityHexSpell",
                                                                                                         "",
                                                                                                         Common.common_no_title,
                                                                                                         Common.common_no_title,
                                                                                                         Common.common_no_icon,
                                                                                                         weapon_feature,
                                                                                                         a =>
                                                                                                         {
                                                                                                             a.triggerCondition = (RuleDefinitions.AdditionalDamageTriggerCondition)ExtendedEnums.AdditionalDamageTriggerCondition.MagicalAttacksOnTargetWithConditionFromMe;
                                                                                                             a.requiredTargetCondition = null;
                                                                                                             a.attackModeOnly = false;
                                                                                                             a.notificationTag = "VulnerabilityHex";
                                                                                                         }
                                                                                                         );
            var condition = Helpers.CopyFeatureBuilder<ConditionDefinition>.createFeatureCopy("HexSpellTargetCondition",
                                                                                              "",
                                                                                              "Rules/&ConditionVulnerabilityHexTitle",
                                                                                              "Rules/&ConditionVulnerabilityHexDescription",
                                                                                              null,
                                                                                              DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter,
                                                                                              a =>
                                                                                              {
                                                                                                  a.conditionTags = new List<string> { "Curse" };
                                                                                              }
                                                                                              );
            spells_feature.requiredTargetCondition = condition;
            weapon_feature.requiredTargetCondition = condition;

            var caster_condition = Helpers.CopyFeatureBuilder<ConditionDefinition>.createFeatureCopy("HexSpellCasterCondition",
                                                                                              "",
                                                                                              "",
                                                                                              "",
                                                                                              null,
                                                                                              DatabaseHelper.ConditionDefinitions.ConditionHuntersMark,
                                                                                              a =>
                                                                                              {
                                                                                                  a.features = new List<FeatureDefinition>
                                                                                                  {
                                                                                                      weapon_feature,
                                                                                                      spells_feature
                                                                                                  };
                                                                                              }
                                                                                              );
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.HuntersMark.EffectDescription);
            effect.effectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = caster_condition;
            effect_form.ConditionForm.applyToSelf = true;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect.EffectForms.Add(effect_form);

            vulnerability_hex = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("VulnerabilityHexSpell",
                                                               "",
                                                               title_string,
                                                               description_string,
                                                               sprite,
                                                               effect,
                                                               RuleDefinitions.ActivationTime.BonusAction,
                                                               1,
                                                               true,
                                                               true,
                                                               true,
                                                               Helpers.SpellSchools.Enchantment
                                                               );
            vulnerability_hex.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
        }


        static void fixSurfaceSpell(SpellDefinition spell)
        {
            var effect = spell.effectDescription;
            effect.targetType = RuleDefinitions.TargetType.Cylinder;
            effect.targetParameter--;
            effect.targetParameter2 = 1;
            effect.effectParticleParameters.zoneParticleReference = null;            
        }


        static void createSpikeGrowth()
        {
            var title_string = "Spell/&SpikeGrowthTitle";
            var description_string = "Spell/&SpikeGrowthDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("SpikeGrowthSpellImage",
                                         $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/SpikeGrowth.png",
                                         128, 128);

            var spikes_proxy = Helpers.CopyFeatureBuilder<EffectProxyDefinition>.createFeatureCopy("SpikeGrowthProxy",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       DatabaseHelper.EffectProxyDefinitions.ProxyEntangle
                                                                                       );

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Entangle.effectDescription);
            effect.hasSavingThrow = false;
            effect.effectForms.Clear();
            var effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.summonForm = new SummonForm();
            effect_form.FormType = EffectForm.EffectFormType.Summon;
            effect_form.summonForm.summonType = SummonForm.Type.EffectProxy;
            effect_form.summonForm.effectProxyDefinitionName = spikes_proxy.name;
            effect.EffectForms.Add(effect_form);
            effect.effectForms.Add(DatabaseHelper.SpellDefinitions.Entangle.effectDescription.effectForms.Find(e => e.formType == EffectForm.EffectFormType.Topology));

            var damage_form = new EffectForm();
            damage_form.formType = EffectForm.EffectFormType.Damage;
            damage_form.damageForm = new DamageForm();
            damage_form.damageForm.dieType = RuleDefinitions.DieType.D4;
            damage_form.damageForm.diceNumber = 2;
            damage_form.damageForm.damageType = Helpers.DamageTypes.Piercing;

            var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyEffectFormsOnTargetMoved>.createFeature("SpikeGrowthDamageFeature",
                                                                                                                    "",
                                                                                                                    title_string,
                                                                                                                    title_string,
                                                                                                                    Common.common_no_icon,
                                                                                                                    a =>
                                                                                                                    {
                                                                                                                        a.effectForms = new List<EffectForm> { damage_form };
                                                                                                                    }
                                                                                                                    );

            var condition = Helpers.ConditionBuilder.createCondition("SpikeGrowthDamageCondition",
                                                            "",
                                                            "Rules/&SpikeGrowthDamageConditionTitle",
                                                            "Rules/&SpikeGrowthDamageConditionDescription",
                                                            DatabaseHelper.ConditionDefinitions.ConditionSlowed.guiPresentation.spriteReference,
                                                            DatabaseHelper.ConditionDefinitions.ConditionSlowed,
                                                            feature
                                                            );
            condition.interruptionRequiresSavingThrow = false;

            effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect.EffectForms.Add(effect_form);
            effect.durationParameter = 10;
            effect.rangeParameter = 30;
            spike_growth = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("SpikeGrowthSpell",
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
            spike_growth.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, spike_growth);
        }



        static void createWinterBlast()
        {
            var title_string = "Spell/&WinterBlastTitle";
            var description_string = "Spell/&WinterBlastDescription";
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("WinterBlastSpellImage",
                                         $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/WinterBlast.png",
                                         128, 128);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerWinterWolfBreath.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.durationType = RuleDefinitions.DurationType.Instantaneous;
            effect.hasSavingThrow = true;
            effect.savingThrowAbility = Helpers.Stats.Dexterity;
            effect.difficultyClassComputation = RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency;
            effect.effectParticleParameters.casterParticleReference = DatabaseHelper.SpellDefinitions.ConeOfCold.effectDescription.effectParticleParameters.casterParticleReference;

            var effect_form = new EffectForm();
            effect_form.damageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.damageForm.diceNumber = 4;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D8;
            effect_form.damageForm.damageType = Helpers.DamageTypes.Cold;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
            effect.EffectForms.Add(effect_form);


            effect_form = new EffectForm();
            effect_form.motionForm = new MotionForm();
            effect_form.FormType = EffectForm.EffectFormType.Motion;
            effect_form.MotionForm.type = MotionForm.MotionType.FallProne;
            effect_form.hasSavingThrow = true;
            effect_form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            effect.effectAdvancement.additionalDicePerIncrement = 1;
            effect.effectAdvancement.incrementMultiplier = 1;
            effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;

            winter_blast = Helpers.GenericSpellBuilder<SpellDefinition>.createSpell("WinterBlastSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       effect,
                                                                                       RuleDefinitions.ActivationTime.Action,
                                                                                       3,
                                                                                       false,
                                                                                       true,
                                                                                       true,
                                                                                       Helpers.SpellSchools.Conjuration
                                                                                       );
            winter_blast.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, winter_blast);
        }


        static void createConjureSpiritAnimal()
        {
            var attack_bonus = Helpers.FeatureBuilder<NewFeatureDefinitions.AttackBonusEqualToCasterSpellcastingBonus>
                                                       .createFeature("ConjureSpiritBeastAttackBonus",
                                                                       "",
                                                                       "Spell/&ConjureSpiritAnimalSpellTitle",
                                                                       Common.common_no_title,
                                                                       Common.common_no_icon
                                                                       );

            var condition = Helpers.ConditionBuilder.createCondition("ConjureSpiritBeastAttackBonusCondition",
                                                                        "",
                                                                        "Spell/&ConjureSpiritAnimalSpellTitle",
                                                                        Common.common_no_title,
                                                                        null,
                                                                        DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                        attack_bonus
                                                                        );

            List<SpellDefinition> variants = new List<SpellDefinition>();

            List<(MonsterDefinition base_monster, MonsterDefinition fx_monster, string prefix, int base_hd, FeatureDefinition[] features)> summon_infos
                = new List<(MonsterDefinition base_monster, MonsterDefinition fx_monster, string prefix, int base_hd, FeatureDefinition[] features)>()
                {
                    (DatabaseHelper.MonsterDefinitions.Wolf, DatabaseHelper.MonsterDefinitions.Earth_Elemental, "Land", 4, new FeatureDefinition[]{DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6, DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityPackTactics }),
                    (DatabaseHelper.MonsterDefinitions.Giant_Eagle, DatabaseHelper.MonsterDefinitions.Air_Elemental, "Air", 2, new FeatureDefinition[]{DatabaseHelper.FeatureDefinitionMoveModes.MoveModeFly12, DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityFlyby }),
                };

            foreach (var summon_info in summon_infos)
            {
                var prefix = "ConjureSpiritAnimal" + summon_info.prefix + "Spell";
                List<EffectDescription> effect_descriptions = new List<EffectDescription>();
                for (int i = 2; i <= 9; i++)
                {
                    var monster = createSpiritAnimal(prefix, i, summon_info.base_hd, summon_info.base_monster, summon_info.fx_monster, summon_info.features);
                    var effect_description = new EffectDescription();
                    effect_description.Copy(DatabaseHelper.SpellDefinitions.ConjureAnimalsOneBeast.EffectDescription);
                    effect_description.SetDurationType(RuleDefinitions.DurationType.Hour);
                    effect_description.durationParameter = 1;
                    effect_description.effectForms.Clear();
                    var form = new EffectForm();
                    form.formType = EffectForm.EffectFormType.Summon;
                    form.summonForm = new SummonForm();
                    form.summonForm.conditionDefinition = condition;
                    form.summonForm.decisionPackage = DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default;
                    form.summonForm.monsterDefinitionName = monster.name;
                    effect_description.effectForms.Add(form);

                    effect_descriptions.Add(effect_description);
                    effect_description.effectAdvancement.Clear();
                    effect_description.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;
                    effect_description.effectAdvancement.incrementMultiplier = 1;
                }


                var variant = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithSlotLevelDependentEffects>
                                                                                    .createSpell(prefix,
                                                                                                "",
                                                                                                $"Spell/&{prefix}Title",
                                                                                                $"Spell/&{prefix}Description",
                                                                                                DatabaseHelper.SpellDefinitions.ConjureAnimals.GuiPresentation.spriteReference,
                                                                                                effect_descriptions[0],
                                                                                                RuleDefinitions.ActivationTime.Action,
                                                                                                2,
                                                                                                true,
                                                                                                true,
                                                                                                true,
                                                                                                Helpers.SpellSchools.Conjuration
                                                                                                );
                variant.materialComponentType = RuleDefinitions.MaterialComponentType.Mundane;


                variant.minCustomEffectLevel = 3;
                for (int i = 3; i <= 9; i++)
                {
                    variant.levelEffectList.Add((i, effect_descriptions[i - 2]));
                }
                variants.Add(variant);
            }

            conjure_spirit_animal = Helpers.CopyFeatureBuilder<SpellDefinition>.createFeatureCopy("ConjureSpiritAnimalSpell",
                                                                                                  "",
                                                                                                  "Spell/&ConjureSpiritAnimalSpellTitle",
                                                                                                  "Spell/&ConjureSpiritAnimalSpellDescription",
                                                                                                  null,
                                                                                                  DatabaseHelper.SpellDefinitions.ConjureAnimals,
                                                                                                  a =>
                                                                                                  {
                                                                                                      a.subspellsList = variants;
                                                                                                      a.spellLevel = 2;
                                                                                                      a.effectDescription.Copy(a.EffectDescription);
                                                                                                      a.effectDescription.SetEffectAdvancement(variants[0].effectDescription.EffectAdvancement);
                                                                                                  }
                                                                                                  );

            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, conjure_spirit_animal);
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListRanger, conjure_spirit_animal);
            NewFeatureDefinitions.SpellData.registerSpell(conjure_spirit_animal);
        }


        static MonsterDefinition createSpiritAnimal(string prefix, int level, int base_hd, MonsterDefinition base_monster, MonsterDefinition fx_monster, params FeatureDefinition[] extra_features)
        {
            var attack = Helpers.CopyFeatureBuilder<MonsterAttackDefinition>.createFeatureCopy(prefix + "Attack" + level.ToString(),
                                                                                               "",
                                                                                               "",
                                                                                               "",
                                                                                               null,
                                                                                               base_monster.AttackIterations[0].monsterAttackDefinition,
                                                                                               a =>
                                                                                               {
                                                                                                   a.toHitBonus = 0;
                                                                                                   var effect = new EffectDescription();
                                                                                                   effect.Copy(a.EffectDescription);
                                                                                                   effect.EffectForms.Clear();
                                                                                                   effect.HasSavingThrow = false;

                                                                                                   var dmg = new EffectForm();
                                                                                                   dmg.FormType = EffectForm.EffectFormType.Damage;
                                                                                                   dmg.DamageForm = new DamageForm();
                                                                                                   dmg.DamageForm.bonusDamage = 4 + level;
                                                                                                   dmg.DamageForm.diceNumber = 1;
                                                                                                   dmg.DamageForm.dieType = RuleDefinitions.DieType.D8;
                                                                                                   dmg.DamageForm.damageType = Helpers.DamageTypes.Piercing;
                                                                                                   effect.EffectForms.Add(dmg);
                                                                                                   a.effectDescription = effect;
                                                                                               }
                                                                                               );


            var unit = Helpers.CopyFeatureBuilder<MonsterDefinition>.createFeatureCopy(prefix + "Unit" + level.ToString(),
                                                                                             "",
                                                                                             $"Monster/&{prefix}Title",
                                                                                             $"Spell/&{prefix}Description",
                                                                                             null,
                                                                                             base_monster,
                                                                                             a =>
                                                                                             {
                                                                                                 a.SetDefaultFaction("Party");
                                                                                                 a.fullyControlledWhenAllied = true;
                                                                                                 a.armorClass = 11 + level;
                                                                                                 a.hitDice = level + base_hd;
                                                                                                 a.hitPointsBonus = 0;
                                                                                                 a.standardHitPoints = a.hitDice * 5 ;
                                                                                                 a.skillScores = new List<MonsterSkillProficiency>
                                                                                                 {
                                                                                                 };
                                                                                                 a.savingThrowScores = new List<MonsterSavingThrowProficiency>
                                                                                                 {
                                                                                                 };
                                                                                                 a.abilityScores = new int[] { 18, 12, 16, 4, 14, 4 };
                                                                                                 a.features = new List<FeatureDefinition>
                                                                                                 {
                                                                                                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                                                                                                     DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision,
                                                                                                 };
                                                                                                 a.features.AddRange(extra_features);
                                                                                                 a.attackIterations = new List<MonsterAttackIteration>
                                                                                                 {
                                                                                                     new MonsterAttackIteration
                                                                                                     {
                                                                                                         monsterAttackDefinition = attack,
                                                                                                         number = level / 2
                                                                                                     }
                                                                                                 };
                                                                                                 a.characterFamily = "Beast";
                                                                                                 a.challengeRating = (level / 2);
                                                                                                 a.droppedLootDefinition = null;
                                                                                                 //a.monsterPresentation.attachedParticlesReference = fx_monster.monsterPresentation.attachedParticlesReference;
                                                                                             }
                                                                                             );
            unit.bestiaryEntry = BestiaryDefinitions.BestiaryEntry.None;
            return unit;
        }


        static void createFlameBlade()
        {
            var title_string = "Spell/&FlameBladeSpellTitle";
            var description_string = "Spell/&FlameBladeSpellDescription";
            var sprite = DatabaseHelper.SpellDefinitions.FlameBlade.guiPresentation.spriteReference;
            var feature = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("FlameBladeSpellWeaponFeature",
                                                                                             "",
                                                                                             title_string,
                                                                                             Common.common_no_title,
                                                                                             sprite);

            var caster_stat_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ReplaceWeaponAbilityScoreWithHighestStatIfWeaponHasFeature>
                                                  .createFeature("FlameBladeAbilityScoreFeature",
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



            var effects = new List<EffectDescription>();
            for (int i = 2; i <= 5; i++)
            {
                var damage_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.OverwriteDamageOnWeaponWithFeature>
                                                                  .createFeature($"FlameBladeSpell{i}DamageFeature",
                                                                                 "",
                                                                                 Common.common_no_title,
                                                                                 Common.common_no_title,
                                                                                 Common.common_no_icon,
                                                                                 a =>
                                                                                 {
                                                                                     a.numDice = i;
                                                                                     a.dieType = RuleDefinitions.DieType.D8;
                                                                                     a.weaponFeature = feature;
                                                                                     a.ovewriteDamageType = Helpers.DamageTypes.Fire;
                                                                                 }
                                                                                 );


                var condition = Helpers.ConditionBuilder.createCondition($"FlameBladeSpell{i}Condition",
                                                                        "",
                                                                        title_string,
                                                                        description_string,
                                                                        DatabaseHelper.ConditionDefinitions.ConditionBrandingSmite.guiPresentation.spriteReference,
                                                                        DatabaseHelper.ConditionDefinitions.ConditionDivineFavor,
                                                                        caster_stat_feature,
                                                                        damage_feature
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
                effect.durationParameter = 10;
                effect.itemSelectionType = ActionDefinitions.ItemSelectionType.Weapon;
                effect.targetType = RuleDefinitions.TargetType.Self;
                effect.rangeType = RuleDefinitions.RangeType.Self;
                effect.durationType = RuleDefinitions.DurationType.Minute;

                var effect_form = new EffectForm();
                effect_form.createdByCharacter = true;
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = condition;
                effect_form.ConditionForm.applyToSelf = true;
                effect.EffectForms.Add(effect_form);

                effect_form = new EffectForm();
                effect_form.lightSourceForm = new LightSourceForm();
                effect_form.createdByCharacter = true;
                effect_form.FormType = EffectForm.EffectFormType.LightSource;
                effect_form.lightSourceForm.brightRange = 2;
                effect_form.lightSourceForm.dimAdditionalRange = 2;
                effect_form.lightSourceForm.color = DatabaseHelper.SpellDefinitions.Light.effectDescription.effectForms[0].lightSourceForm.color;
                effect_form.lightSourceForm.graphicsPrefabReference = DatabaseHelper.SpellDefinitions.Light.effectDescription.effectForms[0].lightSourceForm.graphicsPrefabReference;
                effect.EffectForms.Add(effect_form);

                effect_form = new EffectForm();
                effect_form.createdByCharacter = true;
                effect_form.itemPropertyForm = new ItemPropertyForm();
                effect_form.FormType = EffectForm.EffectFormType.ItemProperty;
                effect_form.itemPropertyForm.featureBySlotLevel = new List<FeatureUnlockByLevel>(){ new FeatureUnlockByLevel(feature, 0) };
                effect.EffectForms.Add(effect_form);
                effect.effectAdvancement.Clear();
                effect.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;
                effect.EffectAdvancement.additionalDicePerIncrement = 1;
                effect.EffectAdvancement.incrementMultiplier = 2;
                effects.Add(effect);
            }


            flame_blade = Helpers.GenericSpellBuilder<NewFeatureDefinitions.SpellWithSlotLevelDependentEffects>
                                                                        .createSpell("FlameBladeSpell",
                                                                                       "",
                                                                                       title_string,
                                                                                       description_string,
                                                                                       sprite,
                                                                                       effects[0],
                                                                                       RuleDefinitions.ActivationTime.BonusAction,
                                                                                       2,
                                                                                       true,
                                                                                       true,
                                                                                       false,
                                                                                       Helpers.SpellSchools.Evocation
                                                                                       );
            flame_blade.minCustomEffectLevel = 4;
            for (int i = 1; i < effects.Count(); i++)
            {
                flame_blade.levelEffectList.Add((2 + 2*i, effects[i]));
            }
            flame_blade.materialComponentType = RuleDefinitions.MaterialComponentType.None;

            var allowed_weapons = new List<string> { Helpers.WeaponProficiencies.Scimitar};
            flame_blade.restrictions = new List<NewFeatureDefinitions.IRestriction> { new NewFeatureDefinitions.SpecificWeaponInMainHandRestriction(allowed_weapons) };
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, flame_blade);
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
            condition.possessive = true;

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
                                            "Rules/&ConditionAllAttacsksStatsDisadvantageDescription",
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

            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("CallLightninglSpellImage",
                                         $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/CallLightning.png",
                                         128, 128);


            var condition = Helpers.ConditionBuilder.createCondition("CallLightningCondition",
                                                                    "",
                                                                    title_string,
                                                                    description_string,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked.guiPresentation.SpriteReference,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionTraditionShockArcanistArcaneShocked
                                                                    );
            condition.conditionType = RuleDefinitions.ConditionType.Beneficial;
            condition.possessive = true;

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
