using BepInEx;
using BepInEx.Logging;
using BetterScrapScan.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterScrapScan
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class BetterScrapScanBase : BaseUnityPlugin
    {
        private const string modGUID = "BetterScrapScan";
        private const string modName = "Better Scrap Scan";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static BetterScrapScanBase modInstance;

        internal ManualLogSource mls;

        void Awake()
        {
            if (modInstance == null)
            {
                modInstance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("BetterScrapScan mod has awaken");

            harmony.PatchAll(typeof(BetterScrapScanBase));
            harmony.PatchAll(typeof(TerminalPatch));
        }
    }
}
