using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolastaExtraContent.Patches
{
    class GameLocationCharacterPatcher
    {
        [HarmonyPatch(typeof(GameLocationCharacter), "StartBattleTurn")]
        class GameLocationCharacter_StartBattleTurn
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = instructions.ToList();
                var remove_condition = codes.FindLastIndex(x => x.opcode == System.Reflection.Emit.OpCodes.Callvirt && x.operand.ToString().Contains("RemoveCondition"));
                codes[remove_condition] = new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Ldarg_0);
                codes.Insert(remove_condition + 1, new HarmonyLib.CodeInstruction(System.Reflection.Emit.OpCodes.Call,
                                                   new Action<RulesetCondition, bool, bool, GameLocationCharacter>(maybeRemoveRageCondtion).Method
                                                   )
                                                   );
                return codes.AsEnumerable();
            }

            static void maybeRemoveRageCondtion(RulesetCondition rulesetCondition, bool refresh, bool showGraphics, GameLocationCharacter game_location_character)
            {
                List<RulesetCondition> conditions_to_remove = new List<RulesetCondition>();
                foreach (var cc in game_location_character.rulesetActor.conditionsByCategory)
                {
                    foreach (var c in cc.Value)
                    {
                        if (c.conditionDefinition == Barbarian.shared_rage_condition)
                        {
                            conditions_to_remove.Add(c);
                        }
                    }
                }

                if (conditions_to_remove.Empty())
                {
                    game_location_character.RulesetCharacter.RemoveCondition(rulesetCondition, refresh, showGraphics);
                }
                else
                {
                    foreach (var c in conditions_to_remove)
                    {
                        game_location_character.RulesetCharacter.RemoveCondition(c, refresh, showGraphics);
                    }
                }                   
            }
        }
    }
}
