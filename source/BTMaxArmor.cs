using Harmony;
using HBS;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using IRBTModUtils.Logging;

namespace BTMaxArmor
{

    public static class Mod
    {
        public const string HarmonyPackage = "BTMaxArmor";
        public const string LogName = "Sysinfo";

        public static DeferringLogger Log;
        public static string ModDir;

        public static readonly Random Random = new Random();

        public static void Init(string modDirectory)
        {
            ModDir = modDirectory;

            Log = new DeferringLogger(modDirectory, LogName, false, false);

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
            Log.Info?.Write($"Assembly version: {fvi.ProductVersion}");

            Log.Debug?.Write($"ModDir is:{modDirectory}");

            var harmony = HarmonyInstance.Create(HarmonyPackage);

            // Enable DEBUG below to print a log of emitted IL to the desktop. Useful for debugging transpilers
            //HarmonyInstance.DEBUG = true;
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            // Setup the diag for me 
            //Helper.DiagnosticLogger.PatchAllMethods();
        }
    }
}
