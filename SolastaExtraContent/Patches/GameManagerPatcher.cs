using HarmonyLib;
using SolastaModHelpers;
using UnityModManagerNet;

namespace SolastaExtraContent.Patches
{
    class GameManagerPatcher
    {
        [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
        internal static class GameManager_BindPostDatabase_Patch
        {
            internal static void Postfix()
            {
#if DEBUG
                bool allow_guid_generation = true;
#else
                bool allow_guid_generation = false; //no guids should be ever generated in release
#endif
                GuidStorage.load(Properties.Resources.blueprints, allow_guid_generation);
                Main.ModEntryPoint();

#if DEBUG
                string guid_file_name = ProjectPath.ProjectPath.Path + "blueprints.txt";
                GuidStorage.dump(guid_file_name);
#endif
                GuidStorage.dump($@"{UnityModManager.modsPath}/SolastaExtraContent/loaded_blueprints.txt");
            }
        }
    }
}
