
using   System;
using System.IO;
using System.Linq;
using System.Reflection;
using Harmony12;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Root;
using Kingmaker.Cheats;
using Kingmaker.GameModes;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using UnityEngine.SceneManagement;
using ExtensionMethods;
using Kingmaker.Utility;
using Kingmaker.Controllers.Dialog;
using System.Collections.Generic;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.PubSubSystem;

namespace CustomNpcPortraits
{
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(BlueprintUnit), "PortraitSafe", MethodType.Getter)]
    public static class GetPortraitSafe_Patch
    {
        // Token: 0x06000010 RID: 16 RVA: 0x00002718 File Offset: 0x00000918

        public static bool Prefix(BlueprintUnit __instance, ref BlueprintPortrait __result, BlueprintPortrait ___m_Portrait)
        {
            //return true;
            // if (!Main.areaLoaded) return true;

           // Main.DebugLog("GetPortraitSafe() : " + __instance.CharacterName);

            
            //  Main.DebugLog("GetPortraitSafe() SceneManager.GetActiveScene().name : " + SceneManager.GetActiveScene().name);
            //               Main.DebugLog("Game.Instance.CurrentMode : " + Game.Instance.CurrentMode);


            if (!Main.enabled || Main.pauseGetPortraitsafe)
            {
                return true;
            }


           // Main.DebugLog("GetPortraitSafe() 1");
            try
            {
                
                if ((Game.Instance.CurrentMode == GameModeType.GlobalMap || Game.Instance.CurrentMode == GameModeType.CutsceneGlobalMap )
&&                        Main.companions.Contains(__instance.CharacterName))
                {
                   // Main.DebugLog("getportraitsafe: in");
                    string characterName = Main.GetCompanionDirName(__instance.CharacterName);
                    string prefix = Main.GetCompanionPortraitDirPrefix();
                    string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
                   // Main.DebugLog("2");
 
                    string portraitDirectoryName = prefix + characterName;

                  // 	Main.DebugLog("2b - "+Path.Combine(portraitsDirectoryPath, portraitDirectoryName));
                    string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                   // Main.DebugLog("2c");

                    Directory.CreateDirectory(portraitDirectoryPath);

                  //  Main.DebugLog("3");

                    BlueprintPortrait blueprintPortrait = ___m_Portrait;


                    if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
                    {
                        string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

                        string dir = Path.GetFileNameWithoutExtension(dirs[0]);
                        // Main.DebugLog(dir);
                        if (!dir.Equals("root"))
                            portraitDirectoryPath = Path.Combine(portraitDirectoryPath, dir);

                    }

                    // Main.DebugLog("4");


                    if (blueprintPortrait != null && blueprintPortrait.Data != null && blueprintPortrait.Data.IsCustom && !blueprintPortrait.Data.CustomId.IsNullOrEmpty())
                    {
                        if(File.Exists(Path.Combine(blueprintPortrait.Data.CustomId,"Medium.png")))
                        portraitDirectoryPath = blueprintPortrait.Data.CustomId;

                      //  Main.DebugLog("found custom at: " + blueprintPortrait.Data.CustomId);

                    }
                    bool missing = false;

                    //Main.DebugLog("6");

                    foreach (string fileName in Main.PortraitFileNames)
                    {
                        if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
                        {
                            missing = true;
                        }
                    }

                    //Main.DebugLog("7");


                    if (!missing)
                    {
                       // Main.DebugLog("8");

                        blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;

                        CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
                        CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
                        CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));
                        //Main.DebugLog("SetPortrait() 9");

                        blueprintPortrait.Data = new PortraitData(portraitDirectoryPath);

                        //	Main.DebugLog("SetPortrait() 10");

                        __result = blueprintPortrait;
                        return false;
         

                
                    }
                    else
                        return true;
                }
                

                // Main.DebugLog("GetPortraitSafe() 2");
                if (Game.Instance.CurrentMode == GameModeType.Cutscene || Game.Instance.CurrentMode == GameModeType.Dialog)
                {
                    //   Main.DebugLog("GetPortraitSafe() 3");
                    if (Main.settings.ManageCompanions && (Game.Instance.DialogController != null) && (Game.Instance.DialogController.CurrentSpeaker != null) && __instance.CharacterName.Equals("Nenio") && Game.Instance.DialogController.CurrentCue.AssetGuid.ToString().Equals("45450b2f327797e41bce701b91118cb4"))
                    {

                     //   CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
                     //   EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);

                        /*
                         Main.DebugLog("GetportraitSafe we are in for Nenio renamed to NenioFox!");
                //        Main.DebugLog("GetPortraitSafe() 4");

                        string characterName = "NenioFox_Portrait";
                        string prefix = Main.GetCompanionPortraitDirPrefix();
                        string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
                        string portraitDirectoryName = prefix + characterName;
                        string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                        if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
                        {
                            string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

                            string dir = Path.GetFileNameWithoutExtension(dirs[0]);
                            //Main.DebugLog(dir);
                            if (!dir.Equals("root"))
                                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName, dir);
                 //           Main.DebugLog(portraitDirectoryPath);
                        }

                        bool missing = false;
              //               Main.DebugLog("Getportraitsafe() 4");
                        // Main.DebugLog(Main.PortraitFileNames[0]);

                        foreach (string fileName in Main.PortraitFileNames)
                        {
                            //  Main.DebugLog("p: "+portraitDirectoryPath);

                            //   Main.DebugLog("f: " + fileName);


                            if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
                            {

                                missing = true;
                                break;
                            }
                        }
                   //            Main.DebugLog("Getportraitsafe() 7");

                        if (!missing)
                        {

                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));



                            PortraitData Data = new PortraitData(portraitDirectoryPath);
                  

                            BlueprintPortrait bp = new BlueprintPortrait();
                            bp.Data = Data;

                            __result = bp;
                            //   Main.DebugLog("Getportraitsafe() loaded portrait for Companion " + characterName);

                            //  Main.DebugLog("IsPolymorphed: " + Game.Instance.DialogController.CurrentSpeaker.Body.IsPolymorphed.ToString());

                            //Game.Instance.DialogController.CurrentSpeaker.UISettings.SetPortrait(bp);


   

                            return false;
                        }
                        else
                        {
                            Main.DebugLog("Getportraitsafe() 6");

                            __result = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");

                            return false;
                        }


                        */
                        return true;
                    }
                    //     Main.DebugLog("GetPortraitSafe() 7");

                    if ((Game.Instance.DialogController.CurrentSpeakerName.Equals("Wirlong Black Mask") && __instance.CharacterName.Equals("Wirlong Black Mask"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Nulkineth") && __instance.CharacterName.Equals("Nulkineth"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Suture") && __instance.CharacterName.Equals("Suture"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Kenabres Crusader") && __instance.CharacterName.Equals("Kenabres Crusader"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Commoner") && __instance.CharacterName.Equals("Commoner"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Marilith") && __instance.CharacterName.Equals("Marilith"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Balor") && __instance.CharacterName.Equals("Balor"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Zermangaleth") && __instance.CharacterName.Equals("Zermangaleth"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Langrat Messini") && __instance.CharacterName.Equals("Langrat Messini"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Terta") && __instance.CharacterName.Equals("Terta"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Baphomet Cultist") && __instance.CharacterName.Equals("Baphomet Cultist"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Eagle Watch Crusader") && __instance.CharacterName.Equals("Eagle Watch Crusader"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Rvveg") && __instance.CharacterName.Equals("Rvveg"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Succubus Guard") && __instance.CharacterName.Equals("Succubus Guard"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Slave") && __instance.CharacterName.Equals("Slave"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Cavalry Sculptor") && __instance.CharacterName.Equals("Cavalry Sculptor"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Halfling Warrior") && __instance.CharacterName.Equals("Halfling Warrior"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Skerenthal the Rock Cleaver") && __instance.CharacterName.Equals("Skerenthal the Rock Cleaver"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Red Mask") && __instance.CharacterName.Equals("Red Mask"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("The Sinner") && __instance.CharacterName.Equals("The Sinner"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Mad Glowworm") && __instance.CharacterName.Equals("Mad Glowworm"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Morevet Honeyed Tongue") && __instance.CharacterName.Equals("Morevet Honeyed Tongue"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Ygefeles") && __instance.CharacterName.Equals("Ygefeles"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Lathimas") && __instance.CharacterName.Equals("Lathimas"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Fulsome Queen") && __instance.CharacterName.Equals("Fulsome Queen"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Shadow") && __instance.CharacterName.Equals("Shadow"))


                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Katair") && __instance.CharacterName.Equals("Katair"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Nocticula") && __instance.CharacterName.Equals("Nocticula"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Areelu Vorlesh") && __instance.CharacterName.Equals("Areelu Vorlesh"))


                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Arsinoe") && __instance.CharacterName.Equals("Arsinoe"))

                       //    || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Crinukh") && __instance.CharacterName.Equals("Crinukh"))


                       )
                    {
                               // Main.DebugLog("GetPortraitSafe() 1");


                        string characterName = __instance.CharacterName;

                        string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();
                        string portraitDirectoryName = characterName;

                        string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                        Directory.CreateDirectory(portraitDirectoryPath);

                        if (GetPortrait_Patch.GetUnitPortraitPath(__instance, portraitDirectoryName)?.Length > 3)
                        portraitDirectoryPath = GetPortrait_Patch.GetUnitPortraitPath(__instance, portraitDirectoryName);

                       // Main.DebugLog("GetPortraitSafe() 2");

                        //Directory.CreateDirectory(portraitDirectoryPath);
                        //Main.DebugLog("GetPortraitSafe() 3");

                        if (Main.settings.AutoBackup && !File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), Main.mediumName)))
                        {
                          //  Main.DebugLog("GetPortraitSafe() 3b");


                            Main.SaveOriginals(__instance, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                            if (characterName != __instance.name)
                                Main.SaveOriginals(__instance, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, __instance.name));

                            /*
                            if ((___m_Portrait != null) && (___m_Portrait.Data != null))
                            {
                                 Main.DebugLog("GetPortraitSafe() 4");

                                SpriteLink mHalfLengthImage = ___m_Portrait.Data.m_HalfLengthImage;

                                if ((mHalfLengthImage != null) && (mHalfLengthImage.AssetId != null) && (mHalfLengthImage.AssetId.Length > 5))
                                {
                                       Main.DebugLog("GetPortraitSafe() 5");



                           

                                }
                            }*/
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
                    else
                        return true;

                }
                else
                {
                    //          Main.DebugLog("GetPortraitSafe() 7");
                    if ((Game.Instance.CurrentMode == GameModeType.CutsceneGlobalMap
                        || Game.Instance.CurrentMode == GameModeType.GlobalMap
                        || Game.Instance.CurrentMode == GameModeType.TacticalCombat
                        || Game.Instance.CurrentMode == GameModeType.Kingdom)
                        && !__instance.IsCompanion)
                    {
                        //          Main.DebugLog("GetPortraitSafe() 8");

                        // Main.DebugLog("setportraits prefix Army");
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
           // Main.DebugLog(__instance.CharacterName);
          //  Main.DebugLog(___m_CustomPortrait?.name);

            if (!Main.enabled)
            {
                return;
            }
            try
            {
 
                
            


                if (Main.settings.AutoBackup && Game.Instance.CurrentMode == GameModeType.GlobalMap && !__instance.IsCompanion && !Main.pauseGetPortraitsafe)
                {
                   // Main.DebugLog("GetPortraitSafeArmy() postfix: ");
                    
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
