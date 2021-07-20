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

namespace SolastaExtraContent
{
    public class Main
    {
        [Conditional("DEBUG")]
        internal static void Log(string msg) => Logger.Log(msg);
        internal static void Error(Exception ex) => Logger?.Error(ex.ToString());
        internal static void Error(string msg) => Logger?.Error(msg);
        internal static UnityModManager.ModEntry.ModLogger Logger { get; private set; }

        internal static void LoadTranslations()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo($@"{UnityModManager.modsPath}/SolastaExtraContent");
            FileInfo[] files = directoryInfo.GetFiles($"Translations-??.txt");

            foreach (var file in files)
            {
                var filename = $@"{UnityModManager.modsPath}/SolastaExtraContent/{file.Name}";
                var code = file.Name.Substring(13, 2);
                var languageSourceData = LocalizationManager.Sources[0];
                var languageIndex = languageSourceData.GetLanguageIndexFromCode(code);

                if (languageIndex < 0)
                    Main.Error($"language {code} not currently loaded.");
                else
                    using (var sr = new StreamReader(filename))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            var splitted = line.Split(new[] { '\t', ' ' }, 2);
                            var term = splitted[0];
                            var text = splitted[1];
                            languageSourceData.AddTerm(term).Languages[languageIndex] = text;
                        }
                    }
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
            Cantrips.create();
            Spells.create();
            EldritchKnight.create();
        }
    }
}

