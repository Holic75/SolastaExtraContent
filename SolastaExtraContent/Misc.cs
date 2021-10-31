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
            createStaffFocus();
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
