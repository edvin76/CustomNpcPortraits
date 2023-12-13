using System;
using System.IO;
using System.Reflection;
using Harmony12;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.GameModes;
using Kingmaker.UI.UnitSettings;
using Kingmaker.ResourceLinks;
using UnityModManagerNet;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Blueprints.Root;
using System.Linq;
using Kingmaker.UnitLogic.Customization;
using Kingmaker.Cheats;
using UnityEngine;
using Kingmaker.Controllers.Dialog;
using Kingmaker.Localization;
using Kingmaker.UnitLogic;
using Kingmaker.View;
using System.Collections.Generic;
using static Kingmaker.Visual.CharacterSystem.BakedCharacter;
using Kingmaker.PubSubSystem;
using UnityEngine.SceneManagement;
using Kingmaker.DialogSystem;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.AreaLogic.Etudes;

namespace CustomNpcPortraits
{
    // Token: 0x02000015 RID: 21
    [Harmony12.HarmonyPatch(typeof(UnitUISettings), "Portrait", MethodType.Getter)]
    public static class GetPortrait_Patch
    {
        // Token: 0x06000053 RID: 83 RVA: 0x00005694 File Offset: 0x00003894
        public static bool Prefix(UnitUISettings __instance, ref PortraitData __result, BlueprintPortrait ___m_Portrait, BlueprintPortrait ___m_CustomPortrait)
        {
            // return true;

            // Main.DebugLog("Getportrait(): " + __instance.Owner.CharacterName);

            // Main.DebugLog(__instance.Owner.Unit.Blueprint.PortraitSafe.name);

            if (!Main.enabled || Main.pauseGetPortraitsafe)
            {
                return true;
            }


         //    Main.DebugLog("Getportrait() " + __instance.Owner.CharacterName);

            //BCT_Rand_Crusader_Soldier

            //   Main.DebugLog("GetPortrait(): SceneManager.GetActiveScene().name : " + SceneManager.GetActiveScene().name);
            //   Main.DebugLog("GetPortrait(): Game.Instance.CurrentMode : " + Game.Instance.CurrentMode);
            // return true;

            //     Main.DebugLog("Getportrait(): Main.areaLoaded: " + Main.areaLoaded.ToString());
            //if ((__instance.Owner.IsMainCharacter  /*|| Main.companions.Contains(__instance.Owner.Unit.CharacterName) */|| (Game.Instance.CurrentMode != GameModeType.Dialog && Game.Instance.CurrentMode != GameModeType.Cutscene)) && Main.areaLoaded && __instance.Owner.Unit.CharacterName != "Player Character")
            
            
            if ((Game.Instance.Player.AllCharacters.Contains(__instance.Owner.Unit) || Main.companions.Contains(__instance.Owner.Unit.CharacterName)) /*&& Main.areaLoaded */&& __instance.Owner.Unit.CharacterName != "Player Character")
            {

              //  Main.DebugLog("We are in all set for " + __instance.Owner.Unit.CharacterName);
              if(!Main.settings.ManageCompanions)
                {
                    return true;
                }
                __result = Main.SetPortrait(__instance.Owner.Unit);
                        //Main.SetPortrait(__instance.Owner.Unit);
                        if (__result != null)
                        return false;
                    else
                    return true;
             }


            if (!Main.enabled || Main.isSetPortrait)
                return true;
            try
            {




             //   Main.DebugLog("Getportrait() GameModeType : " + Game.Instance.CurrentMode);

                /*
                if (Game.Instance == null || Game.Instance.DialogController == null || Game.Instance.DialogController.CurrentSpeakerBlueprint == null || Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName == null)
                {
//Main.DebugLog("Getportrait() CurrentSpeakerBlueprint == null");
                   // return true;
                }
                else
                {
  //                   Main.DebugLog("Getportrait() CurrentSpeakerBlueprint != null");


                }
                */
                /*
                if (Game.Instance.CurrentMode != GameModeType.Dialog && Game.Instance.CurrentMode != GameModeType.Cutscene)
                {

                    if (!Main.isSetPortrait)
                        if (__instance.Owner.CharacterName.Equals("Nenio"))
                        {
                            __result = Main.SetPortrait(__instance.Owner.Unit);
                            //Main.SetPortrait(__instance.Owner.Unit);

                            return false;
                        }

                    return true;




                }
                */
              //  Main.DebugLog("1");

                /*
                Main.DebugLog("CurrentSpeakerBlueprint.CharacterName : " + Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName);
                if (Game.Instance.DialogController.CurrentCue.Listener != null)
                Main.DebugLog("CurrentCue.Listener.CharacterName : " + Game.Instance.DialogController.CurrentCue.Listener.CharacterName);
                if (Game.Instance.DialogController.CurrentCue.Speaker != null)
                {
                    Main.DebugLog("CurrentCue.Speaker != null");

                    if (Game.Instance.DialogController.CurrentSpeakerName != null)
                        Main.DebugLog("DialogController.CurrentSpeakerName : " + Game.Instance.DialogController.CurrentSpeakerName);
                }
                    Main.DebugLog("-------------------------------------------");


  */


                if (Game.Instance == null ||
                    Game.Instance.CurrentMode == null ||
                    Game.Instance.CurrentMode != GameModeType.Dialog ||
                    Game.Instance.DialogController == null || 
                    Game.Instance.DialogController.CurrentSpeaker == null ||
                    Game.Instance.DialogController.CurrentSpeakerBlueprint == null ||
                    Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName == null)
                {

                    return true;
                }
                //    Main.DebugLog("Getportrait() CurrentSpeakerBlueprint != null");

             //   Main.DebugLog("dialog instance : we are in" );

                string characterName = __instance.Owner.Unit.CharacterName;

              
                    if (Game.Instance.DialogController.CurrentSpeakerName != null && Game.Instance.DialogController.CurrentSpeakerName.Length > 0)
                        characterName = Game.Instance.DialogController.CurrentSpeakerName;
/*
                    else if (Game.Instance.DialogController.CurrentSpeaker.CharacterName != null && Game.Instance.DialogController.CurrentSpeaker.CharacterName.Length > 0)
                        characterName = Game.Instance.DialogController.CurrentSpeaker.CharacterName;

                    else if (Game.Instance.DialogController.CurrentSpeakerBlueprint != null && Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName.Length > 0)
                        characterName = Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName;
                    */

                //__instance.Owner.CharacterName;


                /*
                BlueprintPortrait mPortrait;

                if (___m_CustomPortrait != null)
                {
                    mPortrait = ___m_CustomPortrait;
                    Main.DebugLog("BPUnitName: " + Game.Instance.DialogController.CurrentSpeaker.View.name);
                    Main.DebugLog("customPortrait: " + mPortrait.Data.CustomId);

                }
                else
                {
                    if (___m_Portrait != null)
                    {
                        mPortrait = ___m_Portrait;
                        Main.DebugLog("BPUnitName: " + Game.Instance.DialogController.CurrentSpeaker.View.name);
                        Main.DebugLog("m_Portrait: " + mPortrait.name);

                    }
                    else
                    {
                        mPortrait = Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.PortraitSafe;
                        Main.DebugLog("BPUnitName: " + Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.name);
                        Main.DebugLog("unitBlueprintPortrait: " + mPortrait.name);
                    }
                }
                */
                if (characterName.Equals(Main.AstyName))
                {
                    //Main.SaveOriginals2(Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.PortraitSafe.Data, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                    //return true;
                    if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                        characterName = Main.AstyName+" - Drow";

                }
                if (characterName.Equals(Main.VelhmName))
                {
                    if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                        characterName = Main.VelhmName+" - Drow";

                }
                if (characterName.Equals(Main.TranName))
                {
                    if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                        characterName = Main.TranName+" - Drow";

                }

                //Irabeth scar
                if (characterName.Contains(Main.IrabethName))
                    if (Game.Instance.Player.EtudesSystem.EtudeIsStarted(ResourcesLibrary.TryGetBlueprint<BlueprintEtude>("b4f08736cf124ae4996fcef7c0a33bf1")))
                    {

                        Directory.CreateDirectory(Path.Combine(Main.GetCompanionPortraitsDirectory(), Main.IrabethName+" - Scar"));
                        characterName = Main.IrabethName+" - Scar";
                    }
                /*
                    if (characterName.ToLower().Contains("aruesh"))
                    {
                        if (Game.Instance.DialogController.CurrentSpeaker.Blueprint.Race.name.ToLower().Contains("succubusrace"))
                        {

                            characterName = characterName + " - Evil";


                        }
                    }
                    if (characterName.Equals("Nenio"))
                    {

                        if (Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component == null)
                        {


                        //         Main.DebugLog("fox");
                            characterName = "NenioFox_Portrait";

                        }
                        else
                        {
                           //    Main.DebugLog("human"); 
                        }
                    }

                    if (characterName.Equals("Ciar"))
                    {
                        if (Game.Instance.DialogController.CurrentSpeaker.Blueprint.Alignment.ToString().ToLower().Contains("evil"))
                            characterName = "Ciar - Undead";

                    }

                    if (characterName.Equals("Queen Galfrey"))
                    {
                        if (Game.Instance.DialogController.CurrentSpeaker.Blueprint.Alignment.ToString().ToLower().Contains("evil"))
                            characterName = "Queen Galfrey - Undead";

                    }
                    if (characterName.Equals("Staunton Vhane"))
                    {
                        if (Game.Instance.DialogController.CurrentSpeaker.Descriptor.IsUndead)
                            characterName = "Staunton Vhane - Undead";

                    }

                    */


              //   Main.DebugLog("we're NOT YET in for " + characterName);

                if (!__instance.Owner.Unit.IsMainCharacter || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Wirlong Black Mask")/* ||Game.Instance.DialogController.CurrentSpeakerName.Equals("Nenio"))*/&& (Game.Instance.DialogController.CurrentCue.Speaker.Blueprint.CharacterName != Game.Instance.Player.GetMainPartyUnit().CharacterName)))
                {
                //      Main.DebugLog("we're in for " + characterName);
                    //  return true;

                    //  if(Game.Instance.DialogController.CurrentCue.AssetGuid.ToString().Equals("45450b2f327797e41bce701b91118cb4"))
                    //    characterName = "NenioFox_Portrait";


                    //bool companion = Game.Instance.Player.ActiveCompanions.Contains(__instance.Owner.Unit) || Game.Instance.Player.RemoteCompanions.Contains(__instance.Owner.Unit);
                    //bool companion = Game.Instance.DialogController.CurrentSpeakerBlueprint.IsCompanion;

                    /*
                                        if (Main.settings.ManageCompanions && (Game.Instance.Player.ActiveCompanions.Contains(__instance.Owner.Unit) || Game.Instance.Player.RemoteCompanions.Contains(__instance.Owner.Unit)))
                                        {
                                         //    Main.DebugLog("Getportrait() 1" + companion.ToString());
                                        //   Main.DebugLog("Getportrait() 3 " +characterName  +" - "+ Main.companions.Contains(characterName).ToString());

                                            string prefix = Main.GetCompanionPortraitDirPrefix();
                                            string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
                                            string portraitDirectoryName = prefix + characterName;
                                            string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);



                                            if (Main.compCycle.Length > 1)
                                                portraitDirectoryPath = Path.Combine(Main.GetCompanionPortraitsDirectory(), portraitDirectoryName, Main.compCycle);


                                            Directory.CreateDirectory(portraitDirectoryPath);


                                            bool missing = false;
                                             //    Main.DebugLog("Getportrait() 4");
                                            // Main.DebugLog(Main.PortraitFileNames[0]);

                                            foreach (string fileName in Main.PortraitFileNames)
                                            {
                                                //  Main.DebugLog("p: "+portraitDirectoryPath);

                                                //   Main.DebugLog("f: " + fileName);

                                                if(characterName != "Ciar" && !fileName.Contains("Fullength"))
                                                if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
                                                {
                                                             //  Main.DebugLog("Getportrait() 6");

                                                    missing = true;
                                                    break;
                                                }
                                            }
                                             //    Main.DebugLog("Getportrait() 7");

                                            if (!missing)
                                            {

                                                CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
                                                CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
                                                CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));



                                                PortraitData Data = new PortraitData(portraitDirectoryPath);
                                                Main.pauseGetPortraitsafe = true;
                                             //   Main.DebugLog("Getportrait() 8");

                                                Data.m_PetEyeImage = Game.Instance.DialogController.CurrentSpeakerBlueprint.PortraitSafe.Data.m_PetEyeImage;
                                                Main.pauseGetPortraitsafe = false;
                                            //Main.DebugLog("Getportrait() 9");


                                                __result = Data;
                                          //      Main.DebugLog("Getportrait() loaded portrait for Companion " + characterName);

                                               // Main.DebugLog("are we");


                                                return false;
                                            }
                                            else
                                            {
                                                 BlueprintPortrait blueprintPortrait = null;
                                                 if (characterName.Equals("Ciar - Undead"))
                                                   blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("dc2f02dd42cfe2b40923eb014591a009");

                                                if (characterName.Equals("Queen Galfrey - Undead"))
                                                    blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("767456b1656ca064dadac544d39d7e40");

                                                if (characterName.Equals("Staunton Vhane - Undead"))
                                                    blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("f4bbe08217bcaa54c91fe73bcea70ede");

                                                if (characterName.Equals("Arueshalae - Evil"))
                                                    blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("484588d56f2c2894ab6d48b91509f5e3");

                                                if (characterName.Equals("NenioFox_Portrait"))
                                                    blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");

                                            //    Main.DebugLog("8b");


                                                if (blueprintPortrait == null)
                                                {
                                                    bool enterHere = false;

                                                    if (Main.settings.AutoBackup)
                                                    {
                                                        enterHere = !File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), Main.mediumName));

                                                    }

                                                    if (enterHere)
                                                    {
                                                        Main.pauseGetPortraitsafe = true;

                                                        Main.SaveOriginals2(Game.Instance.DialogController.CurrentSpeakerBlueprint.PortraitSafe.Data, portraitDirectoryPath);
                                                        Main.pauseGetPortraitsafe = false;

                                                    }

                                                //    Main.DebugLog("9");
                                                    Main.pauseGetPortraitsafe = true;

                                                    __result = Game.Instance.DialogController.CurrentSpeakerBlueprint.PortraitSafe.Data;
                                                    Main.pauseGetPortraitsafe = false;

                                                    return false;
                                                }
                                                else
                                                {
                                                //   Main.DebugLog("10");
                                                    Main.SaveOriginals2(blueprintPortrait.Data, portraitDirectoryPath);

                                                    __result = blueprintPortrait.Data;
                                                    return false;
                                                }




                                            }


                                        }
                                        else if (!Game.Instance.Player.ActiveCompanions.Contains(__instance.Owner.Unit) && !Game.Instance.Player.RemoteCompanions.Contains(__instance.Owner.Unit))// Npc
                                        {
                    */

                       // Main.DebugLog("Getportrait() 2b");
                    //string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();
                    //string portraitDirectoryName = characterName;
                    //string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);
                    BlueprintUnit blueprintUnit = Game.Instance.DialogController.CurrentSpeakerBlueprint;

                    //    Main.DebugLog("Getportrait() 3b");

                    string portraitDirectoryPath = GetUnitPortraitPath(blueprintUnit, characterName);

                    //   Main.DebugLog("Getportrait() 4b");



                    if (Game.Instance.DialogController.CurrentCue.Speaker.Blueprint != null)
                    {
                        //   Main.DebugLog("Getportrait() 5b");

                        blueprintUnit = Game.Instance.DialogController.CurrentCue.Speaker.Blueprint;
                    }

                    //   Main.DebugLog("6b");
                    Main.pauseGetPortraitsafe = true;

                    BlueprintPortrait blueprintPortrait = blueprintUnit.PortraitSafe;
                    Main.pauseGetPortraitsafe = false;


                    //    Main.DebugLog("Getportrait() has turnbased portrait? "+ blueprintPortrait.Data.InitiativePortrait);
                    bool enterHere = false;

                    Directory.CreateDirectory(Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                    if (characterName != blueprintUnit.name && 
                        !characterName.Equals(Main.AstyName+" - Drow") && 
                        !characterName.Equals(Main.TranName+" - Drow") &&
                        !characterName.Equals(Main.VelhmName + " - Drow") &&
                        !characterName.Equals(Main.IrabethName + " - Scar"))
                    Directory.CreateDirectory(Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, blueprintUnit.name));





                    if ((portraitDirectoryPath == null) || (portraitDirectoryPath.Length == 0))
                    {


                        if (!Main.settings.AutoSecret && blueprintPortrait.Data.InitiativePortrait)
                        {
                            return true;
                        }
                        enterHere = true;
                    }
                    else
                    {
                        if (Main.settings.AutoBackup)
                        {
                            enterHere = !File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), Main.mediumName));

                        }
                    }




                    if (enterHere &&
                        !characterName.Equals(Main.AstyName) &&
                        !characterName.Equals(Main.TranName) &&
                        !characterName.Equals(Main.VelhmName) &&
                        !characterName.Equals(Main.IrabethName))
                    {
                        //             Main.DebugLog("Getportrait() 3");


                        BlueprintPortrait bp = blueprintUnit.m_Portrait;

                        if (
                                (blueprintPortrait != null) &&
                                (blueprintPortrait.Data != null) &&
                                (
                                    !(!characterName.Contains("Crusader") && bp.name.Contains("BCT_Rand_Crusader_Soldier"))
                                )
                            )
                        {
                            //              Main.DebugLog("Getportrait() 4");

                            SpriteLink mHalfLengthImage = blueprintPortrait.Data.m_HalfLengthImage;

                            if ((mHalfLengthImage != null) && (mHalfLengthImage.AssetId != null) && (mHalfLengthImage.AssetId.Length > 5))
                            {

                                //                         Main.DebugLog("Getportrait() 5");


                                if (!characterName.Equals(Main.AstyName) &&
                                    !characterName.Equals(Main.TranName) &&
                                    !characterName.Equals(Main.VelhmName) &&
                                    !characterName.Equals(Main.IrabethName))
                                Main.SaveOriginals(blueprintUnit, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                                
                                
                                if (characterName != blueprintUnit.name &&
                                    !characterName.Equals(Main.AstyName + " - Drow") &&
                                    !characterName.Equals(Main.TranName + " - Drow") &&
                                    !characterName.Equals(Main.VelhmName + " - Drow") &&
                                    !characterName.Equals(Main.IrabethName + " - Scar"))
                                Main.SaveOriginals(blueprintUnit, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, blueprintUnit.name));


   


                            }
                        }
                    }




                    // 184
                    /*
                                        Main.DebugLog(blueprintPortrait.Data.FullLengthPortrait.texture.width.ToString());
                                        Main.DebugLog(blueprintPortrait.Data.HalfLengthPortrait.texture.width.ToString());
                                        Main.DebugLog(blueprintPortrait.Data.SmallPortrait.texture.width.ToString());



                                        Sprite sprite = CustomPortraitsManager.Instance.LoadPortrait(Path.Combine(portraitDirectoryPath, "Small.png"), pdata.SmallPortrait, true);



                                        if (!TextureIsDefaultPortrait(sprite.texture, PortraitType.SmallPortrait))
                                        {
                                            // Main.DebugLog("huh 5");

                                            //__instance.Unit.Portrait.SmallPortrait.texture

                                            //   if (GetPseudoHash(pdata.SmallPortrait.texture) != GetPseudoHash(sprite.texture))
                                            //  {

                                            */






                    if (Main.npcCycle.Length > 1)
                        portraitDirectoryPath = Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, Main.npcCycle);
                    else if (Main.npcSubCycle.Length > 1)
                        portraitDirectoryPath = Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, blueprintUnit.name, Main.npcSubCycle);
                    else
                        portraitDirectoryPath = GetUnitPortraitPath(blueprintUnit, characterName);

