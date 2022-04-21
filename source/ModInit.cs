using Harmony;
using System;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;

namespace BTMaxArmor
{

    public static class Mod
    {

        public const string HarmonyPackage = "BTMaxArmor";
        public const string LogName = "BTMaxArmor";

        public static string ModDir;

        internal static ModConfig Settings;

        public static void Init(string modDirectory, string settings)
        {
            ModDir = modDirectory;

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            try
            {
                Settings = JsonConvert.DeserializeObject<ModConfig>(settings);
            }
            catch (Exception e)
            {
                Settings = new ModConfig();
            }

            var harmony = HarmonyInstance.Create(HarmonyPackage);

            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
