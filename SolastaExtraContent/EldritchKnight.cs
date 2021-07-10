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
    public class EldritchKnight
    {
        static public Dictionary<int, NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection> any_school_spell_selection = new Dictionary<int, NewFeatureDefinitions.FeatureDefinitionExtraSpellSelection>();
        static public FeatureDefinitionCastSpell eldritch_knight_spell_casting;
        static public FeatureDefinitionAttackModifier magic_weapons;
        static public NewFeatureDefinitions.ExtraMainWeaponAttack spell_combat;
        static public NewFeatureDefinitions.InitiatorApplyConditionOnDamageDone accursed_strike;

        internal static void create()
        {
            createSpellcastingAndExtraSpells();
            createMagicWeapon();
            createSpellCombat();
            createAccursedStrike();

            var fighter_archetype_selection = DatabaseHelper.FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

            var gui_presentation = new GuiPresentationBuilder(
                    "Subclass/&MartialEldritchKnightDescripton",
                    "Subclass/&MartialEldritchKnightTitle")
                    .SetSpriteReference(DatabaseHelper.CharacterSubclassDefinitions.MartialSpellblade.GuiPresentation.SpriteReference)
                    .Build();

            CharacterSubclassDefinition definition = new CharacterSubclassDefinitionBuilder("MartialEldritchKnight", "ed06fd05-3634-42ad-a546-9cbdc9afad10")
                    .SetGuiPresentation(gui_presentation)
                    .AddFeatureAtLevel(magic_weapons, 3)
                    .AddFeatureAtLevel(eldritch_knight_spell_casting, 3)
                    .AddFeatureAtLevel(any_school_spell_selection[3], 3)
                    .AddFeatureAtLevel(spell_combat, 7)
                    .AddFeatureAtLevel(any_school_spell_selection[8], 8)
                    .AddFeatureAtLevel(accursed_strike, 10)
                    .AddFeatureAtLevel(any_school_spell_selection[14], 14)
                    .AddFeatureAtLevel(any_school_spell_selection[20], 20)
                    .AddToDB();

            fighter_archetype_selection.Subclasses.Add(definition.Name);
        }


        static void createAccursedStrike()
        {
            string title = "Feature/&FighterSubclassEldritchKnightAccursedStrikeTitle";
            var description = "Feature/&FighterSubclassEldritchKnightAccursedStrikeDescription";
            var accursed_strike_effect = Helpers.FeatureBuilder<NewFeatureDefinitions.SavingthrowAffinityUnderRestrictionIfHasConditionFromCaster>
                                                                                                .createFeature("FighterSubclassEldritchKnightAccursedStrikeEffect",
                                                                                                                "",
                                                                                                                title,
                                                                                                                description,
                                                                                                                Common.common_no_icon,
                                                                                                                d =>
                                                                                                                {
                                                                                                                    d.affinityGroups = new List<SavingThrowAffinityGroup>
                                                                                                                    {

                                                                                                                    };
                                                                                                                    d.restrictions = new List<NewFeatureDefinitions.IRestriction>
                                                                                                                    {
                                                                                                                    };
                                                                                                                    
                                                                                                                }
                                                                                                                );
            foreach (var s in Helpers.Stats.getAllStats())
            {
                accursed_strike_effect.affinityGroups.Add(new SavingThrowAffinityGroup
                {
                    abilityScoreName = s,
                    affinity = RuleDefinitions.CharacterSavingThrowAffinity.Disadvantage
                });
            }

            var condition = Helpers.ConditionBuilder.createCondition("FighterSubclassEldritchKnightAccursedStrikeCondition",
                                                                     "",
                                                                     "Rules/&FighterSubclassEldritchKnightAccursedStrikeConditionTitle",
                                                                     "Rules/&FighterSubclassEldritchKnightAccursedStrikeConditionDescription",
                                                                     null,
                                                                     DatabaseHelper.ConditionDefinitions.ConditionBaned,
                                                                     accursed_strike_effect);
            accursed_strike_effect.condition =  condition;

            accursed_strike = Helpers.FeatureBuilder<NewFeatureDefinitions.InitiatorApplyConditionOnDamageDone>
                .createFeature("FighterSubclassEldritchKnightAccursedStrike",
                               "",
                               "Feature/&FighterSubclassEldritchKnightAccursedStrikeTitle",
                               "Feature/&FighterSubclassEldritchKnightAccursedStrikeDescription",
                               Common.common_no_icon,
                               a =>
                               {
                                   a.durationType = RuleDefinitions.DurationType.Round;
                                   a.durationValue = 1;
                                   a.turnOccurence = RuleDefinitions.TurnOccurenceType.EndOfTurn;
                                   a.condition = condition;
                                   a.onlyWeapon = true;
                               }
                               );
        }


        static void createSpellCombat()
        {
            spell_combat = Helpers.FeatureBuilder<NewFeatureDefinitions.ExtraMainWeaponAttack>.createFeature("FighterSubclassEldritchKnightSpellCombat",
                                                                                                                "",
                                                                                                                "Feature/&FighterSubclassEldritchKnightSpellCombatTitle",
                                                                                                                "Feature/&FighterSubclassEldritchKnightSpellCombatDescription",
                                                                                                                null,
                                                                                                                a =>
                                                                                                                {
                                                                                                                    a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                                                                                                                    {
                                                                                                                        new NewFeatureDefinitions.UsedCantrip(),
                                                                                                                    };
                                                                                                                    a.actionType = ActionDefinitions.ActionType.Bonus;
                                                                                                                }
                                                                                                                );
        }


        static void createMagicWeapon()
        {
            magic_weapons = Helpers.CopyFeatureBuilder<FeatureDefinitionAttackModifier>.createFeatureCopy("FighterSubclassEldritchKnightMagicWeapons",
                                                                                                          "",
                                                                                                          "",
                                                                                                          "",
                                                                                                          null,
                                                                                                          DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMartialSpellBladeMagicWeapon
                                                                                                          );
        }


        static void createSpellcastingAndExtraSpells()
        {
            eldritch_knight_spell_casting = Helpers.SpellcastingBuilder.createSpontaneousSpellcasting("FighterSubclassEldritchKnightSpellcasting",
                                                                                              "",
                                                                                              "Feature/&FighterSubclassEldritchKnightSpellcastingTitle",
                                                                                              "Feature/&FighterSubclassEldritchKnightSpellcastingDescription",
                                                                                              DatabaseHelper.SpellListDefinitions.SpellListWizard,
                                                                                              Helpers.Stats.Intelligence,
                                                                                              new List<int> {0, 0, 2, 2, 2, 2, 2, 2, 2, 3,
                                                                                                             3, 3, 3, 3, 3, 3, 3, 3, 3, 3 },
                                                                                              new List<int> { 0,  0,  2,  3,  3,  3,  4,  4,  4,  5,
                                                                                                              6,  6,  7,  7,  7,  8,  8,  8,  9,  9},
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
            eldritch_knight_spell_casting.restrictedSchools = new List<string>() {Helpers.SpellSchools.Abjuration, Helpers.SpellSchools.Evocation };
            eldritch_knight_spell_casting.SetSpellCastingLevel(-1);
            eldritch_knight_spell_casting.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass);
            eldritch_knight_spell_casting.replacedSpells = new List<int> {0, 0, 0, 1, 1, 1, 1, 1, 1, 1,
                                                                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

            var any_spell_selection_levels = new int[] { 3, 8, 14, 20 };
            foreach (var lvl in any_spell_selection_levels)
            {
                any_school_spell_selection[lvl] = Helpers.ExtraSpellSelectionBuilder.createExtraSpellSelection("FighterSubclassEldritchKnightAnySpellSelection" + lvl.ToString(),
                                                                                                            "",
                                                                                                            "Feature/&FighterSubclassEldritchKnightAnySpellSelectionTitle",
                                                                                                            "Feature/&FighterSubclassEldritchKnightAnySpellSelectionDescription",
                                                                                                            DatabaseHelper.CharacterClassDefinitions.Fighter,
                                                                                                            lvl,
                                                                                                            1,
                                                                                                            DatabaseHelper.SpellListDefinitions.SpellListWizard
                                                                                                            );
            }
        }
    }
}
