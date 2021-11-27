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
    internal class BardClassBuilder : CharacterClassDefinitionBuilder
    {
        const string BardClassName = "BardClass";
        const string BardClassNameGuid = "274106b8-0376-4bcd-bd1b-440633a394ae";
        const string BardClassSubclassesGuid = "be865126-d7c3-45f3-b891-e77bd8b00cb1";

        static public RuleDefinitions.DieType[] inspiration_dice = new RuleDefinitions.DieType[] { RuleDefinitions.DieType.D6, RuleDefinitions.DieType.D8, RuleDefinitions.DieType.D10, RuleDefinitions.DieType.D12 };
        static public CharacterClassDefinition bard_class;
        static public Dictionary<RuleDefinitions.DieType, FeatureDefinitionPower> inspiration_powers = new Dictionary<RuleDefinitions.DieType, FeatureDefinitionPower>();
        static public FeatureDefinition font_of_inspiration;
        static public FeatureDefinitionPointPool expertise;
        static public FeatureDefinitionAbilityCheckAffinity jack_of_all_trades;
        static public Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionExtraHealingDieOnShortRest> song_of_rest = new Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionExtraHealingDieOnShortRest>();
        static public SpellListDefinition bard_spelllist;
        static public FeatureDefinitionCastSpell bard_spellcasting;
        static public SpellListDefinition magical_secrets_spelllist;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection magical_secrets;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection magical_secrets14;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection magical_secrets18;
        static public FeatureDefinitionPower countercharm;

        static public Dictionary<RuleDefinitions.DieType, FeatureDefinitionFeatureSet> cutting_words = new Dictionary<RuleDefinitions.DieType, FeatureDefinitionFeatureSet>();

        static public FeatureDefinitionPointPool lore_college_bonus_proficiencies;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection additional_magical_secrets;

        static public FeatureDefinitionFeatureSet virtue_college_bonus_proficiencies;
        static public Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt> music_of_spheres
            = new Dictionary<RuleDefinitions.DieType, NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt>();
        static public FeatureDefinitionAttributeModifier virtue_college_extra_attack;

        static public FeatureDefinitionFeatureSet nature_college_bonus_proficiencies;
        static public NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection nature_college_extra_cantrip;
        static public FeatureDefinitionFeatureSet natural_focus;
        static public FeatureDefinition environmental_magical_secrets;

        protected BardClassBuilder(string name, string guid) : base(name, guid)
        {
            var bard_class_image = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("BardClassImage",
                                                                                           $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/BardClass.png",
                                                                                           1024, 576);
            var rogue = DatabaseHelper.CharacterClassDefinitions.Rogue;
            bard_class = Definition;
            Definition.GuiPresentation.Title = "Class/&BardClassTitle";
            Definition.GuiPresentation.Description = "Class/&BardClassDescription";
            Definition.GuiPresentation.SetSpriteReference(bard_class_image);

            Definition.SetClassAnimationId(AnimationDefinitions.ClassAnimationId.Cleric);
            Definition.SetClassPictogramReference(rogue.ClassPictogramReference);
            Definition.SetDefaultBattleDecisions(rogue.DefaultBattleDecisions);
            Definition.SetHitDice(RuleDefinitions.DieType.D8);
            Definition.SetIngredientGatheringOdds(rogue.IngredientGatheringOdds);
            Definition.SetRequiresDeity(false);

            Definition.AbilityScoresPriority.Clear();
            Definition.AbilityScoresPriority.AddRange(new List<string> {Helpers.Stats.Charisma,
                                                                        Helpers.Stats.Dexterity,
                                                                        Helpers.Stats.Constitution,
                                                                        Helpers.Stats.Intelligence,
                                                                        Helpers.Stats.Strength,
                                                                        Helpers.Stats.Wisdom});

            Definition.FeatAutolearnPreference.AddRange(rogue.FeatAutolearnPreference);
            Definition.PersonalityFlagOccurences.AddRange(rogue.PersonalityFlagOccurences);

            Definition.SkillAutolearnPreference.Clear();
            Definition.SkillAutolearnPreference.AddRange(new List<string> { Helpers.Skills.Persuasion,
                                                                            Helpers.Skills.Deception,
                                                                            Helpers.Skills.Acrobatics,
                                                                            Helpers.Skills.Stealth,
                                                                            Helpers.Skills.Intimidation,
                                                                            Helpers.Skills.Arcana,
                                                                            Helpers.Skills.History,
                                                                            Helpers.Skills.Insight });

            Definition.ToolAutolearnPreference.Clear();
            Definition.ToolAutolearnPreference.AddRange(new List<string> { Helpers.Tools.ThievesTool, Helpers.Tools.EnchantingTool, Helpers.Tools.Lyre });


            Definition.EquipmentRows.AddRange(rogue.EquipmentRows);
            Definition.EquipmentRows.Clear();

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Rapier, EquipmentDefinitions.OptionWeapon, 1),
                                    },
                                new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Longsword, EquipmentDefinitions.OptionWeapon, 1),
                                    }
            );
            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ExplorerPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    },
                                new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.BurglarPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    }
            );
            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.EnchantingTool, EquipmentDefinitions.OptionTool, 1),
                                    },
                                new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                         EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ThievesTool, EquipmentDefinitions.OptionTool, 1),
                                    }
            );

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                 EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Bolt, EquipmentDefinitions.OptionWeapon, 20),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Leather, EquipmentDefinitions.OptionArmor, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ComponentPouch, EquipmentDefinitions.OptionFocus, 1)
            });

            var saving_throws = Helpers.ProficiencyBuilder.CreateSavingthrowProficiency("BardSavingthrowProficiency",
                                                                                        "88d8752b-4956-4daf-91fc-84e6196c3985",
                                                                                        Helpers.Stats.Charisma, Helpers.Stats.Dexterity);

            var armor_proficiency = Helpers.ProficiencyBuilder.createCopy("BardArmorProficiency",
                                                                          "06d31b31-69db-40d7-8701-a8547c4dd063",
                                                                          "Feature/&BardArmorProficiencyTitle",
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyRogueArmor
                                                                          );

            var weapon_proficiency = Helpers.ProficiencyBuilder.createCopy("BardWeaponProficiency",
                                                                          "9a0ef52f-052a-4838-b3d4-2096ab67453e",
                                                                          "Feature/&BardWeaponProficiencyTitle",
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyRogueWeapon
                                                                          );

            var tools_proficiency = Helpers.ProficiencyBuilder.CreateToolsProficiency("BardToolsProficiency",
                                                                                      "96d8987b-e682-44a6-afdb-763cbe5361ad",
                                                                                      "Feature/&BardToolsProficiencyTitle",
                                                                                      Helpers.Tools.Lyre
                                                                                      );
            tools_proficiency.guiPresentation.hidden = true;
            var skills = Helpers.PoolBuilder.createSkillProficiency("BardSkillProficiency",
                                                                    "029f6c7e-f1fc-4030-9012-9c698c714f00",
                                                                    "Feature/&BardClassSkillPointPoolTitle",
                                                                    "Feature/&BardClassSkillPointPoolDescription",
                                                                    3,
                                                                    Helpers.Skills.getAllSkills());

            var tools_proficiency2 = Helpers.PoolBuilder.createToolProficiency("BardToolsProficiency2",
                                                                               "8333d184-c6d2-4429-a5fd-6810e2003833",
                                                                               "Feature/&BardToolsProficiencyTitle",
                                                                               "Feature/&BardToolsProficiencyDescription",
                                                                               1,
                                                                               Helpers.Tools.EnchantingTool, Helpers.Tools.ThievesTool);

            expertise = Helpers.CopyFeatureBuilder<FeatureDefinitionPointPool>.createFeatureCopy("BardExpertise",
                                                                                                 "",
                                                                                                 "Feature/&BardClassExpertisePointPoolTitle",
                                                                                                 "Feature/&BardClassExpertisePointPoolDescription",
                                                                                                 null,
                                                                                                 DatabaseHelper.FeatureDefinitionPointPools.PointPoolRogueExpertise);
            expertise.RestrictedChoices.Clear();
            expertise.RestrictedChoices.Add(Helpers.Tools.Lyre);


            var ritual_spellcasting = Helpers.RitualSpellcastingBuilder.createRitualSpellcasting("BardRitualSpellcasting",
                                                                                                 "25c48b9b-e2e9-4ea7-8a80-e6c413275980",
                                                                                                 "Feature/&BardClassRitualCastingDescription",
                                                                                                 (RuleDefinitions.RitualCasting)ExtendedEnums.ExtraRitualCasting.Spontaneous);

            bard_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("BardClassSpelllist", "0f3d14a7-f9a1-41ec-a164-f3e0f3800104", "",
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.AnnoyingBee,
                                                                                    DatabaseHelper.SpellDefinitions.DancingLights,
                                                                                    DatabaseHelper.SpellDefinitions.Dazzle,
                                                                                    DatabaseHelper.SpellDefinitions.Light,
                                                                                    DatabaseHelper.SpellDefinitions.ShadowArmor,
                                                                                    DatabaseHelper.SpellDefinitions.ShadowDagger,
                                                                                    DatabaseHelper.SpellDefinitions.Shine,
                                                                                    DatabaseHelper.SpellDefinitions.Sparkle,
                                                                                    DatabaseHelper.SpellDefinitions.TrueStrike,
                                                                                    Cantrips.vicious_mockery,
                                                                                    Cantrips.thunder_strike
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.AnimalFriendship,
                                                                                    DatabaseHelper.SpellDefinitions.Bane,
                                                                                    DatabaseHelper.SpellDefinitions.CharmPerson,
                                                                                    DatabaseHelper.SpellDefinitions.ColorSpray,
                                                                                    //DatabaseHelper.SpellDefinitions.Command,
                                                                                    DatabaseHelper.SpellDefinitions.ComprehendLanguages,
                                                                                    DatabaseHelper.SpellDefinitions.CureWounds,
                                                                                    DatabaseHelper.SpellDefinitions.DetectMagic,
                                                                                    DatabaseHelper.SpellDefinitions.FaerieFire,
                                                                                    DatabaseHelper.SpellDefinitions.FeatherFall,
                                                                                    DatabaseHelper.SpellDefinitions.HealingWord,
                                                                                    DatabaseHelper.SpellDefinitions.Heroism,
                                                                                    DatabaseHelper.SpellDefinitions.Identify,
                                                                                    DatabaseHelper.SpellDefinitions.Longstrider,
                                                                                    DatabaseHelper.SpellDefinitions.Sleep,
                                                                                    DatabaseHelper.SpellDefinitions.HideousLaughter,
                                                                                    DatabaseHelper.SpellDefinitions.Thunderwave
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.Aid,
                                                                                    DatabaseHelper.SpellDefinitions.Blindness,
                                                                                    DatabaseHelper.SpellDefinitions.CalmEmotions,
                                                                                    DatabaseHelper.SpellDefinitions.EnhanceAbility,
                                                                                    DatabaseHelper.SpellDefinitions.HoldPerson,
                                                                                    DatabaseHelper.SpellDefinitions.Invisibility,
                                                                                    DatabaseHelper.SpellDefinitions.Knock,
                                                                                    DatabaseHelper.SpellDefinitions.LesserRestoration,
                                                                                    //DatabaseHelper.SpellDefinitions.MirrorImage,
                                                                                    DatabaseHelper.SpellDefinitions.HeatMetal,
                                                                                    DatabaseHelper.SpellDefinitions.SeeInvisibility,
                                                                                    DatabaseHelper.SpellDefinitions.Shatter,
                                                                                    DatabaseHelper.SpellDefinitions.Silence
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.BestowCurse,
                                                                                    DatabaseHelper.SpellDefinitions.DispelMagic,
                                                                                    DatabaseHelper.SpellDefinitions.Fear,
                                                                                    DatabaseHelper.SpellDefinitions.HypnoticPattern,
                                                                                    DatabaseHelper.SpellDefinitions.MassHealingWord,
                                                                                    DatabaseHelper.SpellDefinitions.Slow,
                                                                                    DatabaseHelper.SpellDefinitions.StinkingCloud,
                                                                                    DatabaseHelper.SpellDefinitions.Tongues
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.Confusion,
                                                                                    DatabaseHelper.SpellDefinitions.DimensionDoor,
                                                                                    DatabaseHelper.SpellDefinitions.FreedomOfMovement,
                                                                                    DatabaseHelper.SpellDefinitions.GreaterInvisibility,
                                                                                    DatabaseHelper.SpellDefinitions.PhantasmalKiller
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.DominatePerson,
                                                                                    DatabaseHelper.SpellDefinitions.GreaterRestoration,
                                                                                    DatabaseHelper.SpellDefinitions.HoldMonster,
                                                                                    DatabaseHelper.SpellDefinitions.MassCureWounds,
                                                                                    DatabaseHelper.SpellDefinitions.RaiseDead
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.Eyebite,
                                                                                    DatabaseHelper.SpellDefinitions.HeroesFeast,
                                                                                    DatabaseHelper.SpellDefinitions.TrueSeeing,
                                                                                },
                                                                                new List<SpellDefinition>{
                                                                                    DatabaseHelper.SpellDefinitions.Resurrection,
                                                                                }
                                                                                );

            bard_spellcasting = Helpers.SpellcastingBuilder.createSpontaneousSpellcasting("BardClassSpellcasting",
                                                                                              "f720edaf-92c4-43e3-8228-c48c0b41b93b",
                                                                                              "Feature/&BardClassSpellcastingTitle",
                                                                                              "Feature/&BardClassSpellcastingDescription",
                                                                                              bard_spelllist,
                                                                                              Helpers.Stats.Charisma,
                                                                                              new List<int> { 2,  2,  2,  3,  3,  3, 3, 3, 3, 4,
                                                                                                              4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
                                                                                              new List<int> { 4,  5,  6,  7,  8,  9, 10, 11, 12, 12,
                                                                                                             13, 13, 14, 14, 15, 15, 16, 16, 16, 16},
                                                                                              DatabaseHelper.FeatureDefinitionCastSpells.CastSpellWizard.SlotsPerLevels
                                                                                              );
            bard_spellcasting.replacedSpells = new List<int> {0, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                                                              1, 1, 1, 1, 1, 1, 1, 1, 1, 1};
            bard_spellcasting.focusType = EquipmentDefinitions.FocusType.Arcane;
            jack_of_all_trades = Helpers.AbilityCheckAffinityBuilder.createAbilityCheckAffinity("BardClassJackOfAllTradesFeature",
                                                                                                 "",
                                                                                                "Feature/&BardClassJackOfAllTradesFeatureTitle",
                                                                                                "Feature/&BardClassJackOfAllTradesFeatureDescription",
                                                                                                 null,
                                                                                                 RuleDefinitions.CharacterAbilityCheckAffinity.HalfProficiencyWhenNotProficient,
                                                                                                 0,
                                                                                                 RuleDefinitions.DieType.D1,
                                                                                                 Helpers.Stats.getAllStats().ToArray()
                                                                                                 );



            createInspiration();
            createSongOfRest();
            createMagicalSecrets();
            createCountercharm();
            Definition.FeatureUnlocks.Clear();
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(saving_throws, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(armor_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(weapon_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(skills, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(tools_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(tools_proficiency2, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(bard_spellcasting, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(ritual_spellcasting, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(inspiration_powers[RuleDefinitions.DieType.D6], 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(song_of_rest[RuleDefinitions.DieType.D6], 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(jack_of_all_trades, 3));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(expertise, 3));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(inspiration_powers[RuleDefinitions.DieType.D8], 5));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(font_of_inspiration, 5));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(countercharm, 6));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(song_of_rest[RuleDefinitions.DieType.D8], 9));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(magical_secrets, 10));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(inspiration_powers[RuleDefinitions.DieType.D10], 10));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(expertise, 10));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(song_of_rest[RuleDefinitions.DieType.D10], 13));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(magical_secrets14, 14));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(inspiration_powers[RuleDefinitions.DieType.D12], 15));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(song_of_rest[RuleDefinitions.DieType.D12], 17));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(magical_secrets18, 18));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19));

            var subclassChoicesGuiPresentation = new GuiPresentation();
            subclassChoicesGuiPresentation.Title = "Subclass/&BardSubclassCollegeTitle";
            subclassChoicesGuiPresentation.Description = "Subclass/&BardSubclassCollegeDescription";
            BardFeatureDefinitionSubclassChoice = this.BuildSubclassChoice(3, "College", false, "SubclassChoiceBardSpecialistArchetypes", subclassChoicesGuiPresentation, BardClassSubclassesGuid);

            var itemlist = new List<ItemDefinition>
            {
                DatabaseHelper.ItemDefinitions.WandOfLightningBolts,
                //DatabaseHelper.ItemDefinitions.StaffOfMetis,              // devs removed class restrictions for HF 1.1.11 so not needed now
                DatabaseHelper.ItemDefinitions.StaffOfHealing,
                DatabaseHelper.ItemDefinitions.ArcaneShieldstaff,
                DatabaseHelper.ItemDefinitions.WizardClothes_Alternate
            };

            foreach (ItemDefinition item in itemlist)
            {
                item.RequiredAttunementClasses.Add(bard_class);
            };
        }


        static CharacterSubclassDefinition createNatureCollege()
        {
            createNatureCollegeBonusProficienies();
            createNatureCollegeExtraCantrip();
            createNaturalFocus();
            createEnvironmentalMagicalSecrets();


            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BardSubclassCollegeOfNatureDescription",
                    "Subclass/&BardSubclassCollegeOfNatureTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionGreenmage.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BardSubclassCollegeOfNature", "c943389a-f8a1-4ad4-9400-2ab97a4a3541")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(nature_college_bonus_proficiencies, 3)
                                                                                            .AddFeatureAtLevel(nature_college_extra_cantrip, 3)
                                                                                            .AddFeatureAtLevel(natural_focus, 3)
                                                                                            .AddFeatureAtLevel(environmental_magical_secrets, 6)
                                                                                            .AddToDB();

            return definition;
        }


        static void createEnvironmentalMagicalSecrets()
        {
            var nature_focus_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("BardNatureSubclassEnvironmentalMagicalSecretsSpelllist", "", "",
                                                                                        new List<SpellDefinition>
                                                                                        {

                                                                                        },
                                                                                        new List<SpellDefinition>
                                                                                        {
                                                                                                        DatabaseHelper.SpellDefinitions.Entangle,
                                                                                                        DatabaseHelper.SpellDefinitions.FogCloud,
                                                                                                        DatabaseHelper.SpellDefinitions.HuntersMark,
                                                                                                        DatabaseHelper.SpellDefinitions.Jump,
                                                                                                        DatabaseHelper.SpellDefinitions.Goodberry,
                                                                                        },
                                                                                        new List<SpellDefinition>
                                                                                        {
                                                                                                        DatabaseHelper.SpellDefinitions.Barkskin,
                                                                                                        DatabaseHelper.SpellDefinitions.Darkvision,
                                                                                                        DatabaseHelper.SpellDefinitions.FindTraps,
                                                                                                        DatabaseHelper.SpellDefinitions.PassWithoutTrace,
                                                                                                        DatabaseHelper.SpellDefinitions.ProtectionFromPoison,
                                                                                                        DatabaseHelper.SpellDefinitions.FlameBlade,
                                                                                                        DatabaseHelper.SpellDefinitions.FlamingSphere,
                                                                                                        DatabaseHelper.SpellDefinitions.SpikeGrowth,
                                                                                                        Spells.conjure_spirit_animal
                                                                                        },
                                                                                        new List<SpellDefinition>
                                                                                        {
                                                                                                        DatabaseHelper.SpellDefinitions.ConjureAnimals,
                                                                                                        DatabaseHelper.SpellDefinitions.CreateFood,
                                                                                                        DatabaseHelper.SpellDefinitions.Daylight,
                                                                                                        DatabaseHelper.SpellDefinitions.ProtectionFromEnergy,
                                                                                                        DatabaseHelper.SpellDefinitions.SleetStorm,
                                                                                                        DatabaseHelper.SpellDefinitions.WindWall,
                                                                                                        DatabaseHelper.SpellDefinitions.CallLightning,
                                                                                                        Spells.earth_tremor,
                                                                                                        Spells.winter_blast
                                                                                        },
                                                                                        new List<SpellDefinition>
                                                                                        {
                                                                                                        DatabaseHelper.SpellDefinitions.Blight,
                                                                                                        DatabaseHelper.SpellDefinitions.FireShield,
                                                                                                        DatabaseHelper.SpellDefinitions.IceStorm,
                                                                                                        DatabaseHelper.SpellDefinitions.IdentifyCreatures,
                                                                                                        DatabaseHelper.SpellDefinitions.GiantInsect,
                                                                                                        DatabaseHelper.SpellDefinitions.Stoneskin,
                                                                                                        DatabaseHelper.SpellDefinitions.WallOfFire,
                                                                                        },
                                                                                        new List<SpellDefinition>
                                                                                        {
                                                                                                        DatabaseHelper.SpellDefinitions.ConjureElemental,
                                                                                                        DatabaseHelper.SpellDefinitions.ConeOfCold,
                                                                                                        DatabaseHelper.SpellDefinitions.Contagion,
                                                                                                        DatabaseHelper.SpellDefinitions.InsectPlague,
                                                                                        },
                                                                                        new List<SpellDefinition>
                                                                                        {
                                                                                                        DatabaseHelper.SpellDefinitions.ConjureFey,
                                                                                                        DatabaseHelper.SpellDefinitions.Heal,
                                                                                                        DatabaseHelper.SpellDefinitions.Sunbeam,
                                                                                                        DatabaseHelper.SpellDefinitions.WallOfThorns,
                                                                                        }
                                                                                        );

            string title = "Feature/&BardNatureSubclassEnvironmentalMagicalSecretsTitle";
            string description = "Feature/&BardNatureSubclassEnvironmentalMagicalSecretsDescription";
            environmental_magical_secrets = Helpers.CopyFeatureBuilder<FeatureDefinitionMagicAffinity>.createFeatureCopy("BardNatureSubclassEnvironmentalMagicalSecrets",
                                                                                                                         "",
                                                                                                                         title,
                                                                                                                         description,
                                                                                                                         null,
                                                                                                                         DatabaseHelper.FeatureDefinitionMagicAffinitys.MagicAffinityGreenmageGreenMagicList,
                                                                                                                         c =>
                                                                                                                         {
                                                                                                                             c.SetExtendedSpellList(nature_focus_spelllist);
                                                                                                                         }
                                                                                                                         );
        }

        static void createNaturalFocus()
        {
            var forest_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionMovementAffinity>.createFeatureCopy("BardNatureSubclassNaturalFocusForestFeature",
                                                                                                                 "",
                                                                                                                 Common.common_no_title,
                                                                                                                 Common.common_no_title,
                                                                                                                 null,
                                                                                                                 DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityDarkweaverSpiderOnWall,
                                                                                                                 a =>
                                                                                                                 {
                                                                                                                     a.SetExpertClimber(false);
                                                                                                                 }
                                                                                                                 );

            var grassland_movement_bonus_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionMovementAffinity>.createFeatureCopy("BardNatureSubclassNaturalFocusGrasslandBonus",
                                                                                                                                   "",
                                                                                                                                   "",
                                                                                                                                   "",
                                                                                                                                   null,
                                                                                                                                   DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider,
                                                                                                                                   a =>
                                                                                                                                   {
                                                                                                                                       a.baseSpeedAdditiveModifier = 1;
                                                                                                                                   }
                                                                                                                                   );
                                      
            var grassland_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.MovementBonusWithRestrictions>.createFeature("BardNatureSubclassNaturalFocusGrasslandEffect",
                                                                                                                      "",
                                                                                                                      Common.common_no_title,
                                                                                                                      Common.common_no_title,
                                                                                                                      null,
                                                                                                                      a =>
                                                                                                                      {
                                                                                                                          a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                          {
                                                                                                                              new NewFeatureDefinitions.ArmorTypeRestriction(DatabaseHelper.ArmorCategoryDefinitions.HeavyArmorCategory, inverted: true)
                                                                                                                          };
                                                                                                                          a.modifiers = new List<FeatureDefinition> { grassland_movement_bonus_feature };
                                                                                                                      }
                                                                                                                      );
            var bonus_spells = Helpers.FeatureBuilder<NewFeatureDefinitions.FeatureDefinitionExtraSpellsKnown>.createFeature("BardNatureSubclassNaturalFocusBonusSpells",
                                                                                                                             "",
                                                                                                                             Common.common_no_title,
                                                                                                                             Common.common_no_title,
                                                                                                                             null,
                                                                                                                             b =>
                                                                                                                             {
                                                                                                                                 b.caster_class = bard_class;
                                                                                                                                 b.level = 6;
                                                                                                                                 b.max_spells = 2;
                                                                                                                             }
                                                                                                                             );
            string title = "Feature/&BardNatureSubclassNaturalFocusTitle";
            string description = "Feature/&BardNatureSubclassNaturalFocusDescription";
            Dictionary<string, (FeatureDefinition feature, SpellDefinition lvl2_spell, SpellDefinition lvl3_spell)> foci
                            = new Dictionary<string, (FeatureDefinition feature, SpellDefinition lvl2_spell, SpellDefinition lvl3_spell)>()
                            {
                                {"Arctic", (DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance, DatabaseHelper.SpellDefinitions.HoldPerson, DatabaseHelper.SpellDefinitions.SleetStorm) },
                                {"Desert", (DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance, DatabaseHelper.SpellDefinitions.ScorchingRay, DatabaseHelper.SpellDefinitions.WindWall) },
                                {"Forest", (forest_feature, DatabaseHelper.SpellDefinitions.Barkskin, DatabaseHelper.SpellDefinitions.ConjureAnimals) },
                                {"Grassland", (grassland_feature, DatabaseHelper.SpellDefinitions.PassWithoutTrace, DatabaseHelper.SpellDefinitions.WindWall) },
                                {"Mountain", (DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance, DatabaseHelper.SpellDefinitions.SpiderClimb, DatabaseHelper.SpellDefinitions.Fly) },
                                {"Swamp", (DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance, DatabaseHelper.SpellDefinitions.AcidArrow, DatabaseHelper.SpellDefinitions.StinkingCloud) }
                            };

            List<FeatureDefinitionFeatureSet> features = new List<FeatureDefinitionFeatureSet>();


            foreach (var kv in foci)
            {
                var extra_spells = Helpers.FeatureBuilder<NewFeatureDefinitions.GrantSpells>.createFeature(kv.Key + "BardNatureSubclassBonusSpells",
                                                                                                           "",
                                                                                                           Common.common_no_title,
                                                                                                           Common.common_no_title,
                                                                                                           null,
                                                                                                           f =>
                                                                                                           {
                                                                                                               f.spellcastingClass = bard_class;
                                                                                                               f.spellGroups = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>()
                                                                                                               {
                                                                                                                   new FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup()
                                                                                                                   {
                                                                                                                       ClassLevel = 6,
                                                                                                                       SpellsList = new List<SpellDefinition>() { kv.Value.lvl2_spell, kv.Value.lvl3_spell }
                                                                                                                   }
                                                                                                               };
                                                                                                           }
                                                                                                           );

                var feature = Helpers.FeatureSetBuilder.createFeatureSet(kv.Key + "BardNatureSubclassNaturalFocus",
                                                                         "",
                                                                         "Feature/&BardNatureSubclass" + kv.Key + "NaturalFocusTitle",
                                                                         "Feature/&BardNatureSubclass" + kv.Key + "NaturalFocusDescription",
                                                                         false,
                                                                         FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                         false,
                                                                         extra_spells, kv.Value.feature, bonus_spells
                                                                         );
                features.Add(feature);
            }
            natural_focus = Helpers.FeatureSetBuilder.createFeatureSet("BardNatureSubclassNaturalFocus",
                                                                        "",
                                                                        title,
                                                                        description,
                                                                        false,
                                                                        FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion,
                                                                        false,
                                                                        features.ToArray()
                                                                        );
        }


        static void createNatureCollegeExtraCantrip()
        {
            string title = "Feature/&BardNatureSubclassBonusCantripTitle";
            string description = "Feature/&BardNatureSubclassBonusCantripDescription";

            var cantrips = new List<SpellDefinition> {  DatabaseHelper.SpellDefinitions.AnnoyingBee,
                                                        DatabaseHelper.SpellDefinitions.Guidance,
                                                        DatabaseHelper.SpellDefinitions.PoisonSpray,
                                                        DatabaseHelper.SpellDefinitions.Resistance,
                                                        DatabaseHelper.SpellDefinitions.Shine,
                                                        DatabaseHelper.SpellDefinitions.Sparkle,
                                                        DatabaseHelper.SpellDefinitions.Shillelagh,
                                                        DatabaseHelper.SpellDefinitions.ProduceFlame,
                                                        DatabaseHelper.SpellDefinitions.VenomousSpike,
                                                        Cantrips.air_blast,
                                                        Cantrips.frostbite,
                                                        Cantrips.acid_claws};

            List<FeatureDefinition> learn_features = new List<FeatureDefinition>();

            var spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("BardNatureSubclassBonusCantripSpelllist",
                                                                           "",
                                                                           Common.common_no_title,
                                                                           cantrips
                                                                           );
            nature_college_extra_cantrip = Helpers.ExtraSpellSelectionBuilder.createExtraCantripSelection("BardNatureSubclassBonusCantrip",
                                                                                                            "",
                                                                                                            title,
                                                                                                            description,
                                                                                                            bard_class,
                                                                                                            3,
                                                                                                            1,
                                                                                                            spelllist
                                                                                                            );
        }


        static void createNatureCollegeBonusProficienies()
        {
            var tools_proficiency = Helpers.ProficiencyBuilder.CreateToolsProficiency("BardNatureSubclassToolsProficiency",
                                                                                      "",
                                                                                      Common.common_no_title,
                                                                                      Helpers.Tools.HerbalismKit
                                                                                      );

            var skills = Helpers.PoolBuilder.createSkillProficiency("BardNatureSubclassSkillsProficiency",
                                                                    "",
                                                                    Common.common_no_title,
                                                                    Common.common_no_title,
                                                                    2,
                                                                    Helpers.Skills.Nature, Helpers.Skills.Medicine, Helpers.Skills.AnimalHandling, Helpers.Skills.Survival);

            nature_college_bonus_proficiencies = Helpers.FeatureSetBuilder.createFeatureSet("BardNatureSubclassBonusProficiencies",
                                                                                            "",
                                                                                            "Feature/&BardNatureSubclassBonusProficiencieslTitle",
                                                                                            "Feature/&BardNatureSubclassBonusProficiencieslDescription",
                                                                                            false,
                                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                            false,
                                                                                            skills,
                                                                                            tools_proficiency
                                                                                            );
        }


        static CharacterSubclassDefinition createVirtueCollege()
        {
            createVirtueCollegeProficiencies();
            createVirtueCollegeExtraAttack();
            createMusicOfTheSpheres();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BardSubclassCollegeOfVirtueDescription",
                    "Subclass/&BardSubclassCollegeOfVirtueTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.OathOfTirmar.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BardSubclassCollegeOfVirtue", "b931f6f0-2d62-450c-a8d1-5379473d8538")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(virtue_college_bonus_proficiencies, 3)
                    .AddFeatureAtLevel(music_of_spheres[RuleDefinitions.DieType.D6], 3)
                    .AddFeatureAtLevel(music_of_spheres[RuleDefinitions.DieType.D8], 5)
                    .AddFeatureAtLevel(virtue_college_extra_attack, 6)
                    .AddFeatureAtLevel(music_of_spheres[RuleDefinitions.DieType.D10], 10)
                    .AddFeatureAtLevel(music_of_spheres[RuleDefinitions.DieType.D12], 15)
                    .AddToDB();

            return definition;
        }


        static void createVirtueCollegeProficiencies()
        {
            var armor_proficiency = Helpers.ProficiencyBuilder.CreateArmorProficiency("BardVirtueSubclassArmorProficiency",
                                                              "",
                                                              Common.common_no_title,
                                                              Common.common_no_title,
                                                              Helpers.ArmorProficiencies.MediumArmor,
                                                              Helpers.ArmorProficiencies.Shield
                                                              );

            var wis_proficiency = Helpers.ProficiencyBuilder.CreateSavingthrowProficiency("BardVirtueSubclassWisSavingthrowsProficiency",
                                                                                          "",
                                                                                          Helpers.Stats.Wisdom
                                                                                          );

            virtue_college_bonus_proficiencies = Helpers.FeatureSetBuilder.createFeatureSet("BardVirtueSubclassBonusProficiency",
                                                                                            "",
                                                                                            "Feature/&BardVirtueSublclassBonusProficiencieslTitle",
                                                                                            "Feature/&BardVirtueSublclassBonusProficiencieslDescription",
                                                                                            false,
                                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                            false,
                                                                                            armor_proficiency,
                                                                                            wis_proficiency
                                                                                            );

        }


        static void createVirtueCollegeExtraAttack()
        {
            virtue_college_extra_attack = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("BardVirtueSubclassExtraAttack",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           "",
                                                                                                                           null,
                                                                                                                           DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack
                                                                                                                           );
        }

        static void createMusicOfTheSpheres()
        {
            string music_of_the_spheres_condition_title_string = "Rules/&BardVirtueSubclassMusicOfTheSpheresConditionTitle";
            string music_of_the_spheres_title_string = "Feature/&BardVirtueSubclassMusicOfTheSpheresTitle";
            string music_of_the_spheres_description_string = "Feature/&BardVirtueSubclassMusicOfTheSpheresDescription";

            string use_music_of_the_spheres_react_description = "Reaction/&UseBardVirtueSubclassMusicOfTheSpheresPowerReactDescription";
            string use_music_of_the_spheres_react_title = "Reaction/&CommonUsePowerReactTitle";
            string use_music_of_the_spheres_description = use_music_of_the_spheres_react_description;
            string use_music_of_the_spheres_title = music_of_the_spheres_title_string;

            NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt previous_power = null;
            var dice = inspiration_dice;

            for (int i = 0; i < dice.Length; i++)
            {
                var attack_bonus = Helpers.AttackBonusBuilder.createAttackBonus("BardVirtueSubclassMusicOfTheSpheresAttackBonus" + dice[i].ToString(),
                                                                                    "",
                                                                                    "",
                                                                                    "",
                                                                                    null,
                                                                                    1,
                                                                                    dice[i],
                                                                                    substract: false
                                                                                    );

                var dmg_bonus_fiend = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("BardVirtueSubclassMusicOfTheSpheresFiendDamage" + dice[i].ToString(),
                                                                                                                      "",
                                                                                                                      "",
                                                                                                                      "",
                                                                                                                      null,
                                                                                                                      DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor,
                                                                                                                      a =>
                                                                                                                      {
                                                                                                                          a.notificationTag = "BardVirtueSubclassMusicOfTheSpheres";
                                                                                                                          a.triggerCondition = RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily;
                                                                                                                          a.requiredCharacterFamily = DatabaseHelper.CharacterFamilyDefinitions.Fiend;
                                                                                                                          a.damageDieType = dice[i];
                                                                                                                      }
                                                                                                                      );

                var dmg_bonus_undead = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("BardVirtueSubclassMusicOfTheSpheresUndeadDamage" + dice[i].ToString(),
                                                                                                      "",
                                                                                                      "",
                                                                                                      "",
                                                                                                      null,
                                                                                                      DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDivineFavor,
                                                                                                      a =>
                                                                                                      {
                                                                                                          a.notificationTag = "BardVirtueSubclassMusicOfTheSpheres";
                                                                                                          a.triggerCondition = RuleDefinitions.AdditionalDamageTriggerCondition.SpecificCharacterFamily;
                                                                                                          a.requiredCharacterFamily = DatabaseHelper.CharacterFamilyDefinitions.Undead;
                                                                                                          a.damageDieType = dice[i];
                                                                                                      }
                                                                                                      );


                var condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BardVirtueSubclassMusicOfTheSpheresCondition" + dice[i].ToString(),
                                                                                            "",
                                                                                            Helpers.StringProcessing.addStringCopy(music_of_the_spheres_condition_title_string,
                                                                                                                                   "Rules/&BardVirtueSubclassMusicOfTheSpheresCondition" + dice[i].ToString()
                                                                                                                                   ),
                                                                                            music_of_the_spheres_description_string,
                                                                                            null,
                                                                                            DatabaseHelper.ConditionDefinitions.ConditionDivineFavor,
                                                                                            new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks },
                                                                                            attack_bonus, dmg_bonus_fiend, dmg_bonus_undead
                                                                                            );
                NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(condition);

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.DivineFavor.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Distance);
                effect.SetTargetType(RuleDefinitions.TargetType.Individuals);
                effect.SetRangeParameter(12);
                effect.SetTargetParameter(1);
                effect.SetTargetParameter2(2);
                effect.DurationParameter = 1;
                effect.SetTargetSide(RuleDefinitions.Side.Ally);
                effect.DurationType = RuleDefinitions.DurationType.Round;
                effect.EffectForms.Clear();

                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = condition;
                effect.EffectForms.Add(effect_form);
                effect.SetTargetFilteringTag((RuleDefinitions.TargetFilteringTag)ExtendedEnums.ExtraTargetFilteringTag.NonCaster);

                var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt>
                                                      .createPower("BardVirtueSubclassMusicOfTheSpheres" + dice[i].ToString(),
                                                                    "",
                                                                    Helpers.StringProcessing.appendToString(music_of_the_spheres_title_string,
                                                                                                             music_of_the_spheres_title_string + dice[i].ToString(),
                                                                                                             $" ({dice[i].ToString().ToString().ToLower()})"),
                                                                    music_of_the_spheres_description_string,
                                                                    DatabaseHelper.SpellDefinitions.DivineFavor.GuiPresentation.SpriteReference,
                                                                    effect,
                                                                    RuleDefinitions.ActivationTime.Reaction,
                                                                    0,
                                                                    RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                    previous_power == null ? RuleDefinitions.RechargeRate.LongRest : RuleDefinitions.RechargeRate.ShortRest,
                                                                    Helpers.Stats.Charisma,
                                                                    Helpers.Stats.Charisma
                                                                    );
                power.linkedPower = inspiration_powers[dice[i]];
                power.worksOnMelee = true;
                power.worksOnRanged = true;
                power.onlyOnFailure = true;
                power.SetShortTitleOverride(music_of_the_spheres_title_string);
                if (previous_power != null)
                {
                    power.SetOverriddenPower(previous_power);
                }
                previous_power = power;
                Helpers.StringProcessing.addPowerReactStrings(power, use_music_of_the_spheres_title, use_music_of_the_spheres_description,
                                                                                    use_music_of_the_spheres_react_title, use_music_of_the_spheres_react_description);
                music_of_spheres.Add(dice[i], power);
            }
        }


        static CharacterSubclassDefinition createLoreCollege()
        {
            createCuttingWords();
            createLoreCollegeBonusProficiencies();
            createLoreCollegeMagicalSecrets();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&BardSubclassCollegeOfLoreDescription",
                    "Subclass/&BardSubclassCollegeOfLoreTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("BardSubclassCollegeOfLore", "3bcb4d6b-0a7a-470f-b172-002092b8f464")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(lore_college_bonus_proficiencies, 3)
                    .AddFeatureAtLevel(cutting_words[RuleDefinitions.DieType.D6], 3)
                    .AddFeatureAtLevel(cutting_words[RuleDefinitions.DieType.D8], 5)
                    .AddFeatureAtLevel(additional_magical_secrets, 6)
                    .AddFeatureAtLevel(cutting_words[RuleDefinitions.DieType.D10], 10)
                    .AddFeatureAtLevel(cutting_words[RuleDefinitions.DieType.D12], 15)
                    .AddToDB();

            return definition;
        }


        static void createLoreCollegeBonusProficiencies()
        {
            lore_college_bonus_proficiencies = Helpers.PoolBuilder.createSkillProficiency("BardLoreSubclassSkillProficiency",
                                                        "",
                                                        "Feature/&BardLoreSublclassExtraSkillPointPoolTitle",
                                                        "Feature/&BardLoreSublclassExtraSkillPointPoolDescription",
                                                        3,
                                                        Helpers.Skills.getAllSkills());
        }


        static void createLoreCollegeMagicalSecrets()
        {
            additional_magical_secrets = Helpers.ExtraSpellSelectionBuilder.createExtraSpellSelection("BardLoreSubclassAdditionalMagicalSecrets",
                                                                                                        "",
                                                                                                        "Feature/&BardLoreSubclassAdditionalMagicalSecretsTitle",
                                                                                                        "Feature/&BardLoreSubclassAdditionalMagicalSecretsDescription",
                                                                                                        bard_class,
                                                                                                        6,
                                                                                                        2,
                                                                                                        magical_secrets_spelllist
                                                                                                        );
        }


        static void createCuttingWords()
        {
            //TODO: add enemies immune to charmed be unaffected
            //TODO: check distance to the attacker
            string cutting_words_title_string = "Feature/&BardClassCuttingWordsTitle";
            string cutting_words_description_string = "Feature/&BardClassCuttingWordsDescription";
            string cutting_words_attack_roll_title_string = "Feature/&BardClassCuttingWordsPowerAttackRollTitle";
            string cutting_words_damage_roll_title_string = "Feature/&BardClassCuttingWordsPowerDamageRollTitle";

            string use_cutting_words_attack_roll_react_description = "Reaction/&UseBardClassCuttingWordsAttackRollsPenaltyPowerReactDescription";
            string use_cutting_words_attack_roll_react_title = "Reaction/&CommonUsePowerReactTitle";
            string use_cutting_words_attack_roll_description = use_cutting_words_attack_roll_react_description;
            string use_cutting_words_attack_roll_title = cutting_words_attack_roll_title_string;

            string use_cutting_words_damage_roll_react_description = "Reaction/&UseBardClassCuttingWordsDamageRollsPenaltyPowerReactDescription";
            string use_cutting_words_damage_roll_react_title = "Reaction/&CommonUsePowerReactTitle";
            string use_cutting_words_damage_roll_description = use_cutting_words_damage_roll_react_description;
            string use_cutting_words_damage_roll_title = cutting_words_damage_roll_title_string;

            NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt previous_attack_roll_penalty_power = null;
            NewFeatureDefinitions.FeatureDefinitionReactionPowerOnDamage previous_damage_roll_penalty_power = null;
            var dice = inspiration_dice;

            for (int i = 0; i < dice.Length; i++)
            {
                var penalty_attack = Helpers.AttackBonusBuilder.createAttackBonus("BardClassCuttingWordsAttackPenalty" + dice[i].ToString(),
                                                                                    "",
                                                                                    "",
                                                                                    "",
                                                                                    null,
                                                                                    1,
                                                                                    dice[i],
                                                                                    substract: true
                                                                                    );

                var attack_penalty_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BardClassCuttingWordsAttackPenaltyCondition" + dice[i].ToString(),
                                                                                                          "",
                                                                                                          Helpers.StringProcessing.concatenateStrings(Common.common_condition_prefix,
                                                                                                                                                      cutting_words_attack_roll_title_string,
                                                                                                                                                      "Rules/&BardClassCuttingWordsAttackPenaltyCondition" + dice[i].ToString()
                                                                                                                                                      ),
                                                                                                          cutting_words_description_string,
                                                                                                          null,
                                                                                                          DatabaseHelper.ConditionDefinitions.ConditionDazzled,
                                                                                                          new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.Attacks },
                                                                                                          penalty_attack
                                                                                                          );
                NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(attack_penalty_condition);
                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.Dazzle.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Distance);
                effect.SetRangeParameter(12);
                effect.DurationParameter = 1;
                effect.SetTargetSide(RuleDefinitions.Side.Enemy);
                effect.DurationType = RuleDefinitions.DurationType.Round;
                effect.EffectForms.Clear();

                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = attack_penalty_condition;
                effect.EffectForms.Add(effect_form);

                var attack_penalty_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt>
                                                    .createPower("BardClassCuttingWordsAttackRollsPenaltyPower" + dice[i].ToString(),
                                                                    "",
                                                                    cutting_words_attack_roll_title_string,
                                                                    cutting_words_description_string,
                                                                    DatabaseHelper.SpellDefinitions.Dazzle.GuiPresentation.SpriteReference,
                                                                    effect,
                                                                    RuleDefinitions.ActivationTime.Reaction,
                                                                    0,
                                                                    RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                    previous_attack_roll_penalty_power == null ? RuleDefinitions.RechargeRate.LongRest : RuleDefinitions.RechargeRate.ShortRest,
                                                                    Helpers.Stats.Charisma,
                                                                    Helpers.Stats.Charisma
                                                                    );
                attack_penalty_power.onlyOnSuccess = true;
                attack_penalty_power.linkedPower = inspiration_powers[dice[i]];
                attack_penalty_power.worksOnMelee = true;
                attack_penalty_power.worksOnRanged = true;
                attack_penalty_power.checkImmunityToCondtions = new List<ConditionDefinition> { DatabaseHelper.ConditionDefinitions.ConditionCharmed };
                attack_penalty_power.SetShortTitleOverride(cutting_words_attack_roll_title_string);
                if (previous_attack_roll_penalty_power != null)
                {
                    attack_penalty_power.SetOverriddenPower(previous_attack_roll_penalty_power);
                }
                previous_attack_roll_penalty_power = attack_penalty_power;
                Helpers.StringProcessing.addPowerReactStrings(attack_penalty_power, use_cutting_words_attack_roll_title, use_cutting_words_attack_roll_description,
                                                                                    use_cutting_words_attack_roll_react_title, use_cutting_words_attack_roll_react_description);


                var penalty_damage = Helpers.FeatureBuilder<NewFeatureDefinitions.ModifyDiceRollValue>.createFeature("BardClassCuttingWordsDamagePenalty" + dice[i].ToString(),
                                                                                                                     "",
                                                                                                                     "",
                                                                                                                     "",
                                                                                                                     null,
                                                                                                                     m =>
                                                                                                                     {
                                                                                                                         m.diceType = dice[i];
                                                                                                                         m.numDice = -1;
                                                                                                                         m.contexts = new List<RuleDefinitions.RollContext>() { RuleDefinitions.RollContext.AttackDamageValueRoll, RuleDefinitions.RollContext.MagicDamageValueRoll };
                                                                                                                     }
                                                                                                                     );

                var damage_penalty_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("BardClassCuttingWordsDamagePenaltyCondition" + dice[i].ToString(),
                                                                                                          "",
                                                                                                          Helpers.StringProcessing.concatenateStrings(Common.common_condition_prefix,
                                                                                                                                                      cutting_words_damage_roll_title_string,
                                                                                                                                                      "Rules/&BardClassCuttingWordsDamagePenaltyCondition" + dice[i].ToString()
                                                                                                                                                      ),
                                                                                                          cutting_words_description_string,
                                                                                                          null,
                                                                                                          DatabaseHelper.ConditionDefinitions.ConditionDazzled,
                                                                                                          new RuleDefinitions.ConditionInterruption[] { (RuleDefinitions.ConditionInterruption)ExtendedEnums.ExtraConditionInterruption.RollsForDamage },
                                                                                                          penalty_damage
                                                                                                          );
                NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(damage_penalty_condition);

                effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.Dazzle.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Distance);
                effect.SetRangeParameter(12);
                effect.DurationParameter = 1;
                effect.SetTargetSide(RuleDefinitions.Side.Enemy);
                effect.DurationType = RuleDefinitions.DurationType.Round;
                effect.EffectForms.Clear();

                effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = damage_penalty_condition;
                effect.EffectForms.Add(effect_form);

                var damage_penalty_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.FeatureDefinitionReactionPowerOnDamage>
                                                    .createPower("BardClassCuttingWordsDamageRollsPenaltyPower" + dice[i].ToString(),
                                                                    "",
                                                                    cutting_words_damage_roll_title_string,
                                                                    cutting_words_description_string,
                                                                    DatabaseHelper.SpellDefinitions.Dazzle.GuiPresentation.SpriteReference,
                                                                    effect,
                                                                    RuleDefinitions.ActivationTime.Reaction,
                                                                    0,
                                                                    RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                    previous_damage_roll_penalty_power == null ? RuleDefinitions.RechargeRate.LongRest : RuleDefinitions.RechargeRate.ShortRest,
                                                                    Helpers.Stats.Charisma,
                                                                    Helpers.Stats.Charisma
                                                                    );
                Helpers.StringProcessing.addPowerReactStrings(damage_penalty_power, use_cutting_words_damage_roll_title, use_cutting_words_damage_roll_description,
                                                                    use_cutting_words_damage_roll_react_title, use_cutting_words_damage_roll_react_description);
                damage_penalty_power.linkedPower = inspiration_powers[dice[i]];

                damage_penalty_power.worksOnMelee = true;
                damage_penalty_power.worksOnRanged = true;
                damage_penalty_power.worksOnMagic = true;
                damage_penalty_power.checkImmunityToCondtions = new List<ConditionDefinition> { DatabaseHelper.ConditionDefinitions.ConditionCharmed };
                damage_penalty_power.SetShortTitleOverride(cutting_words_damage_roll_title_string);
                if (previous_damage_roll_penalty_power != null)
                {
                    damage_penalty_power.SetOverriddenPower(previous_damage_roll_penalty_power);
                }
                previous_damage_roll_penalty_power = damage_penalty_power;

                var feature_set = Helpers.FeatureSetBuilder.createFeatureSet("BardClassCuttingWordsFeature" + dice[i].ToString(),
                                                                             "",
                                                                             Helpers.StringProcessing.appendToString(cutting_words_title_string,
                                                                                                                     cutting_words_title_string + dice[i].ToString(),
                                                                                                                     $" ({dice[i].ToString().ToString().ToLower()})"),
                                                                             cutting_words_description_string,
                                                                             false,
                                                                             FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                             false,
                                                                             attack_penalty_power,
                                                                             damage_penalty_power
                                                                             );


                cutting_words.Add(dice[i], feature_set);
            }
        }


        static void createCountercharm()
        {
            string countercharm_title_string = "Feature/&BardClassCountercharmPowerTitle";
            string countercharm_description_string = "Feature/&BardClassCountercharmPowerDescription";
            string countercharm_effect_description_string = "Feature/&BardClassCountercharmPowerEffectDescription";

            var affinity_frightened = Helpers.ConditionAffinityBuilder.createConditionAffinity("BardClassCountercharmFrightenedAffinity",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            null,
                                                                                            Helpers.Conditions.Frightened,
                                                                                            RuleDefinitions.ConditionAffinityType.None,
                                                                                            RuleDefinitions.AdvantageType.Advantage,
                                                                                            RuleDefinitions.AdvantageType.None
                                                                                            );
            var affinity_charmed = Helpers.ConditionAffinityBuilder.createConditionAffinity("BardClassCountercharmCharmedAffinity",
                                                                                            "",
                                                                                            "",
                                                                                            "",
                                                                                            null,
                                                                                            Helpers.Conditions.Charmed,
                                                                                            RuleDefinitions.ConditionAffinityType.None,
                                                                                            RuleDefinitions.AdvantageType.Advantage,
                                                                                            RuleDefinitions.AdvantageType.None
                                                                                            );

            var effect_condition = Helpers.ConditionBuilder.createCondition("BardClassCountercharmEffectCondition",
                                                                            "",
                                                                            Helpers.StringProcessing.concatenateStrings(Common.common_condition_prefix,
                                                                                                                        countercharm_title_string,
                                                                                                                        "Rules/&BardClassCountercharmEffectCondition"
                                                                                                                        ),
                                                                            countercharm_effect_description_string,
                                                                            null,
                                                                            DatabaseHelper.ConditionDefinitions.ConditionResisting,
                                                                            affinity_charmed,
                                                                            affinity_frightened
                                                                            );

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Resistance.EffectDescription);
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetTargetType(RuleDefinitions.TargetType.Sphere);
            effect.SetRangeParameter(0);
            effect.SetTargetParameter(6);
            effect.SetTargetProximityDistance(6);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = effect_condition;
            effect.EffectForms.Add(effect_form);
            effect.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnActivation | RuleDefinitions.RecurrentEffect.OnEnter | RuleDefinitions.RecurrentEffect.OnTurnStart);
            countercharm = Helpers.PowerBuilder.createPower("BardCountercharmPower",
                                                            "",
                                                            countercharm_title_string,
                                                            countercharm_description_string,
                                                            DatabaseHelper.SpellDefinitions.Resistance.GuiPresentation.SpriteReference,
                                                            DatabaseHelper.FeatureDefinitionPowers.PowerPaladinAuraOfProtection,
                                                            effect,
                                                            RuleDefinitions.ActivationTime.Action,
                                                            1,
                                                            RuleDefinitions.UsesDetermination.Fixed,
                                                            RuleDefinitions.RechargeRate.AtWill,
                                                            Helpers.Stats.Charisma,
                                                            Helpers.Stats.Charisma
                                                            );
        }


        static void createMagicalSecrets()
        {
            var spelllist = Helpers.SpelllistBuilder.createCombinedSpellList("BardClassMagicalSecretSpelllist", "", "",
                                                                             bard_spelllist,
                                                                             DatabaseHelper.SpellListDefinitions.SpellListWizard,
                                                                             DatabaseHelper.SpellListDefinitions.SpellListCleric,
                                                                             DatabaseHelper.SpellListDefinitions.SpellListPaladin,
                                                                             DatabaseHelper.SpellListDefinitions.SpellListRanger,
                                                                             DatabaseHelper.SpellListDefinitions.SpellListDruid
                                                                             );
            spelllist.SpellsByLevel[0].Spells = bard_spelllist.SpellsByLevel[0].Spells; //do not affect cantrips for the time being
            Helpers.Misc.addSpellToSpelllist(bard_spelllist, Spells.hellish_rebuke);
            Helpers.Misc.addSpellToSpelllist(bard_spelllist, Spells.vulnerability_hex);

            magical_secrets = Helpers.ExtraSpellSelectionBuilder.createExtraSpellSelection("BardClassMagicalSecrets",
                                                                                            "",
                                                                                            "Feature/&BardClassMagicalSecretsTitle",
                                                                                            "Feature/&BardClassMagicalSecretsDescription",
                                                                                            bard_class,
                                                                                            10,
                                                                                            2,
                                                                                            spelllist
                                                                                            );

            magical_secrets14 = Helpers.ExtraSpellSelectionBuilder.createExtraSpellSelection("BardClassMagicalSecrets14",
                                                                                             "",
                                                                                             "Feature/&BardClassMagicalSecretsTitle",
                                                                                             "Feature/&BardClassMagicalSecretsDescription",
                                                                                             bard_class,
                                                                                             14,
                                                                                             2,
                                                                                             spelllist
                                                                                             );

            magical_secrets18 = Helpers.ExtraSpellSelectionBuilder.createExtraSpellSelection("BardClassMagicalSecrets18",
                                                                                             "",
                                                                                             "Feature/&BardClassMagicalSecretsTitle",
                                                                                             "Feature/&BardClassMagicalSecretsDescription",
                                                                                             bard_class,
                                                                                             18,
                                                                                             2,
                                                                                             spelllist
                                                                                             );


            magical_secrets_spelllist = spelllist;
        }


        static void createSongOfRest()
        {
            string song_of_rest_title_string = "Feature/&BardClassSongOfRestTitle";
            string song_of_rest_description_string = "Feature/&BardClassSongOfRestDescription";

            var dice = inspiration_dice;

            for (int i = 0; i < dice.Length; i++)
            {
                var feature = Helpers.FeatureBuilder<NewFeatureDefinitions.FeatureDefinitionExtraHealingDieOnShortRest>.createFeature("BardClassSongOfRestFeature" + dice[i].ToString(),
                                                                                                                 "",
                                                                                                                 Helpers.StringProcessing.appendToString(song_of_rest_title_string,
                                                                                                                                                         song_of_rest_title_string + dice[i].ToString(),
                                                                                                                                                         $" ({dice[i].ToString().ToString().ToLower()})"),
                                                                                                                 song_of_rest_description_string,
                                                                                                                 null);
                feature.ApplyToParty = true;
                feature.tag = "SongOfRest";
                feature.DieType = dice[i];
                song_of_rest[dice[i]] = feature;
            }
        }


        static void createInspiration()
        {
            string inspired_condition_string = "Rules/&BardClassInspiredCondition";
            string inspiration_title_string = "Feature/&BardClassInspirationPowerTitle";
            string inspiration_description_string = "Feature/&BardClassInspirationPowerDescription";

            string inspiration_use_title_string = "Feature/&BardClassInspirationUsePowerTitle";
            string inspiration_use_description_string = "Feature/&BardClassInspirationUsePowerDescription";

            string use_inspiration_react_description = "Reaction/&UseBardInspirationUsePowerReactDescription";
            string use_inspiration_saves_description = "Reaction/&UseBardInspirationSavesUsePowerDescription";
            string use_inspiration_react_title = "Reaction/&CommonUsePowerSpendTitle";
            string use_inspiration_description = "Reaction/&UseBardInspirationUsePowerDescription";
            string use_inspiration_title = inspiration_use_title_string;

            FeatureDefinitionPower previous_power = null;
            FeatureDefinitionPower previous_use_power = null;
            FeatureDefinitionPower previous_use_power_saves = null;
            var dice = inspiration_dice;
            for (int i = 0; i < dice.Length; i++)
            {
                var eff = new EffectDescription();
                eff.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.effectDescription);
                eff.effectForms.Clear();
                eff.targetSide = RuleDefinitions.Side.Ally;
                var use_power_saves = Helpers.GenericPowerBuilder<NewFeatureDefinitions.FeatureDefinitionAddRandomBonusOnFailedSavePower>
                                                                    .createPower("BardInspirationSavesUsePower" + dice[i].ToString(),
                                                                                 "",
                                                                                 inspiration_use_title_string,
                                                                                 inspiration_use_description_string,
                                                                                 DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.guiPresentation.SpriteReference,
                                                                                 eff,
                                                                                 RuleDefinitions.ActivationTime.NoCost,
                                                                                 1,
                                                                                 RuleDefinitions.UsesDetermination.Fixed,
                                                                                 RuleDefinitions.RechargeRate.ShortRest
                                                                                 );
                use_power_saves.diceNumber = 1;
                use_power_saves.dieType = dice[i];
                use_power_saves.SetShortTitleOverride(inspiration_use_title_string);

                Helpers.StringProcessing.addStringCopy(inspiration_use_title_string,
                                                      $"Reaction/&ConsumePowerUse{use_power_saves.name}Title");
                Helpers.StringProcessing.addStringCopy(use_inspiration_react_title,
                                                       $"Reaction/&ConsumePowerUse{use_power_saves.name}ReactTitle");
                Helpers.StringProcessing.addStringCopy(use_inspiration_react_description,
                                                       $"Reaction/&ConsumePowerUse{use_power_saves.name}ReactDescription");
                Helpers.StringProcessing.addStringCopy(use_inspiration_saves_description,
                                                       $"Reaction/&ConsumePowerUse{use_power_saves.name}Description");

                var inspiration_attack = Helpers.AttackBonusBuilder.createAttackBonus("BardClassInspirationAttackBonus" + dice[i].ToString(),
                                                                                                         "",
                                                                                                         "",
                                                                                                         "",
                                                                                                         null,
                                                                                                         1,
                                                                                                         dice[i]
                                                                                                         );

                var condition_power = Helpers.ConditionBuilder.createConditionWithInterruptions("BardClassInspirationUseConditionPower" + dice[i].ToString(),
                                                                                                "",
                                                                                                inspired_condition_string,
                                                                                                inspiration_description_string,
                                                                                                null,
                                                                                                DatabaseHelper.ConditionDefinitions.ConditionShieldedByFaith,
                                                                                                new RuleDefinitions.ConditionInterruption[] {RuleDefinitions.ConditionInterruption.Attacks},
                                                                                                inspiration_attack
                                                                                                );
                NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(condition_power);
                condition_power.SetSilentWhenAdded(true);
                condition_power.SetSilentWhenRemoved(true);

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.DivineFavor.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Self);
                effect.SetTargetType(RuleDefinitions.TargetType.Self);
                effect.SetRangeParameter(1);
                effect.SetTargetParameter(1);
                effect.SetTargetParameter2(2);
                effect.DurationParameter = 1;
                effect.SetTargetSide(RuleDefinitions.Side.Ally);
                effect.DurationType = RuleDefinitions.DurationType.Round;
                effect.EffectForms.Clear();

                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = condition_power;
                effect.EffectForms.Add(effect_form);

                var use_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.FeatureDefinitionReactionPowerOnAttackAttempt>
                                                      .createPower("BardInspirationUsePower" + dice[i].ToString(),
                                                                    "",
                                                                    Helpers.StringProcessing.appendToString(inspiration_use_title_string,
                                                                                                             inspiration_use_title_string + dice[i].ToString(),
                                                                                                             $" ({dice[i].ToString().ToString().ToLower()})"),
                                                                    inspiration_use_description_string,
                                                                    DatabaseHelper.SpellDefinitions.DivineFavor.GuiPresentation.SpriteReference,
                                                                    effect,
                                                                    RuleDefinitions.ActivationTime.NoCost,
                                                                    1,
                                                                    RuleDefinitions.UsesDetermination.Fixed,
                                                                    previous_use_power == null ? RuleDefinitions.RechargeRate.LongRest : RuleDefinitions.RechargeRate.ShortRest,
                                                                    Helpers.Stats.Charisma,
                                                                    Helpers.Stats.Charisma
                                                                    );
                use_power.worksOnMelee = true;
                use_power.worksOnRanged = true;
                use_power.onlyOnFailure = true;
                use_power.worksOnMagic = true;
                use_power.SetShortTitleOverride(inspiration_use_title_string);
                var grant_power_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.GrantPowerOnConditionApplication>.createFeature("BardInspirationGrantUsePower" + dice[i].ToString(),
                                                                                                                                       "",
                                                                                                                                       Common.common_no_title,
                                                                                                                                       Common.common_no_title,
                                                                                                                                       Common.common_no_icon,
                                                                                                                                       a =>
                                                                                                                                       {
                                                                                                                                           a.power = use_power;
                                                                                                                                           a.removAfterUse = true;
                                                                                                                                       }
                                                                                                                                       );
                var grant_power_feature2 = Helpers.FeatureBuilder<NewFeatureDefinitions.GrantPowerOnConditionApplication>.createFeature("BardInspirationGrantUsePowerSaves" + dice[i].ToString(),
                                                                                                                       "",
                                                                                                                       Common.common_no_title,
                                                                                                                       Common.common_no_title,
                                                                                                                       Common.common_no_icon,
                                                                                                                       a =>
                                                                                                                       {
                                                                                                                           a.power = use_power_saves;
                                                                                                                           a.removAfterUse = true;
                                                                                                                       }
                                                                                                                       );

                var inspiration_condition = Helpers.ConditionBuilder.createCondition("BardClassInspirationCondition" + dice[i].ToString(),
                                                                                        "",
                                                                                        inspired_condition_string,
                                                                                        "Rules/&BardClassInspiredConditionDescription",
                                                                                        null,
                                                                                        DatabaseHelper.ConditionDefinitions.ConditionGuided,
                                                                                        grant_power_feature,
                                                                                        grant_power_feature2
                                                                                        );
                grant_power_feature.condition = inspiration_condition;
                grant_power_feature2.condition = inspiration_condition;

                effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.Guidance.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Distance);
                effect.SetRangeParameter(12);
                effect.DurationParameter = 10;
                effect.DurationType = RuleDefinitions.DurationType.Minute;
                effect.EffectForms.Clear();
                effect.SetTargetFilteringTag((RuleDefinitions.TargetFilteringTag)ExtendedEnums.ExtraTargetFilteringTag.NonCaster);

                effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = inspiration_condition;
                effect.EffectForms.Add(effect_form);

                var inspiration_power = Helpers.PowerBuilder.createPower("BardInspirationPower" + dice[i].ToString(),
                                                                         "",
                                                                         Helpers.StringProcessing.appendToString(inspiration_title_string,
                                                                                                                 inspiration_title_string + dice[i].ToString(),
                                                                                                                 $" ({dice[i].ToString().ToString().ToLower()})"),
                                                                         inspiration_description_string,
                                                                         DatabaseHelper.SpellDefinitions.Guidance.GuiPresentation.SpriteReference,
                                                                         DatabaseHelper.FeatureDefinitionPowers.PowerPaladinLayOnHands,
                                                                         effect,
                                                                         RuleDefinitions.ActivationTime.BonusAction,
                                                                         0,
                                                                         RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                         previous_power == null ? RuleDefinitions.RechargeRate.LongRest : RuleDefinitions.RechargeRate.ShortRest,
                                                                         Helpers.Stats.Charisma,
                                                                         Helpers.Stats.Charisma
                                                                         );
                inspiration_power.SetShortTitleOverride(inspiration_title_string);

                if (previous_power != null)
                {
                    inspiration_power.SetOverriddenPower(previous_power);
                }
                if (previous_use_power != null)
                {
                    use_power.SetOverriddenPower(previous_use_power);
                }
                if (previous_use_power_saves != null)
                {
                    use_power_saves.SetOverriddenPower(previous_use_power_saves);
                }
                previous_power = inspiration_power;
                previous_use_power = use_power;
                previous_use_power_saves = use_power_saves;

                inspiration_powers.Add(dice[i], inspiration_power);

                Helpers.StringProcessing.addPowerReactStrings(use_power, use_inspiration_title, use_inspiration_description,
                                                                    use_inspiration_react_title, use_inspiration_react_description);
            }

            string font_of_inspiration_title_string = "Feature/&BardClassFontOfInspirationFeatureTitle";
            string font_of_inspiration_description_string = "Feature/&BardClassFontOfInspirationFeatureDescription";
            font_of_inspiration = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("BardClassFontOfInspirationFeature",
                                                                                                     "",
                                                                                                     font_of_inspiration_title_string,
                                                                                                     font_of_inspiration_description_string);
        }


        public static void BuildAndAddClassToDB()
        {
            var BardClass = new BardClassBuilder(BardClassName, BardClassNameGuid).AddToDB();
            BardClass.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                                          {
                                              return a.Level - b.Level;
                                          }
                                         );

            BardFeatureDefinitionSubclassChoice.Subclasses.Add(createLoreCollege().Name);
            BardFeatureDefinitionSubclassChoice.Subclasses.Add(createVirtueCollege().Name);
            BardFeatureDefinitionSubclassChoice.Subclasses.Add(createNatureCollege().Name);
        }

        private static FeatureDefinitionSubclassChoice BardFeatureDefinitionSubclassChoice;
    }
}
