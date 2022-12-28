using System;
using System.Linq;
using Harmony12;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.GameModes;

namespace CustomNpcPortraits
{
    [HarmonyPatch(typeof(Player), "OnAreaLoaded", new Type[] { })]
    internal static class Player_OnAreaLoaded_Patch
    {
        private static void Postfix()
        {
            if (!Main.enabled || !Main.settings.ManageCompanions)
                return;
            try
            {
                //ResourcesLibrary.TryUnloadResource("45450b2f327797e41bce701b91118cb4");
                /*
                if (Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys.FirstOrDefault(x => x.name.Contains("Fox")))
                    GetPortrait_Patch.maxNenioFixRun = 5;
                else
                    GetPortrait_Patch.maxNenioFixRun = 0;
                */
                Main.prevMode = GameModeType.None;
               // Main.DebugLog("Refresh companion potraits called after arealoded.");

                Main.SafeLoad(new Action(Main.SetPortraits), "Apply custom portraits after saved game loaded");
            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }
        }
    }
}