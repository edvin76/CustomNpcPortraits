﻿
using   System;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony12;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root;
using Kingmaker.GameModes;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using UnityEngine.SceneManagement;

namespace CustomNpcPortraits
{
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(BlueprintUnit), "PortraitSafe", MethodType.Getter)]
    public static class GetPortraitSafe_Patch
    {
        // Token: 0x06000010 RID: 16 RVA: 0x00002718 File Offset: 0x00000918

        public static bool Prefix(BlueprintUnit __instance, ref BlueprintPortrait __result, BlueprintPortrait ___m_Portrait)
        {
            /*
              Main.DebugLog("Getportraitsafe_patch()");
            Main.DebugLog("GetPortraitSafe() : " + __instance.CharacterName);
            Main.DebugLog("GetPortraitSafe() Game.Instance.CurrentMode : " + Game.Instance.CurrentMode);
            Main.DebugLog("GetPortraitSafe() SceneManager.GetActiveScene().name : " + SceneManager.GetActiveScene().name);
            */
           // Main.DebugLog("__instance.CharacterName : " + __instance.CharacterName);


            if (!Main.enabled)
            {
                return true;
            }
            try
            {
                if (Game.Instance.CurrentMode == GameModeType.Dialog)
                {

                    if ((Game.Instance.DialogController.CurrentSpeakerName.Equals("Wirlong Black Mask") && __instance.CharacterName.Equals("Wirlong Black Mask")) ||
                        (Game.Instance.DialogController.CurrentSpeakerName.Equals("Nulkineth") && __instance.CharacterName.Equals("Nulkineth")))
                    {
                        string characterName = __instance.CharacterName;

                        string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();
                        string portraitDirectoryName = characterName;

                      //  string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);
                        string portraitDirectoryPath = GetPortrait_Patch.GetUnitPortraitPath(__instance, portraitDirectoryName);


                        if (Main.settings.AutoBackup && !File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), Main.mediumName)))
                        {
                            if ((___m_Portrait != null) && (___m_Portrait.Data != null))
                            {
                                // Main.DebugLog("Getportrait() 4");

                                SpriteLink mHalfLengthImage = ___m_Portrait.Data.m_HalfLengthImage;

                                if ((mHalfLengthImage != null) && (mHalfLengthImage.AssetId != null) && (mHalfLengthImage.AssetId.Length > 5))
                                {
                                    //    Main.DebugLog("Getportrait() 5");



                                    Main.SaveOriginals(__instance, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                                    if (characterName != __instance.name)
                                        Main.SaveOriginals(__instance, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, __instance.name));

                                }
                            }
                        }


                        Directory.CreateDirectory(portraitDirectoryPath);

                        if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
                        {
                            //  Main.SaveOriginals2(__result, portraitDirectoryPath);
                            return true;
                        }


                        PortraitData Data = new PortraitData(portraitDirectoryPath);
                        BlueprintPortrait bp = new BlueprintPortrait();

                        bp.Data = Data;

                        __result = bp;
                        return false;


                    }

                }

                if ((Game.Instance.CurrentMode == GameModeType.GlobalMap || Game.Instance.CurrentMode == GameModeType.TacticalCombat || Game.Instance.CurrentMode == GameModeType.Kingdom) && !__instance.IsCompanion && !Main.pauseGetPortraitsafe)
                {

                    //var dirs = Directory.GetDirectories(Main.GetArmyPortraitsDirectory());

                    // if (dirs.Contains(characterName))
                    //  {}
                    string characterName = __instance.CharacterName;

                    string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                    string portraitDirectoryName = characterName;

                    string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);



                    Directory.CreateDirectory(portraitDirectoryPath);

                    if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
                    {
                        //  Main.SaveOriginals2(__result, portraitDirectoryPath);

                        return true;
                    }


                    PortraitData Data = new PortraitData(portraitDirectoryPath);
                    BlueprintPortrait bp = new BlueprintPortrait();

                    bp.Data = Data;

                    __result = bp;
                    return false;

                }
                else
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

        // Token: 0x06000010 RID: 16 RVA: 0x00002718 File Offset: 0x00000918

        public static void Postfix(BlueprintUnit __instance, ref BlueprintPortrait __result, BlueprintPortrait ___m_Portrait)
        {
            if (!Main.enabled)
            {
                return;
            }
            try
            {

                if (Main.settings.AutoBackup && Game.Instance.CurrentMode == GameModeType.GlobalMap && !__instance.IsCompanion && !Main.pauseGetPortraitsafe)
                {
                    //Main.DebugLog("GetPortraitSafeGeneral() : " + characterName);

                    //var dirs = Directory.GetDirectories(Main.GetArmyPortraitsDirectory());

                    // if (dirs.Contains(characterName))
                    //  {}
                    string characterName = __instance.CharacterName;

                    string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                    string portraitDirectoryName = characterName;

                    string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);


                    

             //       Main.DebugLog(__instance.name);
             //       Main.DebugLog(__instance.Name);

                    Directory.CreateDirectory(portraitDirectoryPath);
                    Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, "Game Default Portraits"));
                    

                    if (!File.Exists(Path.Combine(portraitDirectoryPath, "Game Default Portraits", "Medium.png")))
                    {

                        //BlueprintPortrait bp = ___m_Portrait;
                        BlueprintPortrait bp = __result;

                        Main.SaveOriginals2(bp.Data, portraitDirectoryPath);
                        return;
                    }


                    return;

                }
                else
                {

                    return;
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
                return;

            }
        }

    }
}
