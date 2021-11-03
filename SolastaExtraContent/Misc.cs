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

    public class Misc
    {
        public static NewFeatureDefinitions.AllowToUseWeaponCategoryAsSpellFocus staff_focus;

        static internal void run()
        {
            if (Main.settings.use_staff_as_arcane_or_druidic_focus)
            {
                createStaffFocus();
            }
            
            if (Main.settings.fix_barbarian_unarmed_defense_stacking)
            {
                fixBarbarianUnarmoredDefense();
            }
        }


        static void fixBarbarianUnarmoredDefense()
        {
            //fix barbarian unarmored defense not to stack with other ac calcualtion methods
            var unarmored_defense = Helpers.FeatureBuilder<NewFeatureDefinitions.ArmorClassStatBonus>.createFeature("BarbarianClassUnarmoredDefense",
                                                                                                    "dcb2cd61-8453-4e45-9df6-9475ebeff88e",
                                                                                                    "Feature/&UnarmoredDefenseTitle",
                                                                                                    "Feature/&UnarmoredDefenseDescription",
                                                                                                    null,
                                                                                                    a =>
                                                                                                    {
                                                                                                        a.armorAllowed = false;
                                                                                                        a.shieldAlowed = true;
                                                                                                        a.stat = Helpers.Stats.Constitution;
                                                                                                        a.exclusive = true;
                                                                                                        a.forbiddenFeatures = new List<FeatureDefinition>
                                                                                                        {
                                                                                                            DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierSorcererDraconicResilienceAC,
                                                                                                            DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierMageArmor,
                                                                                                            DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin
                                                                                                        };
                                                                                                    }
                                                                                                    );

            DatabaseHelper.CharacterClassDefinitions.Barbarian.featureUnlocks
                .First(f => f.featureDefinition == DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierBarbarianUnarmoredDefense)
                .featureDefinition = unarmored_defense;

        }

        static void createStaffFocus()
        {
            staff_focus = Helpers.FeatureBuilder<NewFeatureDefinitions.AllowToUseWeaponCategoryAsSpellFocus>.createFeature("UseStaffAsSpellcastingFocus",
                                                                                                                   "67785d07-4320-419e-8c70-634a67c74e4c",
                                                                                                                   Common.common_no_title,
                                                                                                                   Common.common_no_title,
                                                                                                                   Common.common_no_icon,
                                                                                                                   a =>
                                                                                                                   {
                                                                                                                       a.weaponTypes = new List<string> { Helpers.WeaponProficiencies.QuarterStaff };
                                                                                                                   }
                                                                                                                   );
            staff_focus.guiPresentation.hidden = true;
            DatabaseHelper.CharacterClassDefinitions.Wizard.FeatureUnlocks.Insert(0, new FeatureUnlockByLevel(staff_focus, 1));
            DatabaseHelper.CharacterClassDefinitions.Sorcerer.FeatureUnlocks.Insert(0, new FeatureUnlockByLevel(staff_focus, 1));
            DatabaseHelper.CharacterClassDefinitions.Druid.FeatureUnlocks.Insert(0, new FeatureUnlockByLevel(staff_focus, 1));

            Action<RulesetCharacterHero> fix_action = c =>
            {
                if (c.activeFeatures.Any(cc => cc.Value.Contains(staff_focus)))
                {
                    return;
                }

                if (c.classesAndLevels.ContainsKey(DatabaseHelper.CharacterClassDefinitions.Wizard))
                {
                    c.activeFeatures[AttributeDefinitions.GetClassTag(DatabaseHelper.CharacterClassDefinitions.Wizard, 1)].Add(staff_focus);
                }

                if (c.classesAndLevels.ContainsKey(DatabaseHelper.CharacterClassDefinitions.Sorcerer))
                {
                    c.activeFeatures[AttributeDefinitions.GetClassTag(DatabaseHelper.CharacterClassDefinitions.Sorcerer, 1)].Add(staff_focus);
                }
            };

            Common.postload_actions.Add(fix_action);
        }
    }
}
