using System;
using Harmony12;
using Kingmaker.UI.Kingdom;
using Kingmaker.GameModes;
using UnityEngine.UI;
using UnityEngine;
using Kingmaker.UI;
using System.Reflection;

namespace CustomNpcPortraits
{
    [HarmonyPatch(typeof(KingdomUILeaderCharacterController), "SetPortrait")]
    public static class KingdomUILeaderCharacterController_SetPortrait_Patch
    {
        private static void Postfix(KingdomUILeaderCharacterController __instance)
        {
            if (!Main.enabled)
                return;
            //Main.prevMode = GameModeType.None;
            try
            {

                Type baseType = typeof(KingdomUILeaderCharacterController).BaseType;


                baseType.GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, null);


            }
            catch (Exception e)
            {
                Main.DebugError(e);
            }
        }
    }
}
