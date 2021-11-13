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
using System;

namespace SolastaExtraContent
{
    internal class AlchemistClassBuilder : CharacterClassDefinitionBuilder
    {
        const string AlchemistClassName = "AlchemistClass";
        const string AlchemistClassNameGuid = "3ddeed7b-ddd0-4466-81b0-18c25f5db1d1";
        const string AlchemistClassSubclassesGuid = "09a01352-f4d6-4f78-9f7c-3a70659bdd03";

        static public CharacterClassDefinition alchemist_class;
        static public SpellListDefinition alchemist_spelllist;
        static public FeatureDefinitionCastSpell alchemist_spellcasting;
        static public FeatureDefinitionFeatureSet mutagen;
        static public FeatureDefinitionFeatureSet mutagen_selection;
        static public NewFeatureDefinitions.PowerBundle base_mutagen;
        static public List<FeatureDefinitionPower> mutagen_powers = new List<FeatureDefinitionPower>();
        static public FeatureDefinitionCraftingAffinity crafting_expertise;
        static public FeatureDefinitionCraftingAffinity crafting_adept;
        static public NewFeatureDefinitions.FeatureDefinitionAddAbilityBonusOnFailedSavePower flash_of_genius;
        //Archetypes: 
        //Vivisectionist
        static public FeatureDefinitionFeatureSet vivisectionist_proficiencies;
        static public FeatureDefinitionAdditionalDamage vivisectionist_sneak_attack;
        static public NewFeatureDefinitions.ReplaceWeaponAbilityScoreForRangedOrFinessableWeapons knowledge_of_anatomy;
        static public FeatureDefinitionAutoPreparedSpells vivisectionist_spells;
        static public FeatureDefinitionPower homunculus_destroy_self;
        static public FeatureDefinitionFeatureSet summon_homunculus;
        static public NewFeatureDefinitions.PowerWithRestrictions death_strike;
        //Grenadier
        static public FeatureDefinitionFeatureSet grenadier_proficiencies;
        static public FeatureDefinitionAutoPreparedSpells grenadier_spells;
        static public FeatureDefinitionFeatureSet bombs;
        static public FeatureDefinitionAdditionalDamage destruction_affinity;
        static public FeatureDefinitionFeatureSet powerful_bombs;
        //Runemaster
        static public FeatureDefinitionFeatureSet runemaster_proficiencies;
        static public FeatureDefinitionAutoPreparedSpells runemaster_spells;
        static public NewFeatureDefinitions.PowerBundle base_weapon_runes;
        static public NewFeatureDefinitions.PowerBundle base_armor_runes;
        static public FeatureDefinitionFeatureSet weapon_runes;
        static public FeatureDefinitionFeatureSet armor_runes;
        static public List<FeatureDefinitionPower> rune_powers = new List<FeatureDefinitionPower>();
        static public NewFeatureDefinitions.IncreaseNumberOfPowerUses rune_specialist;
        //Proficiency with all armor and weapons
        //Weapon Runes:
        //Focus Rune - use int for attack/damage and weapon serves as a casting focus
        //Destruction Rune - 1d6 force damage 1/round
        //Distraction Rune - on hit enemies get disadvantage against everyone except you for 1 round
        //Armor Runes:
        //Dampening Rune: advantage on stealth
        //Protection Rune: temporary hp proficiency bonus times
        //Propulsion Rune: + 10 feet speed bonus
        //Weapons use int and can be used as a focus
        //At lvl 9: +1 armor / weapon rune
        static public FeatureDefinitionFeatureSet fast_alchemy;

