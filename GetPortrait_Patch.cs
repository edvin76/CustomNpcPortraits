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

namespace CustomNpcPortraits
{
    // Token: 0x02000015 RID: 21
    [Harmony12.HarmonyPatch(typeof(UnitUISettings), "Portrait", MethodType.Getter)]
    public static class GetPortrait_Patch
    {
        // Token: 0x06000053 RID: 83 RVA: 0x00005694 File Offset: 0x00003894
        public static bool Prefix(UnitUISettings __instance, ref PortraitData __result, BlueprintPortrait ___m_Portrait, BlueprintPortrait ___m_CustomPortrait)
        {
            if (!Main.enabled)
                return true;
            try
            {


                if (Game.Instance.CurrentMode != GameModeType.Dialog)
                {
                   // Main.DebugLog("Getportrait() : " + Game.Instance.CurrentMode);

                    return true;




                }

                /*
                Main.DebugLog("__instance.Owner.CharacterName : " + __instance.Owner.CharacterName);
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


                if (Game.Instance == null || Game.Instance.DialogController == null || Game.Instance.DialogController.CurrentSpeakerBlueprint == null || Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName == null)
                {
                    Main.DebugLog("Getportrait() CurrentSpeakerBlueprint == null");

                    return true;
                }

                string characterName = Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName;
                //__instance.Owner.CharacterName;

                if (Game.Instance.DialogController.CurrentSpeaker != null)
                {
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
                    if (characterName.Equals("Asty"))
                    {
                        //Main.SaveOriginals2(Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.PortraitSafe.Data, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                        //return true;
                        if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                            characterName = "Asty - Drow";

                    }
                    if (characterName.Equals("Velhm"))
                    {
                        if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                            characterName = "Velhm - Drow";

                    }
                    if (characterName.Equals("Tran"))
                    {
                        if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                            characterName = "Tran - Drow";

                    }

                    if (characterName.ToLower().Contains("aruesh"))
                    {
                        if (Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.Race.name.ToLower().Contains("succubusrace"))
                        {

                            characterName = characterName + " - Evil";


                        }
                    }
                }


                if (!__instance.Owner.IsMainCharacter || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Wirlong Black Mask") && (Game.Instance.DialogController.CurrentCue.Speaker.Blueprint.CharacterName != Game.Instance.Player.GetMainPartyUnit().CharacterName)))
                {
                    // Main.DebugLog("skip Getportrait for: " + characterName);
                    //  return true;





                    bool companion = Game.Instance.DialogController.CurrentSpeakerBlueprint.IsCompanion;


                    if (Main.settings.ManageCompanions && (companion || Main.companions.Contains(characterName)))
                    {
                      //  Main.DebugLog("Getportrait() 1");

                        string prefix = Main.GetCompanionPortraitDirPrefix();
                        string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
                        string portraitDirectoryName = prefix + characterName;
                        string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                        if (!Directory.Exists(portraitDirectoryPath))
                        {
                            return true;
                        }
                        //Main.DebugLog("Getportrait() 3");
                        bool missing = false;
                        //Main.DebugLog("Getportrait() 4");
                        // Main.DebugLog(Main.PortraitFileNames[0]);

                        foreach (string fileName in Main.PortraitFileNames)
                        {
                            //  Main.DebugLog("p: "+portraitDirectoryPath);

                            //   Main.DebugLog("f: " + fileName);


                            if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
                            {
                                //  Main.DebugLog("Getportrait() 6");

                                missing = true;
                                break;
                            }
                        }
                        // Main.DebugLog("Getportrait() 7");

                        if (!missing)
                        {
                            //Main.DebugLog("Getportrait() loaded portrait for Companion "+characterName);
                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));


                            PortraitData Data = new PortraitData(portraitDirectoryPath);

                            Data.m_PetEyeImage = Game.Instance.DialogController.CurrentCue.Speaker.Blueprint.PortraitSafe.Data.m_PetEyeImage;


                            __result = Data;
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }
                    else if (!companion && !Main.companions.Contains(characterName))// Npc
                    {


                        //  Main.DebugLog("Getportrait() 2");
                        //string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();
                        //string portraitDirectoryName = characterName;
                        //string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);
                        BlueprintUnit blueprintUnit = Game.Instance.DialogController.CurrentSpeakerBlueprint;
                        string portraitDirectoryPath = GetUnitPortraitPath(blueprintUnit, characterName);




                        if (Game.Instance.DialogController.CurrentCue.Speaker.Blueprint != null)
                        {
                            blueprintUnit = Game.Instance.DialogController.CurrentCue.Speaker.Blueprint;
                        }
                        //Main.DebugLog("bup: "+bup.name);

                        BlueprintPortrait blueprintPortrait = blueprintUnit.PortraitSafe;


                        //  Main.DebugLog("Getportrait() 2 "+ Path.Combine(portraitDirectoryPath, Main.mediumName));
                        bool enterHere = false;

                        if ((portraitDirectoryPath == null) || (portraitDirectoryPath.Length == 0))
                        {

                            if (!Main.settings.AutoSecret && blueprintPortrait.Data.InitiativePortrait)
                            {
                                Directory.CreateDirectory(Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, blueprintUnit.name));
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





                        if (enterHere && !characterName.Equals("Asty") && !characterName.Equals("Tran") && !characterName.Equals("Velhm"))
                        {
                            //  Main.DebugLog("Getportrait() 3");



                            if ((blueprintPortrait != null) && (blueprintPortrait.Data != null))
                            {
                                // Main.DebugLog("Getportrait() 4");

                                SpriteLink mHalfLengthImage = blueprintPortrait.Data.m_HalfLengthImage;

                                if ((mHalfLengthImage != null) && (mHalfLengthImage.AssetId != null) && (mHalfLengthImage.AssetId.Length > 5))
                                {
                                    //    Main.DebugLog("Getportrait() 5");



                                    Main.SaveOriginals(blueprintUnit, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                                    if (characterName != blueprintUnit.name)
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







                        portraitDirectoryPath = GetUnitPortraitPath(blueprintUnit, characterName);

                        if (!Directory.Exists(portraitDirectoryPath))
                        {
                            return true;
                        }



                        if (File.Exists(Path.Combine(portraitDirectoryPath, Main.mediumName)))
                        {
                            //Main.DebugLog("Getportrait() loaded portrait for NPC " + characterName);
                            //if (Main.settings.AutoBackup) Main.SaveOriginals(Game.Instance.DialogController.CurrentSpeakerBlueprint, portraitDirectoryPath);
                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));

                            PortraitData Data = new PortraitData(portraitDirectoryPath);
                            Data.m_PetEyeImage = Game.Instance.DialogController.CurrentSpeakerBlueprint.PortraitSafe.Data.m_PetEyeImage;

                            __result = Data;
                            return false;
                        }
                        else
                        {
                            return true;
                        }

                    }



                    return true;

                }
                return true;

            }
            catch (Exception e)
            {
                Main.DebugError(e);
                return true;
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



                // Kressle
                string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName);
                string unitCharNameDirNames = Path.Combine(charcterNameDirectoryName);


                // Dovan From Nisroch/Dovan
                if (File.Exists(Path.Combine(portraitsDirectoryPath, charcterNameDirectoryName, unitNameDirectoryName, "Medium.png")))
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
                else
                {

                    portraitDirectoryPath = "";

                }


            // return Tuple.Create(portraitDirectoryPath, unitCharNameDirNames);
            return portraitDirectoryPath;
            }


    }
}

