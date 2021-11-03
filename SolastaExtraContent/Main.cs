using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityModManagerNet;
using HarmonyLib;
using I2.Loc;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaExtraContent;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolastaExtraContent
{
    public class Main
    {
        [Conditional("DEBUG")]
        internal static void Log(string msg) => Logger.Log(msg);
        internal static void Error(Exception ex) => Logger?.Error(ex.ToString());
        internal static void Error(string msg) => Logger?.Error(msg);
        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        public class Settings
        {
            public bool fix_cleric_subclasses { get; }
            public bool use_intelligence_as_main_stat_for_warlock { get; }
            public bool fix_barbarian_unarmed_defense_stacking { get; }
            public bool use_staff_as_arcane_or_druidic_focus { get; }
            public bool allow_control_summoned_creatures { get; }

            internal Settings()
            {

                using (StreamReader settings_file = File.OpenText(UnityModManager.modsPath + @"/SolastaExtraContent/settings.json"))
                using (JsonTextReader reader = new JsonTextReader(settings_file))
                {
                    JObject jo = (JObject)JToken.ReadFrom(reader);
                    fix_cleric_subclasses = (bool)jo["fix_cleric_subclasses"];
                    use_intelligence_as_main_stat_for_warlock = (bool)jo["use_intelligence_as_main_stat_for_warlock"];
                    fix_barbarian_unarmed_defense_stacking = (bool)jo["fix_barbarian_unarmed_defense_stacking"];
                    use_staff_as_arcane_or_druidic_focus = (bool)jo["use_staff_as_arcane_or_druidic_focus"];
                    allow_control_summoned_creatures = (bool)jo["allow_control_summoned_creatures"];
                }
            }
        }
        static public Settings settings = new Settings();

        internal static void LoadTranslations()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($@"{UnityModManager.modsPath}/SolastaExtraContent/Translations");
            var directories = directoryInfo.GetDirectories();

            foreach (var dir in directories)
            {
                SolastaModHelpers.Translations.Load(dir.FullName);
            }
        }

        internal static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                Logger = modEntry.Logger;

                LoadTranslations();

                var harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Error(ex);
                throw;
            }

            return true;
        }

        internal static void ModEntryPoint()
        {
            Misc.run();
            Races.create();
            Cantrips.create();
            Spells.create();
            EldritchKnight.create();
            LoremasterFix.run();
            if (settings.fix_cleric_subclasses)
            {
                Main.Logger.Log("Fixing cleric subclasses");
                FixCleric.run();
            }
            Barbarian.create();
            Druid.create();

            AlchemistClassBuilder.BuildAndAddClassToDB();
            BardClassBuilder.BuildAndAddClassToDB();
            MonkClassBuilder.BuildAndAddClassToDB();
            WarlockClassBuilder.BuildAndAddClassToDB();
        }
    }
}

