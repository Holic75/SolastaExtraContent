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
        //power attack
        //elemental adept
        //mobile
        //combat casting
        //magic initiate


        internal static void load()
        {
            createPolearmMaster();
            createSentinel();
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
