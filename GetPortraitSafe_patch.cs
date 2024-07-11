
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
using Kingmaker.EntitySystem.Entities;

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

           // Main.DebugLog("GetPortraitSafe() : " + __instance.CharacterName.cleanCharName());

            
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
&&                        Main.companions.Contains(__instance.CharacterName.cleanCharName())
||
(Game.Instance.CurrentMode == GameModeType.Dialog && Main.companions.Contains(__instance.CharacterName.cleanCharName()))
                    )
                {
                    if (!Main.settings.ManageCompanions)
                    {
                        return true;
                    }

                    // Main.DebugLog("getportraitsafe: in");
                    string characterName = Main.GetCompanionDirName(__instance.CharacterName.cleanCharName());
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


                    //UnitEntityData unitEntityData = null;
                    if (Game.Instance.CurrentMode == GameModeType.Dialog &&
                        Game.Instance.DialogController != null &&
                        Game.Instance.DialogController.CurrentSpeaker != null &&
                        File.Exists(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController.Dialog.name, "Medium.png")))
                    {
                        //  unitEntityData = Game.Instance.DialogController.CurrentSpeaker;
                        portraitDirectoryPath = Path.Combine(portraitDirectoryPath, Game.Instance.DialogController.Dialog.name);

                    }
                    else
                    if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
                    {
                        string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

                        string dir = Path.GetFileNameWithoutExtension(dirs[0]);
                        // Main.DebugLog(dir);
                        if (!dir.Equals("root"))
                            portraitDirectoryPath = Path.Combine(portraitDirectoryPath, dir);

                    }
                    else
                    if(File.Exists(Path.Combine(portraitDirectoryPath, __instance.name, "Medium.png")))
                    {
                       // Main.DebugLog("!!!!!!!!!!!!!!!!!" + Path.Combine(portraitDirectoryPath, __instance.name, "Medium.png"));
                            portraitDirectoryPath = Path.Combine(portraitDirectoryPath, __instance.name);
                            //useSetPortrait = false;
                       
                    }
                    else
                    if (blueprintPortrait != null && blueprintPortrait.Data != null && blueprintPortrait.Data.IsCustom && !blueprintPortrait.Data.CustomId.IsNullOrEmpty())
                    {
                        if (File.Exists(Path.Combine(blueprintPortrait.Data.CustomId, "Medium.png")))
                            portraitDirectoryPath = blueprintPortrait.Data.CustomId;

                        //  Main.DebugLog("found custom at: " + blueprintPortrait.Data.CustomId);

                    }



                    // Main.DebugLog("4");














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



                if(__instance.CharacterName.cleanCharName() == Main.FinneanName)
                {

                    string dir = Path.Combine(Main.GetCompanionPortraitsDirectory(), Main.GetCompanionPortraitDirPrefix() + __instance.CharacterName.cleanCharName());
                    Directory.CreateDirectory(dir);


                    if (Main.settings.AutoBackup)
                        if (!File.Exists(Path.Combine(dir, Main.GetDefaultPortraitsDirName(), "Medium.png")))
                            Main.SaveOriginals2((__instance.m_Portrait.GetBlueprint() as BlueprintPortrait).Data, dir);

                    if (File.Exists(Path.Combine(dir, "Medium.png")))
                    {

                        BlueprintPortrait bp = new BlueprintPortrait();

                        bp.Data = new PortraitData(dir);

                        __result = bp;

                        return false;
                    }
                    else
                        return true;
                }

                if (__instance.CharacterName.cleanCharName() == Main.PillarName)
                {

                    string dir = Path.Combine(Main.GetNpcPortraitsDirectory(), __instance.CharacterName.cleanCharName());
                    Directory.CreateDirectory(dir);

                    if (Main.settings.AutoBackup && !__instance.m_Portrait.NameSafe().Equals("-not set-"))
                        if (!File.Exists(Path.Combine(dir, Main.GetDefaultPortraitsDirName(), "Medium.png")))
                        {
                            Main.SaveOriginals2((__instance.m_Portrait.GetBlueprint() as BlueprintPortrait).Data, dir);
                        }



                    if (File.Exists(Path.Combine(dir, "Medium.png")))
                    {
                        BlueprintPortrait bp = new BlueprintPortrait();

                        bp.Data = new PortraitData(dir);

                        __result = bp;

                        return false;
                    }
                    else
                    {

                        return true;

                    }
                }
                // Main.DebugLog("GetPortraitSafe() 2");
                if (Game.Instance.CurrentMode == GameModeType.Cutscene || Game.Instance.CurrentMode == GameModeType.Dialog)
                {
                    /*
                    bool companion = false;
                        //Main.companions.Contains(Game.Instance.DialogController.CurrentSpeakerName.cleanCharName())
                        //|| Game.Instance.Player.AllCharacters.Contains(Main.RealCurrentSpeakerEntity(Game.Instance.DialogController.CurrentSpeakerName.cleanCharName()));



                    if (companion && Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals(__instance.CharacterName.cleanCharName()) )
                    {
                        
                        if(__instance.CharacterName.cleanCharName().Equals(Main.GalfreyName) && !Main.GalfreyChurchDoubled)
                        {
                            if(Game.Instance.DialogController.Dialog.name.Equals("Church_Dialogue") &&
                                Game.Instance.DialogController.CurrentCue.name.Equals("Cue_0022"))
                                {
                                Main.GalfreyChurchDoubled = true;
                           //     CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
                           //     EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);
                                return false;


                            }
                        }
                        
                        BlueprintPortrait bp = new BlueprintPortrait();

                        bp.Data = Main.GetCustomPortrait(Main.RealCurrentSpeakerEntity(__instance.CharacterName.cleanCharName())); ;

                        __result = bp;
                        return false;
                    }
                    */

                    //   Main.DebugLog("GetPortraitSafe() 3");
                    if (Main.settings.ManageCompanions && (Game.Instance.DialogController != null) && (Game.Instance.DialogController.CurrentSpeaker != null) && __instance.CharacterName.cleanCharName().Equals("Nenio") && Game.Instance.DialogController.CurrentCue.AssetGuid.ToString().Equals("45450b2f327797e41bce701b91118cb4"))
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

                    if ((
                        Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals(__instance.CharacterName.cleanCharName()) ||
                        Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Wirlong Black Mask") && __instance.CharacterName.cleanCharName().Equals("Wirlong Black Mask"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Nulkineth") && __instance.CharacterName.cleanCharName().Equals("Nulkineth"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Suture") && __instance.CharacterName.cleanCharName().Equals("Suture"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Kenabres Crusader") && __instance.CharacterName.cleanCharName().Equals("Kenabres Crusader"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Commoner") && __instance.CharacterName.cleanCharName().Equals("Commoner"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Marilith") && __instance.CharacterName.cleanCharName().Equals("Marilith"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Balor") && __instance.CharacterName.cleanCharName().Equals("Balor"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Zermangaleth") && __instance.CharacterName.cleanCharName().Equals("Zermangaleth"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Langrat Messini") && __instance.CharacterName.cleanCharName().Equals("Langrat Messini"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Terta") && __instance.CharacterName.cleanCharName().Equals("Terta"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Baphomet Cultist") && __instance.CharacterName.cleanCharName().Equals("Baphomet Cultist"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Eagle Watch Crusader") && __instance.CharacterName.cleanCharName().Equals("Eagle Watch Crusader"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Rvveg") && __instance.CharacterName.cleanCharName().Equals("Rvveg"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Succubus Guard") && __instance.CharacterName.cleanCharName().Equals("Succubus Guard"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Slave") && __instance.CharacterName.cleanCharName().Equals("Slave"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Cavalry Sculptor") && __instance.CharacterName.cleanCharName().Equals("Cavalry Sculptor"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Halfling Warrior") && __instance.CharacterName.cleanCharName().Equals("Halfling Warrior"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Skerenthal the Rock Cleaver") && __instance.CharacterName.cleanCharName().Equals("Skerenthal the Rock Cleaver"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Red Mask") && __instance.CharacterName.cleanCharName().Equals("Red Mask"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("The Sinner") && __instance.CharacterName.cleanCharName().Equals("The Sinner"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Mad Glowworm") && __instance.CharacterName.cleanCharName().Equals("Mad Glowworm"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Morevet Honeyed Tongue") && __instance.CharacterName.cleanCharName().Equals("Morevet Honeyed Tongue"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Ygefeles") && __instance.CharacterName.cleanCharName().Equals("Ygefeles"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Lathimas") && __instance.CharacterName.cleanCharName().Equals("Lathimas"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Hand of the Inheritor") && __instance.CharacterName.cleanCharName().Equals("Hand of the Inheritor"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Fulsome Queen") && __instance.CharacterName.cleanCharName().Equals("Fulsome Queen"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Shadow") && __instance.CharacterName.cleanCharName().Equals("Shadow"))
                       
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("The Spinner of Nightmares") && __instance.CharacterName.cleanCharName().Equals("The Spinner of Nightmares"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Katair") && __instance.CharacterName.cleanCharName().Equals("Katair"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Nocticula") && __instance.CharacterName.cleanCharName().Equals("Nocticula"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Areelu Vorlesh") && __instance.CharacterName.cleanCharName().Equals("Areelu Vorlesh"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Siabrae") && __instance.CharacterName.cleanCharName().Equals("Siabrae"))

                       // || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Targona") && __instance.CharacterName.cleanCharName().Equals("Targona"))


                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Arsinoe") && __instance.CharacterName.cleanCharName().Equals("Arsinoe"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Baphomet's Archer") && __instance.CharacterName.cleanCharName().Equals("Baphomet's Archer"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Baphomet's Evoker") && __instance.CharacterName.cleanCharName().Equals("Baphomet's Evoker"))

                       || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Baphomet's Cutthroat") && __instance.CharacterName.cleanCharName().Equals("Baphomet's Cutthroat"))


                       //    || (Game.Instance.DialogController.CurrentSpeakerName.cleanCharName().Equals("Crinukh") && __instance.CharacterName.cleanCharName().Equals("Crinukh"))


                       )
                    {
                               // Main.DebugLog("GetPortraitSafe() 1");


                        string characterName = __instance.CharacterName.cleanCharName();

                        string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();
                        string portraitDirectoryName = characterName;

                        string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                        Directory.CreateDirectory(portraitDirectoryPath);

                        if (!string.IsNullOrEmpty(GetPortrait_Patch.GetUnitPortraitPath(__instance)))
                        portraitDirectoryPath = GetPortrait_Patch.GetUnitPortraitPath(__instance);

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
                        string characterName = __instance.CharacterName.cleanCharName();

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
           // Main.DebugLog(__instance.CharacterName.cleanCharName());
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
                    string characterName = __instance.CharacterName.cleanCharName();

                    string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                    string portraitDirectoryName = characterName;

                    string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);


                    

             //       Main.DebugLog(__instance.name);
             //       Main.DebugLog(__instance.Name);

                    Directory.CreateDirectory(portraitDirectoryPath);
                    Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName()));
                    

                    if (!File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), "Medium.png")))
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
