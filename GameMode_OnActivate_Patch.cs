using System;
using System.Linq;
using Harmony12;
//using Harmony12;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.GameModes;
using Kingmaker.UI.Common;

namespace CustomNpcPortraits
{
    [HarmonyPatch(typeof(GameMode), "OnActivate")]
    public static class GameMode_OnActivate_Patch
    {
        private static void Postfix()
        {
            if (!Main.enabled || !Main.settings.ManageCompanions)
                return;
            
            try
            {
               //  Main.DebugLog("prevMode:" + Main.prevMode);
                if (Main.prevMode == GameModeType.Dialog &&
                    Game.Instance.CurrentMode != GameModeType.Cutscene
                    /*Game.Instance.CurrentMode != GameModeType.BugReport &&
                    Game.Instance.CurrentMode != GameModeType.EscMode &&
                    Game.Instance.CurrentMode != GameModeType.None &&
                    Game.Instance.CurrentMode != GameModeType.Pause*/ )

                {
                    //Main.DebugLog("Refresh companion potraits called after GameModeType.Dialog. The current GameModeType is: " + Game.Instance.CurrentMode + ":");

                 //  Main.SafeLoad(new Action(Main.SetPortraits), "Apply custom portraits after dialog ended");


                    //Main.DebugLog("SetPortraits()");
                }
                // Main.DebugLog("CurrentMode:" + Game.Instance.CurrentMode.ToString());

                Main.prevMode = Game.Instance.CurrentMode;
            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }
        }
    }


    [HarmonyPatch(typeof(GameMode), "OnDeactivate")]
    public static class GameMode_OnDeactivate_Patch
    {
        private static void Prefix(GameMode __instance)
        {

           // UIUtility.SendWarning("GameMode: "+ __instance.ToString() +" - "+ __instance.Type.ToString() );

           // Main.DebugLog("GameMode: " + __instance.ToString() + " - " + __instance.Type.ToString());

            if (!Main.enabled || !Main.settings.ManageCompanions)
                return;

            try
            {
                if (__instance.Type == GameModeType.Dialog )
                {

                    Main.DebugLog("SetPortraits()");

                    Main.SafeLoad(new Action(Main.SetPortraits), "Apply custom portraits after dialog ended");


                }

            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }
        }
    }
}

