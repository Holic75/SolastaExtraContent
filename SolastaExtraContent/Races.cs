using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Helpers = SolastaModHelpers.Helpers;
using NewFeatureDefinitions = SolastaModHelpers.NewFeatureDefinitions;
using ExtendedEnums = SolastaModHelpers.ExtendedEnums;

namespace SolastaExtraContent
{
    public class Races
    {
        internal static void create()
        {
            createGnomes();
            createFirbolg();
        }


        static void createFirbolg()
        {
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("FirbolgRaceImage",
                            $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/FirbolgRace.png",
                            1024, 512);

            var ability_score_modifer = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AttributeModifierFirbolgWisdomAbilityScoreIncrease",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           null,
                                                                                                                           DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                                                                                                                           a =>
                                                                                                                           {
                                                                                                                               a.modifiedAttribute = Helpers.Stats.Wisdom;
                                                                                                                           }
                                                                                                                           );
            var ability_score_modifer2 = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AttributeModifierFirbolgStrengthAbilityScoreIncrease",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           null,
                                                                                                                           DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                                                                                                                           a =>
                                                                                                                           {
                                                                                                                               a.modifiedAttribute = Helpers.Stats.Strength;
                                                                                                                               a.modifierValue = 1;
                                                                                                                           }
                                                                                                                           );
            var language_proficiency = Helpers.CopyFeatureBuilder<FeatureDefinitionProficiency>.createFeatureCopy("ProficiencyFirbolgLanguages",
                                                                                                    "",
                                                                                                    "",
                                                                                                    "Feature/&FirbolgLanguagesDescription",
                                                                                                    null,
                                                                                                    DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDwarfLanguages,
                                                                                                    a =>
                                                                                                    {
                                                                                                        a.proficiencies = new List<string>
                                                                                                        {
                                                                                                            "Language_Common",
                                                                                                            "Language_Giant",
                                                                                                            "Language_Elvish",
                                                                                                        };
                                                                                                    }
                                                                                                    );

            var firbolg_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("FirbolgDruidicMagicSpellList",
                                                                                   "",
                                                                                   SolastaModHelpers.Common.common_no_title,
                                                                                   DatabaseHelper.SpellListDefinitions.SpellListDruid.spellsByLevel[0].spells
                                                                                  );

            var druidic_magic = Helpers.CopyFeatureBuilder<FeatureDefinitionCastSpell>.createFeatureCopy("FirbolgDruidicMagic",
                                                                                                         "",
                                                                                                         "Feature/&FirbolgDruidicMagicTitle",
                                                                                                         "Feature/&FirbolgDruidicMagicDescription",
                                                                                                         null,
                                                                                                         DatabaseHelper.FeatureDefinitionCastSpells.CastSpellElfHigh,
                                                                                                         a =>
                                                                                                         {
                                                                                                             a.spellListDefinition = firbolg_spelllist;
                                                                                                             a.spellcastingAbility = Helpers.Stats.Wisdom;
                                                                                                         }
                                                                                                         );

            var powerful_build = Helpers.CopyFeatureBuilder<FeatureDefinitionEquipmentAffinity>
                                                                                .createFeatureCopy("FirbolgPowerfulBuild",
                                                                                                    "",
                                                                                                    "Feature/&PowerfulBuildTitle",
                                                                                                    "Feature/&PowerfulBuildDescription",
                                                                                                    null,
                                                                                                    DatabaseHelper.FeatureDefinitionEquipmentAffinitys.EquipmentAffinityFeatHauler);

            var invisibility_effect = new EffectDescription();
            invisibility_effect.Copy(DatabaseHelper.SpellDefinitions.Invisibility.effectDescription);
            invisibility_effect.targetType = RuleDefinitions.TargetType.Self;
            invisibility_effect.rangeType = RuleDefinitions.RangeType.Self;
            invisibility_effect.effectAdvancement.Clear();
            invisibility_effect.durationParameter = 1;
            invisibility_effect.durationType = RuleDefinitions.DurationType.Round;
            invisibility_effect.endOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;

