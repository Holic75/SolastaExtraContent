using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;
using SolastaModHelpers;
using SolastaModApi;
using SolastaModHelpers.NewFeatureDefinitions;

namespace SolastaExtraContent
{
    public class Feats
    {
        static public FeatDefinition polearm_master;
        static public FeatDefinition sentinel;
        static public FeatDefinition warcaster;
        static public Dictionary<CharacterClassDefinition, FeatDefinition> magic_initiate = new Dictionary<CharacterClassDefinition, FeatDefinition>();
        static public FeatDefinition furious;
        static public Dictionary<string, FeatDefinition> elemental_adept = new Dictionary<string, FeatDefinition>();
        static public FeatDefinition mobile;
        //fast_shooter


        internal static void load()
        {
            createPolearmMaster();
            createSentinel();
            createMagicInitiate();
            createWarcaster();
            createFurious();
            createElementalAdept();
            createCombatMobility();
        }


        static void createCombatMobility()
        {
            var speed_bonus = DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider;

            var ignore_difficult_terrain = Helpers.CopyFeatureBuilder<FeatureDefinitionMovementAffinity>
                                                        .createFeatureCopy("MobileFeatIgnoreDifficultTerrain",
                                                                           "",
                                                                           "",
                                                                           "",
                                                                           null,
                                                                           DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityFreedomOfMovement,
                                                                           a =>
                                                                           {
                                                                               a.appliesToAllModes = false;
                                                                           }
                                                                           );

            var ignore_difficult_terrain_if_dashing = Helpers.FeatureBuilder<NewFeatureDefinitions.MovementBonusWithRestrictions>
                                                                .createFeature("MobileFeatIgnoreDifficultTerrainIfDashing",
                                                                               "",
                                                                               Common.common_no_title,
                                                                               Common.common_no_title,
                                                                               Common.common_no_icon,
                                                                               a =>
                                                                               {
                                                                                   a.modifiers = new List<FeatureDefinition> { ignore_difficult_terrain };
                                                                                   a.restrictions.Add(new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(DatabaseHelper.ConditionDefinitions.ConditionDashing,
                                                                                                                                                                          DatabaseHelper.ConditionDefinitions.ConditionDashingAdditional,
                                                                                                                                                                          DatabaseHelper.ConditionDefinitions.ConditionDashingBonus,
                                                                                                                                                                          DatabaseHelper.ConditionDefinitions.ConditionDashingExpeditiousRetreat));
                                                                               }
                                                                               );
            var condition_no_aoo_mark = Helpers.ConditionBuilder.createCondition("MobileFeatPreventAooCondition",
                                                                      "",
                                                                     "Rules/&MobileFeatPreventAooConditionTitle",
                                                                     "Rules/&MobileFeatPreventAooConditionDescription",
                                                                      null,
                                                                      DatabaseHelper.ConditionDefinitions.ConditionMarkedByHunter
                                                                      );

            var feature_no_aoo = Helpers.FeatureBuilder<NewFeatureDefinitions.OpportunityAttackImmunityIfAttackerHasConditionFromCaster>.createFeature("MobileFeatPreventAooCasterFeature",
                                                                                                                                                         "",
                                                                                                                                                         Common.common_no_title,
                                                                                                                                                         Common.common_no_title,
                                                                                                                                                         Common.common_no_icon,
                                                                                                                                                         a =>
                                                                                                                                                         {
                                                                                                                                                             a.condition = condition_no_aoo_mark;
                                                                                                                                                         }
                                                                                                                                                         );

            var apply_no_aoo_mark = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnAttackToTarget>
                                                        .createFeature("MobileFeatApplyPreventAooMark",
                                                                       "",
                                                                       Common.common_no_title,
                                                                       Common.common_no_title,
                                                                       Common.common_no_icon,
                                                                       a =>
                                                                       {
                                                                           a.condition = condition_no_aoo_mark;
                                                                           a.durationType = RuleDefinitions.DurationType.Round;
                                                                           a.durationValue = 0;
                                                                           a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                                           a.onlyMelee = true;
                                                                       }
                                                                       );

            mobile = Helpers.CopyFeatureBuilder<FeatDefinition>.createFeatureCopy("MobileFeat",
                                                                                  "",
                                                                                  "Feature/&MobileFeatTitle",
                                                                                  "Feature/&MobileFeatDescription",
                                                                                  null,
                                                                                  DatabaseHelper.FeatDefinitions.FollowUpStrike,
                                                                                  a =>
                                                                                  {
                                                                                      a.features = new List<FeatureDefinition>
                                                                                      {
                                                                                        speed_bonus,
                                                                                        ignore_difficult_terrain_if_dashing,
                                                                                        apply_no_aoo_mark,
                                                                                        feature_no_aoo
                                                                                      };
                                                                                  }
                                                                                  );
        }


