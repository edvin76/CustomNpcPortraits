﻿
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

namespace CustomNpcPortraits
{
    // Token: 0x02000007 RID: 7
    [HarmonyPatch(typeof(BlueprintUnit), "PortraitSafe", MethodType.Getter)]
    public static class GetPortraitSafe_Patch
    {
        // Token: 0x06000010 RID: 16 RVA: 0x00002718 File Offset: 0x00000918

        public static bool Prefix(BlueprintUnit __instance, ref BlueprintPortrait __result, BlueprintPortrait ___m_Portrait)
        {
           // Main.DebugLog("GetPortraitSafe() : " + __instance.CharacterName);

              // Main.DebugLog("GetPortraitSafe() SceneManager.GetActiveScene().name : " + SceneManager.GetActiveScene().name);
                //              Main.DebugLog("Game.Instance.CurrentMode : " + Game.Instance.CurrentMode);


            if (!Main.enabled || Main.pauseGetPortraitsafe)
            {
                return true;
            }


            try
            {
                if ((Game.Instance.CurrentMode == GameModeType.GlobalMap || Game.Instance.CurrentMode == GameModeType.CutsceneGlobalMap)
&&                        Main.companions.Contains(__instance.CharacterName))
                {
                    //Main.DebugLog("getportraitsafe: companion for quest panel on map");
                    string characterName = __instance.CharacterName.cleanCharname();
                    string prefix = Main.GetCompanionPortraitDirPrefix();
                    string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
                    //				Main.DebugLog("SetPortraits()"+ characterName);
                    if (characterName.ToLower().Contains("aruesh"))
                    {
                        if (__instance.Race.name.ToLower().Contains("succubusrace"))
                        {

                            characterName = characterName + " - Evil";


                        }
                       
                    }

                    if (characterName.Equals("Ciar"))
                    {
                        if (__instance.Alignment.ToString().ToLower().Contains("evil"))
                            characterName = "Ciar - Undead";

                    }

                    if (characterName.Equals("Queen Galfrey"))
                    {
                        if (__instance.Alignment.ToString().ToLower().Contains("evil"))
                            characterName = "Queen Galfrey - Undead";

                    }


                    if (characterName.Equals("Staunton Vhane"))
                    {
                        if (__instance.Alignment.ToString().ToLower().Contains("evil"))
                            characterName = "Staunton Vhane - Undead";

                    }


                    string portraitDirectoryName = prefix + characterName;

                    //	Main.DebugLog("1 - "+Path.Combine(portraitsDirectoryPath, portraitDirectoryName));
                    string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                    //Main.DebugLog("SetPortrait() 2");

                    Directory.CreateDirectory(portraitDirectoryPath);

                    //Main.DebugLog("SetPortrait() 3");

                    BlueprintPortrait blueprintPortrait = ___m_Portrait;

         


                    //Main.DebugLog("SetPortrait() 4");


                    if (blueprintPortrait != null && blueprintPortrait.Data != null && blueprintPortrait.Data.IsCustom && !blueprintPortrait.Data.CustomId.IsNullOrEmpty())
                    {

                        portraitDirectoryPath = blueprintPortrait.Data.CustomId;

                        Main.DebugLog("SetPortrait() found custom at: " + blueprintPortrait.Data.CustomId);

                    }
                    bool missing = false;

                    //Main.DebugLog("SetPortrait() 6");

                    foreach (string fileName in Main.PortraitFileNames)
                    {
                        if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
                        {
                            missing = true;
                        }
                    }

                    //Main.DebugLog("SetPortrait() 7");


                    if (!missing)
                    {
                        //	Main.DebugLog("SetPortrait() 8");

                        blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;

                        CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
                        CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
                        CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));
                        //Main.DebugLog("SetPortrait() 9");

                        blueprintPortrait.Data = new PortraitData(portraitDirectoryPath);

                        //	Main.DebugLog("SetPortrait() 10");

                        __result = blueprintPortrait;
                        return false;
                        /*			DialogController d = new DialogController();

                                    d.SpeakerPortait.gameObject.SetActive(blueprintPortrait.HalfLengthPortrait != null);
                                    d.SpeakerPortait.sprite = blueprintPortrait.HalfLengthPortrait;

                                    Canvas.ForceUpdateCanvases();*/

                        /*
                        ReactiveProperty<Kingmaker.UI.MVVM._VM.Party.PartyCharacterPetVM> reactiveCharacter = new ReactiveProperty<Kingmaker.UI.MVVM._VM.Party.PartyCharacterPetVM>(null);

                        Kingmaker.UI.MVVM._VM.Party.PartyCharacterPetVM partyCharacterPetVM = new Kingmaker.UI.MVVM._VM.Party.PartyCharacterPetVM();
                        reactiveCharacter.Value = partyCharacterPetVM;
                        //AddDisposable(partyCharacterPetVM1);

                        reactiveCharacter.Value.SetUnitData(unitEntityData);
                    */
                        //unitEntityData.UISettings.SetPortrait(blueprintPortrait);

                        /*
                        EventBus.RaiseEvent<IUnitPortraitChangedHandler>(delegate (IUnitPortraitChangedHandler h)
                        {
                            h.HandlePortraitChanged(unitEntityData);
                        });*/
                    }
                    else
                        return true;
                }



