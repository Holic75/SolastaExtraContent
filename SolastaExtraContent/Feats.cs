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

namespace SolastaExtraContent
{
    public class Feats
    {
        static public FeatDefinition polearm_master;
        static public FeatDefinition sentinel;
        static public Dictionary<CharacterClassDefinition, FeatDefinition> magic_initiate = new Dictionary<CharacterClassDefinition, FeatDefinition>();
        //power attack
        //elemental adept
        //mobile
        //combat casting


        internal static void load()
        {
            createPolearmMaster();
            createSentinel();
            createMagicInitiate();
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