        static void createElementalAdept()
        {
            string[] damage_types = new string[] { Helpers.DamageTypes.Acid, Helpers.DamageTypes.Cold, Helpers.DamageTypes.Fire, Helpers.DamageTypes.Lightning };

            foreach (var damage_type in damage_types)
            {
                var ignore_resistance = Helpers.FeatureBuilder<NewFeatureDefinitions.IgnoreDamageResistance>
                                                                    .createFeature($"ElementalAdeptFeatIgnore{damage_type}Resistance",
                                                                                   "",
                                                                                   Common.common_no_title,
                                                                                   Common.common_no_title,
                                                                                   Common.common_no_icon,
                                                                                   a =>
                                                                                   {
                                                                                       a.damageTypes = new List<string> { damage_type };
                                                                                   }
                                                                                   );

                var reroll_dice = Helpers.FeatureBuilder<NewFeatureDefinitions.ModifyDamageRollTypeDependent>
                                                                .createFeature($"ElementalAdeptFeatReroll{damage_type}",
                                                                               "",
                                                                               Common.common_no_title,
                                                                               Common.common_no_title,
                                                                               Common.common_no_icon,
                                                                               a =>
                                                                               {
                                                                                   a.damageTypes = new List<string> { damage_type };
                                                                                   a.minRerollValue = 1;
                                                                                   a.validityContext = RuleDefinitions.RollContext.MagicDamageValueRoll;
                                                                                   a.rerollLocalizationKey = "Feature/&ElementalAdeptFeatRerollDescription";
                                                                               }
                                                                               );


                string damage_title = DatabaseRepository.GetDatabase<DamageDefinition>().GetElement(damage_type).GuiPresentation.Title;

                var title_string = Helpers.StringProcessing.insertStrings("Feature/&ElementalAdeptFeatTitle",
                                                                          damage_title,
                                                                          $"Feature/&ElementalAdeptFeat{damage_type}Title",
                                                                          "<DAMAGE>"
                                                                          );
                var description_string = Helpers.StringProcessing.insertStrings("Feature/&ElementalAdeptFeatDescription",
                                                                                damage_title,
                                                                                $"Feature/&ElementalAdeptFeat{damage_type}Description",
                                                                                "<DAMAGE>"
                                                                               );

                elemental_adept[damage_type] = Helpers.CopyFeatureBuilder<FeatDefinition>.createFeatureCopy("ElementalAdeptFeat" + damage_type,
                                                                                                            "",
                                                                                                            title_string,
                                                                                                            description_string,
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatDefinitions.PowerfulCantrip,
                                                                                                            a =>
                                                                                                            {
                                                                                                                a.features = new List<FeatureDefinition>
                                                                                                                {
                                                                                                                     ignore_resistance,
                                                                                                                     reroll_dice,
                                                                                                                };
                                                                                                            }
                                                                                                            );
            }
        }