        protected AlchemistClassBuilder(string name, string guid) : base(name, guid)
        {
            var alchemist_class_image = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("AlchemistClassImage",
                                                                                           $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/AlchemistClass.png",
                                                                                           1024, 576);
            var ranger = DatabaseHelper.CharacterClassDefinitions.Ranger;
            alchemist_class = Definition;
            Definition.GuiPresentation.Title = "Class/&AlchemistClassTitle";
            Definition.GuiPresentation.Description = "Class/&AlchemistClassDescription";
            Definition.GuiPresentation.SetSpriteReference(alchemist_class_image);

            Definition.SetClassAnimationId(AnimationDefinitions.ClassAnimationId.Ranger);
            Definition.SetClassPictogramReference(ranger.ClassPictogramReference);
            Definition.SetDefaultBattleDecisions(ranger.DefaultBattleDecisions);
            Definition.SetHitDice(RuleDefinitions.DieType.D8);
            Definition.SetIngredientGatheringOdds(ranger.IngredientGatheringOdds);
            Definition.SetRequiresDeity(false);

            Definition.AbilityScoresPriority.Clear();
            Definition.AbilityScoresPriority.AddRange(new List<string> {Helpers.Stats.Intelligence,
                                                                        Helpers.Stats.Dexterity,
                                                                        Helpers.Stats.Constitution,
                                                                        Helpers.Stats.Wisdom,
                                                                        Helpers.Stats.Strength,
                                                                        Helpers.Stats.Charisma});

            Definition.FeatAutolearnPreference.AddRange(ranger.FeatAutolearnPreference);
            Definition.PersonalityFlagOccurences.AddRange(ranger.PersonalityFlagOccurences);

            Definition.SkillAutolearnPreference.Clear();
            Definition.SkillAutolearnPreference.AddRange(new List<string> { Helpers.Skills.Arcana,
                                                                            Helpers.Skills.History,
                                                                            Helpers.Skills.Investigation,
                                                                            Helpers.Skills.Nature,
                                                                            Helpers.Skills.Perception,
                                                                            Helpers.Skills.SleightOfHand,
                                                                            Helpers.Skills.Acrobatics,
                                                                            Helpers.Skills.Medicine});

            Definition.ToolAutolearnPreference.Clear();
            Definition.ToolAutolearnPreference.AddRange(new List<string> { Helpers.Tools.ThievesTool, Helpers.Tools.EnchantingTool, Helpers.Tools.SmithTool });


            Definition.EquipmentRows.AddRange(ranger.EquipmentRows);
            Definition.EquipmentRows.Clear();

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Rapier, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                                    }
            );
            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Rapier, EquipmentDefinitions.OptionWeaponSimpleChoice, 1),
                                    }
            );
            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.StuddedLeather, EquipmentDefinitions.OptionArmor, 1)
                                    },
                                new List<CharacterClassDefinition.HeroEquipmentOption>
                                    {
                                        EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ScaleMail, EquipmentDefinitions.OptionArmor, 1),
                                    }
            );

            this.AddEquipmentRow(new List<CharacterClassDefinition.HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ComponentPouch_Belt, EquipmentDefinitions.OptionFocus, 1),
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1)
            });

            var saving_throws = Helpers.ProficiencyBuilder.CreateSavingthrowProficiency("AlchemistSavingthrowProficiency",
                                                                                        "",
                                                                                        Helpers.Stats.Constitution, Helpers.Stats.Intelligence);

            var armor_proficiency = Helpers.ProficiencyBuilder.createCopy("AlchemistArmorProficiency",
                                                                          "",
                                                                          "Feature/&AlchemistArmorProficiencyTitle",
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyClericArmor
                                                                          );

            var weapon_proficiency = Helpers.ProficiencyBuilder.createCopy("AlchemistWeaponProficiency",
                                                                          "",
                                                                          "Feature/&AlchemistWeaponProficiencyTitle",
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyClericWeapon
                                                                          );

            var tools_proficiency = Helpers.ProficiencyBuilder.CreateToolsProficiency("AlchemistToolsProficiency",
                                                                                      "",
                                                                                      "Feature/&AlchemistToolsProficiencyTitle",
                                                                                      Helpers.Tools.HerbalismKit
                                                                                      );
            tools_proficiency.guiPresentation.hidden = true;
            var skills = Helpers.PoolBuilder.createSkillProficiency("AlchemistSkillProficiency",
                                                                    "",
                                                                    "Feature/&AlchemistClassSkillPointPoolTitle",
                                                                    "",
                                                                    2,
                                                                    Helpers.Skills.Arcana, Helpers.Skills.History, Helpers.Skills.Investigation, Helpers.Skills.Medicine,
                                                                    Helpers.Skills.Nature, Helpers.Skills.Perception, Helpers.Skills.SleightOfHand);

            var tools_proficiency2 = Helpers.PoolBuilder.createToolProficiency("AlchemistToolsProficiency2",
                                                                               "",
                                                                               "Feature/&AlchemistToolsProficiencyTitle",
                                                                               "Feature/&AlchemistToolsProficiencyDescription",
                                                                               1,
                                                                               Helpers.Tools.ThievesTool, Helpers.Tools.EnchantingTool, Helpers.Tools.SmithTool, Helpers.Tools.PoisonerKit);


            var ritual_spellcasting = Helpers.RitualSpellcastingBuilder.createRitualSpellcasting("AlchemistRitualSpellcasting",
                                                                                                 "2",
                                                                                                 "Feature/&AlchemistClassRitualCastingDescription",
                                                                                                 (RuleDefinitions.RitualCasting)ExtendedEnums.ExtraRitualCasting.Prepared);

            alchemist_spelllist = Helpers.SpelllistBuilder.create9LevelSpelllist("AlchemistClassSpelllist", "", "",
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.AcidSplash,
                                                                                    DatabaseHelper.SpellDefinitions.DancingLights,
                                                                                    DatabaseHelper.SpellDefinitions.Dazzle,
                                                                                    DatabaseHelper.SpellDefinitions.FireBolt,
                                                                                    DatabaseHelper.SpellDefinitions.Guidance,
                                                                                    DatabaseHelper.SpellDefinitions.Light,
                                                                                    DatabaseHelper.SpellDefinitions.PoisonSpray,
                                                                                    DatabaseHelper.SpellDefinitions.RayOfFrost,
                                                                                    DatabaseHelper.SpellDefinitions.Resistance,
                                                                                    DatabaseHelper.SpellDefinitions.ShockingGrasp,
                                                                                    DatabaseHelper.SpellDefinitions.SpareTheDying,
                                                                                    Cantrips.frostbite,
                                                                                    Cantrips.sunlight_blade,
                                                                                    Cantrips.thunder_strike
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.CureWounds, //
                                                                                    DatabaseHelper.SpellDefinitions.DetectMagic,
                                                                                    DatabaseHelper.SpellDefinitions.ExpeditiousRetreat,
                                                                                    DatabaseHelper.SpellDefinitions.FaerieFire,
                                                                                    DatabaseHelper.SpellDefinitions.FalseLife, //
                                                                                    DatabaseHelper.SpellDefinitions.FeatherFall,
                                                                                    DatabaseHelper.SpellDefinitions.Grease,
                                                                                    DatabaseHelper.SpellDefinitions.Identify,
                                                                                    DatabaseHelper.SpellDefinitions.Jump, //
                                                                                    DatabaseHelper.SpellDefinitions.Longstrider, //
                                                                                    //
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.Aid, //
                                                                                    DatabaseHelper.SpellDefinitions.Blur, //?
                                                                                    DatabaseHelper.SpellDefinitions.Darkvision, //
                                                                                    DatabaseHelper.SpellDefinitions.EnhanceAbility, //?
                                                                                    DatabaseHelper.SpellDefinitions.HeatMetal,
                                                                                    DatabaseHelper.SpellDefinitions.Invisibility, //?
                                                                                    DatabaseHelper.SpellDefinitions.Levitate, //?
                                                                                    DatabaseHelper.SpellDefinitions.LesserRestoration, //
                                                                                    DatabaseHelper.SpellDefinitions.MagicWeapon,
                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison, //
                                                                                    DatabaseHelper.SpellDefinitions.SeeInvisibility, //
                                                                                    DatabaseHelper.SpellDefinitions.SpiderClimb //?
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.CreateFood,
                                                                                    DatabaseHelper.SpellDefinitions.DispelMagic,
                                                                                    DatabaseHelper.SpellDefinitions.Fly,
                                                                                    DatabaseHelper.SpellDefinitions.Haste,
                                                                                    DatabaseHelper.SpellDefinitions.ProtectionFromEnergy,
                                                                                    DatabaseHelper.SpellDefinitions.Revivify
                                                                                    //elemental weapon
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.FreedomOfMovement,
                                                                                    DatabaseHelper.SpellDefinitions.Stoneskin
                                                                                },
                                                                                new List<SpellDefinition>
                                                                                {
                                                                                    DatabaseHelper.SpellDefinitions.GreaterRestoration
                                                                                }
                                                                                );

            alchemist_spellcasting = Helpers.SpellcastingBuilder.createDivinePreparedSpellcasting("AlchemistClassSpellcasting",
                                                                                                    "",
                                                                                                    "Feature/&AlchemistClassSpellcastingTitle",
                                                                                                    "Feature/&AlchemistClassSpellcastingDescription",
                                                                                                    alchemist_spelllist,
                                                                                                    Helpers.Stats.Intelligence,
                                                                                                    new List<int> { 2,  2,  2,  2,  2,  2, 2, 2, 2, 3,
                                                                                                                    3,  3,  3,  4,  4,  4, 4, 4, 4, 4},
                                                                                                    RuleDefinitions.SpellPreparationCount.AbilityBonusPlusHalfLevel,
                                                                                                    Helpers.Misc.createSpellSlotsByLevel(new List<int> { 2, 0, 0, 0, 0 },//1
                                                                                                                                        new List<int> { 2, 0, 0, 0, 0 },//2
                                                                                                                                        new List<int> { 3, 0, 0, 0, 0 },//3
                                                                                                                                        new List<int> { 3, 0, 0, 0, 0 },//4
                                                                                                                                        new List<int> { 4, 2, 0, 0, 0 },//5
                                                                                                                                        new List<int> { 4, 2, 0, 0, 0 },//6
                                                                                                                                        new List<int> { 4, 3, 0, 0, 0 },//7
                                                                                                                                        new List<int> { 4, 3, 0, 0, 0 },//8
                                                                                                                                        new List<int> { 4, 3, 2, 0, 0 },//9
                                                                                                                                        new List<int> { 4, 3, 2, 0, 0 },//10
                                                                                                                                        new List<int> { 4, 3, 3, 0, 0 },//11
                                                                                                                                        new List<int> { 4, 3, 3, 0, 0 },//12
                                                                                                                                        new List<int> { 4, 3, 3, 1, 0 },//13
                                                                                                                                        new List<int> { 4, 3, 3, 1, 0 },//14
                                                                                                                                        new List<int> { 4, 3, 3, 2, 0 },//15
                                                                                                                                        new List<int> { 4, 3, 3, 2, 0 },//16
                                                                                                                                        new List<int> { 4, 3, 3, 3, 1 },//17
                                                                                                                                        new List<int> { 4, 3, 3, 3, 1 },//18
                                                                                                                                        new List<int> { 4, 3, 3, 3, 2 },//19
                                                                                                                                        new List<int> { 4, 3, 3, 3, 2 }//20
                                                                                                                                        )
                                                                                                    );
            alchemist_spellcasting.focusType = EquipmentDefinitions.FocusType.Arcane;
            createMutagen();
            createCraftingExpertise();
            createCraftingAdept();
            createFlashOfGenius();
            createFastAlchemy();
            Definition.FeatureUnlocks.Clear();
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(saving_throws, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(armor_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(weapon_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(skills, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(tools_proficiency, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(tools_proficiency2, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(alchemist_spellcasting, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(ritual_spellcasting, 1));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen, 2));//3
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 4)); //4
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 6)); //5
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(crafting_expertise, 6));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(flash_of_genius , 7));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 8)); //6
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 10)); //7
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(crafting_adept, 10));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(fast_alchemy, 11));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 12)); //8
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 14)); //9
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 16)); //10
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 18)); //11
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19));
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(mutagen_selection, 20)); //12

            var subclassChoicesGuiPresentation = new GuiPresentation();
            subclassChoicesGuiPresentation.Title = "Subclass/&AlchemistSubclassSpecializationTitle";
            subclassChoicesGuiPresentation.Description = "Subclass/&AlchemistSubclassSpecializationDescription";
            AlchemistFeatureDefinitionSubclassChoice = this.BuildSubclassChoice(3, "Specialization", false, "SubclassChoiceAlchemistSpecialistArchetypes", subclassChoicesGuiPresentation, AlchemistClassSubclassesGuid);

            var itemlist = new List<ItemDefinition>
            {
                DatabaseHelper.ItemDefinitions.WandOfLightningBolts,
                //DatabaseHelper.ItemDefinitions.StaffOfMetis,              // devs removed class restrictions for HF 1.1.11 so not needed now
                DatabaseHelper.ItemDefinitions.StaffOfHealing,
                DatabaseHelper.ItemDefinitions.ArcaneShieldstaff
            };

            foreach (ItemDefinition item in itemlist)
            {
                item.RequiredAttunementClasses.Add(alchemist_class);
            };
        }


        static void createFastAlchemy()
        {
            string potion_image_path = $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/SwiftAlchemy.png";
            var base_image = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("SwiftAlchemyBaseImage",
                                                                                  $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/SwiftAlchemyBase.png",
                                                                                  128, 64);
            var base_eff = new EffectDescription();
            base_eff.Copy(DatabaseHelper.SpellDefinitions.CureWounds.EffectDescription);
            base_eff.effectForms.Clear();
            base_eff.targetType = RuleDefinitions.TargetType.Self;
            base_eff.rangeType = RuleDefinitions.RangeType.Self;
            base_eff.durationType = RuleDefinitions.DurationType.UntilLongRest;

            var base_fast_alchemy = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerBundle>
                                       .createPower("AlchemistClassFastAlchemyBasePower",
                                                       "",
                                                       "Feature/&AlchemistClassFastAlchemyFeatureSetTitle",
                                                       "Feature/&AlchemistClassFastAlchemyFeatureSetDescription",
                                                       base_image,
                                                       base_eff,
                                                       RuleDefinitions.ActivationTime.Action,
                                                       4,
                                                       RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                       RuleDefinitions.RechargeRate.LongRest,
                                                       Helpers.Stats.Intelligence,
                                                       Helpers.Stats.Intelligence,
                                                       1,
                                                       true
                                                       );

            fast_alchemy = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistClassFastAlchemyFeatureSet",
                                                                        "",
                                                                        "Feature/&AlchemistClassFastAlchemyFeatureSetTitle",
                                                                        "Feature/&AlchemistClassFastAlchemyFeatureSetDescription",
                                                                        false,
                                                                        FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                        false,
                                                                        base_fast_alchemy
                                                                        );

            var spells = new List<SpellDefinition>
            {
                DatabaseHelper.SpellDefinitions.CureWounds,
                DatabaseHelper.SpellDefinitions.FalseLife,
                DatabaseHelper.SpellDefinitions.Jump,
                DatabaseHelper.SpellDefinitions.Longstrider,
                DatabaseHelper.SpellDefinitions.Darkvision,
                DatabaseHelper.SpellDefinitions.LesserRestoration,
                DatabaseHelper.SpellDefinitions.ProtectionFromPoison,
                DatabaseHelper.SpellDefinitions.SeeInvisibility,
            };

            foreach (var s in spells)
            {
                var potion_effect = new EffectDescription();
                potion_effect.Copy(s.effectDescription);
                potion_effect.rangeType = RuleDefinitions.RangeType.Self;
                potion_effect.targetType = RuleDefinitions.TargetType.Self;
                potion_effect.rangeParameter = 1;
                potion_effect.targetParameter = 1;
                potion_effect.effectAdvancement.Clear();
                if (s == DatabaseHelper.SpellDefinitions.CureWounds)
                {
                    potion_effect.effectForms.Clear();
                    var eff = new EffectForm();
                    eff.formType = EffectForm.EffectFormType.Healing;
                    eff.createdByCharacter = true;
                    eff.healingForm = new HealingForm();
                    eff.healingForm.bonusHealing = 5;
                    eff.healingForm.diceNumber = 2;
                    eff.healingForm.dieType = RuleDefinitions.DieType.D8;
                    potion_effect.effectForms.Add(eff);
                }
                else if (s == DatabaseHelper.SpellDefinitions.FalseLife)
                {
                    potion_effect.effectForms.Clear();
                    var eff = new EffectForm();
                    eff.formType = EffectForm.EffectFormType.TemporaryHitPoints;
                    eff.createdByCharacter = true;
                    eff.temporaryHitPointsForm = new TemporaryHitPointsForm();
                    eff.temporaryHitPointsForm.bonusHitPoints = 9;
                    eff.temporaryHitPointsForm.diceNumber = 1;
                    eff.temporaryHitPointsForm.dieType = RuleDefinitions.DieType.D4;
                    potion_effect.effectForms.Add(eff);
                }

                
                //var power_output_path = $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/SwiftAlchemyPower{s.name}.png";
                var potion_output_path = $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/SwiftAlchemyPotion{s.name}.png";
                if (!System.IO.File.Exists(potion_output_path))
                {
                    var spell_icon_path = $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/{s.name}_For_Swift_Alchemy.png";
                    SolastaModHelpers.CustomIcons.Tools.saveSpriteFromAssetReferenceAsPNG(s.GuiPresentation.SpriteReference, spell_icon_path);
                    //SolastaModHelpers.CustomIcons.Tools.combineImages(new string[] { potion_image_path, spell_icon_path }, (128, 64), power_output_path);
                    SolastaModHelpers.CustomIcons.Tools.merge2Images(potion_image_path, spell_icon_path, (40, 40), (40, 70), potion_output_path);
                    //System.IO.File.Delete(spell_icon_path);
                }
                /*var power_image = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon($"SwiftAlchemyPower{s.name}Image",
                                                                                      power_output_path,
                                                                                      128, 64);*/

                var potion_image = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon($"SwiftAlchemyPotion{s.name}Image",
                                                                                      potion_output_path,
                                                                                      128, 128);

                var potion_power = Helpers.GenericPowerBuilder<FeatureDefinitionPower>.createPower("FastAlchemyPotionFunctionPower" + s.name,
                                                                                            GuidStorage.mergeGuids(s.guid, "edcb2a48-70ea-4818-bfa2-40c9e7527a90"),
                                                                                            s.guiPresentation.title,
                                                                                            s.guiPresentation.description,
                                                                                            s.guiPresentation.spriteReference,
                                                                                            potion_effect,
                                                                                            RuleDefinitions.ActivationTime.Action,
                                                                                            1,
                                                                                            RuleDefinitions.UsesDetermination.Fixed,
                                                                                            RuleDefinitions.RechargeRate.None,
                                                                                            show_casting: false);

                var item = Helpers.CopyFeatureBuilder<ItemDefinition>.createFeatureCopy("FastAlchemyPotion" + s.name,
                                                                                        GuidStorage.mergeGuids(s.guid, "8aa17aa7-6832-4412-a55e-f1187ad9908c"),
                                                                                        Helpers.StringProcessing.concatenateStrings($"Feature/&AlchemistClassFastAlchemyPotion{s.name}Title",
                                                                                                                                    ("Feature/&AlchemistClassFastAlchemyPotionTitle", " "),
                                                                                                                                    (s.guiPresentation.title, "")),                                                                                                       
                                                                                        s.guiPresentation.description,
                                                                                        potion_image,
                                                                                        DatabaseHelper.ItemDefinitions.PotionOfInvisibility,
                                                                                        a =>
                                                                                        {
                                                                                            a.costs = new int[] { 0, 0, 0, 0, 0 };
                                                                                            a.usableDeviceDescription.deviceFunctions = new List<DeviceFunctionDescription>
                                                                                            {
                                                                                                new DeviceFunctionDescription(a.usableDeviceDescription.deviceFunctions[0])
                                                                                                {
                                                                                                    type = DeviceFunctionDescription.FunctionType.Power,
                                                                                                    useAmount = 1,
                                                                                                    featureDefinitionPower = potion_power
                                                                                                }
                                                                                            };
                                                                                            a.requiresIdentification = false;
                                                                                        }
                                                                                        );
                NewFeatureDefinitions.ItemsData.items_to_remove_on_long_rest.Add(item);

                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.Goodberry.effectDescription);
                effect.effectForms.Clear();
                var effect_form = new EffectForm();
                effect_form.formType = EffectForm.EffectFormType.Summon;
                effect_form.createdByCharacter = true;
                effect_form.summonForm = new SummonForm();
                effect_form.summonForm.summonType = SummonForm.Type.InventoryItem;
                effect_form.summonForm.itemDefinition = item;
                effect_form.summonForm.monsterDefinitionName = "Acolyte";
                effect_form.summonForm.effectProxyDefinitionName = "";
                effect_form.summonForm.number = 1;
                effect.effectForms.Add(effect_form);
                effect.effectParticleParameters = DatabaseHelper.FeatureDefinitionPowers.PowerFunctionRemedyOther.effectDescription.effectParticleParameters;
                var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>.createPower("FastAlchemyFunctionCreatePotion" + s.name,
                                                                            GuidStorage.mergeGuids(s.guid, "6e7865ec-5323-4065-968e-689ffd1bf504"),
                                                                            Helpers.StringProcessing.concatenateStrings($"Feature/&AlchemistClassFastAlchemyCreatePotion{s.name}Title",
                                                                                                                                    ("Feature/&AlchemistClassFastAlchemyFeatureSetTitle", ": "),
                                                                                                                                    (s.guiPresentation.title, "")),
                                                                            s.guiPresentation.description,
                                                                            s.GuiPresentation.spriteReference,
                                                                            effect,
                                                                            RuleDefinitions.ActivationTime.Action,
                                                                            4,
                                                                            RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                            RuleDefinitions.RechargeRate.LongRest,
                                                                            Helpers.Stats.Intelligence,
                                                                            Helpers.Stats.Intelligence);
                base_fast_alchemy.addSubPower(power);
                power.linkedPower = base_fast_alchemy;
                fast_alchemy.featureSet.Add(power);
                power.showCasting = false;
            }
        }


        static void createFlashOfGenius()
        {
            var eff = new EffectDescription();
            eff.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.effectDescription);
            eff.effectForms.Clear();
            eff.durationType = RuleDefinitions.DurationType.Instantaneous;
            eff.targetSide = RuleDefinitions.Side.Ally;
            eff.rangeType = RuleDefinitions.RangeType.Distance;
            eff.SetTargetType(RuleDefinitions.TargetType.Individuals);
            eff.rangeParameter = 6;

            flash_of_genius = Helpers.GenericPowerBuilder<NewFeatureDefinitions.FeatureDefinitionAddAbilityBonusOnFailedSavePower>
                                                                .createPower("AlchemistClassFlashOfGenius",
                                                                             "",
                                                                             "Feature/&AlchemistClassFlashOfGeniusTitle",
                                                                             "Feature/&AlchemistClassFlashOfGeniusDescription",
                                                                             DatabaseHelper.FeatureDefinitionPowers.PowerMountaineerCloseQuarters.guiPresentation.SpriteReference,
                                                                             eff,
                                                                             RuleDefinitions.ActivationTime.Reaction,
                                                                             0,
                                                                             RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                             RuleDefinitions.RechargeRate.LongRest,
                                                                             uses_ability: Helpers.Stats.Intelligence
                                                                             );
            flash_of_genius.ability = Helpers.Stats.Intelligence;


            Helpers.StringProcessing.addStringCopy("Feature/&AlchemistClassFlashOfGeniusTitle",
                                                  $"Reaction/&ConsumePowerUse{flash_of_genius.name}Title");
            Helpers.StringProcessing.addStringCopy("Reaction/&CommonUsePowerReactTitle",
                                                   $"Reaction/&ConsumePowerUse{flash_of_genius.name}ReactTitle");
        }


        static void createCraftingAdept()
        {
            var tools = new List<ToolTypeDefinition>
            {
                DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType,
                DatabaseHelper.ToolTypeDefinitions.HerbalismKitType,
                DatabaseHelper.ToolTypeDefinitions.EnchantingToolType,
                DatabaseHelper.ToolTypeDefinitions.PoisonersKitType
            };
            crafting_adept = Helpers.CopyFeatureBuilder<FeatureDefinitionCraftingAffinity>
                                                                                    .createFeatureCopy("AlchemistClassCraftingAdept",
                                                                                                       "",
                                                                                                       "Feature/&AlchemistClassCraftingAdeptTitle",
                                                                                                       "Feature/&AlchemistClassCraftingAdeptDescription",
                                                                                                       Common.common_no_icon,
                                                                                                       DatabaseHelper.FeatureDefinitionCraftingAffinitys.CraftingAffinityFeatMasterAlchemist,
                                                                                                       a =>
                                                                                                       {
                                                                                                           a.affinityGroups = tools.Select(t =>
                                                                                                                                           new FeatureDefinitionCraftingAffinity.CraftingAffinityGroup()
                                                                                                                                           {
                                                                                                                                               tooltype = t,
                                                                                                                                               doubleProficiencyBonus = false,
                                                                                                                                               durationMultiplier = 0.5f
                                                                                                                                           }).ToList();
                                                                                                       }
                                                                                                       );
        }


        static void createCraftingExpertise()
        {
            var tools = new List<ToolTypeDefinition>
            {
                DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType,
                DatabaseHelper.ToolTypeDefinitions.HerbalismKitType,
                DatabaseHelper.ToolTypeDefinitions.EnchantingToolType,
                DatabaseHelper.ToolTypeDefinitions.PoisonersKitType
            };
            crafting_expertise = Helpers.CopyFeatureBuilder<FeatureDefinitionCraftingAffinity>
                                                                                    .createFeatureCopy("AlchemistClassCraftingExpertise",
                                                                                                       "",
                                                                                                       "Feature/&AlchemistClassCraftingExpertiseTitle",
                                                                                                       "Feature/&AlchemistClassCraftingExpertiseDescription",
                                                                                                       Common.common_no_icon,
                                                                                                       DatabaseHelper.FeatureDefinitionCraftingAffinitys.CraftingAffinityFeatMasterAlchemist,
                                                                                                       a =>
                                                                                                       {
                                                                                                           a.affinityGroups = tools.Select(t => 
                                                                                                                                           new FeatureDefinitionCraftingAffinity.CraftingAffinityGroup()
                                                                                                                                           {
                                                                                                                                               tooltype = t,
                                                                                                                                               doubleProficiencyBonus = true,
                                                                                                                                               durationMultiplier = 1.0f
                                                                                                                                           }).ToList();
                                                                                                       }
                                                                                                       );
        }


        static void createMutagen()
        {
            var base_image = SolastaModHelpers.CustomIcons.Tools.storeCustomIcon("MutagenBaseImage",
                                                                      $@"{UnityModManagerNet.UnityModManager.modsPath}/SolastaExtraContent/Sprites/Mutagen.png",
                                                                      128, 64);
            var base_eff = new EffectDescription();
            base_eff.Copy(DatabaseHelper.SpellDefinitions.CureWounds.EffectDescription);
            base_eff.effectForms.Clear();
            base_eff.durationType = RuleDefinitions.DurationType.UntilLongRest;


            base_mutagen = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerBundle>
                                                .createPower("AlchemistClassBaseMutagenPower",
                                                             "",
                                                             "Feature/&AlchemistClassMutagenFeatureSetTitle",
                                                             "Feature/&AlchemistClassMutagenFeatureSetDescription",
                                                             base_image,
                                                             base_eff,
                                                             RuleDefinitions.ActivationTime.Action,
                                                             2,
                                                             RuleDefinitions.UsesDetermination.Fixed,
                                                             RuleDefinitions.RechargeRate.LongRest,
                                                             Helpers.Stats.Intelligence,
                                                             Helpers.Stats.Intelligence,
                                                             1,
                                                             true
                                                             );
            mutagen_powers.Add(base_mutagen);
            var strength_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AlchemistClassMutagenStrenghtBonus",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    null,
                                                                                                                    DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierDwarfAbilityScoreIncrease,
                                                                                                                    a =>
                                                                                                                    {
                                                                                                                        a.modifiedAttribute = Helpers.Stats.Strength;
                                                                                                                    }
                                                                                                                    );
            var intelligence_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AlchemistClassMutagenIntelligenceBonus",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    "",
                                                                                                                    null,
                                                                                                                    DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierDwarfAbilityScoreIncrease,
                                                                                                                    a =>
                                                                                                                    {
                                                                                                                        a.modifiedAttribute = Helpers.Stats.Intelligence;
                                                                                                                    }
                                                                                                                    );
            var wisdom_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AlchemistClassMutagenWisdomeBonus",
                                                                                                                        "",
                                                                                                                        "",
                                                                                                                        "",
                                                                                                                        null,
                                                                                                                        DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierDwarfAbilityScoreIncrease,
                                                                                                                        a =>
                                                                                                                        {
                                                                                                                            a.modifiedAttribute = Helpers.Stats.Wisdom;
                                                                                                                        }
                                                                                                                        );

            var str_mutagen = createMutagenPower("AlchemistClassMutagenStrength",
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityBullsStrength.EffectDescription.EffectParticleParameters,
                                                  DatabaseHelper.ConditionDefinitions.ConditionBullsStrength.guiPresentation.spriteReference,
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityBullsStrength.GuiPresentation.spriteReference,
                                                  strength_feature
                                                  );
            var dex_mutagen = createMutagenPower("AlchemistClassMutagenDexterity",
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityCatsGrace.EffectDescription.EffectParticleParameters,
                                                  DatabaseHelper.ConditionDefinitions.ConditionCatsGrace.guiPresentation.spriteReference,
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityCatsGrace.GuiPresentation.spriteReference,
                                                  DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalflingAbilityScoreIncrease
                                                  );
            var con_mutagen = createMutagenPower("AlchemistClassMutagenConstitution",
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityBearsEndurance.EffectDescription.EffectParticleParameters,
                                                  DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance.guiPresentation.spriteReference,
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityBearsEndurance.GuiPresentation.spriteReference,
                                                  DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierDwarfAbilityScoreIncrease
                                                  );
            var int_mutagen = createMutagenPower("AlchemistClassMutagenIntelligence",
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityFoxsCunning.EffectDescription.EffectParticleParameters,
                                                  DatabaseHelper.ConditionDefinitions.ConditionFoxsCunning.guiPresentation.spriteReference,
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityFoxsCunning.GuiPresentation.spriteReference,
                                                  intelligence_feature
                                                  );
            var wis_mutagen = createMutagenPower("AlchemistClassMutagenWisdom",
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityOwlsWisdom.EffectDescription.EffectParticleParameters,
                                                  DatabaseHelper.ConditionDefinitions.ConditionOwlsWisdom.guiPresentation.spriteReference,
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityOwlsWisdom.GuiPresentation.spriteReference,
                                                  wisdom_feature
                                                  );
            var cha_mutagen = createMutagenPower("AlchemistClassMutagenCharisma",
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityEaglesSplendor.EffectDescription.EffectParticleParameters,
                                                  DatabaseHelper.ConditionDefinitions.ConditionEaglesSplendor.guiPresentation.spriteReference,
                                                  DatabaseHelper.SpellDefinitions.EnhanceAbilityEaglesSplendor.GuiPresentation.spriteReference,
                                                  DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierHalfElfAbilityScoreIncreaseCha
                                                  );

            var fire_resistance_mutagen = createMutagenPower("AlchemistClassMutagenFireResistance",
                                                  DatabaseHelper.SpellDefinitions.ProtectionFromEnergyFire.EffectDescription.EffectParticleParameters,
                                                  DatabaseHelper.ConditionDefinitions.ConditionProtectedFromEnergyFire.guiPresentation.spriteReference,
                                                  DatabaseHelper.SpellDefinitions.ProtectionFromEnergyFire.GuiPresentation.spriteReference,
                                                  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityFireResistance
                                                  );
            var acid_resistance_mutagen = createMutagenPower("AlchemistClassMutagenAcidResistance",
                                                              DatabaseHelper.SpellDefinitions.ProtectionFromEnergyAcid.EffectDescription.EffectParticleParameters,
                                                              DatabaseHelper.ConditionDefinitions.ConditionProtectedFromEnergyAcid.guiPresentation.spriteReference,
                                                              DatabaseHelper.SpellDefinitions.ProtectionFromEnergyAcid.GuiPresentation.spriteReference,
                                                              DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityAcidResistance
                                                              );
            var cold_resistance_mutagen = createMutagenPower("AlchemistClassMutagenColdResistance",
                                                              DatabaseHelper.SpellDefinitions.ProtectionFromEnergyCold.EffectDescription.EffectParticleParameters,
                                                              DatabaseHelper.ConditionDefinitions.ConditionProtectedFromEnergyCold.guiPresentation.spriteReference,
                                                              DatabaseHelper.SpellDefinitions.ProtectionFromEnergyCold.GuiPresentation.spriteReference,
                                                              DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance
                                                              );
            var lightning_resistance_mutagen = createMutagenPower("AlchemistClassMutagenLightningResistance",
                                                              DatabaseHelper.SpellDefinitions.ProtectionFromEnergyLightning.EffectDescription.EffectParticleParameters,
                                                              DatabaseHelper.ConditionDefinitions.ConditionProtectedFromEnergyLightning.guiPresentation.spriteReference,
                                                              DatabaseHelper.SpellDefinitions.ProtectionFromEnergyLightning.GuiPresentation.spriteReference,
                                                              DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityLightningResistance
                                                              );

            var necrotic_resistance_mutagen = createMutagenPower("AlchemistClassMutagenNecroticResistance",
                                                                  DatabaseHelper.SpellDefinitions.DeathWard.EffectDescription.EffectParticleParameters,
                                                                  DatabaseHelper.ConditionDefinitions.ConditionDeathWarded.guiPresentation.spriteReference,
                                                                  DatabaseHelper.SpellDefinitions.DeathWard.GuiPresentation.spriteReference,
                                                                  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance
                                                                  );

            var slashing_resistance_mutagen = createMutagenPower("AlchemistClassMutagenSlashingResistance",
                                                                  DatabaseHelper.SpellDefinitions.Stoneskin.EffectDescription.EffectParticleParameters,
                                                                  DatabaseHelper.ConditionDefinitions.ConditionStoneskin.guiPresentation.spriteReference,
                                                                  DatabaseHelper.SpellDefinitions.Stoneskin.GuiPresentation.spriteReference,
                                                                  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinitySlashingResistance
                                                                  );

            var piercing_resistance_mutagen = createMutagenPower("AlchemistClassMutagenPiercingResistance",
                                                                  DatabaseHelper.SpellDefinitions.Stoneskin.EffectDescription.EffectParticleParameters,
                                                                  DatabaseHelper.ConditionDefinitions.ConditionStoneskin.guiPresentation.spriteReference,
                                                                  DatabaseHelper.SpellDefinitions.Stoneskin.GuiPresentation.spriteReference,
                                                                  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPiercingResistance
                                                                  );

            var bludgeoning_resistance_mutagen = createMutagenPower("AlchemistClassMutagenBludgeoningResistance",
                                                                  DatabaseHelper.SpellDefinitions.Stoneskin.EffectDescription.EffectParticleParameters,
                                                                  DatabaseHelper.ConditionDefinitions.ConditionStoneskin.guiPresentation.spriteReference,
                                                                  DatabaseHelper.SpellDefinitions.Stoneskin.GuiPresentation.spriteReference,
                                                                  DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityBludgeoningResistance
                                                                  );

            var darkvision_mutagen = createMutagenPower("AlchemistClassMutagenDarkvision",
                                                            DatabaseHelper.SpellDefinitions.Darkvision.EffectDescription.EffectParticleParameters,
                                                            DatabaseHelper.ConditionDefinitions.ConditionDarkvision.guiPresentation.spriteReference,
                                                            DatabaseHelper.SpellDefinitions.Darkvision.GuiPresentation.spriteReference,
                                                            DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision
                                                            );

            var initiative_mutagen = createMutagenPower("AlchemistClassMutagenInitiative",
                                                        DatabaseHelper.SpellDefinitions.Haste.EffectDescription.EffectParticleParameters,
                                                        DatabaseHelper.ConditionDefinitions.ConditionHasted.guiPresentation.spriteReference,
                                                        DatabaseHelper.SpellDefinitions.Haste.GuiPresentation.spriteReference,
                                                        DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle
                                                        );

            var speed_mutagen = createMutagenPower("AlchemistClassMutagenSpeed",
                                                    DatabaseHelper.SpellDefinitions.Longstrider.EffectDescription.EffectParticleParameters,
                                                    DatabaseHelper.ConditionDefinitions.ConditionLongstrider.guiPresentation.spriteReference,
                                                    DatabaseHelper.SpellDefinitions.ExpeditiousRetreat.GuiPresentation.spriteReference,
                                                    DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider,
                                                    DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityJump
                                                    );


            var poison_immunity_mutagen = createMutagenPower("AlchemistClassMutagenPoisonImmunity",
                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison.EffectDescription.EffectParticleParameters,
                                                    DatabaseHelper.ConditionDefinitions.ConditionProtectedFromPoison.guiPresentation.spriteReference,
                                                    DatabaseHelper.SpellDefinitions.ProtectionFromPoison.GuiPresentation.spriteReference,
                                                    DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                                                    DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity
                                                    );

            var spider_climb = createMutagenPower("AlchemistClassMutagenSpiderClimb",
                                                    DatabaseHelper.SpellDefinitions.SpiderClimb.EffectDescription.EffectParticleParameters,
                                                    DatabaseHelper.ConditionDefinitions.ConditionSpiderClimb.guiPresentation.spriteReference,
                                                    DatabaseHelper.SpellDefinitions.SpiderClimb.GuiPresentation.spriteReference,
                                                    DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinitySpiderClimb
                                                    );

            var critical_hit_immunity = createMutagenPower("AlchemistClassMutagenCriticalHitImmunity",
                                                    DatabaseHelper.SpellDefinitions.Barkskin.EffectDescription.EffectParticleParameters,
                                                    DatabaseHelper.ConditionDefinitions.ConditionBarkskin.guiPresentation.spriteReference,
                                                    DatabaseHelper.SpellDefinitions.Barkskin.GuiPresentation.spriteReference,
                                                    DatabaseHelper.FeatureDefinitionCombatAffinitys.CombatAffinityAdamantinePlateArmor
                                                    );

            NewFeatureDefinitions.FeatureData.addFeatureRestrictions(darkvision_mutagen, new NewFeatureDefinitions.MinClassLevelRestriction(alchemist_class, 6));
            NewFeatureDefinitions.FeatureData.addFeatureRestrictions(initiative_mutagen, new NewFeatureDefinitions.MinClassLevelRestriction(alchemist_class, 6));
            NewFeatureDefinitions.FeatureData.addFeatureRestrictions(poison_immunity_mutagen, new NewFeatureDefinitions.MinClassLevelRestriction(alchemist_class, 10));
            NewFeatureDefinitions.FeatureData.addFeatureRestrictions(spider_climb, new NewFeatureDefinitions.MinClassLevelRestriction(alchemist_class, 10));
            NewFeatureDefinitions.FeatureData.addFeatureRestrictions(critical_hit_immunity, new NewFeatureDefinitions.MinClassLevelRestriction(alchemist_class, 10));


            var extra_mutagens = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseNumberOfPowerUsesPerClassLevel>.createFeature("AlchemistClassMutagenExtraUses",
                                                                                                        "",
                                                                                                        Common.common_no_title,
                                                                                                        Common.common_no_title,
                                                                                                        Common.common_no_icon,
                                                                                                        a =>
                                                                                                        {
                                                                                                            a.powers = mutagen_powers;
                                                                                                            a.characterClass = alchemist_class;
                                                                                                            a.levelIncreaseList = new List<(int, int)>();
                                                                                                            for (int i = 6; i <= 20; i+= 4)
                                                                                                            {
                                                                                                                a.levelIncreaseList.Add((i, 1));
                                                                                                            }
                                                                                                        }
                                                                                                       );

            mutagen = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistClassMutagenFeatureSet",
                                                                "",
                                                                "Feature/&AlchemistClassMutagenFeatureSetTitle",
                                                                "Feature/&AlchemistClassMutagenFeatureSetDescription",
                                                                false,
                                                                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                false,
                                                                extra_mutagens,
                                                                base_mutagen,
                                                                str_mutagen,
                                                                dex_mutagen,
                                                                con_mutagen
                                                                );
            
            mutagen_selection = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistClassMutagenSelectionFeatureSet",
                                                                            "",
                                                                            "Feature/&AlchemistClassMutagenSelectionFeatureSetTitle",
                                                                            "Feature/&AlchemistClassMutagenSelectionFeatureSetDescription",
                                                                            false,
                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion,
                                                                            true,
                                                                            int_mutagen,
                                                                            wis_mutagen,
                                                                            cha_mutagen,
                                                                            fire_resistance_mutagen,
                                                                            acid_resistance_mutagen,
                                                                            lightning_resistance_mutagen,
                                                                            cold_resistance_mutagen,
                                                                            necrotic_resistance_mutagen,
                                                                            slashing_resistance_mutagen,
                                                                            piercing_resistance_mutagen,
                                                                            bludgeoning_resistance_mutagen,
                                                                            darkvision_mutagen,
                                                                            initiative_mutagen,
                                                                            speed_mutagen,
                                                                            poison_immunity_mutagen,
                                                                            spider_climb,
                                                                            critical_hit_immunity
                                                                            );
        }


        static FeatureDefinitionPower createMutagenPower(string name, EffectParticleParameters effect_particle_parameters, 
                                                    AssetReferenceSprite sprite_condition, AssetReferenceSprite sprite_power, params FeatureDefinition[] features)
        {
            var title_string = "Feature/&" + name + "Title";
            var description_string = "Feature/&" + name + "Description";

            var condition = Helpers.ConditionBuilder.createCondition(name + "Condition",
                                                                     "",
                                                                     title_string,
                                                                     description_string,
                                                                     sprite_condition,
                                                                     DatabaseHelper.ConditionDefinitions.ConditionBearsEndurance,
                                                                     features);
            condition.specialDuration = true;
            condition.durationParameter = 1;
            condition.durationType = RuleDefinitions.DurationType.UntilLongRest;

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.EnhanceAbilityBullsStrength.EffectDescription);
            effect.effectForms.Clear();
            effect.SetEffectParticleParameters(effect_particle_parameters);
            effect.effectAdvancement.Clear();
            var effect_form = new EffectForm();
            effect_form.formType = EffectForm.EffectFormType.Condition;
            effect_form.conditionForm = new ConditionForm();
            effect_form.conditionForm.operation = ConditionForm.ConditionOperation.Add;
            effect_form.conditionForm.conditionDefinition = condition;
            effect.effectForms.Add(effect_form);
            effect.durationType = RuleDefinitions.DurationType.UntilLongRest;
            effect.immuneCreatureFamilies.Add(Helpers.Misc.createImmuneIfHasConditionFamily(condition));

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>
                                                               .createPower(name + "Power",
                                                                            "",
                                                                            title_string,
                                                                            description_string,
                                                                            sprite_power,
                                                                            effect,
                                                                            RuleDefinitions.ActivationTime.Action,
                                                                            2,
                                                                            RuleDefinitions.UsesDetermination.Fixed,
                                                                            RuleDefinitions.RechargeRate.LongRest,
                                                                            Helpers.Stats.Intelligence,
                                                                            Helpers.Stats.Intelligence,
                                                                            1,
                                                                            false
                                                                            );
            power.linkedPower = base_mutagen;
            mutagen_powers.Add(power);
            base_mutagen.addSubPower(power);
            return power;
        }


        static void createDeathStrike()
        {
            string death_strike_title_string = "Feature/&AlchemistVivisectionistSubclassDeathStrikeDamageTitle";
            string death_strike_description_string = "Feature/&AlchemistVivisectionistSubclassDeathStrikeDamageDescription";
            string use_death_strike_react_description = "Reaction/&SpendAlchemistVivisectionistSubclassDeathStrikeDamagePowerReactDescription";
            string use_death_strike_react_title = "Reaction/&CommonUsePowerReactTitle";

            var death_strike_used_condition = Helpers.ConditionBuilder.createCondition("AlchemistVivisectionistSubclassDeathStrikeUsedCondition",
                                                                                        "",
                                                                                        "",
                                                                                        "",
                                                                                        null,
                                                                                        DatabaseHelper.ConditionDefinitions.ConditionDummy
                                                                                        );
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(death_strike_used_condition);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.EffectDescription);
            effect.DurationParameter = 1;
            effect.durationType = RuleDefinitions.DurationType.Round;
            effect.endOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
            effect.SetSavingThrowDifficultyAbility(Helpers.Stats.Intelligence);
            effect.SavingThrowAbility = Helpers.Stats.Intelligence;
            effect.hasSavingThrow = false;
            effect.effectParticleParameters.impactParticleReference = DatabaseHelper.FeatureDefinitionPowers.PowerVampiricTouch.effectDescription.effectParticleParameters.impactParticleReference;
            effect.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            effect.EffectForms.Clear();

            var effect_form = new EffectForm();
            effect_form.DamageForm = new DamageForm();
            effect_form.FormType = EffectForm.EffectFormType.Damage;
            effect_form.DamageForm.dieType = RuleDefinitions.DieType.D6;
            effect_form.DamageForm.diceNumber = 2;
            effect_form.DamageForm.damageType = Helpers.DamageTypes.Necrotic;
            effect.EffectForms.Add(effect_form);

            effect_form = new EffectForm();
            effect_form.conditionForm = new ConditionForm();
            effect_form.FormType = EffectForm.EffectFormType.Condition;
            effect_form.conditionForm.conditionDefinition = death_strike_used_condition;
            effect_form.conditionForm.operation = ConditionForm.ConditionOperation.Add;
            effect_form.conditionForm.applyToSelf = true;
            effect_form.conditionForm.forceOnSelf = true;
            effect.EffectForms.Add(effect_form);

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("AlchemistVivisectionistSubclassDeathStrikeDamage",
                                                                     "",
                                                                     death_strike_title_string,
                                                                     death_strike_description_string,
                                                                     DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.guiPresentation.SpriteReference,
                                                                     effect,
                                                                     RuleDefinitions.ActivationTime.OnAttackHit,
                                                                     0,
                                                                     RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                     RuleDefinitions.RechargeRate.LongRest,
                                                                     uses_ability: Helpers.Stats.Intelligence
                                                                     );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                            {
                                                new NewFeatureDefinitions.NoConditionRestriction(death_strike_used_condition)
                                            };
            power.checkReaction = true;


            Helpers.StringProcessing.addPowerReactStrings(power, death_strike_title_string, use_death_strike_react_description,
                                                        use_death_strike_react_title, use_death_strike_react_description, "SpendPower");

            death_strike = power;
        }


        static CharacterSubclassDefinition createVivisectionist()
        {
            createVivisectionistProficiencies();
            createVivisectionistSneakAttack();
            createVivisectionistKnowledgeOfAnatomy();
            createVivisectionistSpells();
            createSummonHomunculus();
            createDeathStrike();
            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&AlchemistSubclassSpecializationVivisectionistDescription",
                    "Subclass/&AlchemistSubclassSpecializationVivisectionistTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainOblivion.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("AlchemistSubclassSpecializationVivisectionist", "91854dd1-ebe5-4e66-84eb-fcc0fd51414b")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(vivisectionist_spells, 3)
                                                                                            .AddFeatureAtLevel(vivisectionist_proficiencies, 3)
                                                                                            .AddFeatureAtLevel(vivisectionist_sneak_attack, 3)
                                                                                            .AddFeatureAtLevel(knowledge_of_anatomy, 3)
                                                                                            .AddFeatureAtLevel(summon_homunculus, 5)
                                                                                            .AddFeatureAtLevel(death_strike, 9)
                                                                                            .AddToDB();

            return definition;
        }


        static void createSummonHomunculus()
        {
            var destroy_effect_description = new EffectDescription();
            destroy_effect_description.Copy(DatabaseHelper.SpellDefinitions.Banishment.EffectDescription);
            destroy_effect_description.SetDurationType(RuleDefinitions.DurationType.Instantaneous);
            destroy_effect_description.durationParameter = 0;
            destroy_effect_description.rangeType = RuleDefinitions.RangeType.Self;
            destroy_effect_description.effectForms.Clear();
            destroy_effect_description.rangeParameter = 1;
            destroy_effect_description.SetTargetSide(RuleDefinitions.Side.Ally);
            destroy_effect_description.targetType = RuleDefinitions.TargetType.Self;
            destroy_effect_description.hasSavingThrow = false;
            destroy_effect_description.effectForms.Clear();

            var destroy_self_form = new EffectForm();
            destroy_self_form.formType = EffectForm.EffectFormType.Kill;
            destroy_self_form.killForm = new KillForm();
            destroy_self_form.killForm.killCondition = RuleDefinitions.KillCondition.Always;
            destroy_effect_description.effectForms.Add(destroy_self_form);

            homunculus_destroy_self = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                        .createPower("AlchemistVivisectionistSubclassSummonHomunculusDestroySelfPower",
                                                                    "",
                                                                    "Feature/&AlchemistVivisectionistSubclassHomunculusDestroySelfPowerTitle",
                                                                    "Feature/&AlchemistVivisectionistSubclassHomunculusDestroySelfPowerDescription",
                                                                    DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.spriteReference,
                                                                    destroy_effect_description,
                                                                    RuleDefinitions.ActivationTime.Action,
                                                                    1,
                                                                    RuleDefinitions.UsesDetermination.Fixed,
                                                                    RuleDefinitions.RechargeRate.AtWill
                                                                    );

            var uncontrolled_action_feature = Helpers.CopyFeatureBuilder<FeatureDefinitionActionAffinity>.createFeatureCopy("AlchemistVivisectionistSubclassHomunculusUncontrolledActionFeature",
                                                                                                                     "",
                                                                                                                     "",
                                                                                                                     "",
                                                                                                                     null,
                                                                                                                     DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityConditionTurned,
                                                                                                                     a =>
                                                                                                                     {
                                                                                                                         a.allowedActionTypes = new bool[]
                                                                                                                         {
                                                                                                                             true, false, true, false, true, false
                                                                                                                         };
                                                                                                                         a.forbiddenActions = new List<ActionDefinitions.Id> { ActionDefinitions.Id.AttackMain, ActionDefinitions.Id.Shove,
                                                                                                                                                                               ActionDefinitions.Id.ShoveBonus, ActionDefinitions.Id.AttackFree,
                                                                                                                                                                               ActionDefinitions.Id.AttackReadied, ActionDefinitions.Id.Ready };
                                                                                                                         a.authorizedActions = new List<ActionDefinitions.Id> { ActionDefinitions.Id.Dodge };
                                                                                                                     }
                                                                                                                     );
            var uncontrolled_condition = Helpers.ConditionBuilder.createCondition("AlchemistVivisectionistSubclassHomunculusUncontrolledCondition",
                                                                 "",
                                                                 "Rules/&AlchemistVivisectionistSubclassHomunculusUncontrolledConditionTitle",
                                                                 "Rules/&AlchemistVivisectionistSubclassHomunculusUncontrolledConditionDescription",
                                                                 null,
                                                                 DatabaseHelper.ConditionDefinitions.ConditionRestrained,
                                                                 uncontrolled_action_feature
                                                                 );

            var control_condition = Helpers.ConditionBuilder.createCondition("AlchemistVivisectionistSubclassHomunculusControlCondition",
                                                                             "",
                                                                             "Feature/&AlchemistVivisectionistSubclassSummonHomunculusControlPowerTitle",
                                                                             "Feature/&AlchemistVivisectionistSubclassSummonHomunculusControlPowerDescription",
                                                                             null,
                                                                             DatabaseHelper.ConditionDefinitions.ConditionInvisibleBase
                                                                             );


            var control_effect_description = new EffectDescription();
            control_effect_description.Copy(DatabaseHelper.SpellDefinitions.FaerieFire.EffectDescription);
            control_effect_description.SetDurationType(RuleDefinitions.DurationType.Round);
            control_effect_description.durationParameter = 1;
            control_effect_description.rangeType = RuleDefinitions.RangeType.Self;
            control_effect_description.endOfEffect = RuleDefinitions.TurnOccurenceType.StartOfTurn;
            control_effect_description.effectForms.Clear();
            control_effect_description.rangeParameter = 1;
            control_effect_description.SetTargetSide(RuleDefinitions.Side.Ally);
            control_effect_description.targetType = RuleDefinitions.TargetType.Self;
            control_effect_description.hasSavingThrow = false;
            control_effect_description.effectForms.Clear();

            var control_form = new EffectForm();
            control_form.formType = EffectForm.EffectFormType.Condition;
            control_form.conditionForm = new ConditionForm();
            control_form.conditionForm.conditionDefinition = control_condition;
            control_form.conditionForm.operation = ConditionForm.ConditionOperation.Add;
            control_effect_description.effectForms.Add(control_form);

            var control_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                                    .createPower("AlchemistVivisectionistSubclassSummonHomunculusControlPower",
                                                                                "",
                                                                                "Feature/&AlchemistVivisectionistSubclassSummonHomunculusControlPowerTitle",
                                                                                "Feature/&AlchemistVivisectionistSubclassSummonHomunculusControlPowerDescription",
                                                                                DatabaseHelper.FeatureDefinitionPowers.PowerWizardArcaneRecovery.GuiPresentation.spriteReference,
                                                                                control_effect_description,
                                                                                RuleDefinitions.ActivationTime.BonusAction,
                                                                                1,
                                                                                RuleDefinitions.UsesDetermination.Fixed,
                                                                                RuleDefinitions.RechargeRate.AtWill
                                                                                );

            var control_watcher = Helpers.FeatureBuilder<NewFeatureDefinitions.ApplyConditionOnTurnStartIfCasterHasNoCondition>.createFeature("AlchemistVivisectionistSubclassHomunculusUncontrolledFeature",
                                                                                                                                  "",
                                                                                                                                  Common.common_no_title,
                                                                                                                                  Common.common_no_title,
                                                                                                                                  Common.common_no_icon,
                                                                                                                                  a =>
                                                                                                                                  {
                                                                                                                                      a.conditionToApply = uncontrolled_condition;
                                                                                                                                      a.casterCondition = control_condition;
                                                                                                                                      a.ignoreIfCasterUnconcious = true;
                                                                                                                                  }
                                                                                                                                  );

            var attack_damage_bonus = Helpers.FeatureBuilder<NewFeatureDefinitions.AttackDamageBonusBasedOnCasterStat>
                                                                   .createFeature("AlchemistVivisectionistSubclassHomunculusIntAttackDamageBonus",
                                                                                   "",
                                                                                   "Monster/&HomunculusTitle",
                                                                                   Common.common_no_title,
                                                                                   Common.common_no_icon,
                                                                                   a =>
                                                                                   {
                                                                                       a.abilityScore = Helpers.Stats.Intelligence;
                                                                                       a.doNotApplyToDamage = true;
                                                                                   }
                                                                                   );

            var hp_bonus = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseMonsterHitPointsOnConditionApplicationBasedOnCasterAbilityScore>
                                                    .createFeature("AlchemistVivisectionistSubclassHomunculusIntHPBonus",
                                                                                               "",
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_title,
                                                                                               Common.common_no_icon,
                                                                                               a =>
                                                                                               {
                                                                                                   a.abilityScore = Helpers.Stats.Intelligence;
                                                                                                   a.multiplier = 1;
                                                                                               }
                                                                                               );

            var mark_condition = Helpers.ConditionBuilder.createCondition("AlchemistVivisectionistSubclassHomunculusMarkCondition",
                                                                          "",
                                                                          "",
                                                                          "",
                                                                          null,
                                                                          DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                          attack_damage_bonus,
                                                                          control_watcher
                                                                          );
            mark_condition.terminateWhenRemoved = true;
            hp_bonus.requiredCondition = mark_condition;

            var condition_summoned = Helpers.ConditionBuilder.createCondition("AlchemistVivisectionistSubclassHomunculusSummonedCondition",
                                                                              "",
                                                                              "",
                                                                              "",
                                                                              null,
                                                                              DatabaseHelper.ConditionDefinitions.ConditionDummy
                                                                              );
            condition_summoned.terminateWhenRemoved = true;
            control_power.restrictions.Add(new NewFeatureDefinitions.HasConditionRestriction(condition_summoned));

            List<EffectDescription> effect_descriptions = new List<EffectDescription>();
            for (int i = 5; i <= 20; i++)
            {
                var monster = createHomunculus(i);
                var effect_description = new EffectDescription();
                effect_description.Copy(DatabaseHelper.SpellDefinitions.ConjureAnimalsOneBeast.EffectDescription);
                effect_description.SetDurationType(RuleDefinitions.DurationType.UntilLongRest);
                effect_description.durationParameter = 1;
                effect_description.effectForms.Clear();
                effect_description.rangeParameter = 1;
                var form = new EffectForm();
                form.formType = EffectForm.EffectFormType.Summon;
                form.summonForm = new SummonForm();
                form.summonForm.conditionDefinition = mark_condition;
                form.summonForm.decisionPackage = DatabaseHelper.DecisionPackageDefinitions.IdleGuard_Default;
                form.summonForm.monsterDefinitionName = monster.name;
                effect_description.effectForms.Add(form);

                form = new EffectForm();
                form.formType = EffectForm.EffectFormType.Condition;
                form.conditionForm = new ConditionForm();
                form.conditionForm.conditionDefinition = condition_summoned;
                form.conditionForm.forceOnSelf = true;
                form.conditionForm.applyToSelf = true;
                form.conditionForm.operation = ConditionForm.ConditionOperation.Add;
                effect_description.effectForms.Add(form);

                effect_descriptions.Add(effect_description);
            }


            var summon_homunculus_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictionsAndCasterLevelDependentEffect>
                                                                                .createPower("AlchemistVivisectionistSubclassSummonHomunculusPower",
                                                                                            "",
                                                                                            "Feature/&AlchemistVivisectionistSubclassSummonHomunculusPowerTitle",
                                                                                            "Feature/&AlchemistVivisectionistSubclassSummonHomunculusPowerDescription",
                                                                                            DatabaseHelper.FeatureDefinitionPowers.PowerClericTurnUndead.GuiPresentation.spriteReference,
                                                                                            effect_descriptions[0],
                                                                                            RuleDefinitions.ActivationTime.Action,
                                                                                            1,
                                                                                            RuleDefinitions.UsesDetermination.Fixed,
                                                                                            RuleDefinitions.RechargeRate.SpellSlot
                                                                                            );
            summon_homunculus_power.spellcastingFeature = alchemist_spellcasting;
            summon_homunculus_power.restrictions.Add(new NewFeatureDefinitions.NoConditionRestriction(condition_summoned));
            summon_homunculus_power.minCustomEffectLevel = 6;
            for (int i = 6; i <= 20; i++)
            {
                summon_homunculus_power.levelEffectList.Add((i, effect_descriptions[i - 5]));
            }

            summon_homunculus = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistVivisectionistSubclassSummonHomunculusFeatureSet",
                                                                                "",
                                                                                "Feature/&AlchemistVivisectionistSubclassSummonHomunculusPowerTitle",
                                                                                "Feature/&AlchemistVivisectionistSubclassSummonHomunculusPowerDescription",
                                                                                false,
                                                                                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                false,
                                                                                hp_bonus,
                                                                                control_power,
                                                                                summon_homunculus_power
                                                                                );
        }


        static MonsterDefinition createHomunculus(int level)
        {
            int pb = 2 + level / 5;
            var attack = Helpers.CopyFeatureBuilder<MonsterAttackDefinition>.createFeatureCopy("AlchemistVivisectionistSubclassHomunculusSlamAttack" + level.ToString(),
                                                                                               "",
                                                                                               "",
                                                                                               "",
                                                                                               null,
                                                                                               DatabaseHelper.MonsterAttackDefinitions.Attack_Zombie_Slam,
                                                                                               a =>
                                                                                               {
                                                                                                   a.toHitBonus = pb;
                                                                                                   var effect = new EffectDescription();
                                                                                                   effect.Copy(a.EffectDescription);
                                                                                                   effect.EffectForms.Clear();
                                                                                                   effect.HasSavingThrow = false;

                                                                                                   var dmg = new EffectForm();
                                                                                                   dmg.FormType = EffectForm.EffectFormType.Damage;
                                                                                                   dmg.DamageForm = new DamageForm();
                                                                                                   dmg.DamageForm.bonusDamage = pb;
                                                                                                   dmg.DamageForm.diceNumber = 1;
                                                                                                   dmg.DamageForm.dieType = RuleDefinitions.DieType.D8;
                                                                                                   dmg.DamageForm.damageType = Helpers.DamageTypes.Necrotic;
                                                                                                   effect.EffectForms.Add(dmg);
                                                                                                   a.effectDescription = effect;
                                                                                               }
                                                                                               );

            
            var homunculus = Helpers.CopyFeatureBuilder<MonsterDefinition>.createFeatureCopy("AlchemistVivisectionistSubclassHomunculus" + level.ToString(),
                                                                                             "",
                                                                                             "Monster/&HomunculusTitle",
                                                                                             "Monster/&HomunculusDescription",
                                                                                             null,
                                                                                             DatabaseHelper.MonsterDefinitions.Zombie,
                                                                                             a =>
                                                                                             {
                                                                                                 a.SetDefaultFaction("Party");
                                                                                                 a.fullyControlledWhenAllied = true;
                                                                                                 a.armorClass = 15;
                                                                                                 a.hitDice = level;
                                                                                                 a.hitPointsBonus = 2;
                                                                                                 a.standardHitPoints = 5 * level + 2;
                                                                                                 a.skillScores = new List<MonsterSkillProficiency>
                                                                                                 {
                                                                                                     new MonsterSkillProficiency()
                                                                                                     {
                                                                                                         skillName = Helpers.Skills.Perception,
                                                                                                         bonus = pb *2
                                                                                                     },
                                                                                                     new MonsterSkillProficiency()
                                                                                                     {
                                                                                                         skillName = Helpers.Skills.Athletics,
                                                                                                         bonus = pb
                                                                                                     }
                                                                                                 };
                                                                                                 a.savingThrowScores = new List<MonsterSavingThrowProficiency>
                                                                                                 {
                                                                                                     new MonsterSavingThrowProficiency()
                                                                                                     {
                                                                                                         abilityScoreName = Helpers.Stats.Dexterity,
                                                                                                         bonus = pb
                                                                                                     },
                                                                                                     new MonsterSavingThrowProficiency()
                                                                                                     {
                                                                                                         abilityScoreName = Helpers.Stats.Constitution,
                                                                                                         bonus = pb
                                                                                                     },
                                                                                                 };
                                                                                                 a.abilityScores = new int []{14, 12, 14, 4, 10, 6 };
                                                                                                 a.features = new List<FeatureDefinition>
                                                                                                 {
                                                                                                     DatabaseHelper.FeatureDefinitionMoveModes.MoveModeMove8,
                                                                                                     DatabaseHelper.FeatureDefinitionDamageAffinitys.DamageAffinityPoisonImmunity,
                                                                                                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityPoisonImmunity,
                                                                                                     DatabaseHelper.FeatureDefinitionSenses.SenseNormalVision,
                                                                                                     DatabaseHelper.FeatureDefinitionSenses.SenseDarkvision,
                                                                                                     DatabaseHelper.FeatureDefinitionConditionAffinitys.ConditionAffinityCharmImmunity,
                                                                                                     DatabaseHelper.FeatureDefinitionActionAffinitys.ActionAffinityFightingStyleProtection,
                                                                                                     homunculus_destroy_self
                                                                                                 };
                                                                                                 a.attackIterations = new List<MonsterAttackIteration>
                                                                                                 {
                                                                                                     new MonsterAttackIteration
                                                                                                     {
                                                                                                         monsterAttackDefinition = attack,
                                                                                                         number = 1
                                                                                                     }
                                                                                                 };
                                                                                                 a.characterFamily = "Monstrosity";
                                                                                                 a.challengeRating = (level / 2);
                                                                                                 a.droppedLootDefinition = null;
                                                                                                 a.dungeonMakerPresence = MonsterDefinition.DungeonMaker.None;
                                                                                             }
                                                                                             );
            homunculus.bestiaryEntry = BestiaryDefinitions.BestiaryEntry.None;
            return homunculus;
        }


        static void createVivisectionistSpells()
        {
            vivisectionist_spells = Helpers.CopyFeatureBuilder<FeatureDefinitionAutoPreparedSpells>.createFeatureCopy("AlchemistVivisectionistSubclassAutopreparedSpells",
                                                                                                            "",
                                                                                                            "Feature/&AlchemistSubclassBonusSpells",
                                                                                                            "Feature/&DomainSpellsDescription",
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle,
                                                                                                            a =>
                                                                                                            {
                                                                                                                a.autopreparedTag = "Specialization";
                                                                                                                a.SetSpellcastingClass(alchemist_class);
                                                                                                                a.autoPreparedSpellsGroups
                                                                                                                    = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>()
                                                                                                                    {
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(3, DatabaseHelper.SpellDefinitions.HealingWord, DatabaseHelper.SpellDefinitions.HuntersMark),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(5, DatabaseHelper.SpellDefinitions.RayOfEnfeeblement, DatabaseHelper.SpellDefinitions.Blindness),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(9, DatabaseHelper.SpellDefinitions.MassHealingWord, DatabaseHelper.SpellDefinitions.VampiricTouch),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(13, DatabaseHelper.SpellDefinitions.Blight, DatabaseHelper.SpellDefinitions.DeathWard),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(17, DatabaseHelper.SpellDefinitions.Contagion, DatabaseHelper.SpellDefinitions.MassCureWounds),
                                                                                                                    };

                                                                                                            }
                                                                                                            );
        }


        static void createVivisectionistKnowledgeOfAnatomy()
        {
            knowledge_of_anatomy = Helpers.FeatureBuilder<NewFeatureDefinitions.ReplaceWeaponAbilityScoreForRangedOrFinessableWeapons>
                                                                                            .createFeature("AlchemistVivisectionistSubclassKnowledgeOfAnatomy",
                                                                                                            "",
                                                                                                            "Feature/&AlchemistVivisectionistSubclassKnowledgeOfAnatomyTitle",
                                                                                                            "Feature/&AlchemistVivisectionistSubclassKnowledgeOfAnatomyDescription",
                                                                                                            Common.common_no_icon,
                                                                                                            a =>
                                                                                                            {
                                                                                                                a.abilityScores = new List<string> { Helpers.Stats.Intelligence };
                                                                                                            }
                                                                                                            );
        }


        static void createVivisectionistSneakAttack()
        {
            vivisectionist_sneak_attack = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("AlchemistVivisectionistSubclassSneakAttack",
                                                                                                                          "",
                                                                                                                          "",
                                                                                                                          "Feature/&AlchemistVivisectionistSubclassSneakAttackDescription",
                                                                                                                          null,
                                                                                                                          DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageRogueSneakAttack,
                                                                                                                          a =>
                                                                                                                          {
                                                                                                                              a.diceByRankTable = Helpers.Misc.createDiceRankTable(20,
                                                                                                                                                                                   (5, 1),
                                                                                                                                                                                   (8, 2),
                                                                                                                                                                                   (11, 3),
                                                                                                                                                                                   (14, 4),
                                                                                                                                                                                   (17, 5),
                                                                                                                                                                                   (20, 6)
                                                                                                                                                                                   );
                                                                                                                          }
                                                                                                                          );
        }


        static void createVivisectionistProficiencies()
        {
            var tools_proficiency = Helpers.ProficiencyBuilder.CreateToolsProficiency("AlchemistVivisectionistSubclassToolsProficiency",
                                                                                      "",
                                                                                      Common.common_no_title,
                                                                                      Helpers.Tools.PoisonerKit
                                                                                      );

            var weapons = Helpers.ProficiencyBuilder.CreateWeaponProficiency("AlchemistVivisectionistSubclassWeaponsProficiency",
                                                                                "",
                                                                                Common.common_no_title,
                                                                                Common.common_no_title,
                                                                                Helpers.WeaponProficiencies.LongSword, Helpers.WeaponProficiencies.Rapier, Helpers.WeaponProficiencies.ShortSword);

            vivisectionist_proficiencies = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistVivisectionistSubclassBonusProficiencies",
                                                                                            "",
                                                                                            "Feature/&AlchemistVivisectionistSubclassBonusProficienciesTitle",
                                                                                            "Feature/&AlchemistVivisectionistSubclassBonusProficienciesDescription",
                                                                                            false,
                                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                            false,
                                                                                            weapons,
                                                                                            tools_proficiency
                                                                                            );
        }



        static CharacterSubclassDefinition createGrenadier()
        {
            createGrenadierProficiencies();
            createGrenadierSpells();
            createGrenadierBombs();
            createDestructionAffinity();
            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&AlchemistSubclassSpecializationGrenadierDescription",
                    "Subclass/&AlchemistSubclassSpecializationGrenadierTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.PathMagebane.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("AlchemistSubclassSpecializationGrenadier", "7cee2f03-145b-447c-be2f-5637d56b6818")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(grenadier_spells, 3)
                                                                                            .AddFeatureAtLevel(grenadier_proficiencies, 3)
                                                                                            .AddFeatureAtLevel(bombs, 3)
                                                                                            .AddFeatureAtLevel(destruction_affinity, 5)
                                                                                            .AddFeatureAtLevel(powerful_bombs, 9)
                                                                                            .AddToDB();

            return definition;
        }


        static void createDestructionAffinity()
        {
            destruction_affinity = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("AlchemistGrenadierSubclassDestructionAffinity",
                                                                                                                   "",
                                                                                                                   "Feature/&AlchemistGrenadierSubclassDestructionAffinityTitle",
                                                                                                                   "Feature/&AlchemistGrenadierSubclassDestructionAffinityDescription",
                                                                                                                   null,
                                                                                                                   DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageTraditionShockArcanistArcaneFury,
                                                                                                                   a =>
                                                                                                                   {
                                                                                                                       a.notificationTag = "AlchemistGrenadierSubclassDestructionAffinity";
                                                                                                                       a.damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.Die;
                                                                                                                       a.damageDieType = RuleDefinitions.DieType.D8;
                                                                                                                       a.damageDiceNumber = 1;
                                                                                                                       a.damageAdvancement = RuleDefinitions.AdditionalDamageAdvancement.None;
                                                                                                                   }
                                                                                                                   );
        }


        static void createGrenadierBombs()
        {
            bombs = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistGrenadierSubclassBombsFeatureSet",
                                                                "",
                                                                "Feature/&AlchemistGrenadierSubclassBombsFeatureSetTitle",
                                                                "Feature/&AlchemistGrenadierSubclassBombsFeatureSetDescription",
                                                                true,
                                                                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                false
                                                                );

            /*Helpers.StringProcessing.concatenateStrings("Feature/&AlchemistGrenadierSubclassBombsFeatureCompleteSetDescription",
                                                        ("Feature/&AlchemistGrenadierSubclassBombsFeatureSetDescription", "\n"),
                                                        ("Feature/&AlchemistGrenadierSubclassFlameBlastTitle", ":\n"),
                                                        ("Feature/&AlchemistGrenadierSubclassFlameBlast2Description", "\n\n"),
                                                        ("Feature/&AlchemistGrenadierSubclassForceGrenadeTitle", ":\n"),
                                                        ("Feature/&AlchemistGrenadierSubclassForceGrenade2Description", "\n\n"),
                                                        ("Feature/&AlchemistGrenadierSubclassProtectiveBurstTitle", ":\n"),
                                                        ("Feature/&AlchemistGrenadierSubclassProtectiveBurst2Description", "")
                                                        );*/

            powerful_bombs = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistGrenadierSubclassPowerfulBombsFeatureSet",
                                                                        "",
                                                                        "Feature/&AlchemistGrenadierSubclassPowerfulBombsFeatureSetTitle",
                                                                        "Feature/&AlchemistGrenadierSubclassPowerfulBombsFeatureSetDescription",
                                                                        false,
                                                                        FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                        false
                                                                        );

            var bomb_features = new Dictionary<int, FeatureDefinitionFeatureSet>()
            {
                {2, bombs},
                {3, powerful_bombs}
            };
            var flame_blast_effects = new Dictionary<int, EffectDescription>();
            var force_blast_effects = new Dictionary<int, EffectDescription>();
            var protective_burst_effects = new Dictionary<int, EffectDescription>();

            foreach (var i in bomb_features.Keys)
            {
                var effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.BurningHands.effectDescription);
                effect.savingThrowAbility = Helpers.Stats.Intelligence;
                effect.effectForms.Clear();
                effect.effectAdvancement.Clear();
                var form = new EffectForm();
                form.formType = EffectForm.EffectFormType.Damage;
                form.hasSavingThrow = true;
                form.savingThrowAffinity = RuleDefinitions.EffectSavingThrowType.HalfDamage;
                form.damageForm = new DamageForm();
                form.damageForm.damageType = Helpers.DamageTypes.Fire;
                form.damageForm.dieType = RuleDefinitions.DieType.D8;
                form.damageForm.diceNumber = i;
                effect.effectAdvancement.Clear();
                effect.effectForms.Add(form);
                effect.effectParticleParameters.casterParticleReference = null;
                flame_blast_effects.Add(i, effect);

                effect = new EffectDescription();
                effect.Copy(DatabaseHelper.SpellDefinitions.MagicMissile.EffectDescription);
                effect.SetRangeType(RuleDefinitions.RangeType.RangeHit);
                effect.SetRangeParameter(24);
                effect.EffectForms.Clear();
                effect.effectAdvancement.Clear();
                effect.targetParameter = 1;
                effect.effectAdvancement.Clear();
                form = new EffectForm();
                form.formType = EffectForm.EffectFormType.Damage;
                form.hasSavingThrow = false;
                form.damageForm = new DamageForm();
                form.damageForm.damageType = Helpers.DamageTypes.Force;
                form.damageForm.dieType = RuleDefinitions.DieType.D8;
                form.damageForm.diceNumber = i;
                effect.effectForms.Add(form);
                form = new EffectForm();
                form.formType = EffectForm.EffectFormType.Motion;
                form.hasSavingThrow = false;
                form.motionForm = new MotionForm();
                form.motionForm.distance = 1;
                form.motionForm.type = MotionForm.MotionType.PushFromOrigin;
                effect.effectForms.Add(form);
                effect.effectParticleParameters.casterParticleReference = null;
                force_blast_effects.Add(i, effect);

                effect = new EffectDescription();
                effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerClericTurnUndead.EffectDescription);
                effect.DurationParameter = 1;
                effect.SetRangeType(RuleDefinitions.RangeType.Self);
                effect.SetTargetSide(RuleDefinitions.Side.Ally);
                effect.SetRangeParameter(1);
                effect.SetTargetParameter(2);
                effect.SetTargetParameter(2);
                effect.SetTargetProximityDistance(6);
                effect.hasSavingThrow = false;
                effect.restrictedCreatureFamilies = new List<string>();
                effect.DurationType = RuleDefinitions.DurationType.Instantaneous;
                effect.SetTargetType(RuleDefinitions.TargetType.Sphere);
                effect.EffectForms.Clear();
                effect.effectAdvancement.Clear();
                form = new EffectForm();
                form.temporaryHitPointsForm = new TemporaryHitPointsForm();
                form.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
                form.temporaryHitPointsForm.diceNumber = i - 1;
                form.temporaryHitPointsForm.dieType = RuleDefinitions.DieType.D8;
                form.addBonusMode = RuleDefinitions.AddBonusMode.AbilityBonus;
                effect.EffectForms.Add(form);
                //effect.effectParticleParameters.casterParticleReference = null;
                protective_burst_effects.Add(i, effect);
            }

            var bomb_data = new List<(string prefix, Dictionary<int, EffectDescription> effects, AssetReferenceSprite sprite)>
            {
                    ("FlameBlast", flame_blast_effects, DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst.guiPresentation.spriteReference),
                    ("ForceGrenade", force_blast_effects, DatabaseHelper.FeatureDefinitionPowers.PowerSorcererManaPainterTap.guiPresentation.spriteReference),
                    ("ProtectiveBurst", protective_burst_effects, DatabaseHelper.FeatureDefinitionPowers.PowerRangerPrimevalAwareness.guiPresentation.spriteReference),
            };

            List<FeatureDefinitionPower> all_powers = new List<FeatureDefinitionPower>();
            foreach (var bd in bomb_data)
            {
                FeatureDefinitionPower first_power = null;
                foreach (var dice_num in bd.effects.Keys)
                {
                    var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>.createPower($"AlchemistGrenadierSubclass{bd.prefix}{dice_num}Power",
                                                                                                                      "",
                                                                                                                      $"Feature/&AlchemistGrenadierSubclass{bd.prefix}PowerTitle",
                                                                                                                      $"Feature/&AlchemistGrenadierSubclass{bd.prefix}{dice_num}PowerDescription",
                                                                                                                      bd.sprite,
                                                                                                                      bd.effects[dice_num],
                                                                                                                      RuleDefinitions.ActivationTime.BonusAction,
                                                                                                                      1,
                                                                                                                      RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed,
                                                                                                                      RuleDefinitions.RechargeRate.LongRest,
                                                                                                                      Helpers.Stats.Intelligence,
                                                                                                                      Helpers.Stats.Intelligence,
                                                                                                                      1,
                                                                                                                      true
                                                                                                                      );
                    power.attackHitComputation = RuleDefinitions.PowerAttackHitComputation.AbilityScore;
                    power.proficiencyBonusToAttack = true;
                    power.abilityScoreBonusToAttack = true;
                    if (first_power != null)
                    {
                        power.SetOverriddenPower(first_power);
                    }
                    first_power = power;
                    all_powers.Add(power);
                    bomb_features[dice_num].FeatureSet.Add(power);
                }
            }

            var bomb_uses_increase = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseNumberOfPowerUsesPerClassLevel>.createFeature("AlchemistGrenadierSubclassBombsUsesIncrease",
                                                                                                                                        "",
                                                                                                                                        Common.common_no_title,
                                                                                                                                        Common.common_no_title,
                                                                                                                                        Common.common_no_icon,
                                                                                                                                        a =>
                                                                                                                                        {
                                                                                                                                            a.characterClass = alchemist_class;
                                                                                                                                            a.levelIncreaseList = new List<(int, int)>();
                                                                                                                                            for (int i = 4; i <= 20; i += 1)
                                                                                                                                            {
                                                                                                                                                a.levelIncreaseList.Add((i, 1));
                                                                                                                                            }
                                                                                                                                            a.powers = all_powers;
                                                                                                                                        }
                                                                                                                                        );
            bomb_uses_increase.guiPresentation.hidden = true;
            foreach (var f in bomb_features.Values)
            {
                var powers = f.featureSet.Cast<NewFeatureDefinitions.PowerWithRestrictions>().ToArray();
                for (int i = 1; i < powers.Length; i++)
                {
                    powers[i].linkedPower = powers[0];
                }
            }
            bombs.featureSet.Insert(0, bomb_uses_increase);
        }


        static void createGrenadierSpells()
        {
            grenadier_spells = Helpers.CopyFeatureBuilder<FeatureDefinitionAutoPreparedSpells>.createFeatureCopy("AlchemistGrenadierSubclassAutopreparedSpells",
                                                                                                            "",
                                                                                                            "Feature/&AlchemistSubclassBonusSpells",
                                                                                                            "Feature/&DomainSpellsDescription",
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle,
                                                                                                            a =>
                                                                                                            {
                                                                                                                a.autopreparedTag = "Specialization";
                                                                                                                a.SetSpellcastingClass(alchemist_class);
                                                                                                                a.autoPreparedSpellsGroups
                                                                                                                    = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>()
                                                                                                                    {
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(3, DatabaseHelper.SpellDefinitions.Shield, DatabaseHelper.SpellDefinitions.Thunderwave),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(5, DatabaseHelper.SpellDefinitions.ScorchingRay, DatabaseHelper.SpellDefinitions.Shatter),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(9, DatabaseHelper.SpellDefinitions.Fireball, DatabaseHelper.SpellDefinitions.WindWall),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(13, DatabaseHelper.SpellDefinitions.IceStorm, DatabaseHelper.SpellDefinitions.WallOfFire),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(17, DatabaseHelper.SpellDefinitions.ConeOfCold, DatabaseHelper.SpellDefinitions.CloudKill),
                                                                                                                    };
                                                                                                            }
                                                                                                            );
        }


        static void createGrenadierProficiencies()
        {
            var tools_proficiency = Helpers.ProficiencyBuilder.CreateToolsProficiency("AlchemistGrenadierSubclassToolsProficiency",
                                                                          "",
                                                                          Common.common_no_title,
                                                                          Helpers.Tools.ScrollKit
                                                                          );

            grenadier_proficiencies = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistGrenadierSubclassBonusProficiencies",
                                                                                            "",
                                                                                            "Feature/&AlchemistGrenadierSubclassBonusProficienciesTitle",
                                                                                            "Feature/&AlchemistGrenadierSubclassBonusProficienciesDescription",
                                                                                            false,
                                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                            false,
                                                                                            tools_proficiency
                                                                                            );
        }



        static CharacterSubclassDefinition createRunemaster()
        {
            createRunemasterProficiencies();
            createRunemasterSpells();
            createWeaponRunes();
            createArmorRunes();
            var extra_attack = Helpers.CopyFeatureBuilder<FeatureDefinitionAttributeModifier>.createFeatureCopy("AlchemistRunemasterSubclassExtraAttack",
                                                                                                               "",
                                                                                                               "",
                                                                                                               "",
                                                                                                               null,
                                                                                                               DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierFighterExtraAttack
                                                                                                               );
            createRuneSpecialist();
            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&AlchemistSubclassSpecializationRunemasterDescription",
                    "Subclass/&AlchemistSubclassSpecializationRunemasterTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.DomainBattle.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("AlchemistSubclassSpecializationRunemaster", "158fc116-e0ed-4a5d-a754-e5a70eefeb15")
                                                                                            .SetGuiPresentation(gui_presentation)
                                                                                            .AddFeatureAtLevel(runemaster_spells, 3)
                                                                                            .AddFeatureAtLevel(runemaster_proficiencies, 3)
                                                                                            .AddFeatureAtLevel(weapon_runes, 3)
                                                                                            .AddFeatureAtLevel(armor_runes, 3)
                                                                                            .AddFeatureAtLevel(extra_attack, 5)
                                                                                            .AddFeatureAtLevel(rune_specialist, 9)
                                                                                            .AddToDB();

            Action<RulesetCharacterHero> fix_action = c =>
            {
                if (c.ClassesAndSubclasses.ContainsValue(definition) && !c.UsablePowers.Any(u => u.powerDefinition == base_weapon_runes))
                {
                    c.GrantPowers();
                }
            };
            Common.postload_actions.Add(fix_action);

            return definition;
        }


        static void createRuneSpecialist()
        {
            rune_specialist = Helpers.FeatureBuilder<NewFeatureDefinitions.IncreaseNumberOfPowerUses>.createFeature("AlchemistRunemasterSubclassRuneSpecialist",
                                                                                                                    "",
                                                                                                                    "Feature/&AlchemistRunemasterSubclassRuneSpecialistTitle",
                                                                                                                    "Feature/&AlchemistRunemasterSubclassRuneSpecialistDescription",
                                                                                                                    Common.common_no_icon,
                                                                                                                    a =>
                                                                                                                    {
                                                                                                                        a.powers = rune_powers;
                                                                                                                        a.value = 1;
                                                                                                                    }
                                                                                                                    );
        }


        static void createArmorRunes()
        {
            var base_eff = new EffectDescription();
            base_eff.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription);
            base_eff.effectForms.Clear();
            base_eff.targetType = RuleDefinitions.TargetType.Self;
            base_eff.rangeType = RuleDefinitions.RangeType.Self;
            base_eff.durationParameter = 1;
            base_eff.itemSelectionType = ActionDefinitions.ItemSelectionType.Equiped;
            base_eff.targetType = RuleDefinitions.TargetType.Self;
            base_eff.rangeType = RuleDefinitions.RangeType.Self;
            base_eff.durationType = RuleDefinitions.DurationType.UntilAnyRest;

            base_armor_runes = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerBundle>
                                       .createPower("AlchemistRunemasterSubclassArmorRunePower",
                                                    "",
                                                    "Feature/&AlchemistRunemasterSubclassArmorRunesTitle",
                                                    "Feature/&AlchemistRunemasterSubclassArmorRunesDescription",
                                                    DatabaseHelper.SpellDefinitions.MageArmor.GuiPresentation.spriteReference,
                                                    base_eff,
                                                    RuleDefinitions.ActivationTime.Action,
                                                    1,
                                                    RuleDefinitions.UsesDetermination.Fixed,
                                                    RuleDefinitions.RechargeRate.ShortRest
                                                    );
            rune_powers.Add(base_armor_runes);

            var protection_rune = createArmorRunePower("AlchemistRunemasterSubclassProtectionArmorRune", DatabaseHelper.FeatureDefinitionPowers.PowerOathOfTirmarAuraTruth.guiPresentation.spriteReference);

            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            
            var effect_form = new EffectForm();
            effect_form.createdByCharacter = true;
            effect_form.applyLevel = EffectForm.LevelApplianceType.Multiply;
            effect_form.levelMultiplier = 1;
            effect_form.temporaryHitPointsForm = new TemporaryHitPointsForm();
            effect_form.FormType = EffectForm.EffectFormType.TemporaryHitPoints;
            effect_form.temporaryHitPointsForm.diceNumber = 1;
            effect_form.temporaryHitPointsForm.dieType = RuleDefinitions.DieType.D1;
            effect.EffectForms.Add(effect_form);
            effect.effectAdvancement.Clear();

            var protection_rune_power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerWithRestrictions>
                                                                        .createPower("AlchemistRunemasterSubclassProtectionArmorRuneUsePower",
                                                                                     "",
                                                                                     "Feature/&AlchemistRunemasterSubclassProtectionArmorRuneUsePowerTitle",
                                                                                     "Feature/&AlchemistRunemasterSubclassProtectionArmorRuneUsePowerDescription",
                                                                                     DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDivineWrath.GuiPresentation.spriteReference,
                                                                                     effect,
                                                                                     RuleDefinitions.ActivationTime.BonusAction,
                                                                                     1,
                                                                                     RuleDefinitions.UsesDetermination.ProficiencyBonus,
                                                                                     RuleDefinitions.RechargeRate.LongRest
                                                                                    );
            protection_rune_power.restrictions = new List<NewFeatureDefinitions.IRestriction> { new NewFeatureDefinitions.WearingArmorWithFeature(protection_rune.armor_feature) };

            var dampening_rune = createArmorRunePower("AlchemistRunemasterSubclassDampeningArmorRune", DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionAuraDevotion.guiPresentation.spriteReference);

            var stealth_advantage = Helpers.AbilityCheckAffinityBuilder.createSkillCheckAffinity("AlchemistRunemasterSubclassDampeningArmorRuneStealthAdvantageFeature",
                                                                                                 "",
                                                                                                 Common.common_no_title,
                                                                                                 Common.common_no_title,
                                                                                                 Common.common_no_icon,
                                                                                                 RuleDefinitions.CharacterAbilityCheckAffinity.Advantage,
                                                                                                 0,
                                                                                                 RuleDefinitions.DieType.D1,
                                                                                                 Helpers.Skills.Stealth
                                                                                                 );

            var trigger_stealth_advantage = Helpers.FeatureBuilder<NewFeatureDefinitions.AbilityCheckAffinityUnderRestriction>
                                                                  .createFeature("AlchemistRunemasterSubclassConnectionDampeningArmorRuneTriggerFeature",
                                                                                 "",
                                                                                 Common.common_no_title,
                                                                                 Common.common_no_title,
                                                                                 Common.common_no_icon,
                                                                                 a =>
                                                                                 {
                                                                                     a.feature = stealth_advantage;
                                                                                     a.restrictions.Add(new NewFeatureDefinitions.WearingArmorWithFeature(dampening_rune.armor_feature));
                                                                                 }
                                                                                 );

            var propulsion_rune = createArmorRunePower("AlchemistRunemasterSubclassPropulsionArmorRune", DatabaseHelper.FeatureDefinitionPowers.PowerOathOfMotherlandVolcanicAura.guiPresentation.spriteReference);

            var trigger_speed_bonus = Helpers.FeatureBuilder<NewFeatureDefinitions.MovementBonusWithRestrictions>
                                                      .createFeature("AlchemistRunemasterSubclassConnectionPropulsionArmorRuneTriggerFeature",
                                                                     "",
                                                                     Common.common_no_title,
                                                                     Common.common_no_title,
                                                                     Common.common_no_icon,
                                                                     a =>
                                                                     {
                                                                         a.modifiers.Add(DatabaseHelper.FeatureDefinitionMovementAffinitys.MovementAffinityLongstrider);
                                                                         a.restrictions.Add(new NewFeatureDefinitions.WearingArmorWithFeature(propulsion_rune.armor_feature));
                                                                     }
                                                                     );

            armor_runes = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistRunemasterSubclassArmorRunes",
                                                                    "",
                                                                    "Feature/&AlchemistRunemasterSubclassArmorRunesTitle",
                                                                    "Feature/&AlchemistRunemasterSubclassArmorRunesDescription",
                                                                    false,
                                                                    FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                    false,
                                                                    protection_rune_power,
                                                                    trigger_stealth_advantage,
                                                                    trigger_speed_bonus,
                                                                    base_armor_runes,
                                                                    protection_rune.power,
                                                                    dampening_rune.power,
                                                                    propulsion_rune.power
                                                                    );

            protection_rune.power.linkedPower = base_armor_runes;
            dampening_rune.power.linkedPower = base_armor_runes;
            propulsion_rune.power.linkedPower = base_armor_runes;

            base_armor_runes.addSubPower(protection_rune.power);
            base_armor_runes.addSubPower(dampening_rune.power);
            base_armor_runes.addSubPower(propulsion_rune.power);
        }


        static void createWeaponRunes()
        {
            var base_eff = new EffectDescription();
            base_eff.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription);
            base_eff.effectForms.Clear();
            base_eff.durationParameter = 1;
            base_eff.itemSelectionType = ActionDefinitions.ItemSelectionType.Weapon;
            base_eff.targetType = RuleDefinitions.TargetType.Self;
            base_eff.rangeType = RuleDefinitions.RangeType.Self;
            base_eff.durationType = RuleDefinitions.DurationType.UntilAnyRest;

            base_weapon_runes = Helpers.GenericPowerBuilder<NewFeatureDefinitions.PowerBundle>
                                       .createPower("AlchemistRunemasterSubclassWeaponRunePower",
                                                    "",
                                                    "Feature/&AlchemistRunemasterSubclassWeaponRunesTitle",
                                                    "Feature/&AlchemistRunemasterSubclassWeaponRunesDescription",
                                                    DatabaseHelper.SpellDefinitions.MagicWeapon.GuiPresentation.spriteReference,
                                                    base_eff,
                                                    RuleDefinitions.ActivationTime.Action,
                                                    1,
                                                    RuleDefinitions.UsesDetermination.Fixed,
                                                    RuleDefinitions.RechargeRate.ShortRest
                                                    );
            rune_powers.Add(base_weapon_runes);

            var connection_rune = createWeaponRunePower("AlchemistRunemasterSubclassConnectionWeaponRune", DatabaseHelper.FeatureDefinitionPowers.PowerFighterActionSurge.guiPresentation.spriteReference);
            var use_int_stat_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.ReplaceWeaponAbilityScoreWithHighestStatIfWeaponHasFeature>
                                                  .createFeature("AlchemistRunemasterSubclassConnectionWeaponRuneUseIntStatFeature",
                                                                 "",
                                                                 Common.common_no_title,
                                                                 Common.common_no_title,
                                                                 Common.common_no_icon,
                                                                 a =>
                                                                 {
                                                                     a.abilityScores = new List<string> { Helpers.Stats.Intelligence};
                                                                     a.weaponFeature = connection_rune.weapon_feature;
                                                                 }
                                                                 );

            var use_weapon_as_spellfocus = Helpers.FeatureBuilder<NewFeatureDefinitions.AllowToUseWeaponWithFeatureAsSpellFocus>
                                                      .createFeature("AlchemistRunemasterSubclassConnectionWeaponRuneSpellFocusFeature",
                                                                     "",
                                                                     Common.common_no_title,
                                                                     Common.common_no_title,
                                                                     Common.common_no_icon,
                                                                     a =>
                                                                     {
                                                                         a.weaponFeature = connection_rune.weapon_feature;
                                                                     }
                                                                     );

            var destruction_rune = createWeaponRunePower("AlchemistRunemasterSubclassDestructionWEaponRune", DatabaseHelper.FeatureDefinitionPowers.PowerDomainBattleDecisiveStrike.guiPresentation.spriteReference);

            var destruction_rune_affinity = Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalDamage>.createFeatureCopy("AlchemistRunemasterSubclassDestructionWeaponRuneDestructionAffinity",
                                                                                                       "",
                                                                                                       Common.common_no_title,
                                                                                                       Common.common_no_title,
                                                                                                       null,
                                                                                                       DatabaseHelper.FeatureDefinitionAdditionalDamages.AdditionalDamageDomainLifeDivineStrike,
                                                                                                       a =>
                                                                                                       {
                                                                                                           a.notificationTag = "AlchemistRunemasterSubclassDestructionWeaponRuneDestructionAffinity";
                                                                                                           a.damageValueDetermination = RuleDefinitions.AdditionalDamageValueDetermination.Die;
                                                                                                           a.damageDieType = RuleDefinitions.DieType.D8;
                                                                                                       }
                                                                                                       );

            var destruction_rune_condition = Helpers.ConditionBuilder.createConditionWithInterruptions("AlchemistRunemasterSubclassDestructionWeaponRuneCondition",
                                                                                                       "",
                                                                                                       Common.common_no_title,
                                                                                                       Common.common_no_title,
                                                                                                       Common.common_no_icon,
                                                                                                       DatabaseHelper.ConditionDefinitions.ConditionDummy,
                                                                                                       new RuleDefinitions.ConditionInterruption[] {RuleDefinitions.ConditionInterruption.Attacks },
                                                                                                       destruction_rune_affinity
                                                                                                       );
            destruction_rune_condition.silentWhenAdded = true;
            destruction_rune_condition.silentWhenRemoved = true;
            NewFeatureDefinitions.ConditionsData.no_refresh_conditions.Add(destruction_rune_condition);

            var trigger_destruction_rune_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnAttackToAttackerIfWeaponHasFeature>
                                          .createFeature("AlchemistRunemasterSubclassDestructionWeaponRuneTriggerFeature",
                                                         "",
                                                         Common.common_no_title,
                                                         Common.common_no_title,
                                                         Common.common_no_icon,
                                                         a =>
                                                         {
                                                             a.condition = destruction_rune_condition;
                                                             a.durationType = RuleDefinitions.DurationType.Round;
                                                             a.turnOccurence = RuleDefinitions.TurnOccurenceType.StartOfTurn;
                                                             a.durationValue = 1;
                                                             a.weaponFeature = destruction_rune.weapon_feature;
                                                         }
                                                         );

            var guardian_rune = createWeaponRunePower("AlchemistRunemasterSubclassGuardianRune", DatabaseHelper.FeatureDefinitionPowers.PowerOathOfDevotionSacredWeapon.guiPresentation.spriteReference);

            var disadvantage_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.AttackDisadvantageAgainstNonCaster>
                                                  .createFeature("AlchemistRunemasterSubclassGuardianWeaponRuneDisadvantageFeature",
                                                                 "",
                                                                 Common.common_no_title,
                                                                 Common.common_no_title,
                                                                 Common.common_no_icon
                                                                 );

            var guardian_rune_condition = Helpers.ConditionBuilder.createCondition("AlchemistRunemasterSubclassGuardianWeaponRuneCondition",
                                                                                           "",
                                                                                           "Rules/&AlchemistRunemasterSubclassGuardianRuneConditionTitle",
                                                                                           Common.common_no_title,
                                                                                           null,
                                                                                           DatabaseHelper.ConditionDefinitions.ConditionCursedByBestowCurseAttackRoll,
                                                                                           disadvantage_feature
                                                                                           );
            guardian_rune_condition.conditionTags.Clear();
            disadvantage_feature.condition = guardian_rune_condition;
            var trigger_guardian_rune_feature = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnAttackHitToTargetIfWeaponHasFeature>
                              .createFeature("AlchemistRunemasterSubclassGuardianRuneTriggerFeature",
                                             "",
                                             Common.common_no_title,
                                             Common.common_no_title,
                                             Common.common_no_icon,
                                             a =>
                                             {
                                                 a.condition = guardian_rune_condition;
                                                 a.durationType = RuleDefinitions.DurationType.Round;
                                                 a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                                 a.durationValue = 1;
                                                 a.weaponFeature = guardian_rune.weapon_feature;
                                             }
                                             );

            connection_rune.power.linkedPower = base_weapon_runes;
            destruction_rune.power.linkedPower = base_weapon_runes;
            guardian_rune.power.linkedPower = base_weapon_runes;

            weapon_runes = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistRunemasterSubclassWeaponRunes",
                                                                                "",
                                                                                "Feature/&AlchemistRunemasterSubclassWeaponRunesTitle",
                                                                                "Feature/&AlchemistRunemasterSubclassWeaponRunesDescription",
                                                                                false,
                                                                                FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                false,
                                                                                use_int_stat_feature,
                                                                                use_weapon_as_spellfocus,
                                                                                trigger_destruction_rune_feature,
                                                                                trigger_guardian_rune_feature,
                                                                                base_weapon_runes,
                                                                                connection_rune.power,
                                                                                destruction_rune.power,
                                                                                guardian_rune.power
                                                                                );

            base_weapon_runes.addSubPower(connection_rune.power);
            base_weapon_runes.addSubPower(destruction_rune.power);
            base_weapon_runes.addSubPower(guardian_rune.power);
        }


        static (NewFeatureDefinitions.HiddenPower power, FeatureDefinition weapon_feature) createWeaponRunePower(string prefix, AssetReferenceSprite sprite)
        {
            var title_string = "Feature/&" + prefix + "Title";
            var description_string = "Feature/&" + prefix + "Description";
            var weapon_feature = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature(prefix + "Feature",
                                                                                 "",
                                                                                 title_string,
                                                                                 description_string,
                                                                                 sprite);
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.MagicWeapon.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 1;
            effect.durationParameter = 1;
            effect.itemSelectionType = ActionDefinitions.ItemSelectionType.Weapon;
            effect.targetType = RuleDefinitions.TargetType.Self;
            effect.rangeType = RuleDefinitions.RangeType.Self;

            effect.durationType = RuleDefinitions.DurationType.UntilAnyRest;

            var effect_form = new EffectForm();
            effect_form.itemPropertyForm = new ItemPropertyForm();
            effect_form.FormType = EffectForm.EffectFormType.ItemProperty;
            effect_form.itemPropertyForm.featureBySlotLevel = new List<FeatureUnlockByLevel> { new FeatureUnlockByLevel(weapon_feature, 0) };
            effect.EffectForms.Add(effect_form);
            effect.effectAdvancement.Clear();

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>
                                                                        .createPower(prefix + "Power",
                                                                                     "",
                                                                                     title_string,
                                                                                     description_string,
                                                                                     sprite,
                                                                                     effect,
                                                                                     RuleDefinitions.ActivationTime.Action,
                                                                                     1,
                                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                                     RuleDefinitions.RechargeRate.ShortRest
                                                                                    );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction> { new NewFeatureDefinitions.HasWeaponInMainHandWithoutFeature(weapon_feature) };

            rune_powers.Add(power);
            return (power, weapon_feature);
        }


        static (NewFeatureDefinitions.HiddenPower power, FeatureDefinition armor_feature) createArmorRunePower(string prefix, AssetReferenceSprite sprite)
        {
            var title_string = "Feature/&" + prefix + "Title";
            var description_string = "Feature/&" + prefix + "Description";
            var armor_feature = Helpers.OnlyDescriptionFeatureBuilder.createOnlyDescriptionFeature(prefix + "Feature",
                                                                                 "",
                                                                                 title_string,
                                                                                 description_string,
                                                                                 sprite);
            var effect = new EffectDescription();
            effect.Copy(DatabaseHelper.SpellDefinitions.MageArmor.EffectDescription);
            effect.EffectForms.Clear();
            effect.EffectAdvancement.Clear();
            effect.rangeParameter = 1;
            effect.durationParameter = 1;
            effect.itemSelectionType = ActionDefinitions.ItemSelectionType.Equiped;
            effect.targetType = RuleDefinitions.TargetType.Self;
            effect.rangeType = RuleDefinitions.RangeType.Self;
            effect.targetFilteringTag = RuleDefinitions.TargetFilteringTag.No;

            effect.durationType = RuleDefinitions.DurationType.UntilAnyRest;

            var effect_form = new EffectForm();
            effect_form.itemPropertyForm = new ItemPropertyForm();
            effect_form.FormType = EffectForm.EffectFormType.ItemProperty;
            effect_form.itemPropertyForm.featureBySlotLevel = new List<FeatureUnlockByLevel> { new FeatureUnlockByLevel(armor_feature, 0) };
            effect.EffectForms.Add(effect_form);
            effect.effectAdvancement.Clear();

            var power = Helpers.GenericPowerBuilder<NewFeatureDefinitions.HiddenPower>
                                                                        .createPower(prefix + "Power",
                                                                                     "",
                                                                                     title_string,
                                                                                     description_string,
                                                                                     sprite,
                                                                                     effect,
                                                                                     RuleDefinitions.ActivationTime.Action,
                                                                                     1,
                                                                                     RuleDefinitions.UsesDetermination.Fixed,
                                                                                     RuleDefinitions.RechargeRate.ShortRest
                                                                                    );
            power.restrictions = new List<NewFeatureDefinitions.IRestriction> { new NewFeatureDefinitions.WearingArmorWithoutFeature(armor_feature) };

            rune_powers.Add(power);
            return (power, armor_feature);
        }


        static void createRunemasterSpells()
        {
            runemaster_spells = Helpers.CopyFeatureBuilder<FeatureDefinitionAutoPreparedSpells>.createFeatureCopy("AlchemistRunemasterSubclassAutopreparedSpells",
                                                                                                            "",
                                                                                                            "Feature/&AlchemistSubclassBonusSpells",
                                                                                                            "Feature/&DomainSpellsDescription",
                                                                                                            null,
                                                                                                            DatabaseHelper.FeatureDefinitionAutoPreparedSpellss.AutoPreparedSpellsDomainBattle,
                                                                                                            a =>
                                                                                                            {
                                                                                                                a.autopreparedTag = "Specialization";
                                                                                                                a.SetSpellcastingClass(alchemist_class);
                                                                                                                a.autoPreparedSpellsGroups
                                                                                                                    = new List<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup>()
                                                                                                                    {
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(3, DatabaseHelper.SpellDefinitions.DivineFavor, DatabaseHelper.SpellDefinitions.Shield),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(5, DatabaseHelper.SpellDefinitions.SpiritualWeapon, DatabaseHelper.SpellDefinitions.Shatter),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(9, DatabaseHelper.SpellDefinitions.HypnoticPattern, DatabaseHelper.SpellDefinitions.LightningBolt),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(13, DatabaseHelper.SpellDefinitions.FireShield, DatabaseHelper.SpellDefinitions.GreaterInvisibility),
                                                                                                                        Helpers.Misc.createAutopreparedSpellsGroup(17, DatabaseHelper.SpellDefinitions.DispelEvilAndGood, DatabaseHelper.SpellDefinitions.HoldPerson),
                                                                                                                    };
                                                                                                            }
                                                                                                            );
        }


        static void createRunemasterProficiencies()
        {
            var tools_proficiency = Helpers.ProficiencyBuilder.CreateToolsProficiency("AlchemistRunemasterSubclassToolsProficiency",
                                                                                      "",
                                                                                      Common.common_no_title,
                                                                                      Helpers.Tools.SmithTool
                                                                                      );

            var armor_proficiency = Helpers.ProficiencyBuilder.createCopy("AlchemistRunemasterSubclassArmorProficiency",
                                                                          "",
                                                                          Common.common_no_title,
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterArmor
                                                                          );

            var weapon_proficiency = Helpers.ProficiencyBuilder.createCopy("AlchemistRunemasterSubclassWeaponProficiency",
                                                                          "",
                                                                          Common.common_no_title,
                                                                          "",
                                                                          DatabaseHelper.FeatureDefinitionProficiencys.ProficiencyFighterWeapon
                                                                          );

            runemaster_proficiencies = Helpers.FeatureSetBuilder.createFeatureSet("AlchemistRunemasterSubclassBonusProficiencies",
                                                                                            "",
                                                                                            "Feature/&AlchemistRunemasterSubclassBonusProficienciesTitle",
                                                                                            "Feature/&AlchemistRunemasterSubclassBonusProficienciesDescription",
                                                                                            false,
                                                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                                                            false,
                                                                                            tools_proficiency,
                                                                                            weapon_proficiency,
                                                                                            armor_proficiency
                                                                                            );
        }


        public static void BuildAndAddClassToDB()
        {
            var AlchemistClass = new AlchemistClassBuilder(AlchemistClassName, AlchemistClassNameGuid).AddToDB();
            AlchemistClass.FeatureUnlocks.Sort(delegate (FeatureUnlockByLevel a, FeatureUnlockByLevel b)
                                          {
                                              return a.Level - b.Level;
                                          }
                                         );

            AlchemistFeatureDefinitionSubclassChoice.Subclasses.Add(createGrenadier().Name);
            AlchemistFeatureDefinitionSubclassChoice.Subclasses.Add(createRunemaster().Name);
            AlchemistFeatureDefinitionSubclassChoice.Subclasses.Add(createVivisectionist().Name);
        }

        private static FeatureDefinitionSubclassChoice AlchemistFeatureDefinitionSubclassChoice;
    }
}
