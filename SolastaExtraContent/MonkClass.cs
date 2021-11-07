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
    internal class MonkClassBuilder : CharacterClassDefinitionBuilder
    {
        const string MonkClassName = "MonkClass";
        const string MonkClassNameGuid = "016ebfea-7e5e-46e9-87d4-dbc6c6088d20";
        const string MonkClassSubclassesGuid = "b15810d7-739c-4bc0-95c0-5c274c232d3e";

        static public List<string> monk_weapons = new List<string>
        {
            Helpers.WeaponProficiencies.ShortSword,
            Helpers.WeaponProficiencies.Club,
            Helpers.WeaponProficiencies.Dagger,
            Helpers.WeaponProficiencies.Handaxe,
            Helpers.WeaponProficiencies.Javelin,
            Helpers.WeaponProficiencies.Mace,
            Helpers.WeaponProficiencies.QuarterStaff,
            Helpers.WeaponProficiencies.Spear,
            Helpers.WeaponProficiencies.Unarmed
        };
        static public List<string> way_of_iron_weapons = new List<string>
        {
            Helpers.WeaponProficiencies.BattleAxe,
            Helpers.WeaponProficiencies.Rapier,
            Helpers.WeaponProficiencies.MorningStar,
            Helpers.WeaponProficiencies.Warhammer,
            Helpers.WeaponProficiencies.Scimitar,
            Helpers.WeaponProficiencies.LongSword
        };

        static public List<string> way_of_zen_archery_weapons = new List<string>
        {
            Helpers.WeaponProficiencies.Shortbow,
            Helpers.WeaponProficiencies.Longbow,
        };

        static public CharacterClassDefinition monk_class;
        static public NewFeatureDefinitions.ArmorClassStatBonus unarmored_defense;
        static public FeatureDefinitionFeatureSet martial_arts;
        static public FeatureDefinition way_of_iron_allow_using_monk_features_in_armor;
        static public NewFeatureDefinitions.IRestriction no_armor_restriction;
        static public NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction attacked_with_monk_weapon_restriction;
        static public NewFeatureDefinitions.MovementBonusWithRestrictions unarmored_movement;
        static public Dictionary<int, NewFeatureDefinitions.MovementBonusWithRestrictions> unarmored_movement_improvements = new Dictionary<int, NewFeatureDefinitions.MovementBonusWithRestrictions>();
        static List<int> unarmored_movement_improvement_levels = new List<int> { 6, 10, 14, 18 };
        static public NewFeatureDefinitions.IncreaseNumberOfPowerUsesPerClassLevel ki;
        static public ConditionDefinition flurry_of_blows_condition;
        static public NewFeatureDefinitions.PowerWithRestrictions flurry_of_blows;
        static public NewFeatureDefinitions.PowerWithRestrictions patient_defense;
        static public FeatureDefinitionFeatureSet step_of_the_wind;
        static public List<NewFeatureDefinitions.PowerWithRestrictions> step_of_the_wind_powers = new List<NewFeatureDefinitions.PowerWithRestrictions>();
        static public FeatureDefinitionFeatureSet deflect_missiles;
        static public FeatureDefinitionAttributeModifier extra_attack;
        static public FeatureDefinitionPower slowfall;
        static public NewFeatureDefinitions.PowerWithRestrictions stunning_strike;
        static public NewFeatureDefinitions.AddAttackTagForSpecificWeaponType ki_empowered_strikes;
        static public FeatureDefinitionPower stillness_of_mind;
        static public NewFeatureDefinitions.ProvideConditionForTurnDuration unarmored_movement_vertical_surface;
        static public FeatureDefinitionFeatureSet purity_of_body;
        //way of the open hand
        static public FeatureDefinitionFeatureSet open_hand_technique;
        static public NewFeatureDefinitions.PowerWithRestrictions open_hand_technique_knock;
        static public NewFeatureDefinitions.PowerWithRestrictions open_hand_technique_push;
        static public NewFeatureDefinitions.PowerWithRestrictions open_hand_technique_forbid_reaction;
        static public FeatureDefinitionPower wholeness_of_body;
        static public FeatureDefinitionAbilityCheckAffinity tranquility;
        //Way of Pyrokine
        static public FeatureDefinitionFeatureSet blazing_technique;
        static public NewFeatureDefinitions.PowerWithRestrictions blazing_technique_burn;
        static public NewFeatureDefinitions.PowerWithRestrictions blazing_technique_damage;
        static public NewFeatureDefinitions.PowerWithRestrictions blazing_technique_blind;
        static public FeatureDefinitionFeatureSet burning_devotion;
        static public FeatureDefinitionAbilityCheckAffinity leaping_flames;
        //Way of Iron
        static public FeatureDefinitionFeatureSet roiling_storm_of_iron;
        static public FeatureDefinitionPower test_of_skill;
        static public NewFeatureDefinitions.AddAttackTagForSpecificWeaponType shifting_blades;
        static public FeatureDefinition whirlwind_of_iron;

        //Way of Zen Archery
        static public FeatureDefinitionFeatureSet way_of_the_bow; //bows - monk weapons, archery, do not receive disadvantage in close quarters
        static public NewFeatureDefinitions.AddAttackTagForSpecificWeaponType ki_arrows; //bow attacks considered magical, can use stunning fist with bow attacks
        static public FeatureDefinitionActionAffinity flurry_of_arrows; //like a volley feature of ranger hunter


        
        protected MonkClassBuilder(string name, string guid) : base(name, guid)
        {
            var monk_class_sprite = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("MonkClassSprite",
                                                                                                $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/MonkClass.png",
                                                                                                1024, 576);

            var fighter = DatabaseHelper.CharacterClassDefinitions.Fighter;
            monk_class = Definition;
            Definition.GuiPresentation.Title = "Class/&MonkClassTitle";
            Definition.GuiPresentation.Description = "Class/&MonkClassDescription";
            Definition.GuiPresentation.SetSpriteReference(monk_class_sprite);

            Definition.SetClassAnimationId(AnimationDefinitions.ClassAnimationId.Fighter);
            Definition.SetClassPictogramReference(DatabaseHelper.CharacterClassDefinitions.Rogue.ClassPictogramReference);
            Definition.SetDefaultBattleDecisions(fighter.DefaultBattleDecisions);
            Definition.SetHitDice(RuleDefinitions.DieType.D8);
            Definition.SetIngredientGatheringOdds(fighter.IngredientGatheringOdds);
            Definition.SetRequiresDeity(false);

            Definition.AbilityScoresPriority.Clear();
            Definition.AbilityScoresPriority.AddRange(new List<string> {Helpers.Stats.Dexterity,
                                                                        Helpers.Stats.Wisdom,
                                                                        Helpers.Stats.Constitution,
                                                                        Helpers.Stats.Strength,
                                                                        Helpers.Stats.Charisma,
                                                                        Helpers.Stats.Intelligence});

            Definition.FeatAutolearnPreference.AddRange(fighter.FeatAutolearnPreference);
            Definition.PersonalityFlagOccurences.AddRange(fighter.PersonalityFlagOccurences);

            Definition.SkillAutolearnPreference.Clear();
            Definition.SkillAutolearnPreference.AddRange(new List<string> { Helpers.Skills.Acrobatics,
                                                                            Helpers.Skills.Athletics,
                                                                            Helpers.Skills.History,
                                                                            Helpers.Skills.Insight,
                                                                            Helpers.Skills.Religion,
                                                                            Helpers.Skills.Stealth,
                                                                            Helpers.Skills.Perception,
                                                                            Helpers.Skills.Survival });

            Definition.ToolAutolearnPreference.Clear();
            Definition.ToolAutolearnPreference.AddRange(new List<string> { Helpers.Tools.SmithTool, Helpers.Tools.HerbalismKit });


            Definition.EquipmentRows.AddRange(fighter.EquipmentRows);
            Definition.EquipmentRows.Clear();

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Shortsword, EquipmentDefinitions.OptionWeapon, 1),
                                    },
                                new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Shortsword, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                                    }
            );

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    },
                                    new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ExplorerPack, EquipmentDefinitions.OptionStarterPack, 1),
                                    }
            );
                                

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Dart, EquipmentDefinitions.OptionWeapon, 10)
            });

            var saving_throws = Helpers.ProficiencyBuilder.CreateSavingthrowProficiency("MonkSavingthrowProficiency",
                                                                                        "",
                                                                                        Helpers.Stats.Strength, Helpers.Stats.Dexterity);


            var weapon_proficiency = Helpers.ProficiencyBuilder.CreateWeaponProficiency("MonkWeaponProficiency",
                                                                          "",
                                                                          "Feature/&MonkWeaponProficiencyTitle",
                                                                          "",
                                                                          Helpers.WeaponProficiencies.Simple,
                                                                          Helpers.WeaponProficiencies.ShortSword
                                                                          );

            var skills = Helpers.PoolBuilder.createSkillProficiency("MonkSkillProficiency",
                                                                    "",
                                                                    "Feature/&MonkClassSkillPointPoolTitle",
                                                                    "Feature/&SkillGainChoicesPluralDescription",
                                                                    2,
                                                                    Helpers.Skills.Acrobatics,
                                                                    Helpers.Skills.Athletics,
                                                                    Helpers.Skills.History,
                                                                    Helpers.Skills.Insight,
                                                                    Helpers.Skills.Religion,
                                                                    Helpers.Skills.Stealth
                                                                    );
            createUnarmoredDefense();
            createMartialArts();
            createUnarmoredMovement();
            createKi();
            createDeflectMissiles();
            createExtraAttack();
            createSlowfall();
            createStunningStrike();
            createKiEmpoweredStrikes();
            createStillnessOfMind();
            createUnarmoredMovementVerticalSurfaces();
            createPurityOfBody();
            Definition.FeatureUnlocks.Clear();
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(saving_throws, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(weapon_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(skills, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_defense, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(martial_arts, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_movement, 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(ki, 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(flurry_of_blows, 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(patient_defense, 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(step_of_the_wind, 2));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(deflect_missiles, 3));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(slowfall, 4));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(extra_attack, 5));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(stunning_strike, 5));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_movement_improvements[6], 6));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(ki_empowered_strikes, 6));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionSavingThrowAffinitys.SavingThrowAffinityRogueEvasion, 7)); //evasion is the same as for rogue class
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(stillness_of_mind, 7));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_movement_vertical_surface, 9));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_movement_improvements[10], 10));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(purity_of_body, 10));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_movement_improvements[14], 14));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(unarmored_movement_improvements[18], 18));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19));

            var subclassChoicesGuiPresentation = new GuiPresentation();
            subclassChoicesGuiPresentation.Title = "Subclass/&MonkSubclassMonasticTraditionTitle";
            subclassChoicesGuiPresentation.Description = "Subclass/&MonkSubclassMonasticTraditionDescription";
            MonkFeatureDefinitionSubclassChoice = this.BuildSubclassChoice(3, "MonasticTradition", false, "SubclassChoiceMonkSpecialistArchetypes", subclassChoicesGuiPresentation, MonkClassSubclassesGuid);
        }


        static void createUnarmoredMovementVerticalSurfaces()
        {
            string unarmored_movement9_title_string = "Feature/&MonkClassUnarmoredMovement9Title";
            string unarmored_movement9_description_string = "Feature/&MonkClassUnarmoredMovement9Description";

            var condtion = Helpers.CopyFeatureBuilder<ConditionDefinition>.createFeatureCopy("MonkClassUnarmoredMovement9Condition",
                                                                                             "",
                                                                                             unarmored_movement9_title_string,
                                                                                             unarmored_movement9_description_string,
                                                                                             null,
                                                                                             DatabaseHelper.ConditionDefinitions.ConditionSpiderClimb
                                                                                             );
            unarmored_movement_vertical_surface = Helpers.FeatureBuilder<NewFeatureDefinitions.ProvideConditionForTurnDuration>.createFeature("MonkClassUnarmoredMovement9Condition",
                                                                                                                                              "",
                                                                                                                                              unarmored_movement9_title_string,
                                                                                                                                              unarmored_movement9_description_string,
                                                                                                                                              Common.common_no_icon,
                                                                                                                                              a =>
                                                                                                                                              {
                                                                                                                                                  a.condition = condtion;
                                                                                                                                                  a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                                  {
                                                                                                                                                      no_armor_restriction
                                                                                                                                                  };
                                                                                                                                              }
                                                                                                                                              );
        }


        static void createPurityOfBody()
        {
            string purity_of_body_title_string = "Feature/&MonkClassPurityOfBodyTitle";
            string purity_of_body_description_string = "Feature/&MonkClassPurityOfBodyDescription";

            purity_of_body = Helpers.FeatureSetBuilder.createFeatureSet("MonkClassPurityOfBody",
                                                                        "",
                                                                        purity_of_body_title_string,
                                                                        purity_of_body_description_string,
                                                                        false,
                                                                        FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                        false,
                                                                        DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                                                                        DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityDiseaseImmunity,
                                                                        DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity
                                                                        );
        }


        static void createStillnessOfMind()
        {
            string stillness_of_mind_title_string = "Feature/&MonkClassStillnessOfMindTitle";
            string stillness_of_mind_description_string = "Feature/&MonkClassStillnessOfMindDescription";

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.LesserRestoration.EffectDescription);
            effect.DurationParameter = 1;
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
            effect.SetTargetType(RuleDefinitions.TargetType.Self);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.conditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.conditionForm.operation = ConditionForm.ConditionOperation.RemoveDetrimentalRandom;
            effect_form.conditionForm.detrimentalConditions = new List<ConditionDefinition>()
            {
                DatabaseHelper.ConditionDefinitions.ConditionCharmed,
                DatabaseHelper.ConditionDefinitions.ConditionFrightened
            };
            effect.EffectForms.Add(effect_form);

            stillness_of_mind = Helpers.PowerBuilder.createPower("MonkCLassStillnessOfMindPower",
                                                     "",
                                                     stillness_of_mind_title_string,
                                                     stillness_of_mind_description_string,
                                                     DatabaseHelper.FeatureDefinitionPowers.PowerPaladinDivineSense.GuiPresentation.spriteReference,
                                                     DatabaseHelper.FeatureDefinitionPowers.PowerPaladinLayOnHands,
                                                     effect,
                                                     RuleDefinitions.ActivationTime.Action,
                                                     1,
                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                     RuleDefinitions.RechargeRate.AtWill
                                                     );
        }


        static void createKiEmpoweredStrikes()
        {
            string ki_empowered_strikes_title_string = "Feature/&MonkClassKiEmpoweredStrikesTitle";
            string ki_empowered_strikes_description_string = "Feature/&MonkClassKiEmpoweredStrikesDescription";

            ki_empowered_strikes = Helpers.FeatureBuilder<NewFeatureDefinitions.AddAttackTagForSpecificWeaponType>.createFeature("MonkClassKiEmpoweredStrikes",
                                                                                                                                "",
                                                                                                                                ki_empowered_strikes_title_string,
                                                                                                                                ki_empowered_strikes_description_string,
                                                                                                                                Common.common_no_icon,
                                                                                                                                a =>
                                                                                                                                {
                                                                                                                                    a.weaponTypes = new List<string>
                                                                                                                                    {
                                                                                                                                        Helpers.WeaponProficiencies.Unarmed
                                                                                                                                    };
                                                                                                                                    a.tag = "Magical";
                                                                                                                                }
                                                                                                                                );
        }


        static void createStunningStrike()
        {
            string stunning_strike_title_string = "Feature/&MonkClassStunningStrikeTitle";
            string stunning_strike_description_string = "Feature/&MonkClassStunningStrikeDescription";
            string use_stunning_strike_react_description = "Reaction/&SpendMonkClassStunningStrikePowerReactDescription";
            string use_stunning_strike_react_title = "Reaction/&CommonUsePowerReactTitle";

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Constitution;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect.EffectForms.Clear();
            effect.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionStunned;
            effect_form.hasSavingThrow = true;
            effect_form.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);
            effect.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkClassStunningFist",
                                                                     "",
                                                                     stunning_strike_title_string,
                                                                     stunning_strike_description_string,
                                                                     flurry_of_blows.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.ShortRest
                                                                     );
            
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.NoRangedWeaponRestriction() //TODO: make a proper check that the attack is done with melee weapon
                                            };
            power.checkReaction = true;

            power.linkedPower = flurry_of_blows;

            ki.powers.Add(power);
            Helpers.StringProcessing.addPowerReactStrings(power, stunning_strike_title_string, use_stunning_strike_react_description,
                                            use_stunning_strike_react_title, use_stunning_strike_react_description, "SpendPower");
            stunning_strike = power;
        }


        static void createSlowfall()
        {
            string slowfall_title_string = "Feature/&MonkClasSlowfallTitle";
            string slowfall_description_string = "Feature/&MonkClasSlowfallDescription";

            string use_slowfall_react_description = "Reaction/&UseMonkClasSlowfallPowerReactDescription";
            string use_slowfall_react_title = "Reaction/&CommonUsePowerReactTitle";
            string use_slowfall_description = use_slowfall_react_description;
            string use_slowfall_title = slowfall_title_string;

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.FeatherFall.EffectDescription);
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.EffectForms.Clear();
            effect.SetTargetType(RuleDefinitions.TargetType.Self);

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionFeatherFalling;
            effect.EffectForms.Add(effect_form);

            slowfall = Helpers.GenericPowerBuilder<FeatureDefinitionPower>
                                            .createPower("MonkClassSlowfallPower",
                                                         "",
                                                         slowfall_title_string,
                                                         slowfall_description_string,
                                                         DatabaseHelper.SpellDefinitions.FeatherFall.GuiPresentation.SpriteReference,
                                                         effect,
                                                         RuleDefinitions.ActivationTime.Reaction,
                                                         1,
                                                         RuleDefinitions.UsesDetermination.Fixed,
                                                         RuleDefinitions.RechargeRate.AtWill
                                                         );

            Helpers.StringProcessing.addPowerReactStrings(slowfall, use_slowfall_title, use_slowfall_description,
                                                                    use_slowfall_react_title, use_slowfall_react_description);
        }


        static void createDeflectMissiles()
        {
            string deflect_missiles_title_string = "Feature/&MonkClassDeflectMissilesTitle";
            string deflect_missiles_description_string = "Feature/&MonkClassDeflectMissilesDescription";

            var deflect_missiles_affinity = Helpers.FeatureBuilder<NewFeatureDefinitions.DeflectMissileCustom>.createFeature("MonkClassDeflectMissilesActionAffinity",
                                                                                                                          "",
                                                                                                                          deflect_missiles_title_string,
                                                                                                                          deflect_missiles_description_string,
                                                                                                                          null,
                                                                                                                          a =>
                                                                                                                          {
                                                                                                                              a.characterStat = Helpers.Stats.Dexterity;
                                                                                                                              a.characterClass = monk_class;
                                                                                                                          }
                                                                                                                          );

            deflect_missiles = Helpers.FeatureSetBuilder.createFeatureSet("MonkClassDeflectMissiles",
                                                                         "",
                                                                         deflect_missiles_title_string,
                                                                         deflect_missiles_description_string,
                                                                         false,
                                                                         FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                         false,
                                                                         deflect_missiles_affinity
                                                                         );
        }


        static void createKi()
        {
            string ki_title_string = "Feature/&MonkClassKiTitle";
            string ki_description_string = "Feature/&MonkClassKiDescription";

            createFlurryOfBlows();
            createPatientDefense();
            createStepOfTheWind();

            ki = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseNumberOfPowerUsesPerClassLevel>.createFeature("MonkClassKi",
                                                                                                                    "",
                                                                                                                    ki_title_string,
                                                                                                                    ki_description_string,
                                                                                                                    Common.common_no_icon,
                                                                                                                    a =>
                                                                                                                    {
                                                                                                                        a.powers = new List<FeatureDefinitionPower> { flurry_of_blows, patient_defense,
                                                                                                                                                                      step_of_the_wind_powers[0], step_of_the_wind_powers[1] };
                                                                                                                        a.characterClass = monk_class;
                                                                                                                        a.levelIncreaseList = new List<(int, int)>();
                                                                                                                        for (int i = 3; i <= 20; i++)
                                                                                                                        {
                                                                                                                            a.levelIncreaseList.Add((i, 1));
                                                                                                                        }
                                                                                                                    }
                                                                                                                   );
        }


        static void createStepOfTheWind()
        {
            string step_of_the_wind_title_string = "Feature/&MonkClassStepOfTheWindPowerTitle";
            string step_of_the_wind_dash_title_string = "Feature/&MonkClassStepOfTheWindDashPowerTitle";
            string step_of_the_wind_disengage_title_string = "Feature/&MonkClassStepOfTheWindDisengagePowerTitle";
            string step_of_the_wind_description_string = "Feature/&MonkClassStepOfTheWindPowerDescription";

            var step_of_the_wind_condition = Helpers.ConditionBuilder.createCondition("MonkClassStepOfTheWindCondition",
                                                                                     "",
                                                                                     step_of_the_wind_title_string,
                                                                                     step_of_the_wind_title_string,
                                                                                     null,
                                                                                     DatabaseHelper.ConditionDefinitions.ConditionHasted,
                                                                                     DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityJump
                                                                                     );
            step_of_the_wind_condition.SetSubsequentOnRemoval(null);
            step_of_the_wind_powers = new List<NewFeatureDefinitions.PowerWithRestrictions>();
            step_of_the_wind_condition.possessive = true;

            List<(string title, ConditionDefinition condition, AssetReferenceSprite sprite)> power_tuples = new List<(string title, ConditionDefinition condition, AssetReferenceSprite sprite)>
            {
                (step_of_the_wind_dash_title_string, DatabaseHelper.ConditionDefinitions.ConditionDashing, DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandVolcanicAura.GuiPresentation.SpriteReference),
                (step_of_the_wind_disengage_title_string, DatabaseHelper.ConditionDefinitions.ConditionDisengaging, DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionAuraDevotion.GuiPresentation.SpriteReference),

            };

            foreach (var p in power_tuples)
            {
                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.Haste.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.Self);
                effect.SetRangeParameter(1);
                effect.DurationParameter = 1;
                effect.DurationType = RuleDefinitions.DurationType.Turn;
                effect.EffectForms.Clear();
                effect.SetTargetType(RuleDefinitions.TargetType.Self);

                var effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = p.condition;
                effect.EffectForms.Add(effect_form);
                effect_form = new EffectForm();
                effect_form.ConditionForm = new ConditionForm();
                effect_form.FormType = EffectForm.EffectFormType.Condition;
                effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
                effect_form.ConditionForm.ConditionDefinition = step_of_the_wind_condition;
                effect.EffectForms.Add(effect_form);

                var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                            .createPower("MonkClassStepOfTheWindDashPower" + p.condition.name,
                                                                         "",
                                                                         p.title,
                                                                         step_of_the_wind_description_string,
                                                                         p.sprite,
                                                                         effect,
                                                                         RuleDefinitions.ActivationTime.BonusAction,
                                                                         2,
                                                                         RuleDefinitions.UsesDetermination.Fixed,
                                                                         RuleDefinitions.RechargeRate.ShortRest
                                                                         );
                power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                    {
                                        no_armor_restriction
                                    };
                power.linkedPower = flurry_of_blows;
                step_of_the_wind_powers.Add(power);
            }

            step_of_the_wind = Helpers.FeatureSetBuilder.createFeatureSet("MonkClassStepOfTheWind",
                                                          "",
                                                          step_of_the_wind_title_string,
                                                          step_of_the_wind_description_string,
                                                          false,
                                                          FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                          false,
                                                          step_of_the_wind_powers.ToArray()
                                                          );

        }


        static void createPatientDefense()
        {
            string patient_defense_title_string = "Feature/&MonkClassPatientDefensePowerTitle";
            string patient_defense_description_string = "Feature/&MonkClassPatientDefensePowerDescription";

            var patient_defense_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalAction>.createFeatureCopy("MonkClassPatientDefenseFeature",
                                                                                                                           "",
                                                                                                                           Common.common_no_title,
                                                                                                                           Common.common_no_title,
                                                                                                                           null,
                                                                                                                           DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionHasted,
                                                                                                                           a =>
                                                                                                                           {
                                                                                                                               a.restrictedActions = new List<ActionDefinitions.Id>
                                                                                                                               {
                                                                                                                                   ActionDefinitions.Id.Dodge
                                                                                                                               };
                                                                                                                           }
                                                                                                                           );

            var patient_defense_condition = Helpers.ConditionBuilder.createCondition("MonkClassPatientDefenseCondition",
                                                                                     "",
                                                                                     patient_defense_title_string,
                                                                                     patient_defense_title_string,
                                                                                     null,
                                                                                     DatabaseHelper.ConditionDefinitions.ConditionHasted,
                                                                                     patient_defense_feature
                                                                                     );
            patient_defense_condition.SetSubsequentOnRemoval(null);
            patient_defense_condition.possessive = true;
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Haste.EffectDescription);
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.EffectForms.Clear();
            effect.SetTargetType(RuleDefinitions.TargetType.Self);

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionDodging;
            effect.EffectForms.Add(effect_form);
            effect.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.StartOfTurn);

            patient_defense = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkClassPatientDefensePower",
                                                                     "",
                                                                     patient_defense_title_string,
                                                                     patient_defense_description_string,
                                                                     DatabaseHelper.FeatureDefinitionPowers.PowerShadowcasterShadowDodge.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.BonusAction,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.ShortRest
                                                                     );
            patient_defense.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                no_armor_restriction
                                            };
            patient_defense.linkedPower = flurry_of_blows;
        }


        static void createFlurryOfBlows()
        {
            string flurry_of_blows_title_string = "Feature/&MonkClassFlurryOfBlowsPowerTitle";
            string flurry_of_blows_description_string = "Feature/&MonkClassFlurryOfBlowsPowerDescription";

            var flurry_of_blows_feature_extra_attack = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalAction>.createFeatureCopy("MonkClassFlurryOfBlowsFeature",
                                                                                                                           "",
                                                                                                                           Common.common_no_title,
                                                                                                                           Common.common_no_title,
                                                                                                                           null,
                                                                                                                           DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionHasted,
                                                                                                                           a =>
                                                                                                                           {
                                                                                                                               a.restrictedActions = new List<ActionDefinitions.Id>
                                                                                                                               {
                                                                                                                                   ActionDefinitions.Id.AttackMain
                                                                                                                               };
                                                                                                                               a.SetMaxAttacksNumber(2);
                                                                                                                               //a.actionType = ActionDefinitions.ActionType.Bonus;
                                                                                                                           }
                                                                                                                           );

            var unarmed_attack = Helpers.FeatureBuilder<NewFeatureDefinitions.ExtraUnarmedAttack>.createFeature("MonkClassFlurryOfBlowsUnarmedAttack",
                                                                                                                    "",
                                                                                                                    Common.common_no_title,
                                                                                                                    Common.common_no_title,
                                                                                                                    null,
                                                                                                                    a =>
                                                                                                                    {
                                                                                                                        a.allowedWeaponTypesIfHasRequiredFeature = new List<string>();
                                                                                                                        a.allowedWeaponTypesIfHasRequiredFeature.AddRange(monk_weapons);
                                                                                                                        a.allowedWeaponTypesIfHasRequiredFeature.AddRange(way_of_iron_weapons);
                                                                                                                        a.allowedWeaponTypesIfHasRequiredFeature.Remove(Helpers.WeaponProficiencies.Unarmed);
                                                                                                                        a.requiredFeature = whirlwind_of_iron;
                                                                                                                        a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                        {
                                                                                                                            no_armor_restriction
                                                                                                                        };
                                                                                                                        a.clearAllAttacks = true;
                                                                                                                        a.actionType = ActionDefinitions.ActionType.Main;
                                                                                                                    }
                                                                                                                    );

            var fob_extra_attack = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("MonkClassFlurryOfBlowsExtraAttack",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    null,
                                                                                                                    DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack
                                                                                                                    );

            flurry_of_blows_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("MonkClassFlurryOfBlowsCondition",
                                                                                                  "",
                                                                                                  flurry_of_blows_title_string,
                                                                                                  flurry_of_blows_title_string,
                                                                                                  null,
                                                                                                  DatabaseHelper.ConditionDefinitions.ConditionHasted,
                                                                                                  new RuleDefinitions.ConditionInterruption[] {RuleDefinitions.ConditionInterruption.AnyBattleTurnEnd },
                                                                                                  unarmed_attack,
                                                                                                  fob_extra_attack,
                                                                                                  flurry_of_blows_feature_extra_attack
                                                                                                  );
            flurry_of_blows_condition.SetSubsequentOnRemoval(null);
            flurry_of_blows_condition.possessive = true;
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Haste.EffectDescription);
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.EffectForms.Clear();
            effect.SetTargetType(RuleDefinitions.TargetType.Self);

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = flurry_of_blows_condition;
            effect.EffectForms.Add(effect_form);

            flurry_of_blows = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkClassFlurryOfBlowsPower",
                                                                     "",
                                                                     flurry_of_blows_title_string,
                                                                     flurry_of_blows_description_string,
                                                                     DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionTurnUnholy.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.BonusAction,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.ShortRest
                                                                     );
            flurry_of_blows.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                attacked_with_monk_weapon_restriction,
                                                no_armor_restriction,
                                                new NewFeatureDefinitions.UsedAllMainAttacksRestriction(),
                                                new NewFeatureDefinitions.FreeOffHandRestriciton()
                                            };
        }


        static void createUnarmoredMovement()
        {
            string unarmored_movement_title_string = "Feature/&MonkClassUnarmoredMovementTitle";
            string unarmored_movement_description_string = "Feature/&MonkClassUnarmoredMovementDescription";

            string unarmored_movement_improvement_title_string = "Feature/&MonkClassUnarmoredMovementImprovementTitle";
            string unarmored_movement_improvement_description_string = "Feature/&MonkClassUnarmoredMovementImprovementDescription";

            var unarmored_movement_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionMovementAffinity>.createFeatureCopy("MonkClassUnarmoredMovementEffectFeature",
                                                                                                            "",
                                                                                                            Common.common_no_title,
                                                                                                            Common.common_no_title,
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider
                                                                                                            );
            unarmored_movement = Helpers.FeatureBuilder<NewFeatureDefinitions.MovementBonusWithRestrictions>.createFeature("MonkClassUnarmoredMovementFeature",
                                                                                                                         "",
                                                                                                                         unarmored_movement_title_string,
                                                                                                                         unarmored_movement_description_string,
                                                                                                                         null,
                                                                                                                         a =>
                                                                                                                         {
                                                                                                                             a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                             {
                                                                                                                                 no_armor_restriction
                                                                                                                             };
                                                                                                                             a.modifiers = new List<FeatureDefinition> { unarmored_movement_feature };
                                                                                                                         }
                                                                                                                         );

            var unarmored_movement_improvement_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionMovementAffinity>.createFeatureCopy("MonkClassUnarmoredMovementImprovementEffectFeature",
                                                                                                            "",
                                                                                                            Common.common_no_title,
                                                                                                            Common.common_no_title,
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider,
                                                                                                            c =>
                                                                                                            {
                                                                                                                c.SetBaseSpeedAdditiveModifier(1);
                                                                                                            }
                                                                                                            );

            for (int i = 0; i < unarmored_movement_improvement_levels.Count; i++)
            {
                var lvl = unarmored_movement_improvement_levels[i];
                int bonus_feet = 15 + i * 5;
                unarmored_movement_improvements[lvl] = Helpers.FeatureBuilder<NewFeatureDefinitions.MovementBonusWithRestrictions>.createFeature($"MonkClassUnarmoredMovementImprovementFeature{lvl}",
                                                                                                             "",
                                                                                                             unarmored_movement_improvement_title_string,
                                                                                                             Helpers.StringProcessing
                                                                                                                .replaceTagsInString(unarmored_movement_improvement_description_string,
                                                                                                                                     unarmored_movement_improvement_description_string + lvl.ToString(),
                                                                                                                                     ("<LEVEL>", lvl.ToString()),
                                                                                                                                     ("<FEET>", bonus_feet.ToString())
                                                                                                                                     ),
                                                                                                             null,
                                                                                                             a =>
                                                                                                             {
                                                                                                                 a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                 {
                                                                                                                     no_armor_restriction
                                                                                                                 };
                                                                                                                 a.modifiers = new List<FeatureDefinition> { unarmored_movement_improvement_feature };
                                                                                                             }
                                                                                                             );
            }
        }



        static void createUnarmoredDefense()
        {
            unarmored_defense = Helpers.FeatureBuilder<NewFeatureDefinitions.ArmorClassStatBonus>.createFeature("MonkClassUnarmoredDefense",
                                                                                                                "",
                                                                                                                "Feature/&MonkClassUnarmoredDefenseTitle",
                                                                                                                "Feature/&MonkClassUnarmoredDefenseDescription",
                                                                                                                null,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.armorAllowed = false;
                                                                                                                    a.shieldAlowed = false;
                                                                                                                    a.stat = Helpers.Stats.Wisdom;
                                                                                                                    a.exclusive = true;
                                                                                                                    a.forbiddenFeatures = new List<FeatureDefinition>
                                                                                                                    {
                                                                                                                        DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierSorcererDraconicResilienceAC,
                                                                                                                        DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierMageArmor,
                                                                                                                        DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin
                                                                                                                    };
                                                                                                                }
                                                                                                                );

            way_of_iron_allow_using_monk_features_in_armor = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("MonkSubclassWayOfIronAllowUseMonkFeaturesInArmorFeature",
                                                                                                                                "",
                                                                                                                                Common.common_no_title,
                                                                                                                                Common.common_no_title
                                                                                                                                );
            no_armor_restriction = new NewFeatureDefinitions.AndRestriction(new NewFeatureDefinitions.OrRestriction(new NewFeatureDefinitions.NoArmorRestriction(),
                                                                                                                    new NewFeatureDefinitions.NoShieldRestriction()
                                                                                                                    ),
                                                                            new NewFeatureDefinitions.HasFeatureRestriction(way_of_iron_allow_using_monk_features_in_armor)
                                                                        );


        }


        static void createMartialArts()
        {
            string martial_arts_title_string = "Feature/&MonkClassMartialArtsTitle";
            string martial_arts_description_string = "Feature/&MonkClassMartialArtsDescription";

            whirlwind_of_iron = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature("MonkSubclassWayOfIronWhirlwindOfIronFeature",
                                                                                       "",
                                                                                       "Feature/&MonkSubclassWayOfIronWhirlwindOfIronTitle",
                                                                                       "Feature/&MonkSubclassWayOfIronWhirlwindOfIronDescription",
                                                                                       Common.common_no_icon);

            var attacked_with_monk_weapon_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("MonkClassMartialArtsAttackedWithMonkWeaponCondition",
                                                                                                                "",
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_icon,
                                                                                                                DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                                                                new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.AnyBattleTurnEnd }
                                                                                                                );
            attacked_with_monk_weapon_condition.silentWhenAdded = true;
            attacked_with_monk_weapon_condition.silentWhenRemoved = true;
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(attacked_with_monk_weapon_condition);
            attacked_with_monk_weapon_condition.guiPresentation.hidden = true;
            var attacked_with_monk_weapon_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnAttackToAttackerOnlyWithWeaponCategory>.createFeature("MonkClassMartialArtsAttackedWithMonkWeaponWatcher",
                                                                                                                                                                                "",
                                                                                                                                                                                Common.common_no_title,
                                                                                                                                                                                Common.common_no_title,
                                                                                                                                                                                Common.common_no_icon,
                                                                                                                                                                                a =>
                                                                                                                                                                                {
                                                                                                                                                                                    a.allowedWeaponTypes = monk_weapons;
                                                                                                                                                                                    a.condition = attacked_with_monk_weapon_condition;
                                                                                                                                                                                    a.durationType = RuleDefinitions.DurationType.Turn;
                                                                                                                                                                                    a.durationValue = 1;
                                                                                                                                                                                    a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                                                                                                                                                }
                                                                                                                                                                                );

            attacked_with_monk_weapon_restriction = new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(attacked_with_monk_weapon_condition);

            var dex_on_weapons = Helpers.FeatureBuilder<NewFeatureDefinitions.canUseDexterityWithSpecifiedWeaponTypes>.createFeature("MonkClassMartialArtsDexForWeapons",
                                                                                                                                        "",
                                                                                                                                        Common.common_no_title,
                                                                                                                                        Common.common_no_title,
                                                                                                                                        null,
                                                                                                                                        a =>
                                                                                                                                        {
                                                                                                                                            a.weaponTypes = monk_weapons;
                                                                                                                                            a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                            {
                                                                                                                                                no_armor_restriction
                                                                                                                                            };
                                                                                                                                        }
                                                                                                                                        );

            var damage_dice = Helpers.FeatureBuilder<NewFeatureDefinitions.OverwriteDamageOnSpecificWeaponTypesBasedOnClassLevel>.createFeature("MonkClassMartialArtsDamageDice",
                                                                                                                                    "",
                                                                                                                                    Common.common_no_title,
                                                                                                                                    Common.common_no_title,
                                                                                                                                    null,
                                                                                                                                    a =>
                                                                                                                                    {
                                                                                                                                        a.weaponTypes = monk_weapons;
                                                                                                                                        a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                        {
                                                                                                                                            no_armor_restriction
                                                                                                                                        };
                                                                                                                                        a.characterClass = monk_class;
                                                                                                                                        a.levelDamageList = new List<(int, int, RuleDefinitions.DieType)>
                                                                                                                                        {
                                                                                                                                            (4, 1, RuleDefinitions.DieType.D4),
                                                                                                                                            (10, 1, RuleDefinitions.DieType.D6),
                                                                                                                                            (16, 1, RuleDefinitions.DieType.D8),
                                                                                                                                            (20, 1, RuleDefinitions.DieType.D10)
                                                                                                                                        };
                                                                                                                                    }
                                                                                                                                    );

            var bonus_unarmed_attack = Helpers.FeatureBuilder<NewFeatureDefinitions.ExtraUnarmedAttack>.createFeature("MonkClassMartialArtsBonusUnarmedAttack",
                                                                                                                                "",
                                                                                                                                Common.common_no_title,
                                                                                                                                Common.common_no_title,
                                                                                                                                null,
                                                                                                                                a =>
                                                                                                                                {
                                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature = new List<string>();
                                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature.AddRange(monk_weapons);
                                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature.AddRange(way_of_iron_weapons);
                                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature.Remove(Helpers.WeaponProficiencies.Unarmed);
                                                                                                                                    a.requiredFeature = whirlwind_of_iron;
                                                                                                                                    a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                    {
                                                                                                                                        attacked_with_monk_weapon_restriction,
                                                                                                                                        no_armor_restriction,
                                                                                                                                        new NewFeatureDefinitions.UsedAllMainAttacksRestriction(),
                                                                                                                                        new NewFeatureDefinitions.FreeOffHandRestriciton()
                                                                                                                                    };
                                                                                                                                    a.clearAllAttacks = false;
                                                                                                                                    a.actionType = ActionDefinitions.ActionType.Bonus;
                                                                                                                                }
                                                                                                                                );

            martial_arts = Helpers.FeatureSetBuilder.createFeatureSet("MonkClassMartialArts",
                                                                      "",
                                                                      martial_arts_title_string,
                                                                      martial_arts_description_string,
                                                                      false,
                                                                      FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                      false,
                                                                      dex_on_weapons,
                                                                      damage_dice,
                                                                      bonus_unarmed_attack,
                                                                      attacked_with_monk_weapon_watcher
                                                                      );

        }


        static void createExtraAttack()
        {
            extra_attack = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("MonkClassExtraAttack",
                                                                                                            "",
                                                                                                            "",
                                                                                                            "",
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack
                                                                                                            );
        }


        static void createOpenHandTechnique()
        {
            string open_hand_technique_title_string = "Feature/&MonkSubclassWayOfTheOpenHandTechniqueTitle";
            string open_hand_technique_description_string = "Feature/&MonkSubclassWayOfTheOpenHandTechniqueDescription";

            var open_hand_used_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("MonkSubclassWayOfTheOpenHandUsedCondition",
                                                                                                    "",
                                                                                                    "",
                                                                                                    "",
                                                                                                    null,
                                                                                                    DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                                                    new RuleDefinitions.ConditionInterruption[] {RuleDefinitions.ConditionInterruption.AttacksAndDamages }
                                                                                                    );
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(open_hand_used_condition);
                                                                                                         
            createOpenHandTechniqueKnock();
            createOpenHandTechniquePush();
            createOpenHandTechniqueForbidReaction();

            var open_hand_used_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyConditionOnPowerUseToSelf>.createFeature("MonkSubclassWayOfTheOpenHandUsedFeature",
                                                                                                                                      "",
                                                                                                                                      Common.common_no_title,
                                                                                                                                      Common.common_no_title,
                                                                                                                                      Common.common_no_icon,
                                                                                                                                      a =>
                                                                                                                                      {
                                                                                                                                          a.condition = open_hand_used_condition;
                                                                                                                                          a.durationType = RuleDefinitions.DurationType.Round;
                                                                                                                                          a.durationValue = 1;
                                                                                                                                          a.powers = new List<FeatureDefinitionPower> { open_hand_technique_knock, open_hand_technique_push, open_hand_technique_forbid_reaction };


                                                                                                                                      }
                                                                                                                                      );


            open_hand_technique = Helpers.FeatureSetBuilder.createFeatureSet("MonkSubclassWayOfTheOpenHandTechnique",
                                                                             "",
                                                                             open_hand_technique_title_string,
                                                                             open_hand_technique_description_string,
                                                                             false,
                                                                             FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                             false,
                                                                             open_hand_technique_knock,
                                                                             open_hand_technique_push,
                                                                             open_hand_technique_forbid_reaction,
                                                                             open_hand_used_feature
                                                                             );
            open_hand_technique_knock.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(open_hand_used_condition));
            open_hand_technique_push.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(open_hand_used_condition));
            open_hand_technique_forbid_reaction.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(open_hand_used_condition));
        }


        static void createOpenHandTechniqueForbidReaction()
        {
            string open_hand_forbid_reaction_title_string = "Feature/&MonkSubclassWayOfTheOpenHandForbidReactionTitle";
            string open_hand_forbid_reaction_description_string = "Feature/&MonkSubclassWayOfTheOpenHandForbidReactionDescription";
            string use_open_hand_forbid_reaction_react_description = "Reaction/&SpendMonkSubclassWayOfTheOpenHandForbidReactionPowerReactDescription";
            string use_open_hand_forbid_reaction_react_title = "Reaction/&CommonUsePowerReactTitle";

            string open_hand_forbid_reaction_condition_title_string = "Rules/&ConditionMonkSubclassWayOfTheOpenHandForbidReactionPowerTitle";
            string open_hand_forbid_reaction_condition_description_string = "Rules/&ConditionMonkSubclassWayOfTheOpenHandForbidReactionPowerDescription";

            var condition = Helpers.ConditionBuilder.createCondition("MonkSubclassWayOfTheOpenHandForbidReactionCondition",
                                                                    "",
                                                                    open_hand_forbid_reaction_condition_title_string,
                                                                    open_hand_forbid_reaction_condition_description_string,
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionDazzled,
                                                                    DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityConditionDazzled
                                                                    );

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Strength;
            effect.hasSavingThrow = false;
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect.EffectForms.Add(effect_form);
            effect.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkSubclassWayOfTheOpenHandForbidReaction",
                                                                     "",
                                                                     open_hand_forbid_reaction_title_string,
                                                                     open_hand_forbid_reaction_description_string,
                                                                     flurry_of_blows.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.AtWill
                                                                     );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(flurry_of_blows_condition)
                                            };
            power.checkReaction = true;

            open_hand_technique_forbid_reaction = power;

            Helpers.StringProcessing.addPowerReactStrings(open_hand_technique_forbid_reaction, open_hand_forbid_reaction_title_string, use_open_hand_forbid_reaction_react_description,
                                                                    use_open_hand_forbid_reaction_react_title, use_open_hand_forbid_reaction_react_description, "SpendPower");
        }


        static void createOpenHandTechniquePush()
        {
            string open_hand_push_title_string = "Feature/&MonkSubclassWayOfTheOpenHandPushTitle";
            string open_hand_push_description_string = "Feature/&MonkSubclassWayOfTheOpenHandPushDescription";
            string use_open_hand_push_react_description = "Reaction/&SpendMonkSubclassWayOfTheOpenHandPushPowerReactDescription";
            string use_open_hand_push_react_title = "Reaction/&CommonUsePowerReactTitle";

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Strength;
            effect.hasSavingThrow = true;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.motionForm = new MotionForm();
            effect_form.FormType = EffectForm.EffectFormType.Motion;
            effect_form.motionForm.type = MotionForm.MotionType.PushFromOrigin;
            effect_form.motionForm.distance = 3;
            effect_form.hasSavingThrow = true;
            effect_form.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkSubclassWayOfTheOpenHandPush",
                                                                     "",
                                                                     open_hand_push_title_string,
                                                                     open_hand_push_description_string,
                                                                     flurry_of_blows.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.AtWill
                                                                     );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(flurry_of_blows_condition)
                                            };
            power.checkReaction = true;

            open_hand_technique_push = power;

            Helpers.StringProcessing.addPowerReactStrings(open_hand_technique_push, open_hand_push_title_string, use_open_hand_push_react_description,
                                                        use_open_hand_push_react_title, use_open_hand_push_react_description, "SpendPower");
        }


        static void createOpenHandTechniqueKnock()
        {
            string open_hand_knock_title_string = "Feature/&MonkSubclassWayOfTheOpenHandKnockTitle";
            string open_hand_knock_description_string = "Feature/&MonkSubclassWayOfTheOpenHandKnockDescription";
            string use_open_hand_knock_react_description = "Reaction/&SpendMonkSubclassWayOfTheOpenHandKnockPowerReactDescription";
            string use_open_hand_knock_react_title = "Reaction/&CommonUsePowerReactTitle";

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Dexterity;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.motionForm = new MotionForm();
            effect_form.FormType = EffectForm.EffectFormType.Motion;
            effect_form.motionForm.type = MotionForm.MotionType.FallProne;
            effect_form.motionForm.distance = 3;
            effect_form.hasSavingThrow = true;
            effect_form.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkSubclassWayOfTheOpenHandKnock",
                                                                     "",
                                                                     open_hand_knock_title_string,
                                                                     open_hand_knock_description_string,
                                                                     flurry_of_blows.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.AtWill
                                                                     );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(flurry_of_blows_condition)
                                            };
            power.checkReaction = true;

            open_hand_technique_knock = power;

            Helpers.StringProcessing.addPowerReactStrings(open_hand_technique_knock, open_hand_knock_title_string, use_open_hand_knock_react_description,
                                            use_open_hand_knock_react_title, use_open_hand_knock_react_description, "SpendPower");
        }


        static void createWholenessOfBody()
        {
            string wholeness_of_body_title_string = "Feature/&MonkSubclassWayOfTheOpenHandWholenessOfBodyTitle";
            string wholeness_of_body_description_string = "Feature/&MonkSubclassWayOfTheOpenHandWholenessOfBodyDescription";

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.CureWounds.EffectDescription);
            effect.DurationParameter = 1;
            effect.SetRangeType(RuleDefinitions.RangeType.Self);
            effect.SetRangeParameter(1);
            effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
            effect.SetTargetType(RuleDefinitions.TargetType.Self);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.healingForm = new HealingForm();
            effect_form.FormType = EffectForm.EffectFormType.Healing;
            effect_form.healingForm.diceNumber = 3;
            effect_form.healingForm.dieType = RuleDefinitions.DieType.D1;
            effect_form.SetApplyLevel(EffectForm.LevelApplianceType.Multiply);
            effect_form.SetLevelType(RuleDefinitions.LevelSourceType.ClassLevel);
            effect.EffectForms.Add(effect_form);

            wholeness_of_body = Helpers.PowerBuilder.createPower("MonkSubclassWayOfTheOpenHandWholenessOfBodyPower",
                                                                 "",
                                                                 wholeness_of_body_title_string,
                                                                 wholeness_of_body_description_string,
                                                                 DatabaseHelper.FeatureDefinitionPowers.PowerTraditionShockArcanistArcaneShock.GuiPresentation.spriteReference,
                                                                 DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleHeraldOfBattle,
                                                                 effect,
                                                                 RuleDefinitions.ActivationTime.Action,
                                                                 1,
                                                                 RuleDefinitions.UsesDetermination.Fixed,
                                                                 RuleDefinitions.RechargeRate.LongRest
                                                                 );
        }


        static void createTranquility()
        {
            string tranquility_title_string = "Feature/&MonkSubclassWayOfTheOpenHandTranquilityTitle";
            string tranqulity_description_string = "Feature/&MonkSubclassWayOfTheOpenHandTranquilityDescription";
            tranquility = Helpers.AbilityCheckAffinityBuilder.createSkillCheckAffinity("MonkSubclassWayOfTheOpenHandTranquility",
                                                                                       "",
                                                                                       tranquility_title_string,
                                                                                       tranqulity_description_string,
                                                                                       Common.common_no_icon,
                                                                                       RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                                                                                       1,
                                                                                       RuleDefinitions.DieType.D1,
                                                                                       Helpers.Skills.Stealth
                                                                                       );
        }

        static CharacterSubclassDefinition createWayOfTheOpenHand()
        {
            createOpenHandTechnique();
            createWholenessOfBody();
            createTranquility();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&MonkSubclassWayOfTheOpenHandDescription",
                    "Subclass/&MonkSubclassWayOfTheOpenHandTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainLife.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("MonkSubclassWayOfTheOpenHand", "4c6e8abe-1983-49b7-bc3b-2572865f6c17")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(open_hand_technique, 3)
                    .AddFeatureAtLevel(wholeness_of_body, 6)
                    .AddFeatureAtLevel(tranquility, 11)
                    .AddToDB();

            return definition;
        }

        static CharacterSubclassDefinition createWayOfThePyrokine()
        {
            createBlazingTechnique();
            createBurningDevotion();
            createLeapingFlames();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&MonkSubclassWayOfThePyrokineDescription",
                    "Subclass/&MonkSubclassWayOfThePyrokineTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainElementalFire.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("MonkSubclassWayOfThePyrokine", "e9f29a8c-23a5-4774-8e92-4e4f23fb779a")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(blazing_technique, 3)
                    .AddFeatureAtLevel(burning_devotion, 6)
                    .AddFeatureAtLevel(leaping_flames, 11)
                    .AddToDB();

            return definition;
        }


        static void createLeapingFlames()
        {
            string leaping_flames_title_string = "Feature/&MonkSubclassWayOfThePyrokineLeapingFlamesTitle";
            string leaping_flames_description_string = "Feature/&MonkSubclassWayOfThePyrokineLeapingFlamesDescription";
            leaping_flames = Helpers.AbilityCheckAffinityBuilder.createSkillCheckAffinity("MonkSubclassWayOfThePyrokineLeapingFlames",
                                                                                       "",
                                                                                       leaping_flames_title_string,
                                                                                       leaping_flames_description_string,
                                                                                       Common.common_no_icon,
                                                                                       RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                                                                                       1,
                                                                                       RuleDefinitions.DieType.D1,
                                                                                       Helpers.Skills.Acrobatics
                                                                                       );
        }


        static void createBurningDevotion()
        {
            string burning_devotion_title_string = "Feature/&MonkSubclassWayOfThePyrokineBurningDevotionTitle";
            string burning_devotion_description_string = "Feature/&MonkSubclassWayOfThePyrokineBurningDevotionDescription";

            burning_devotion = Helpers.FeatureSetBuilder.createFeatureSet("MonkSubclassWayOfThePyrokineBurningDevotion",
                                                                            "",
                                                                            burning_devotion_title_string,
                                                                            burning_devotion_description_string,
                                                                            false,
                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                            false,
                                                                            DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance
                                                                            );
        }



        static void createBlazingTechnique()
        {
            string blazing_technique_title_string = "Feature/&MonkSubclassWayOfThePyrokineTechniqueTitle";
            string blazing_technique_description_string = "Feature/&MonkSubclassWayOfThePyrokineTechniqueDescription";

            var blazing_technique_used_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("MonkSubclassWayOfThePyrokineUsedCondition",
                                                                                                    "",
                                                                                                    "",
                                                                                                    "",
                                                                                                    null,
                                                                                                    DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                                                    new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.AttacksAndDamages }
                                                                                                    );
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(blazing_technique_used_condition);

            createBlazingTechniqueBurn();
            createBlazingTechniqueDamage();
            createBlazingTechniqueBlind();

            var open_hand_used_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyConditionOnPowerUseToSelf>.createFeature("MonkSubclassWayOfThePyrokineUsedFeature",
                                                                                                                                      "",
                                                                                                                                      Common.common_no_title,
                                                                                                                                      Common.common_no_title,
                                                                                                                                      Common.common_no_icon,
                                                                                                                                      a =>
                                                                                                                                      {
                                                                                                                                          a.condition = blazing_technique_used_condition;
                                                                                                                                          a.durationType = RuleDefinitions.DurationType.Round;
                                                                                                                                          a.durationValue = 1;
                                                                                                                                          a.powers = new List<FeatureDefinitionPower> { blazing_technique_burn, blazing_technique_damage, blazing_technique_blind };


                                                                                                                                      }
                                                                                                                                      );


            blazing_technique = Helpers.FeatureSetBuilder.createFeatureSet("MonkSubclassWayOfThePyrokineTechnique",
                                                                             "",
                                                                             blazing_technique_title_string,
                                                                             blazing_technique_description_string,
                                                                             false,
                                                                             FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                             false,
                                                                             blazing_technique_burn,
                                                                             blazing_technique_damage,
                                                                             blazing_technique_blind,
                                                                             open_hand_used_feature
                                                                             );
            blazing_technique_burn.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(blazing_technique_used_condition));
            blazing_technique_damage.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(blazing_technique_used_condition));
            blazing_technique_blind.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(blazing_technique_used_condition));
        }


        static void createBlazingTechniqueBurn()
        {
            string pyrokine_burn_title_string = "Feature/&MonkSubclassWayOfThePyrokineBurnTitle";
            string pyrokine_burn_description_string = "Feature/&MonkSubclassWayOfThePyrokineBurnDescription";
            string use_pyrokine_burn_react_description = "Reaction/&SpendMonkSubclassWayOfThePyrokineBurnPowerReactDescription";
            string use_pyrokine_burn_react_title = "Reaction/&CommonUsePowerReactTitle";

            string burning_title_string = "Rules/&ConditionMonkSubclassWayOfThePyrokineBurnPowerTitle";
            string burning_description_string = "Rules/&ConditionMonkSubclassWayOfThePyrokineBurnPowerDescription";
            var condition = Helpers.CopyFeatureBuilder<ConditionDefinition>.createFeatureCopy("MonkSubclassWayOfThePyrokineBurnCondition",
                                                                                              "",
                                                                                              burning_title_string,
                                                                                              burning_description_string,
                                                                                              null,
                                                                                              DatabaseHelper.ConditionDefinitions.ConditionAcidArrowed
                                                                                              );
            condition.specialDuration = false;
            condition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            var damage_form = new EffectForm();
            damage_form.DamageForm = new DamageForm();
            damage_form.FormType = EffectForm.EffectFormType.Damage;
            damage_form.DamageForm.dieType = RuleDefinitions.DieType.D4;
            damage_form.DamageForm.diceNumber = 1;
            damage_form.DamageForm.damageType = Helpers.DamageTypes.Fire;
            condition.recurrentEffectForms.Clear();
            condition.recurrentEffectForms.Add(damage_form);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.UntilAnyRest;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Dexterity;
            effect.hasSavingThrow = true;
            effect.effectParticleParameters.impactParticleReference = DatabaseHelper.SpellDefinitions.FireBolt.EffectDescription.effectParticleParameters.impactParticleReference;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);           
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect_form.saveOccurence = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            effect_form.hasSavingThrow = true;
            effect_form.canSaveToCancel = true;
            effect_form.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkSubclassWayOfThePyrokineBurn",
                                                                     "",
                                                                     pyrokine_burn_title_string,
                                                                     pyrokine_burn_description_string,
                                                                     flurry_of_blows.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.AtWill
                                                                     );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(flurry_of_blows_condition)
                                            };
            power.checkReaction = true;

            blazing_technique_burn = power;

            Helpers.StringProcessing.addPowerReactStrings(blazing_technique_burn, pyrokine_burn_title_string, use_pyrokine_burn_react_description,
                                                        use_pyrokine_burn_react_title, use_pyrokine_burn_react_description, "SpendPower");
        }


        static void createBlazingTechniqueBlind()
        {
            string pyrokine_blind_title_string = "Feature/&MonkSubclassWayOfThePyrokineBlindTitle";
            string pyrokine_blind_description_string = "Feature/&MonkSubclassWayOfThePyrokineBlindDescription";
            string use_pyrokine_blind_react_description = "Reaction/&SpendMonkSubclassWayOfThePyrokineBlindPowerReactDescription";
            string use_pyrokine_blind_react_title = "Reaction/&CommonUsePowerReactTitle";

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Round;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Wisdom;
            effect.hasSavingThrow = true;
            effect.effectParticleParameters.impactParticleReference = DatabaseHelper.SpellDefinitions.FireBolt.EffectDescription.effectParticleParameters.impactParticleReference;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionBlinded;
            effect_form.hasSavingThrow = true;
            effect_form.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);
            effect.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkSubclassWayOfThePyrokineBlind",
                                                                     "",
                                                                     pyrokine_blind_title_string,
                                                                     pyrokine_blind_description_string,
                                                                     flurry_of_blows.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.AtWill
                                                                     );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(flurry_of_blows_condition)
                                            };
            power.checkReaction = true;

            blazing_technique_blind = power;

            Helpers.StringProcessing.addPowerReactStrings(blazing_technique_blind, pyrokine_blind_title_string, use_pyrokine_blind_react_description,
                                                        use_pyrokine_blind_react_title, use_pyrokine_blind_react_description, "SpendPower");
        }


        static void createBlazingTechniqueDamage()
        {
            string pyrokine_damage_title_string = "Feature/&MonkSubclassWayOfThePyrokineDamageTitle";
            string pyrokine_damage_description_string = "Feature/&MonkSubclassWayOfThePyrokineDamageDescription";
            string use_pyrokine_damage_react_description = "Reaction/&SpendMonkSubclassWayOfThePyrokineDamagePowerReactDescription";
            string use_pyrokine_damage_react_title = "Reaction/&CommonUsePowerReactTitle";

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Wisdom;
            effect.hasSavingThrow = false;
            effect.effectParticleParameters.impactParticleReference = DatabaseHelper.SpellDefinitions.FireBolt.EffectDescription.effectParticleParameters.impactParticleReference;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.DamageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D4;
            effect_form.DamageForm.diceNumber = 1;
            effect_form.DamageForm.damageType = Helpers.DamageTypes.Fire;
            effect.EffectForms.Add(effect_form);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkSubclassWayOfThePyrokineDamage",
                                                                     "",
                                                                     pyrokine_damage_title_string,
                                                                     pyrokine_damage_description_string,
                                                                     flurry_of_blows.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     2,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.AtWill
                                                                     );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.HasAtLeastOneConditionFromListRestriction(flurry_of_blows_condition)
                                            };
            power.checkReaction = true;

            blazing_technique_damage = power;

            Helpers.StringProcessing.addPowerReactStrings(blazing_technique_damage, pyrokine_damage_title_string, use_pyrokine_damage_react_description,
                                                        use_pyrokine_damage_react_title, use_pyrokine_damage_react_description, "SpendPower");
        }

        static void createRoilingStormOfIron()
        {
            string roiling_storm_of_iron_title_string = "Feature/&MonkSubclassWayOfIronRoilingStormOfIronTitle";
            string roiling_storm_of_iron_description_string = "Feature/&MonkSubclassWayOfIronRoilingStormOfIronDescription";

            var attacked_with_monk_weapon_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("MonkSubclassWayOfIronRoilingStormOfIronAttackedWithMonkWeaponCondition",
                                                                                                    "",
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_icon,
                                                                                                    DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                                                    new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.AnyBattleTurnEnd }
                                                                                                    );
            attacked_with_monk_weapon_condition.silentWhenAdded = true;
            attacked_with_monk_weapon_condition.silentWhenRemoved = true;
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(attacked_with_monk_weapon_condition);
            attacked_with_monk_weapon_condition.guiPresentation.hidden = true;
            var attacked_with_monk_weapon_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnAttackToAttackerOnlyWithWeaponCategory>.createFeature("MonkSubclassWayOfIronRoilingStormOfIronhMonkWeaponWatcher",
                                                                                                                                                                                "",
                                                                                                                                                                                Common.common_no_title,
                                                                                                                                                                                Common.common_no_title,
                                                                                                                                                                                Common.common_no_icon,
                                                                                                                                                                                a =>
                                                                                                                                                                                {
                                                                                                                                                                                    a.allowedWeaponTypes = way_of_iron_weapons;
                                                                                                                                                                                    a.condition = attacked_with_monk_weapon_condition;
                                                                                                                                                                                    a.durationType = RuleDefinitions.DurationType.Turn;
                                                                                                                                                                                    a.durationValue = 1;
                                                                                                                                                                                    a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                                                                                                                                                }
                                                                                                                                                                                );

            attacked_with_monk_weapon_restriction.conditions.Add(attacked_with_monk_weapon_condition);

            var dex_on_weapons = Helpers.FeatureBuilder<NewFeatureDefinitions.canUseDexterityWithSpecifiedWeaponTypes>.createFeature("MonkSubclassWayOfIronMartialArtsDexForWeapons",
                                                                                                                            "",
                                                                                                                            Common.common_no_title,
                                                                                                                            Common.common_no_title,
                                                                                                                            null,
                                                                                                                            a =>
                                                                                                                            {
                                                                                                                                a.weaponTypes = way_of_iron_weapons;
                                                                                                                                a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                {
                                                                                                                                   no_armor_restriction
                                                                                                                                };
                                                                                                                            }
                                                                                                                            );

            var damage_dice = Helpers.FeatureBuilder<NewFeatureDefinitions.OverwriteDamageOnSpecificWeaponTypesBasedOnClassLevel>.createFeature("MonkSubclassWayOfIronMartialArtsDamageDice",
                                                                                                                                    "",
                                                                                                                                    Common.common_no_title,
                                                                                                                                    Common.common_no_title,
                                                                                                                                    null,
                                                                                                                                    a =>
                                                                                                                                    {
                                                                                                                                        a.weaponTypes = way_of_iron_weapons;
                                                                                                                                        a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                        {
                                                                                                                                           no_armor_restriction
                                                                                                                                        };
                                                                                                                                        a.characterClass = monk_class;
                                                                                                                                        a.levelDamageList = new List<(int, int, RuleDefinitions.DieType)>
                                                                                                                                        {
                                                                                                                                            (4, 1, RuleDefinitions.DieType.D4),
                                                                                                                                            (10, 1, RuleDefinitions.DieType.D6),
                                                                                                                                            (16, 1, RuleDefinitions.DieType.D8),
                                                                                                                                            (20, 1, RuleDefinitions.DieType.D10)
                                                                                                                                        };
                                                                                                                                    }
                                                                                                                                    );


            var bonus_unarmed_attack = Helpers.FeatureBuilder<NewFeatureDefinitions.ExtraUnarmedAttack>.createFeature("MonkSubclassWayOfIronMartialArtsBonusUnarmedAttack",
                                                                                                                        "",
                                                                                                                        Common.common_no_title,
                                                                                                                        Common.common_no_title,
                                                                                                                        null,
                                                                                                                        a =>
                                                                                                                        {
                                                                                                                            a.allowedWeaponTypesIfHasRequiredFeature = new List<string>();
                                                                                                                            a.allowedWeaponTypesIfHasRequiredFeature.AddRange(monk_weapons);
                                                                                                                            a.allowedWeaponTypesIfHasRequiredFeature.AddRange(way_of_iron_weapons);
                                                                                                                            a.allowedWeaponTypesIfHasRequiredFeature.Remove(Helpers.WeaponProficiencies.Unarmed);
                                                                                                                            a.requiredFeature = whirlwind_of_iron;
                                                                                                                            a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                            {
                                                                                                                                attacked_with_monk_weapon_restriction,
                                                                                                                                new NewFeatureDefinitions.HasFeatureRestriction(way_of_iron_allow_using_monk_features_in_armor),
                                                                                                                                new NewFeatureDefinitions.UsedAllMainAttacksRestriction(),
                                                                                                                                new NewFeatureDefinitions.FreeOffHandRestriciton()
                                                                                                                            };
                                                                                                                            a.clearAllAttacks = false;
                                                                                                                            a.actionType = ActionDefinitions.ActionType.Bonus;
                                                                                                                        }
                                                                                                                        );

            roiling_storm_of_iron = Helpers.FeatureSetBuilder.createFeatureSet("MonkSubclassWayOfIronRoilingStormOfIron",
                                                                               "",
                                                                               roiling_storm_of_iron_title_string,
                                                                               roiling_storm_of_iron_description_string,
                                                                               false,
                                                                               FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                               false,
                                                                               way_of_iron_allow_using_monk_features_in_armor,
                                                                               DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyRangerArmor,
                                                                               DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyRangerWeapon,
                                                                               dex_on_weapons,
                                                                               damage_dice,
                                                                               //bonus_unarmed_attack, - the base one will already work
                                                                               attacked_with_monk_weapon_watcher
                                                                               );

            var flurry_unarmed_attack_way_of_iron = Helpers.FeatureBuilder<NewFeatureDefinitions.ExtraUnarmedAttack>.createFeature("MonkSubclassWayOfIronFlurryOfBlowsUnarmedAttack",
                                                                                                                "",
                                                                                                                Common.common_no_title,
                                                                                                                Common.common_no_title,
                                                                                                                null,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature = new List<string>();
                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature.AddRange(monk_weapons);
                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature.AddRange(way_of_iron_weapons);
                                                                                                                    a.allowedWeaponTypesIfHasRequiredFeature.Remove(Helpers.WeaponProficiencies.Unarmed);
                                                                                                                    a.requiredFeature = whirlwind_of_iron;
                                                                                                                    a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                    {
                                                                                                                        new NewFeatureDefinitions.HasFeatureRestriction(way_of_iron_allow_using_monk_features_in_armor)
                                                                                                                    };
                                                                                                                    a.clearAllAttacks = true;
                                                                                                                    a.actionType = ActionDefinitions.ActionType.Main;
                                                                                                                }
                                                                                                                );

            //flurry_of_blows_condition.features.Add(flurry_unarmed_attack_way_of_iron);

        }


        static void createShiftingBlades()
        {
            string shifting_blades_title_string = "Feature/&MonkSubclassWayOfIronShiftingBladesTitle";
            string shifting_blades_description_string = "Feature/&MonkSubclassWayOfIronShiftingBladesDescription";

            shifting_blades = Helpers.FeatureBuilder<NewFeatureDefinitions.AddAttackTagForSpecificWeaponType>.createFeature("MonkSubclassWayOfIronShiftingBlades",
                                                                                                                                "",
                                                                                                                                shifting_blades_title_string,
                                                                                                                                shifting_blades_description_string,
                                                                                                                                Common.common_no_icon,
                                                                                                                                a =>
                                                                                                                                {
                                                                                                                                    a.weaponTypes = new List<string>();
                                                                                                                                    a.weaponTypes.AddRange(monk_weapons.Where(w => w != Helpers.WeaponProficiencies.Unarmed));
                                                                                                                                    a.weaponTypes.AddRange(way_of_iron_weapons);
                                                                                                                                    a.tag = "Magical";
                                                                                                                                }
                                                                                                                                );
        }


        static void createTestOfSkill()
        {
            string test_of_skills_title_string = "Feature/&MonkSubclassWayOfIronTestOfSkillTitle";
            string test_of_skills_description_string = "Feature/&MonkSubclassWayOfIronTestOfSkillDescription";

            var condition = Helpers.ConditionBuilder.createCondition("MonkSubclassWayOfIronTestOfSkillCondition",
                                                                    "",
                                                                    "Rules/&ConditionMonkSubclassWayOfIronTestOfSkillTitle",
                                                                    "Rules/&ConditionMonkSubclassWayOfIronTestOfSkillDescription",
                                                                    null,
                                                                    DatabaseHelper.ConditionDefinitions.ConditionDazzled);

            var disadvantage_against_non_caster_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.AttackDisadvantageAgainstNonCaster>
                                                                            .createFeature("MonkSubclassWayOfIronTestOfSkilllAttackDisadvantage",
                                                                                           "",
                                                                                           Common.common_no_title,
                                                                                           Common.common_no_title,
                                                                                           Common.common_no_icon,
                                                                                           a =>
                                                                                           {
                                                                                               a.condition = condition;
                                                                                           }
                                                                                           );

            var remove_condition_if_affected_by_non_caster_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.TargetRemoveConditionIfAffectedByHostileNonCaster>
                                                                .createFeature("MonkSubclassWayOfIronTestOfSkilllConditionWatcher",
                                                                               "",
                                                                               Common.common_no_title,
                                                                               Common.common_no_title,
                                                                               Common.common_no_icon,
                                                                               a =>
                                                                               {
                                                                                   a.condition = condition;
                                                                               }
                                                                               );
            condition.features.Add(disadvantage_against_non_caster_feature);
            condition.features.Add(remove_condition_if_affected_by_non_caster_feature);


            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.Dazzle.EffectDescription);
            effect.DurationParameter = 1;
            effect.DurationType = RuleDefinitions.DurationType.Minute;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Wisdom);
            effect.SavingThrowAbility = Helpers.Stats.Wisdom;
            effect.hasSavingThrow = true;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect.rangeParameter = 6;
            effect.SetRangeType(RuleDefinitions.RangeType.Distance);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.ConditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            effect_form.ConditionForm.ConditionDefinition = condition;
            effect_form.hasSavingThrow = true;
            effect_form.SavingThrowAffinity = RuleDefinitions.EffectSavingThrowType.Negates;
            effect.EffectForms.Add(effect_form);
            effect.SetEndOfEffect(RuleDefinitions.TurnOccurenceType.EndOfTurn);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("MonkSubclassWayOfIronTestOfSkillPower",
                                                                     "",
                                                                     test_of_skills_title_string,
                                                                     test_of_skills_description_string,
                                                                     DatabaseHelper.FeatureDefinitionPowers.PowerFighterActionSurge.GuiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.NoCost,
                                                                     1,
                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                     RuleDefinitions.RechargeRate.OneMinute
                                                                     );
            test_of_skill = power;
        }


        static CharacterSubclassDefinition createWayOfIron()
        {
            createRoilingStormOfIron();
            createTestOfSkill();
            createShiftingBlades();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&MonkSubclassWayOfIronDescription",
                    "Subclass/&MonkSubclassWayOfIronTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.MartialChampion.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("MonkSubclassWayOfIron", "c4e735e3-0a9e-4586-bd86-0137ce61ff63")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(roiling_storm_of_iron, 3)
                    .AddFeatureAtLevel(test_of_skill, 3)
                    .AddFeatureAtLevel(shifting_blades, 6)
                    .AddFeatureAtLevel(whirlwind_of_iron, 11)
                    .AddToDB();

            return definition;
        }



        static void createWayOfTheBow()
        {
            string way_of_the_bow_title_string = "Feature/&MonkSubclassWayOfZenArcheryWayOfTheBowTitle";
            string way_of_the_bow_description_string = "Feature/&MonkSubclassWayOfZenArcheryWayOfTheBowDescription";

            var attacked_with_monk_weapon_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("MonkSubclassWayOfZenArcheryWayOfTheBowAttackedWithMonkWeaponCondition",
                                                                                                    "",
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_title,
                                                                                                    Common.common_no_icon,
                                                                                                    DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                                                    new RuleDefinitions.ConditionInterruption[] { RuleDefinitions.ConditionInterruption.AnyBattleTurnEnd }
                                                                                                    );
            attacked_with_monk_weapon_condition.silentWhenAdded = true;
            attacked_with_monk_weapon_condition.silentWhenRemoved = true;
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(attacked_with_monk_weapon_condition);
            attacked_with_monk_weapon_condition.guiPresentation.hidden = true;
            var attacked_with_monk_weapon_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnAttackToAttackerOnlyWithWeaponCategory>.createFeature("MonkSubclassWayOfZenArcheryWayOfTheBowhMonkWeaponWatcher",
                                                                                                                                                                                "",
                                                                                                                                                                                Common.common_no_title,
                                                                                                                                                                                Common.common_no_title,
                                                                                                                                                                                Common.common_no_icon,
                                                                                                                                                                                a =>
                                                                                                                                                                                {
                                                                                                                                                                                    a.allowedWeaponTypes = way_of_zen_archery_weapons;
                                                                                                                                                                                    a.condition = attacked_with_monk_weapon_condition;
                                                                                                                                                                                    a.durationType = RuleDefinitions.DurationType.Turn;
                                                                                                                                                                                    a.durationValue = 1;
                                                                                                                                                                                    a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                                                                                                                                                }
                                                                                                                                                                                );

            attacked_with_monk_weapon_restriction.conditions.Add(attacked_with_monk_weapon_condition);

            var damage_dice = Helpers.FeatureBuilder<NewFeatureDefinitions.OverwriteDamageOnSpecificWeaponTypesBasedOnClassLevel>.createFeature("MonkSubclassWayOfZenArcheryMartialArtsDamageDice",
                                                                                                                                    "",
                                                                                                                                    Common.common_no_title,
                                                                                                                                    Common.common_no_title,
                                                                                                                                    null,
                                                                                                                                    a =>
                                                                                                                                    {
                                                                                                                                        a.weaponTypes = way_of_zen_archery_weapons;
                                                                                                                                        a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                        {
                                                                                                                                           no_armor_restriction
                                                                                                                                        };
                                                                                                                                        a.characterClass = monk_class;
                                                                                                                                        a.levelDamageList = new List<(int, int, RuleDefinitions.DieType)>
                                                                                                                                        {
                                                                                                                                            (4, 1, RuleDefinitions.DieType.D4),
                                                                                                                                            (10, 1, RuleDefinitions.DieType.D6),
                                                                                                                                            (16, 1, RuleDefinitions.DieType.D8),
                                                                                                                                            (20, 1, RuleDefinitions.DieType.D10)
                                                                                                                                        };
                                                                                                                                    }
                                                                                                                                    );

            var wis_on_weapons = Helpers.FeatureBuilder<NewFeatureDefinitions.ReplaceWeaponAbilityScoreForWeapons>.createFeature("MonkSubclassWayOfZenArcheryArcheryWisForWeapons",
                                                                                                                            "",
                                                                                                                            Common.common_no_title,
                                                                                                                            Common.common_no_title,
                                                                                                                            null,
                                                                                                                            a =>
                                                                                                                            {
                                                                                                                                a.abilityScores = new List<string> { Helpers.Stats.Wisdom };
                                                                                                                                a.weaponTypes = way_of_zen_archery_weapons;
                                                                                                                                a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                                {
                                                                                                                                     no_armor_restriction
                                                                                                                                };
                                                                                                                            }
                                                                                                                            );

            var archery = Helpers.CopyFeatureBuilder<FeatureDefinitionProficiency>.createFeatureCopy("MonkSubclassWayOfZenArcheryArchery",
                                                               "",
                                                               "",
                                                               "",
                                                               null,
                                                               DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyGreenmageWardenOfTheForestStyle);

            var ignore_proximity_penalty = Helpers.FeatureBuilder<NewFeatureDefinitions.IgnorePhysicalRangeProximityPenaltyWithWeaponCategory>
                                                                        .createFeature("MonkSubclassWayOfZenArcheryIgnoreProximityPenalty",
                                                                                       "",
                                                                                       Common.common_no_title,
                                                                                       Common.common_no_title,
                                                                                       Common.common_no_icon,
                                                                                       a =>
                                                                                       {
                                                                                           a.weaponCategories = way_of_zen_archery_weapons;
                                                                                           a.only_for_close_range_attacks = true;
                                                                                       }
                                                                                       );

            var bow_proficiency = Helpers.ProficiencyBuilder.CreateWeaponProficiency("MonkSubclassWayOfZenArcheryWayOfTheBowProficiency",
                                                                                     "",
                                                                                     "",
                                                                                     "",
                                                                                     way_of_zen_archery_weapons.ToArray()
                                                                                     );
            way_of_the_bow = Helpers.FeatureSetBuilder.createFeatureSet("MonkSubclassWayOfZenArcheryWayOfTheBow",
                                                                               "",
                                                                               way_of_the_bow_title_string,
                                                                               way_of_the_bow_description_string,
                                                                               false,
                                                                               FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                               false,
                                                                               way_of_iron_allow_using_monk_features_in_armor,
                                                                               bow_proficiency,
                                                                               ignore_proximity_penalty,
                                                                               wis_on_weapons,
                                                                               damage_dice,
                                                                               archery,
                                                                               attacked_with_monk_weapon_watcher
                                                                               );
        }


        static void createKiArrows()
        {
            string ki_arrows_title_string = "Feature/&MonkSubclassWayOfZenArcheryKiArrowsTitle";
            string ki_arrows_description_string = "Feature/&MonkSubclassWayOfZenArcheryKiArrowsDescription";

            ki_arrows = Helpers.FeatureBuilder<NewFeatureDefinitions.AddAttackTagForSpecificWeaponType>.createFeature("MonkSubclassWayOfZenArcheryKiArrows",
                                                                                                                                "",
                                                                                                                                ki_arrows_title_string,
                                                                                                                                ki_arrows_description_string,
                                                                                                                                Common.common_no_icon,
                                                                                                                                a =>
                                                                                                                                {
                                                                                                                                    a.weaponTypes = new List<string>();
                                                                                                                                    a.weaponTypes.AddRange(way_of_zen_archery_weapons);
                                                                                                                                    a.tag = "Magical";
                                                                                                                                }
                                                                                                                                );
            stunning_strike.restrictions[0] = new NewFeatureDefinitions.AndRestriction(stunning_strike.restrictions[0],
                                                                                       //ki arrows allows to use stunning strike with ranged weapon
                                                                                       new NewFeatureDefinitions.OrRestriction(new NewFeatureDefinitions.HasFeatureRestriction(ki_arrows),
                                                                                                                               new NewFeatureDefinitions.SpecificWeaponInMainHandRestriction(way_of_zen_archery_weapons)),
                                                                                       //we should be able to use stunning strike if we receive bonus unarmed attack
                                                                                       new NewFeatureDefinitions.OrRestriction(new NewFeatureDefinitions.UsedAllMainAttacksRestriction(),
                                                                                                                               attacked_with_monk_weapon_restriction),
                                                                                       //we should be able to use stunning strike if we are under flurry of blows effect (again, because it will grant only unarmed attacks)
                                                                                       new NewFeatureDefinitions.HasConditionRestriction(flurry_of_blows_condition)
                                                                                       );
        }

        
        static void createFlurryOfArrows()
        {
            flurry_of_arrows = Helpers.CopyFeatureBuilder<FeatureDefinitionActionAffinity>.createFeatureCopy("MonkSubclassWayOfZenArcheryFlurryOfArrows",
                                                                                                             "",
                                                                                                             "",
                                                                                                             "",
                                                                                                             null,
                                                                                                             DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityRangerHunterVolley
                                                                                                             );
        }


        static CharacterSubclassDefinition createWayOfZenArchery()
        {
            createWayOfTheBow();
            createKiArrows();
            createFlurryOfArrows();

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&MonkSubclassWayOfZenArcheryDescription",
                    "Subclass/&MonkSubclassWayOfZenArcheryTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.RangerMarksman.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("MonkSubclassWayOfZenArchery", "f1b84dec-65ff-46d1-a874-d724d0564e1f")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(way_of_the_bow, 3)
                    .AddFeatureAtLevel(ki_arrows, 6)
                    .AddFeatureAtLevel(flurry_of_arrows, 11)
                    .AddToDB();

            return definition;
        }


        public static void BuildAndAddClassToDB()
        {
            var MonkClass = new MonkClassBuilder(MonkClassName, MonkClassNameGuid).AddToDB();
            MonkClass.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                                          {
                                              return a.Level - b.Level;
                                          }
                                         );
            MonkFeatureDefinitionSubclassChoice.Subclasses.Add(createWayOfIron().Name);
            MonkFeatureDefinitionSubclassChoice.Subclasses.Add(createWayOfTheOpenHand().Name);
            MonkFeatureDefinitionSubclassChoice.Subclasses.Add(createWayOfThePyrokine().Name);
            MonkFeatureDefinitionSubclassChoice.Subclasses.Add(createWayOfZenArchery().Name);
        }

        private static FeatureDefinitionSubclassChoice MonkFeatureDefinitionSubclassChoice;
    }
}