        static void createFurious()
        {
            var condition_scored_critical_hit = Helpers.ConditionBuilder.createCondition("FuriousFeatScoredCriticalCondition", "", "", "", null, DatabaseHelper.ConditionDefinitions.ConditionDummy);
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(condition_scored_critical_hit);
            var condition_hit = Helpers.ConditionBuilder.createCondition("FuriousFeatHitCondition", "", "", "", null, DatabaseHelper.ConditionDefinitions.ConditionDummy);
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(condition_hit);

            var apply_condition_on_attack_hit = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnDamageDone>
                                                            .createFeature("FuriousFocusHitWatcher",
                                                                           "",
                                                                           Common.common_no_title,
                                                                           Common.common_no_title,
                                                                           Common.common_no_icon,
                                                                           a =>
                                                                           {
                                                                               a.apply_to_self = true;
                                                                               a.condition = condition_hit;
                                                                               a.durationType = RuleDefinitions.DurationType.Round;
                                                                               a.durationValue = 0;
                                                                               a.onlyWeapon = true;
                                                                               a.onlyMelee = true;
                                                                           }
                                                                           );

            var apply_condition_on_critical_hit = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnDamageDone>
                                                                            .createFeature("FuriousFocusCriticalHitWatcher",
                                                                                           "",
                                                                                           Common.common_no_title,
                                                                                           Common.common_no_title,
                                                                                           Common.common_no_icon,
                                                                                           a =>
                                                                                           {
                                                                                               a.apply_to_self = true;
                                                                                               a.condition = condition_scored_critical_hit;
                                                                                               a.durationType = RuleDefinitions.DurationType.Round;
                                                                                               a.durationValue = 0;
                                                                                               a.onlyWeapon = true;
                                                                                               a.onlyMelee = true;
                                                                                               a.onlyOnCritical = true;
                                                                                           }
                                                                                           );

            List<string> allowed_weapon_types = new List<string>()
                                                {
                                                    Helpers.WeaponProficiencies.QuarterStaff,
                                                    Helpers.WeaponProficiencies.Spear,
                                                    Helpers.WeaponProficiencies.Mace,
                                                    Helpers.WeaponProficiencies.Dagger,
                                                    Helpers.WeaponProficiencies.Handaxe,
                                                    Helpers.WeaponProficiencies.Club,
                                                    Helpers.WeaponProficiencies.LongSword,
                                                    Helpers.WeaponProficiencies.GreatAxe,
                                                    Helpers.WeaponProficiencies.Rapier,
                                                    Helpers.WeaponProficiencies.ShortSword,
                                                    Helpers.WeaponProficiencies.GreatSword,
                                                    Helpers.WeaponProficiencies.Scimitar,
                                                    Helpers.WeaponProficiencies.MorningStar,
                                                    Helpers.WeaponProficiencies.BattleAxe,
                                                    Helpers.WeaponProficiencies.Warhammer,
                                                    Helpers.WeaponProficiencies.Maul
                                                };

            var add_bonus_attack = Helpers.FeatureBuilder<NewFeatureDefinitions.ExtraMainWeaponAttack>.createFeature("FuriousFeatExtraAttack",
                                                                                                                     "",
                                                                                                                     Common.common_no_title,
                                                                                                                     Common.common_no_title,
                                                                                                                     Common.common_no_icon,
                                                                                                                     a =>
                                                                                                                     {
                                                                                                                         a.actionType = ActionDefinitions.ActionType.Bonus;
                                                                                                                         a.restrictions.Add(new NewFeatureDefinitions.HasConditionRestriction(condition_hit));
                                                                                                                         a.restrictions.Add(new NewFeatureDefinitions.AndRestriction(new NewFeatureDefinitions.HasConditionRestriction(condition_scored_critical_hit),
                                                                                                                                                                                     new NewFeatureDefinitions.DownedAnEnemy()
                                                                                                                                                                                     )
                                                                                                                                           );
                                                                                                                         a.restrictions.Add(new NewFeatureDefinitions.SpecificWeaponInMainHandRestriction(allowed_weapon_types));
                                                                                                                     }
                                                                                                                     );

            var disadvantage_on_attacks = Helpers.FeatureBuilder<NewFeatureDefinitions.DisadvantageOnWeaponAttack>.createFeature("FuriousFeatDisadvantageFeature",
                                                                                                                                 "",
                                                                                                                                 Common.common_no_title,
                                                                                                                                 Common.common_no_title,
                                                                                                                                 Common.common_no_icon,
                                                                                                                                 a =>
                                                                                                                                 {
                                                                                                                                     a.onlyMelee = true;
                                                                                                                                 }
                                                                                                                                 );

            var double_damage_on_attacks = Helpers.FeatureBuilder<NewFeatureDefinitions.DoubleDamageOnSpecificWeaponTypes>.createFeature("FuriousFeatDoubleDamageFeature",
                                                                                                                                         "",
                                                                                                                                         Common.common_no_title,
                                                                                                                                         Common.common_no_title,
                                                                                                                                         Common.common_no_icon,
                                                                                                                                         a =>
                                                                                                                                         {
                                                                                                                                             a.weaponTypes = allowed_weapon_types;
                                                                                                                                         }
                                                                                                                                         );

            var condition_power_attack = Helpers.ConditionBuilder.createCondition("FuriousFeatPowerAttackCondition", 
                                                                                  "",
                                                                                 "Rules/&FuriousFeatConditionPowerAttackTitle",
                                                                                 "Rules/&FuriousFeatConditionPowerAttackDescription",
                                                                                  null, 
                                                                                  DatabaseHelper.ConditionDefinitions.ConditionReckless,
                                                                                  double_damage_on_attacks,
                                                                                  disadvantage_on_attacks);
            condition_power_attack.possessive = false;

            var effect_description = new EffectDescription();
            effect_description.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerReckless.effectDescription);
            effect_description.effectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition_power_attack;
            effect_description.EffectForms.Add(effect_form);
            effect_description.durationParameter = 0;

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                                    .createPower("FuriousFeatPowerAttackPower",
                                                                                 "",
                                                                                 "Feature/&FuriousFeatPowerAttackPowerTitle",
                                                                                 "Feature/&FuriousFeatPowerAttackPowerDescription",
                                                                                 DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.guiPresentation.SpriteReference,
                                                                                 effect_description,
                                                                                 RuleDefinitions.ActivationTime.NoCost,
                                                                                 1,
                                                                                 RuleDefinitions.UsesDetermination.Fixed,
                                                                                 RuleDefinitions.RechargeRate.AtWill
                                                                                 );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
            {
                new NewFeatureDefinitions.InverseRestriction(new NewFeatureDefinitions.AttackedRestriction())
            };