                    if (!Directory.Exists(portraitDirectoryPath))
                    {
                        return true;
                    }




                    if (File.Exists(Path.Combine(portraitDirectoryPath, Main.mediumName)))
                    {
                       // Main.DebugLog("Getportrait() loaded portrait for NPC " + characterName);
                        //if (Main.settings.AutoBackup) Main.SaveOriginals(Game.Instance.DialogController.CurrentSpeakerBlueprint, portraitDirectoryPath);
                        CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));

                        PortraitData Data = new PortraitData(portraitDirectoryPath);
                        Main.pauseGetPortraitsafe = true;

                        Data.m_PetEyeImage = Game.Instance.DialogController.CurrentSpeakerBlueprint.PortraitSafe.Data.m_PetEyeImage;
                        Main.pauseGetPortraitsafe = false;

                        __result = Data;
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                    // }



                    //  return true;

                }

                return true;

            }
            catch (Exception e)
            {
                Main.DebugError(e);
                return true;
            }
        }






        public static void Postfix(UnitUISettings __instance, ref PortraitData __result, BlueprintPortrait ___m_Portrait, BlueprintPortrait ___m_CustomPortrait)
        {
            //Main.DebugLog(__instance.Owner.Unit.CharacterName);
            //  Main.DebugLog(___m_Portrait?.name);
            // Main.DebugLog(___m_CustomPortrait?.name);
            return;
            if (!Main.enabled)
            {
                return;
            }
            try
            {

                if (Game.Instance == null ||
                    Game.Instance.CurrentMode != GameModeType.Dialog ||
                    Game.Instance.DialogController == null || 
                    Game.Instance.DialogController.CurrentCue == null || 
                    Game.Instance.DialogController.CurrentCue.Speaker == null)
                {
                    return;
                }

                
                //   if (Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component == null)

                if (Game.Instance.DialogController.CurrentCue.AssetGuid.ToString().Equals("45450b2f327797e41bce701b91118cb4"))
                {
                    if (Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait != null)
                    {
                        // if (Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait.PortraitSafe.AssetGuid.ToString().Equals("2b4b8a23024093e42a5db714c2f52dbc"))
                        // {
                        //       if (typeof(DialogSpeaker).GetField("m_SpeakerPortrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Game.Instance.DialogController.CurrentCue.Speaker) != null)
                        //     {
                     //   Main.DebugLog("------------------------------------here");


                        typeof(DialogSpeaker).GetField("m_SpeakerPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Game.Instance.DialogController.CurrentCue.Speaker, null);

                        CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
                        EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);


                        // Main.DebugLog(Game.Instance.DialogController.CurrentCue.AssetGuid.ToString());
                        //45450b2f327797e41bce701b91118cb4

                        // }
                    }
                }

                





                return;
            }
            catch (Exception e)
            {
                Main.DebugError(e);
                return;

            }
        }

        // public static Tuple<string, string> npcPortrait(BlueprintUnit blueprintUnit, string characterName)
        public static string GetUnitPortraitPath(BlueprintUnit blueprintUnit, string characterName)
        {

            // 1. use Current
            // 2. if empty use set
            // 3. if no portrait (if) fallback
            // 4. if no fallb if placeholder



            string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();


                string charcterNameDirectoryName = characterName;

                string unitNameDirectoryName = blueprintUnit.name;


            //AUTO DIR CREATION SETTING????
            /*
            if (!Directory.Exists(characterNameDirectoryPath))
            {
                Directory.CreateDirectory(characterNameDirectoryPath);
            }
            */

            UnitEntityData unitEntityData = null;
            if (Game.Instance.DialogController != null &&
                Game.Instance.DialogController.CurrentSpeaker != null)
            {
                unitEntityData = Game.Instance.DialogController.CurrentSpeaker;
            }
            else
            {
                foreach (var unit in Game.Instance.State.Units)
                {
                    if (unit.Blueprint.name.Equals(blueprintUnit.name))
                        unitEntityData = unit;
                }
            }
            // Kressle
            string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName);
                string unitCharNameDirNames = Path.Combine(charcterNameDirectoryName);
            if (unitEntityData.Body.IsPolymorphed &&
                unitEntityData.CharacterName.Equals(Main.NenioName)
                
                )
            {
                if (unitEntityData.Blueprint.name != null && unitEntityData.Blueprint.name.Length > 2)
                {
                   if(unitEntityData.Blueprint.CharacterName.Equals(unitEntityData.CharacterName))

                   
                        {

                        if (!Directory.Exists(Path.Combine(portraitDirectoryPath, unitEntityData.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name)))
                            Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, unitEntityData.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name));

                        if (unitEntityData.GetActivePolymorph().Component?.m_Portrait.GetBlueprint() != null)
                            if (!File.Exists(Path.Combine(portraitDirectoryPath, unitEntityData.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name, Main.GetDefaultPortraitsDirName(), "Medium.png")))
                            Main.SaveOriginals2((unitEntityData.GetActivePolymorph().Component.m_Portrait.GetBlueprint() as BlueprintPortrait).Data, Path.Combine(portraitDirectoryPath, unitEntityData.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name));
                    }
                }


            }
            if (unitEntityData.Body.IsPolymorphed &&
    !unitEntityData.CharacterName.Equals(Main.NenioName) &&
    File.Exists(Path.Combine(portraitDirectoryPath, unitEntityData.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name, "Medium.png"))
    )
            {
                
                    portraitDirectoryPath = Path.Combine(portraitDirectoryPath, unitEntityData.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name);

            }
            else if (Game.Instance.DialogController?.Dialog != null && File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, Game.Instance.DialogController.Dialog.name, "Medium.png")))
            {
                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, Game.Instance.DialogController.Dialog.name);

            }
            else if (Directory.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName)) && Directory.GetFiles(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName), "*.current").Length != 0)
            {
                string[] dirs = Directory.GetFiles(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName), "*.current");

                string dir = Path.GetFileNameWithoutExtension(dirs[0]);
                // Main.DebugLog(dir);
                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName, unitNameDirectoryName);
                if (!dir.Equals("root"))
                    portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName, dir);
                else
                    portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName);
            }


            else if (Directory.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName)) && Directory.GetFiles(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName), "*.current").Length != 0)
            {
                string[] dirs = Directory.GetFiles(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName), "*.current");

                string dir = Path.GetFileNameWithoutExtension(dirs[0]);
                // Main.DebugLog(dir);
                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName);
                if (!dir.Equals("root"))
                    portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, dir);
                else
                    portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName);


            }
            // Dovan From Nisroch/Dovan
            else if (File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName, "Medium.png")))
            {

                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName, unitNameDirectoryName);

                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName);

            }
            // Kesten Garess/Shadow of Kesten Garess
            else if (File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, characterName, "Medium.png")))
            {
                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName, characterName);
                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, characterName);

            }
            // Dovan From Nisroch/Dovan From Nisroch
            else if (File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, charcterNameDirectoryName, "Medium.png")))
            {
                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName, charcterNameDirectoryName);
                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, charcterNameDirectoryName);

            }
            // all portraits/Oleg Leveton/medium.png
            else if (File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, "Medium.png")))
            {
                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName);
                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName);

            }
            else if (File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName, Main.GetDefaultPortraitsDirName(), "Medium.png")))
            {

                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName, unitNameDirectoryName);

                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName);

            }
            else if (File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, Main.GetDefaultPortraitsDirName(), "Medium.png")))
            {
                unitCharNameDirNames = Path.Combine(charcterNameDirectoryName);
                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName);

            }

            else
            {

                portraitDirectoryPath = "";

            }


            // return Tuple.Create(portraitDirectoryPath, unitCharNameDirNames);
            return portraitDirectoryPath;
            }


    }
}

