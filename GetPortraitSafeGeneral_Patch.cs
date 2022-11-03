
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony12;
using Kingmaker;
using Kingmaker.Armies;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root;
using Kingmaker.GameModes;
using Kingmaker.Localization;
using UnityEngine.SceneManagement;

namespace CustomNpcPortraits
{
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(BlueprintArmyLeader), "PortraitSafe", MethodType.Getter)]
    public static class GetPortraitSafeGeneral_Patch
    {
        // Token: 0x06000010 RID: 16 RVA: 0x00002718 File Offset: 0x00000918

        public static bool Prefix(BlueprintArmyLeader __instance, ref BlueprintPortrait __result, BlueprintPortraitReference ___m_Portrait)
        {
            //  Main.DebugLog("GetPortraitSafeGeneral_Patch()");
            if (!Main.enabled)
            {
                return true;
            }
            try
            {
               
                    string characterName = __instance.LeaderName;
                    //Main.DebugLog("GetPortraitSafeGeneral() : " + characterName);

                    //var dirs = Directory.GetDirectories(Main.GetArmyPortraitsDirectory());

                    // if (dirs.Contains(characterName))
                    //  {}

                    string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                    string portraitDirectoryName = characterName;

                    string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                    Directory.CreateDirectory(portraitDirectoryPath);

                    if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
                    {

                        return true;
                    }

                    PortraitData Data = new PortraitData(portraitDirectoryPath);
                    BlueprintPortrait bp = new BlueprintPortrait();

                    bp.Data = Data;

                    __result = bp;
                    return false;
                    /*

                        Directory.CreateDirectory()

                        if (__instance == null || string.IsNullOrEmpty(__instance.LeaderName) )
                        {
                            return true;
                        }





                        // if (dirs.Contains(characterName))
                        //  {
                        /*
                        Main.DebugLog("Getportrait() : " + Game.Instance.CurrentMode);
                        string name = SceneManager.GetActiveScene().name;
                        Main.DebugLog("SceneManager.GetActiveScene().name: " + name);
                        Main.DebugLog("Game.Instance.State: " + Game.Instance.State);*/
                    //}

            }
            catch (Exception e)
            {
                Main.DebugError(e);
                return true;

            }
        }

        public static void Postfix(BlueprintArmyLeader __instance, ref BlueprintPortrait __result, BlueprintPortraitReference ___m_Portrait)
        {
            //  Main.DebugLog("GetPortraitSafeGeneral_Patch()");
            if (!Main.enabled)
            {
                return;
            }
            try
            {
                if (Main.settings.AutoBackup)
                {
                    string characterName = __instance.LeaderName;
                    //Main.DebugLog("GetPortraitSafeGeneral() : " + characterName);

                    //var dirs = Directory.GetDirectories(Main.GetArmyPortraitsDirectory());

                    // if (dirs.Contains(characterName))
                    //  {}

                    string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                    string portraitDirectoryName = characterName;

                    string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                    Directory.CreateDirectory(portraitDirectoryPath);

                    if (!File.Exists(Path.Combine(portraitDirectoryPath, "Game Default Portraits", "Medium.png")))
                    {
                        // Main.DebugLog("GetPortraitSafeGeneral() -> Saveoriginals2 ");

                        BlueprintPortrait bp = ___m_Portrait;
                        Main.SaveOriginals2(bp.Data, portraitDirectoryPath);

                        return;
                    }
                    else
                    {
                        return;
                    }

                }
                else
                {
                    return;
                }

            }
            catch (Exception e)
            {
                Main.DebugError(e);
                return;

            }
        }
    }
}