            var furious_attack_action = SolastaModHelpers.Helpers.CopyFeatureBuilder<ActionDefinition>
                     .createFeatureCopy("FuriousAttack", 
                                        "8b7ff00c-be0a-4cb3-b18b-a07d27c666de",
                                        power.guiPresentation.title,
                                        power.guiPresentation.description,
                                        DatabaseHelper.ActionDefinitions.RecallItem.guiPresentation.spriteReference,
                                        DatabaseHelper.ActionDefinitions.RecklessAttack);
            furious_attack_action.id = (ActionDefinitions.Id)ExtendedActionId.Furious;
            furious_attack_action.usesPerTurn = -1;
            ActionData.addActionRestrictions(furious_attack_action, new NewFeatureDefinitions.InverseRestriction(new NewFeatureDefinitions.AttackedRestriction()),
                                                                    new NewFeatureDefinitions.InverseRestriction(new NewFeatureDefinitions.HasConditionRestriction(condition_power_attack)));

            var action_affinity = Helpers.CopyFeatureBuilder<FeatureDefinitionActionAffinity>
                                                .createFeatureCopy("ActionAffinityFuriousAttack",
                                                                   "",
                                                                   Common.common_no_title,
                                                                   Common.common_no_title,
                                                                   Common.common_no_icon,
                                                                   DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityBarbarianRecklessAttack,
                                                                   a =>
                                                                   {
                                                                       a.authorizedActions = new List<ActionDefinitions.Id> { furious_attack_action.id };
                                                                   }
                                                                   );

            furious = Helpers.CopyFeatureBuilder<FeatDefinition>.createFeatureCopy("FuriousFeat",
                                                                      "",
                                                                      "Feature/&FuriousFeatTitle",
                                                                      "Feature/&FuriousFeatDescription",
                                                                      null,
                                                                      DatabaseHelper.FeatDefinitions.FollowUpStrike,
                                                                      a =>
                                                                      {
                                                                          a.features = new List<FeatureDefinition>
                                                                          {
                                                                            apply_condition_on_attack_hit,
                                                                            apply_condition_on_critical_hit,
                                                                            add_bonus_attack,
                                                                            action_affinity
                                                                            //power
                                                                          };
                                                                      }
                                                                      );
        }


        static void createWarcaster()
        {
            var concentration_advantage = Helpers.CopyFeatureBuilder<FeatureDefinitionMagicAffinity>
                                                .createFeatureCopy("WarcasterFeatConcentrationBonus",
                                                                   "",
                                                                   "",
                                                                   "",
                                                                   null,
                                                                   DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityFeatFlawlessConcentration,
                                                                   a =>
                                                                   {
                                                                       a.overConcentrationThreshold = 0;
                                                                   }
                                                                   );

            var aoo_cantrips = Helpers.FeatureBuilder<NewFeatureDefinitions.Warcaster>.createFeature("WarcasterFeatAooCantrips",
                                                                                                     "",
                                                                                                     Common.common_no_title,
                                                                                                     Common.common_no_title,
                                                                                                     Common.common_no_icon);

            warcaster = Helpers.CopyFeatureBuilder<FeatDefinition>.createFeatureCopy("WarcasterFeat",
                                                                  "",
                                                                  "Feature/&WarcasterFeatTitle",
                                                                  "Feature/&WarcasterFeatDescription",
                                                                  null,
                                                                  DatabaseHelper.FeatDefinitions.FlawlessConcentration,
                                                                  a =>
                                                                  {
                                                                      a.features = new List<FeatureDefinition>
                                                                      {
                                                                            DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityBattleMagic,
                                                                            concentration_advantage,
                                                                            aoo_cantrips
                                                                      };
                                                                  }
                                                                  );
        }


