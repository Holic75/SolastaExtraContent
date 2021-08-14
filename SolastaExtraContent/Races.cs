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
                                                                                      a.featureUnlocks.Clear();
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove5, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(ability_score_modifer, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(forest_gnome_score_modifer, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(gnome_cunning, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(natural_illusionist, 1));
                                                                                      a.featureUnlocks.Add(new FeatureUnlockByLevel(language_proficiency, 1));
                                                                                      a.racePresentation = DatabaseHelper.CharacterRaceDefinitions.Halfling.racePresentation;
                                                                                  }                                                                               
                                                                                  );
        }
    }
}
