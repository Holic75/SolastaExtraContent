using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
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
        static public SpellDefinition conjure_spirit_animal;
        static public SpellDefinition winter_blast;
        static public SpellDefinition spike_growth;
        static public SpellDefinition vulnerability_hex;
        static public SpellDefinition earth_tremor;

        internal static void create()
        {
            createHellishRebuke();
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
            fixSurfaceSpell(DatabaseHelper.SpellDefinitions.SpikeGrowth);
            fixGiantInsects();
            fixConjureMinorElemental();
            fixConjureAnimals();

            createVulnerabilityHex();
            createEarthTremor();
        }


        static void fixConjureAnimals()
        {

            for (int i = 0; i <  DatabaseHelper.SpellDefinitions.ConjureAnimals.subspellsList.Count; i++)
            {
                var variant = DatabaseHelper.SpellDefinitions.ConjureAnimals.subspellsList[i];
                variant.effectDescription.effectAdvancement.Clear();
                variant.effectDescription.effectAdvancement.incrementMultiplier = 2;
                variant.effectDescription.effectAdvancement.additionalSummonsPerIncrement = variant.effectDescription.targetParameter;
                variant.effectDescription.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;
            }
            DatabaseHelper.SpellDefinitions.ConjureAnimals.effectDescription.effectAdvancement.Clear();
            DatabaseHelper.SpellDefinitions.ConjureAnimals.effectDescription.effectAdvancement.incrementMultiplier = 2;
            DatabaseHelper.SpellDefinitions.ConjureAnimals.effectDescription.effectAdvancement.additionalSummonsPerIncrement = 1;
            DatabaseHelper.SpellDefinitions.ConjureAnimals.effectDescription.effectAdvancement.effectIncrementMethod = RuleDefinitions.EffectIncrementMethod.PerAdditionalSlotLevel;
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
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListDruid, earth_tremor);
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
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListDruid, winter_blast);
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
            Helpers.Misc.addSpellToSpelllist(DatabaseHelper.SpellListDefinitions.SpellListDruid, conjure_spirit_animal);
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