        static void createMagicInitiate()
        {
            var spellcasting_classes = new CharacterClassDefinition[]
            {
                DatabaseHelper.CharacterClassDefinitions.Cleric,
                DatabaseHelper.CharacterClassDefinitions.Druid,
                DatabaseHelper.CharacterClassDefinitions.Wizard,
                BardClassBuilder.bard_class,
                WarlockClassBuilder.warlock_class
            };

            foreach (var cls in spellcasting_classes)
            {
                var feature = cls.featureUnlocks.Where(fu => fu.level == 1 && fu.featureDefinition is FeatureDefinitionCastSpell).FirstOrDefault().featureDefinition as FeatureDefinitionCastSpell;
                var cantrip_spelllist = Helpers.SpelllistBuilder.createCombinedSpellListWithLevelRestriction(cls.Name + "MagicInititateFeatCantripsList", "", "",
                                                                         (feature.SpellListDefinition, 0)
                                                                         );

                var extra_cantrip = Helpers.ExtraSpellSelectionFromFeatBuilder.createExtraCantripSelection(cls.Name + "MagicInititateFeatCantripsFeature",
                                                                                                            "",
                                                                                                            Common.common_no_title,
                                                                                                            Common.common_no_title,
                                                                                                            2,
                                                                                                            cantrip_spelllist
                                                                                                            );

                var spell_spelllist = Helpers.SpelllistBuilder.createCombinedSpellListWithLevelRestriction(cls.Name + "MagicInititateFeatSpellsList", "", "",
                                                                                                             (feature.spellListDefinition, 1)
                                                                                                             );

                var extra_spell = Helpers.ExtraSpellSelectionFromFeatBuilder.createExtraSpellSelection(cls.Name + "MagicInititateFeatSpellsFeature",
                                                                                                        "",
                                                                                                        Common.common_no_title,
                                                                                                        Common.common_no_title,
                                                                                                        1,
                                                                                                        spell_spelllist
                                                                                                        );

                var title_string = Helpers.StringProcessing.insertStrings("Feature/&MagicInititateFeatTitle",
                                                          cls.guiPresentation.title,
                                                          $"Feature/&MagicInititateFeat{cls.name}Title",
                                                          "<CLASS>"
                                                          );
                var description_string = Helpers.StringProcessing.insertStrings("Feature/&MagicInititateFeatDescription",
                                                                                  cls.guiPresentation.title,
                                                                                  $"Feature/&MagicInititateFeat{cls.name}Description",
                                                                                  "<CLASS>"
                                                                                  );
                magic_initiate[cls] = Helpers.CopyFeatureBuilder<FeatDefinition>.createFeatureCopy(cls.Name + "MagicInititateFeat",
                                                                                                   "",
                                                                                                    title_string,
                                                                                                    description_string,
                                                                                                    null,
                                                                                                    DatabaseHelper.FeatDefinitions.PowerfulCantrip,
                                                                                                    a =>
                                                                                                    {
                                                                                                        a.features = new List<FeatureDefinition>
                                                                                                        {
                                                                                                            extra_cantrip,
                                                                                                            extra_spell,
                                                                                                        };
                                                                                                    }
                                                                                                    );
                NewFeatureDefinitions.FeatureData.addFeatureRestrictions(magic_initiate[cls], new NewFeatureDefinitions.CanCastSpellOfSpecifiedLevelPrerequisite(1));
            }

            DatabaseHelper.SpellListDefinitions.SpellListRanger.hasCantrips = true;
            DatabaseHelper.SpellListDefinitions.SpellListRanger.spellsByLevel.Insert(0, new SpellListDefinition.SpellsByLevelDuplet()
            {
                level = 0,
                spells = new List<SpellDefinition>() { }
            });
            DatabaseHelper.SpellListDefinitions.SpellListPaladin.hasCantrips = true;
            DatabaseHelper.SpellListDefinitions.SpellListPaladin.spellsByLevel.Insert(0, new SpellListDefinition.SpellsByLevelDuplet()
            {
                level = 0,
                spells = new List<SpellDefinition>() { }
            });
        }



