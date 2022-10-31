using System;
using Harmony12;
using Kingmaker;
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

                Main.prevMode = GameModeType.None;
                Main.DebugLog("Refresh companion potraits called after arealoded.");

                Main.SafeLoad(new Action(Main.SetPortraits), "Apply custom portraits after saved game loaded");
            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }
        }
    }
}