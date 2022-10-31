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
        public static bool Prefix(UnitUISettings __instance, ref PortraitData __result)
        {
            if (!Main.enabled)
                return true;
            try
            {

              //  Main.DebugLog("Getportrait() : " + __instance.Owner.CharacterName);



                if (Game.Instance.CurrentMode != GameModeType.Dialog)
                {
                    // Main.DebugLog("Getportrait() : " + Game.Instance.CurrentMode);

                    return true;




                }


                if (Game.Instance == null || Game.Instance.DialogController == null || Game.Instance.DialogController.CurrentSpeaker == null || Game.Instance.DialogController.CurrentSpeaker.CharacterName == null)
                {
                    //  Main.DebugLog("Getportrait() 2");

                    return true;
                }

                string characterName = Game.Instance.DialogController.CurrentSpeaker.CharacterName;
                //__instance.Owner.CharacterName;


                if (__instance.Owner.IsMainCharacter)
                {
                    //  Main.DebugLog("skip Getportrait for: " + characterName);
                    return true;
                }



                bool companion = Game.Instance.DialogController.CurrentSpeaker.Blueprint.IsCompanion;


                if (companion && Main.settings.ManageCompanions)
                {
                    //Main.DebugLog("Getportrait() 1");

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

                        Data.m_PetEyeImage = Game.Instance.DialogController.CurrentSpeaker.Blueprint.PortraitSafe.Data.m_PetEyeImage;


                        __result = Data;
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                else if(!companion)// Npc
                {

                   
                    //Main.DebugLog("Getportrait() 2");
                    //string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();
                    //string portraitDirectoryName = characterName;
                    //string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);
                    BlueprintUnit blueprintUnit = Game.Instance.DialogController.CurrentSpeaker.Blueprint;
                    string portraitDirectoryPath = GetUnitPortraitPath(blueprintUnit, characterName);


                    // Main.DebugLog("Getportrait() 1");


                    BlueprintUnit bup = Game.Instance.DialogController.CurrentSpeakerBlueprint;



                    BlueprintPortrait blueprintPortrait = bup.PortraitSafe;

                    if (Main.settings.AutoSecret)
                    {

                        //  Main.DebugLog("Getportrait() 2 "+ Path.Combine(portraitDirectoryPath, Main.mediumName));
                        bool enterHere = false;

                        if ((portraitDirectoryPath == null) || (portraitDirectoryPath.Length == 0))
                        {
                            enterHere = true;
                        }
                        else
                        {
                            if (Main.settings.AutoBackup)
                            {
                                enterHere = !File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), Main.mediumName));

                            }
                        }

                        if (enterHere)
                        {
                            //    Main.DebugLog("Getportrait() 3");



                            if ((blueprintPortrait != null) && (blueprintPortrait.Data != null))
                            {
                                //      Main.DebugLog("Getportrait() 4");

                                SpriteLink mHalfLengthImage = blueprintPortrait.Data.m_HalfLengthImage;

                                if ((mHalfLengthImage != null) && (mHalfLengthImage.AssetId != null) && (mHalfLengthImage.AssetId.Length > 5))
                                {
                                    //        Main.DebugLog("Getportrait() 5");

                                    

                                    Main.SaveOriginals(bup, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                                    Main.SaveOriginals(bup, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName, blueprintUnit.name));

                                }
                            }
                        }
                    }
                    else
                    {

                        return true;
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
                        Data.m_PetEyeImage = Game.Instance.DialogController.CurrentSpeaker.Blueprint.PortraitSafe.Data.m_PetEyeImage;

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