        static void createSentinel()
        {
            var ignore_disengage = Helpers.FeatureBuilder<NewFeatureDefinitions.CanIgnoreDisengage>
                                    .createFeature("SentinelFeatIgnoreDisengage",
                                                   "",
                                                   Common.common_no_title,
                                                   Common.common_no_title,
                                                   Common.common_no_icon
                                                   );

            var aoo_if_ally_is_attacked = Helpers.FeatureBuilder<NewFeatureDefinitions.AooIfAllyIsAttacked>
                                                         .createFeature("SentinelFeatAooIfAllyIsAttacked",
                                                                        "",
                                                                        Common.common_no_title,
                                                                        Common.common_no_title,
                                                                        Common.common_no_icon
                                                                        );

            var condition_cannot_move = Helpers.ConditionBuilder.createCondition("SentinielFeatConditionCannotMove",
                                                                                 "",
                                                                                 "Rules/&SentinielFeatConditionCannotMoveTitle",
                                                                                 "Rules/&SentinielFeatConditionCannotMoveDescription",
                                                                                 null,
                                                                                 DatabaseHelper.ConditionDefinitions.ConditionRestrained,
                                                                                 DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityConditionRestrained
                                                                                 );

            var apply_condition_on_aoo_hit = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnDamageDone>
                                                    .createFeature("SentineFeatApplyCannotMove",
                                                                   "",
                                                                   Common.common_no_title,
                                                                   Common.common_no_title,
                                                                   Common.common_no_icon,
                                                                   a =>
                                                                   {
                                                                       a.onlyWeapon = true;
                                                                       a.onlyAOO = true;
                                                                       a.durationType = RuleDefinitions.DurationType.Round;
                                                                       a.durationValue = 0;
                                                                       a.condition = condition_cannot_move;
                                                                   }
                                                                   );

            sentinel = Helpers.CopyFeatureBuilder<FeatDefinition>.createFeatureCopy("SentinelFeat",
                                                                              "",
                                                                              "Feature/&SentinelFeatTitle",
                                                                              "Feature/&SentinelFeatDescription",
                                                                              null,
                                                                              DatabaseHelper.FeatDefinitions.FollowUpStrike,
                                                                              a =>
                                                                              {
                                                                                  a.features = new List<FeatureDefinition>
                                                                                  {
                                                                                      ignore_disengage,
                                                                                      aoo_if_ally_is_attacked,
                                                                                      apply_condition_on_aoo_hit
                                                                                  };
                                                                              }
                                                                              );
        }

        static void createPolearmMaster()
        {
            var weapons_categories = new List<string>
            {
                Helpers.WeaponProficiencies.Spear,
                Helpers.WeaponProficiencies.QuarterStaff
            };

            var extra_attack = Helpers.FeatureBuilder<NewFeatureDefinitions.AddExtraMainWeaponAttackModified>
                                        .createFeature("PolearmMasterFeatBonusActionAttack",
                                                       "",
                                                       Common.common_no_title,
                                                       Common.common_no_title,
                                                       Common.common_no_icon,
                                                       a =>
                                                       {
                                                           a.actionType = ActionDefinitions.ActionType.Bonus;
                                                           a.damage_type = Helpers.DamageTypes.Bludgeoning;
                                                           a.new_dice_number = 1;
                                                           a.new_die_type = RuleDefinitions.DieType.D4;
                                                           a.restrictions.Add(new NewFeatureDefinitions.SpecificWeaponInMainHandRestriction(weapons_categories));
                                                           a.restrictions.Add(new NewFeatureDefinitions.UsedAllMainAttacksRestriction());
                                                       }
                                                       );

            var aoo_on_enemy_entering_reach = Helpers.FeatureBuilder<NewFeatureDefinitions.AooWhenEnemyEntersReachWithSpecifiedWeaponGroup>
                                                .createFeature("PolearmMasterFeatAoo",
                                                               "",
                                                               Common.common_no_title,
                                                               Common.common_no_title,
                                                               Common.common_no_icon,
                                                               a =>
                                                               {
                                                                   a.weaponTypes = weapons_categories;
                                                               }
                                                               );

            polearm_master = Helpers.CopyFeatureBuilder<FeatDefinition>.createFeatureCopy("PolearmMasterFeat",
                                                                                          "",
                                                                                          "Feature/&PolearmMasterFeatTitle",
                                                                                          "Feature/&PolearmMasterFeatDescription",
                                                                                          null,
                                                                                          DatabaseHelper.FeatDefinitions.FollowUpStrike,
                                                                                          a =>
                                                                                          {
                                                                                              a.features = new List<FeatureDefinition>
                                                                                              {
                                                                                                  extra_attack,
                                                                                                  aoo_on_enemy_entering_reach
                                                                                              };
                                                                                          }
                                                                                          );
        }
    }
}