                if (Game.Instance.CurrentMode == GameModeType.Cutscene || Game.Instance.CurrentMode == GameModeType.Dialog)
                {

                    if (Main.settings.ManageCompanions && (Game.Instance.DialogController != null) && (Game.Instance.DialogController.CurrentSpeaker != null) && __instance.CharacterName.Equals("Nenio") && Game.Instance.DialogController.CurrentCue.AssetGuid.ToString().Equals("45450b2f327797e41bce701b91118cb4"))
                    {
                       // Main.DebugLog("GetportraitSafe we are in for Nenio renamed to NenioFox!");

                        string characterName = "NenioFox_Portrait";
                        string prefix = Main.GetCompanionPortraitDirPrefix();
                        string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
                        string portraitDirectoryName = prefix + characterName;
                        string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);


                        bool missing = false;
                            // Main.DebugLog("Getportraitsafe() 4");
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
                             //  Main.DebugLog("Getportraitsafe() 7");

                        if (!missing)
                        {

                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
                            CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));



                            PortraitData Data = new PortraitData(portraitDirectoryPath);
                            Main.pauseGetPortraitsafe = true;

                            //Data.m_PetEyeImage = Game.Instance.DialogController.CurrentCue.Speaker.Blueprint.PortraitSafe.Data.m_PetEyeImage;
                            Main.pauseGetPortraitsafe = false;

                            BlueprintPortrait bp = new BlueprintPortrait();
                            bp.Data = Data;

                            __result = bp;
                         //   Main.DebugLog("Getportraitsafe() loaded portrait for Companion " + characterName);

                            // Main.DebugLog("are we");


                            return false;
                        }
                        else
                        {
                       //     Main.DebugLog("Getportraitsafe() 6");

                            __result = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");

                            return false;
                        }

                    }

                    if ((Game.Instance.DialogController.CurrentSpeakerName.Equals("Wirlong Black Mask") && __instance.CharacterName.Equals("Wirlong Black Mask")) 
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Nulkineth") && __instance.CharacterName.Equals("Nulkineth"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Langrat Messini") && __instance.CharacterName.Equals("Langrat Messini"))
                       || (Game.Instance.DialogController.CurrentSpeakerName.Equals("Terta") && __instance.CharacterName.Equals("Terta"))

                       )
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

                if ( ( Game.Instance.CurrentMode == GameModeType.CutsceneGlobalMap 
                    || Game.Instance.CurrentMode == GameModeType.GlobalMap 
                    || Game.Instance.CurrentMode == GameModeType.TacticalCombat 
                    || Game.Instance.CurrentMode == GameModeType.Kingdom ) 
                    && !__instance.IsCompanion)
                {
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