            var power = Helpers.GenericPowerBuilder<FeatureDefinitionPower>.createPower("FirbolgInvisibilityPower",
                                                                                        "",
                                                                                        "Feature/&FirbolgInvisibilityPowerTitle",
                                                                                        "Feature/&FirbolgInvisibilityPowerDescription",
                                                                                        DatabaseHelper.SpellDefinitions.Invisibility.GuiPresentation.spriteReference,
                                                                                        invisibility_effect,
                                                                                        RuleDefinitions.ActivationTime.BonusAction,
                                                                                        1,
                                                                                        RuleDefinitions.UsesDetermination.Fixed,
                                                                                        RuleDefinitions.RechargeRate.ShortRest,
                                                                                        show_casting: true);


            var firbolgs = Helpers.CopyFeatureBuilder<CharacterRaceDefinition>.createFeatureCopy("FirbolgRace",
                                                                                  "",
                                                                                  "Race/&FirbolgTitle",
                                                                                  "Race/&FirbolgDescription",
                                                                                  sprite,
                                                                                  DatabaseHelper.CharacterRaceDefinitions.Human,
                                                                                  a =>
                                                                                  {
                                                                                      a.sizeDefinition = DatabaseHelper.CharacterSizeDefinitions.Medium;
                                                                                      a.minimalAge = 30;
                                                                                      a.maximalAge = 500;
                                                                                      a.baseHeight = 96;
                                                                                      a.baseWeight = 130;
                                                                                      a.featureUnlocks.Clear();
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove6, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(ability_score_modifer, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(ability_score_modifer2, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(language_proficiency, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(druidic_magic, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(powerful_build, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(power, 1));
                                                                                      a.racePresentation = Helpers.Accessors.memberwiseClone(DatabaseHelper.CharacterRaceDefinitions.Elf.racePresentation);
                                                                                      a.racePresentation.bodyAssetPrefix = DatabaseHelper.CharacterRaceDefinitions.Elf.racePresentation.bodyAssetPrefix;
                                                                                      a.racePresentation.morphotypeAssetPrefix = DatabaseHelper.CharacterRaceDefinitions.Elf.racePresentation.morphotypeAssetPrefix;
                                                                                      a.racePresentation.preferedSkinColors = new TA.RangedInt(45, 48);
                                                                                      a.racePresentation.preferedHairColors = new TA.RangedInt(16, 32);
                                                                                      a.racePresentation.maleBeardShapeOptions = DatabaseHelper.CharacterRaceDefinitions.Dwarf.racePresentation.maleBeardShapeOptions;
                                                                                      a.racePresentation.beardBlendShapes = DatabaseHelper.CharacterRaceDefinitions.Dwarf.racePresentation.beardBlendShapes;
                                                                                      a.racePresentation.useBeardBlendShape = DatabaseHelper.CharacterRaceDefinitions.Dwarf.racePresentation.useBeardBlendShape;
                                                                                  }
                                                                                  );
            NewFeatureDefinitions.RaceData.raceScaleMap[firbolgs] = 7.4f / 6f;
            var focused_sleeper = DatabaseHelper.FeatDefinitions.FocusedSleeper;
            focused_sleeper.CompatibleRacesPrerequisite.Add(firbolgs.name);
        }

        static void createGnomes()
        {
            var sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("GnomeRaceImage",
                                        $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/GnomeRace.png",
                                        1024, 512);

            var ability_score_modifer = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AttributeModifierGnomeAbilityScoreIncrease",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           null,
                                                                                                                           DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                                                                                                                           a =>
                                                                                                                           {
                                                                                                                               a.modifiedAttribute = Helpers.Stats.Intelligence;
                                                                                                                           }
                                                                                                                           );
            var forest_gnome_score_modifer = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AttributeModifierForestGnomeAbilityScoreIncrease",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           null,
                                                                                                                           DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease,
                                                                                                                           a =>
                                                                                                                           {
                                                                                                                               a.modifiedAttribute = Helpers.Stats.Dexterity;
                                                                                                                               a.modifierValue = 1;
                                                                                                                           }
                                                                                                                           );
            var language_gnomish = Helpers.CopyFeatureBuilder<LanguageDefinition>.createFeatureCopy("GnomishLanguage",
                                                                                                    "",
                                                                                                    "Language/&GnomishTitle",
                                                                                                    "Language/&GnomishDescription",
                                                                                                    null,
                                                                                                    DatabaseHelper.LanguageDefinitions.Language_Goblin
                                                                                                    );
            var language_proficiency = Helpers.CopyFeatureBuilder<FeatureDefinitionProficiency>.createFeatureCopy("ProficiencyGnomishLanguages",
                                                                                                    "",
                                                                                                    "",
                                                                                                    "Feature/&GnomeLanguagesDescription",
                                                                                                    null,
                                                                                                    DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyDwarfLanguages,
                                                                                                    a =>
                                                                                                    {
                                                                                                        a.proficiencies = new List<string>
                                                                                                        {
                                                                                                            "Language_Common",
                                                                                                            language_gnomish.name,                                                             
                                                                                                        };
                                                                                                    }
                                                                                                    );

            var gnome_cunning = Helpers.SavingThrowAffinityBuilder.createSavingthrowAffinityAgainstSchools("GnomeCunningFeature",
                                                                                                           "",
                                                                                                           "Feature/&GnomeCunningTitle",
                                                                                                           "Feature/&GnomeCunningDescription",
                                                                                                           SolastaModHelpers.Common.common_no_icon,
                                                                                                           RuleDefinitions.CharacterSavingThrowAffinity.Advantage,
                                                                                                           0,
                                                                                                           RuleDefinitions.DieType.D1,
                                                                                                           Helpers.SpellSchools.getAllSchools().ToList(),
                                                                                                           Helpers.Stats.Intelligence,
                                                                                                           Helpers.Stats.Wisdom,
                                                                                                           Helpers.Stats.Charisma
                                                                                                           );

            var gnome_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("NaturalIllusionistSpellList",
                                                                                 "",
                                                                                 SolastaModHelpers.Common.common_no_title,
                                                                                 new List<SpellDefinition> { DatabaseHelper.SpellDefinitions.AnnoyingBee }
                                                                                 );

            var natural_illusionist = Helpers.CopyFeatureBuilder<FeatureDefinitionCastSpell>.createFeatureCopy("GnomeNaturalIllusionist",
                                                                                                               "",
                                                                                                               "Feature/&GnomeNaturalIllusionistTitle",
                                                                                                               "Feature/&GnomeNaturalIllusionistDescription",
                                                                                                               null,
                                                                                                               DatabaseHelper.FeatureDefinitionCastSpells.CastSpellElfHigh,
                                                                                                               a =>
                                                                                                               {
                                                                                                                   a.spellListDefinition = gnome_spelllist;
                                                                                                               }
                                                                                                               );
            var gnomes = Helpers.CopyFeatureBuilder<CharacterRaceDefinition>.createFeatureCopy("GnomeRace",
                                                                                  "",
                                                                                  "Race/&ForestGnomeTitle",
                                                                                  "Race/&ForestGnomeDescription",
                                                                                  sprite,
                                                                                  DatabaseHelper.CharacterRaceDefinitions.Human,
                                                                                  a =>
                                                                                  {
                                                                                      a.sizeDefinition = DatabaseHelper.CharacterSizeDefinitions.Small;
                                                                                      a.minimalAge = 40;
                                                                                      a.maximalAge = 350;
                                                                                      a.baseHeight = 47;
                                                                                      a.baseWeight = 35;
                                                                                      a.featureUnlocks.Clear();
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove5, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(ability_score_modifer, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(forest_gnome_score_modifer, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(gnome_cunning, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(natural_illusionist, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(language_proficiency, 1));
                                                                                      a.racePresentation = Helpers.Accessors.memberwiseClone(DatabaseHelper.CharacterRaceDefinitions.Halfling.racePresentation);
                                                                                      a.racePresentation.bodyAssetPrefix = DatabaseHelper.CharacterRaceDefinitions.Elf.racePresentation.bodyAssetPrefix;
                                                                                      a.racePresentation.morphotypeAssetPrefix = DatabaseHelper.CharacterRaceDefinitions.Elf.racePresentation.morphotypeAssetPrefix;
                                                                                      a.racePresentation.preferedHairColors = new TA.RangedInt(26, 47);
                                                                                  }                                                                               
                                                                                  );
            NewFeatureDefinitions.RaceData.raceScaleMap[gnomes] = 4.8f / 6f; //halflings seem to be 4.5f / 6f
            var focused_sleeper = DatabaseHelper.FeatDefinitions.FocusedSleeper;
            focused_sleeper.CompatibleRacesPrerequisite.Add(gnomes.name);
        }
    }
}
