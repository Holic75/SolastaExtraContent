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
    public class Barbarian
    {
        //War Shaman
        static public NewFeatureDefinitions.SpellcastingForbidden rage_spellcasting_forbiden;
        static public SpellListDefinition war_shaman_spelllist;
        static public FeatureDefinitionCastSpell war_shaman_spellcasting;
        static public NewFeatureDefinitions.ShareRagePower share_rage_power;
        static public NewFeatureDefinitions.ShareRagePower share_rage_power_60;
        static public FeatureDefinitionFeatureSet ragecaster;

        //Frozen Fury
        static NewFeatureDefinitions.ApplyPowerOnTurnEndBasedOnClassLevel frozen_fury_rage_feature;
        static public FeatureDefinition frozen_fury;
        static public NewFeatureDefinitions.ArmorBonusAgainstAttackType frigid_body;
        static public FeatureDefinitionFeatureSet numb;
        static public ConditionDefinition shared_rage_condition;

        public static void create()
        {
            createPathOfWarShaman();
            createPathOfFrozenFury();
        }


        static void createPathOfFrozenFury()
        {
            createWintersFury();
            createFrigidBody();
            createNumb();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassPrimalPathOfFrozenFuryDescription",
                    "Subclass/&BarbarianSubclassPrimalPathOfFrozenFuryTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainElementalCold.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassPrimalPathOfFrozenFury", "facf5253-aa04-40b1-89a6-2e09f812da5a")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(frozen_fury, 3)
                    .AddFeatureAtLevel(frigid_body, 6)
                    .AddFeatureAtLevel(numb, 10)
                    .AddToDB();

            frozen_fury_rage_feature.requiredSubclass = definition;
            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath.Subclasses.Add(definition.Name);
        }


        static void createNumb()
        {
            string numb_title_string = "Feature/&BarbarianSubclassFrozenFuryNumbTitle";
            string numb_description_string = "Feature/&BarbarianSubclassFrozenFuryNumbDescription";

            var conditons = new List<ConditionDefinition> { DatabaseHelper.ConditionDefinitions.ConditionPoisoned, DatabaseHelper.ConditionDefinitions.ConditionFrightened, DatabaseHelper.ConditionDefinitions.ConditionFrightenedFear };
            var numb_immunity = Helpers.FeatureBuilder<NewFeatureDefinitions.ImmunityToCondtionIfHasSpecificConditions>.createFeature("BarbarianSubclassFrozenFuryNumbImmunity",
                                                                                                                          "",
                                                                                                                          numb_title_string,
                                                                                                                          numb_description_string,
                                                                                                                          null,
                                                                                                                          f =>
                                                                                                                          {
                                                                                                                              f.immuneCondtions = conditons;
                                                                                                                              f.requiredConditions = new List<ConditionDefinition>();
                                                                                                                              f.requiredConditions.Add(DatabaseHelper.ConditionDefinitions.ConditionRaging);
                                                                                                                          }
                                                                                                                          );

            var numb_removal = Helpers.FeatureBuilder<NewFeatureDefinitions.RemoveConditionsOnConditionApplication>.createFeature("BarbarianSubclassFrozenFuryNumbRemoval",
                                                                                                              "",
                                                                                                              numb_title_string,
                                                                                                              numb_description_string,
                                                                                                              null,
                                                                                                              f =>
                                                                                                              {
                                                                                                                  f.removeConditions = conditons;
                                                                                                                  f.appliedConditions = new List<ConditionDefinition>();
                                                                                                                  f.appliedConditions.Add(DatabaseHelper.ConditionDefinitions.ConditionRaging);
                                                                                                              }
                                                                                                              );

            numb = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianSubclassFrozenFuryNumb",
                                                              "",
                                                              numb_title_string,
                                                              numb_description_string,
                                                              false,
                                                              FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                              false,
                                                              numb_immunity,
                                                              numb_removal
                                                              );
        }


        static void createFrigidBody()
        {
            string frigid_body_title_string = "Feature/&BarbarianSubclassFrozenFuryFrigidBodyTitle";
            string frigid_body_description_string = "Feature/&BarbarianSubclassFrozenFuryFrigidBodyDescription";

            frigid_body = Helpers.FeatureBuilder<NewFeatureDefinitions.ArmorBonusAgainstAttackType>.createFeature("BarbarianSubclassFrozenFuryFrigidBody",
                                                                                                                  "",
                                                                                                                  frigid_body_title_string,
                                                                                                                  frigid_body_description_string,
                                                                                                                  null,
                                                                                                                  f =>
                                                                                                                  {
                                                                                                                      f.applyToMelee = false;
                                                                                                                      f.applyToRanged = true;
                                                                                                                      f.requiredConditions = new List<ConditionDefinition>();
                                                                                                                      f.requiredConditions.Add(DatabaseHelper.ConditionDefinitions.ConditionRaging);
                                                                                                                      f.value = 2;
                                                                                                                  }
                                                                                                                  );
        }


        static void createWintersFury()
        {
            string winters_fury_title_string = "Feature/&BarbarianSubclassFrozenFuryWintersFuryTitle";
            string winters_fury_description_string = "Feature/&BarbarianSubclassFrozenFuryWintersFuryDescription";

            List<(int level, int dice_number, RuleDefinitions.DieType die_type)> frozen_fury_damages = new List<(int level, int dice_number, RuleDefinitions.DieType die_type)>
            {
                {(5, 1, RuleDefinitions.DieType.D6) },
                {(9, 1, RuleDefinitions.DieType.D10) },
                {(13, 2, RuleDefinitions.DieType.D6) },
                {(20, 2, RuleDefinitions.DieType.D10) }
            };

            List<(int, FeatureDefinitionPower)> power_list = new List<(int, FeatureDefinitionPower)>();
            foreach (var entry in frozen_fury_damages)
            {
                var damage = new DamageForm();
                damage.DiceNumber = entry.dice_number;
                damage.DieType = entry.die_type;
                damage.VersatileDieType = entry.die_type;
                damage.DamageType = Helpers.DamageTypes.Cold;

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.FireShieldCold.EffectDescription);
                effect.rangeType = RuleDefinitions.RangeType.Self;
                effect.targetType = RuleDefinitions.TargetType.Sphere;
                effect.targetSide = RuleDefinitions.Side.All;
                effect.targetParameter = 1;
                effect.targetParameter2 = 1;
                effect.rangeParameter = 0;
                effect.canBePlacedOnCharacter = true;
                effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
                effect.DurationParameter = 0;
                effect.effectParticleParameters =  DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold.EffectDescription.EffectParticleParameters;
                effect.targetExcludeCaster = true;

                effect.EffectForms.Clear();
                var effect_form = new EffectForm();
                effect_form.DamageForm = damage;
                effect_form.FormType = EffectForm.EffectFormType.Damage;
                effect.EffectForms.Add(effect_form);


                var power = Helpers.PowerBuilder.createPower("BarbarianSubclassFrozenFuryWintersFuryPower" + entry.level,
                                                         "",
                                                         winters_fury_title_string,
                                                         winters_fury_description_string,
                                                         null,
                                                         DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalHeraldOfTheElementsCold,
                                                         effect,
                                                         RuleDefinitions.ActivationTime.NoCost,
                                                         1,
                                                         RuleDefinitions.UsesDetermination.Fixed,
                                                         RuleDefinitions.RechargeRate.AtWill,
                                                         show_casting: false);
                power_list.Add((entry.level, power));
            }

            frozen_fury_rage_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyPowerOnTurnEndBasedOnClassLevel>.createFeature("BarbarianSubclassFrozenFuryWintersFuryRageFeature",
                                                                                                                                          "",
                                                                                                                                          Common.common_no_title,
                                                                                                                                          Common.common_no_title,
                                                                                                                                          null,
                                                                                                                                          f =>
                                                                                                                                          {
                                                                                                                                              f.characterClass = DatabaseHelper.CharacterClassDefinitions.Barbarian;
                                                                                                                                              f.powerLevelList = power_list;
                                                                                                                                              //will fill subclass in FrozenFuryPath creation
                                                                                                                                          }
                                                                                                                                          );

            frozen_fury_rage_feature.guiPresentation.hidden = true;
            DatabaseHelper.ConditionDefinitions.ConditionRaging.features.Add(frozen_fury_rage_feature);

            frozen_fury = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("BarbarianSubclassFrozenFuryWintersFuryFeature",
                                                                                             "",
                                                                                             winters_fury_title_string,
                                                                                             winters_fury_description_string
                                                                                             );
        }



        static void createPathOfWarShaman()
        {
            createWarShamanSpellcasting();
            createShareRage();
            createRagecaster();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BarbarianSubclassPrimalPathOfWarShamanDescription",
                    "Subclass/&BarbarianSubclassPrimalPathOfWarShamanTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionGreenmage.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BarbarianSubclassPrimalPathOfWarShaman", "a126dff4-f95b-4352-969a-4b28558f2a82")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(war_shaman_spellcasting, 3)
                    .AddFeatureAtLevel(share_rage_power, 6)
                    .AddFeatureAtLevel(ragecaster, 10)
                    .AddToDB();

            DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath.Subclasses.Add(definition.Name);
        }


        static void createWarShamanSpellcasting()
        {
            rage_spellcasting_forbiden = Helpers.FeatureBuilder<NewFeatureDefinitions.SpellcastingForbidden>
                                                                                        .createFeature("BarbarianClassRageSpellcastingForbidden",
                                                                                                        "",
                                                                                                        DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging.guiPresentation.title,
                                                                                                        DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging.guiPresentation.Description,
                                                                                                        DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging.GuiPresentation.spriteReference,
                                                                                                        r =>
                                                                                                        {
                                                                                                            r.spellcastingExceptionFeatures = new List<FeatureDefinition>();
                                                                                                            r.concentrationExceptionFeatures = new List<FeatureDefinition>();
                                                                                                            r.forbidConcentration = true;
                                                                                                        }
                                                                                                        );
            DatabaseHelper.ConditionDefinitions.ConditionRaging.features.Remove(DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityConditionRaging);
            DatabaseHelper.ConditionDefinitions.ConditionRaging.features.Add(rage_spellcasting_forbiden);

            war_shaman_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("BarbarianSubclassWarshamanSpelllist", "", "",
                                                                        DatabaseHelper.SpellListDefinitions.SpellListDruid.spellsByLevel[0].spells.ToArray().ToList(),
                                                                        DatabaseHelper.SpellListDefinitions.SpellListDruid.spellsByLevel[1].spells.ToArray().ToList(),
                                                                        DatabaseHelper.SpellListDefinitions.SpellListDruid.spellsByLevel[2].spells.ToArray().ToList(),
                                                                        DatabaseHelper.SpellListDefinitions.SpellListDruid.spellsByLevel[3].spells.ToArray().ToList(),
                                                                        DatabaseHelper.SpellListDefinitions.SpellListDruid.spellsByLevel[4].spells.ToArray().ToList()
                                                                    );

            war_shaman_spelllist.maxSpellLevel = 4;
            war_shaman_spelllist.hasCantrips = true;

            war_shaman_spellcasting = Helpers.SpellcastingBuilder.createSpontaneousSpellcasting("BarbarianSubclassWarshamanSpellcasting",
                                                                                              "",
                                                                                              "Feature/&BarbarianSubclassWarShamanClassSpellcastingTitle",
                                                                                              "Feature/&BarbarianSubclassWarShamanClassSpellcastingDescription",
                                                                                              war_shaman_spelllist,
                                                                                              Helpers.Stats.Wisdom,
                                                                                              new List<int> {0, 0, 2, 2, 2, 2, 2, 2, 2, 3,
                                                                                                             3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
                                                                                              new List<int> { 0,  0,  3,  4,  4,  4,  5,  6,  6,  7,
                                                                                                              8,  8,  9, 10, 10, 11, 11, 11, 12, 13},
                                                                                              Helpers.Misc.createSpellSlotsByLevel(new List<int> { 0, 0, 0, 0 },
                                                                                                                                   new List<int> { 0, 0, 0, 0 },
                                                                                                                                   new List<int> { 2, 0, 0, 0 },//3
                                                                                                                                   new List<int> { 3, 0, 0, 0 },//4
                                                                                                                                   new List<int> { 3, 0, 0, 0 },//5
                                                                                                                                   new List<int> { 3, 0, 0, 0 },//6
                                                                                                                                   new List<int> { 4, 2, 0, 0 },//7
                                                                                                                                   new List<int> { 4, 2, 0, 0 },//8
                                                                                                                                   new List<int> { 4, 2, 0, 0 },//9
                                                                                                                                   new List<int> { 4, 3, 0, 0 },//10
                                                                                                                                   new List<int> { 4, 3, 0, 0 },//11
                                                                                                                                   new List<int> { 4, 3, 0, 0 },//12
                                                                                                                                   new List<int> { 4, 3, 2, 0 },//13
                                                                                                                                   new List<int> { 4, 3, 2, 0 },//14
                                                                                                                                   new List<int> { 4, 3, 2, 0 },//15
                                                                                                                                   new List<int> { 4, 3, 3, 0 },//16
                                                                                                                                   new List<int> { 4, 3, 3, 0 },//17
                                                                                                                                   new List<int> { 4, 3, 3, 0 },//18
                                                                                                                                   new List<int> { 4, 3, 3, 1 },//19
                                                                                                                                   new List<int> { 4, 3, 3, 1 }//20
                                                                                                                                   )
                                                                                              );
            war_shaman_spellcasting.spellCastingLevel = -1;
            war_shaman_spellcasting.spellCastingOrigin = FeatureDefinitionCastSpell.CastingOrigin.Subclass;
            war_shaman_spellcasting.replacedSpells = new List<int> {0, 0, 0, 1, 0, 0, 1, 1, 0, 1,
                                                                    1, 0, 1, 1, 0, 1, 0, 0, 1, 1};
            war_shaman_spellcasting.focusType = EquipmentDefinitions.FocusType.Druidic;
            rage_spellcasting_forbiden.concentrationExceptionFeatures.Add(war_shaman_spellcasting);
        }


        static void createRagecaster()
        {
            string ragecaster_title_string = "Feature/&BarbarianSubclassWarShamanClassRagecasterTitle";
            string ragecaster_description_string = "Feature/&BarbarianSubclassWarShamanClassRagecasterDescription";

            var ragecaster_feature = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("BarbarianSubclassWarshamanRagecaster",
                                                                                            "",
                                                                                            ragecaster_title_string,
                                                                                            ragecaster_description_string
                                                                                            );
            rage_spellcasting_forbiden.spellcastingExceptionFeatures.Add(ragecaster_feature);

            ragecaster = Helpers.FeatureSetBuilder.createFeatureSet("BarbarianSubclassWarshamanRagecasterFeatureSet",
                                                    "",
                                                    ragecaster_title_string,
                                                    ragecaster_description_string,
                                                    false,
                                                    FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                    false,
                                                    ragecaster_feature,
                                                    share_rage_power_60
                                                    );
        }


        static void createShareRage()
        {
            string share_rage_title_string = "Feature/&BarbarianSubclassWarShamanClassShareRageTitle";
            string share_rage_description_string = "Feature/&BarbarianSubclassWarShamanClassShareRageDescription";

            DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityConditionRaging.authorizedActions.Add(ActionDefinitions.Id.RageStop);

            shared_rage_condition = Helpers.CopyFeatureBuilder<ConditionDefinition>.createFeatureCopy("BarbarianShareRageCondition",
                                                                                               "",
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_icon,
                                                                                               DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                                               a =>
                                                                                               {
                                                                                                   a.specialDuration = true;
                                                                                                   a.durationParameter = 2;
                                                                                                   a.durationType = RuleDefinitions.DurationType.Round;
                                                                                               });
          
            var share_rage_powers = new NewFeatureDefinitions.ShareRagePower[2];
            for (int i = 0; i < share_rage_powers.Length; i++)
            {
                share_rage_powers[i] = Helpers.GenericPowerBuilder<NewFeatureDefinitions.ShareRagePower>
                                                                                    .createPower($"BarbarianSubclassWarshamanShareRagePower{(i + 1) * 60}",
                                                                                                    "",
                                                                                                    share_rage_title_string,
                                                                                                    share_rage_description_string,
                                                                                                    DatabaseHelper.FeatureDefinitionPowers.PowerDomainLawHolyRetribution.GuiPresentation.SpriteReference,
                                                                                                    DatabaseHelper.FeatureDefinitionPowers.PowerBarbarianRageStart.effectDescription,
                                                                                                    RuleDefinitions.ActivationTime.BonusAction,
                                                                                                    10,
                                                                                                    RuleDefinitions.UsesDetermination.Fixed,
                                                                                                    RuleDefinitions.RechargeRate.SpellSlot
                                                                                                    );

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerBarbarianRageStart.EffectDescription);
                effect.EffectForms.Clear();
                effect.rangeType = RuleDefinitions.RangeType.Distance;
                effect.rangeParameter = 6 + 6*i;
                effect.targetParameter = 1;
                effect.targetParameter2 = 1;
                effect.DurationParameter = 1;
                effect.DurationType = RuleDefinitions.DurationType.Minute;
                effect.targetType = RuleDefinitions.TargetType.Individuals;
                effect.targetSide = RuleDefinitions.Side.Ally;
                effect.targetFilteringTag = (RuleDefinitions.TargetFilteringTag)(ExtendedEnums.ExtraTargetFilteringTag.NonCaster | ExtendedEnums.ExtraTargetFilteringTag.NoHeavyArmor);


                var effect_form_share = new EffectForm();
                effect_form_share.ConditionForm = new ConditionForm();
                effect_form_share.FormType = EffectForm.EffectFormType.Condition;
                effect_form_share.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form_share.ConditionForm.ConditionDefinition = shared_rage_condition;
                effect.EffectForms.Add(effect_form_share);

                share_rage_powers[i].restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(DatabaseHelper.ConditionDefinitions.ConditionRaging));
                share_rage_powers[i].restrictions.Add(new NewFeatureDefinitions.ArmorTypeRestriction(DatabaseHelper.ArmorCategoryDefinitions.HeavyArmorCategory, inverted: true));
                share_rage_powers[i].effectDescription = effect;
                share_rage_powers[i].effectDescription.immuneCreatureFamilies = new List<string> { Helpers.Misc.createImmuneIfHasConditionFamily(DatabaseHelper.ConditionDefinitions.ConditionRaging) };
                share_rage_powers[i].spellcastingFeature = war_shaman_spellcasting;
            }
            share_rage_power = share_rage_powers[0];
            share_rage_power_60 = share_rage_powers[1];
            share_rage_power_60.overriddenPower = share_rage_power;
        }
    }
}
