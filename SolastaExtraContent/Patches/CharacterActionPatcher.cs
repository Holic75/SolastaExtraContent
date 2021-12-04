using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolastaExtraContent.Patches
{
    //allow executing custom actions
    [HarmonyPatch(typeof(CharacterAction), "InstantiateAction")]
    class CharacterActionPatcher
    {
        internal static bool Prefix(CharacterActionParams actionParams, ref CharacterAction __result)
        {
            var type_name = nameof(CharacterAction) + (string.IsNullOrEmpty(actionParams.ActionDefinition.ClassNameOverride) ? actionParams.ActionDefinition.Name : actionParams.ActionDefinition.ClassNameOverride);
            var type = System.Type.GetType(type_name);
            if (type != null)
            {
                //action type is defined locally
                __result = Activator.CreateInstance(type, (object)actionParams) as CharacterAction;
                return false;
            }

            return true;
        }
    }



}
