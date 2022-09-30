using System;
using Harmony12;
using Kingmaker;
using Kingmaker.DLC;
using Kingmaker.GameModes;

namespace CustomNpcPortraits
{
    [HarmonyPatch(typeof(BlueprintDlcReward), "IsAvailable", MethodType.Getter)]
    internal static class BlueprintDlcReward_IsAvailable_Patch
    {
        private static bool Prefix(ref bool __result)
        {
            if (!Main.enabled)
                return true;
            try
            {
                __result = true;
                return false;

            }
            catch (Exception e)
            {
                Main.DebugError(e);
                return true;
            }
        }
    }
}