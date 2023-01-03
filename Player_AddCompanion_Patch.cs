using System;
using Harmony12;
using Kingmaker;
using Kingmaker.GameModes;

namespace CustomNpcPortraits
{
    [HarmonyPatch(typeof(Player), "AddCompanion")]
    public static class Player_AddCompanion_Patch
    {
        private static void Postfix()
        {
            if (!Main.enabled || !Main.settings.ManageCompanions)
                return;
            //Main.prevMode = GameModeType.None;
            try
            {
              // Main.DebugLog("Refresh companion potraits called after addcompanion.");

              //  Main.SafeLoad(new Action(Main.SetPortraits), "Apply custom portraits after new companion added");

            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }
        }
    }
}
