using SolastaModApi;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LoremasterFix
    {
        public static FeatureDefinitionFeatureSet pursuit_of_knowledge;

        internal static void run()
        {
            string title_string = "Feature/&TraditionLoremasterSubclassPursuitOfKnowledgeTitle";
            string description_string = "Feature/&TraditionLoremasterSubclassPursuitOfKnowledgeDescription";

            var loremaster = DatabaseHelper.CharacterSubclassDefinitions.TraditionLoremaster;

            var cantrip_spelllist = Helpers.SpelllistBuilder.createCombinedSpellListWithLevelRestriction("TraditionLoremasterSubclassPursuitOfKnowledgeCantripSpelllist", "", "",
                                                                         (DatabaseHelper.SpellListDefinitions.SpellListCleric, 0),
                                                                         (DatabaseHelper.SpellListDefinitions.SpellListSorcerer, 0),
                                                                         (DatabaseHelper.SpellListDefinitions.SpellListWizard, 0)
                                                                         );
            Helpers.Misc.addSpellToSpelllist(cantrip_spelllist, Cantrips.vicious_mockery);
            Helpers.Misc.addSpellToSpelllist(cantrip_spelllist, Cantrips.shillelagh);
            Helpers.Misc.addSpellToSpelllist(cantrip_spelllist, Cantrips.acid_claws);
            var extra_cantrip = Helpers.ExtraSpellSelectionBuilder.createExtraCantripSelection("TraditionLoremasterSubclassPursuitOfKnowledgeCantrip",
                                                                                            "",
                                                                                            Common.common_no_title,
                                                                                            Common.common_no_title,
                                                                                            DatabaseHelper.CharacterClassDefinitions.Wizard,
                                                                                            2,
                                                                                            1,
                                                                                            cantrip_spelllist
                                                                                            );

            var lvl1_spelllist = Helpers.SpelllistBuilder.createCombinedSpellListWithLevelRestriction("TraditionLoremasterSubclassPursuitOfKnowledgeLvl1Spelllist", "", "",
                                                             (DatabaseHelper.SpellListDefinitions.SpellListCleric, 10),
                                                             (DatabaseHelper.SpellListDefinitions.SpellListSorcerer, 10),
                                                             (DatabaseHelper.SpellListDefinitions.SpellListRanger, 10),
                                                             (DatabaseHelper.SpellListDefinitions.SpellListPaladin, 10),
                                                             (DatabaseHelper.SpellListDefinitions.SpellListWizardGreenmage, 10),
                                                             (DatabaseHelper.SpellListDefinitions.SpellListWizard, 10)
                                                             );
            Helpers.Misc.addSpellToSpelllist(lvl1_spelllist, Spells.hellish_rebuke);
            Helpers.Misc.addSpellToSpelllist(lvl1_spelllist, Spells.vulnerability_hex);
            lvl1_spelllist.spellsByLevel[0].spells.Clear();
            var extra_lvl1_spell = Helpers.ExtraSpellSelectionBuilder.createExtraSpellSelection("TraditionLoremasterSubclassPursuitOfKnowledgeSpell",
                                                                                                "",
                                                                                                Common.common_no_title,
                                                                                                Common.common_no_title,
                                                                                                DatabaseHelper.CharacterClassDefinitions.Wizard,
                                                                                                2,
                                                                                                1,
                                                                                                lvl1_spelllist
                                                                                                );

            pursuit_of_knowledge = Helpers.FeatureSetBuilder.createFeatureSet("TraditionLoremasterSubclassPursuitOfKnowledge",
                                                            "",
                                                            title_string,
                                                            description_string,
                                                            false,
                                                            FeatureDefinitionFeatureSet.FeatureSetMode.Union,
                                                            false,
                                                            extra_cantrip,
                                                            extra_lvl1_spell
                                                            );
            loremaster.featureUnlocks.Insert(1, new FeatureUnlockByLevel(pursuit_of_knowledge, 2));
        }
    }
}
