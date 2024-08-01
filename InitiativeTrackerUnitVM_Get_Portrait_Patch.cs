using UnityEngine;
using System;
using System.IO;
using Kingmaker.Blueprints;
using Harmony12;
using Kingmaker.Blueprints.Root;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI._ConsoleUI.TurnBasedMode;
using Kingmaker.Enums;
using Owlcat.Runtime.UI;
//using Kingmaker.ResourceManagement;
using Kingmaker.UI.MVVM._VM.TacticalCombat.InitiativeTracker;
using static ExtensionMethods.PortraitLoader;
using UnityEngine.Rendering;
using Kingmaker.EntitySystem.Persistence;
using static CustomNpcPortraits.Main;
using System.Collections.Generic;
using ExtensionMethods;
using Kingmaker.Utility;
using Kingmaker.Cheats;

namespace CustomNpcPortraits
{
    // Token: 0x02000015 RID: 21

    [Harmony12.HarmonyPatch(typeof(InitiativeTrackerUnitVM), "Portrait", MethodType.Getter)]
    public static class InitiativeTrackerUnitVM_Get_Portrait_Patch
    {

        public static Dictionary<string, string> turnPortraitPaths = new Dictionary<string, string>();

        public static Dictionary<string, PortraitData> turnPortraits = new Dictionary<string, PortraitData>();

        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        public static bool Prefix(
            InitiativeTrackerUnitVM __instance,
            ref Sprite __result
            )
        {
            if (!Main.enabled)
            {
                return true;
            }


            /*
            UnitEntityData unit = __instance.Unit;
            if (unit == null)
            {
                __result = null;
                return false;
            }
            */
          //  if (__instance.Unit.CharacterName.Contains("Baphomet's Archer"))
                try
                {
                if( __instance.Unit != null && (!__instance.Unit.IsMainCharacter && !Main.companions.Contains(__instance.Unit.Blueprint.CharacterName.cleanCharName())))
                {
                        Main.DebugLog("turn 1 " + __instance.Unit.CharacterName);
                        Main.DebugLog("turn 1 " + __instance.Unit.Blueprint.name);

                        Main.DebugLog("turn 1 " + __instance.Unit.Blueprint.PortraitSafe.name);


                        string characterName = __instance.Unit.CharacterName.cleanCharName();


                    if (characterName.Equals("Asty"))
                    {
                        //Main.SaveOriginals2(Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.PortraitSafe.Data, Path.Combine(Main.GetNpcPortraitsDirectory(), characterName));
                        //return true;
                      //  if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                            characterName = "Asty - Drow";

                    }
                    if (characterName.Equals("Velhm"))
                    {
                        //if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                            characterName = "Velhm - Drow";

                    }
                    if (characterName.Equals("Tran"))
                    {
                       // if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
                            characterName = "Tran - Drow";

                    }



                  
                    

                    string portraitDirectoryPath = "";


  

                        if (!turnPortraitPaths.TryGetValue(__instance.Unit.CharacterName.cleanCharName(), out portraitDirectoryPath))
                    {
                            Main.DebugLog("turn 1.5 THIS STEP TO MINIMIZE!!!!!!!");

                            portraitDirectoryPath = GetPortrait_Patch.GetUnitPortraitPath(__instance.Unit.Blueprint);

                        

                        if (portraitDirectoryPath.Contains("Game Default"))
                            portraitDirectoryPath = "";

                        if (portraitDirectoryPath.Length > 0 && !File.Exists(Path.Combine(portraitDirectoryPath, "Small.png")))
                            portraitDirectoryPath = "";

                        turnPortraitPaths.Add(__instance.Unit.CharacterName.cleanCharName(), portraitDirectoryPath);


                        }


                        //  Main.DebugLog("3");

                        if (portraitDirectoryPath.Length == 0)
                    {
                            Main.DebugLog("turn 2 nothing in portraits-npc");


                        if (characterName.Equals(Main.EnemyLeaderName))
                        {
                                string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                                string portraitDirectoryName = characterName;

                                portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);



                                if (File.Exists(Path.Combine(portraitDirectoryPath, __instance.Unit.Blueprint.name, "Small.png")))
                                {
                                    portraitDirectoryPath = Path.Combine(portraitDirectoryPath, __instance.Unit.Blueprint.name);
                                    Main.DebugLog("turn 3 enemy leader found in " + portraitDirectoryPath);
                                }
                                else
                                    portraitDirectoryPath = "";
                        }

                    }

                        // Main.DebugLog(portraitDirectoryPath.Length.ToString());

                        // Main.DebugLog("4");

                        //  Main.DebugLog(portraitDirectoryPath);


                        // Main.DebugLog("Trying to find icon: " + __instance.Unit.Blueprint.PortraitSafe.name);



                    if (portraitDirectoryPath.Length > 0)
                    {





                        //Vector2Int v2 = ImageHeader.GetDimensions(Path.Combine(portraitDirectoryPath, "Small.png"));

                        //  Main.DebugLog("huh 2 " + pdata.SmallPortrait.texture.width);
                        //if (v2.x > 100)
                        //{

                        // CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
                        /*
                        long ftime = ((DateTimeOffset)File.GetLastWriteTimeUtc(Path.Combine(portraitDirectoryPath, "Small.png"))).ToUnixTimeSeconds();
                        if (!portraitDatas.ContainsKey(ftime))
                        {*/

                        PortraitData pdata;

                            Main.DebugLog("turn 4 pdpath from portrait-npc: " + portraitDirectoryPath);

                            if (!turnPortraits.TryGetValue(portraitDirectoryPath, out pdata))
                        {
                            pdata = new PortraitData(portraitDirectoryPath);

                                Main.DebugLog("turn 4.5 THIS STEP TO MINIMIZE!!!!!!!");
                            //if (pdata != null)
                                turnPortraits.Add(portraitDirectoryPath, pdata);

                        }





                        /*

                             Main.pauseGetPortraitsafe = true;

                             pdata.m_PetEyeImage = __instance.Unit.Blueprint.PortraitSafe.Data.m_PetEyeImage;

                             Main.pauseGetPortraitsafe = false;


                           //  Main.DebugLog("huh 3");

                     */
                        //Main.DebugLog(__instance.Unit.Blueprint.name + " - new one");



                        if (pdata != null && pdata.SmallPortrait != null)
                        {
                            /*
                            Main.pauseGetPortraitsafe = true;
                            __instance.Unit.UISettings.SetPortrait(pdata);
                            Main.pauseGetPortraitsafe = false;
                            */

                                //  __instance.Unit.Portrait.m_PortraitImage = pdata.SmallPortrait;

                                if (!SmallPortraitInjector.Replacements.ContainsKey(__instance.Unit.Portrait))
                                    {
                                        SmallPortraitInjector.Replacements.Add(__instance.Unit.Portrait, pdata.SmallPortrait);
                                        Main.DebugLog("turn 5.5 THIS STEP TO MINIMIZE!!!!!!!");

                                    }

                                //__result = pdata.SmallPortrait;
                                Main.DebugLog("turn end with portraits-npc");

                                return true;
                        }
                        else
                            {
                                Main.DebugLog("turn 5 pdata is null");

                            }

                        // }

                        //}




                        //    Main.DebugLog("huh 6");

                    }
                    else
                    {


                            if (!SmallPortraitInjector.Replacements.ContainsKey(__instance.Unit.Portrait))
                                if (File.Exists(Path.Combine(Main.GetTacticalPortraitsDirectory(), __instance.Unit.Blueprint.PortraitSafe.name + ".png")))
                                {
                                    SmallPortraitInjector.Replacements.Add(__instance.Unit.Portrait, Image2Sprite.Create(Path.Combine(Main.GetTacticalPortraitsDirectory(), __instance.Unit.Blueprint.PortraitSafe.name + ".png"), new UnityEngine.Vector2Int(184, 240), TextureFormat.BGRA32));
                                    Main.DebugLog("turn 6.5 THIS STEP TO MINIMIZE!!!!!!!");

                                }
                            Main.DebugLog("turn end found tactical sprite");

                            return true;
                            /*
                            if (SmallPortraitInjector.Replacements.ContainsKey(__instance.Unit.Portrait))
                            {
                                //Main.DebugLog("Trying to apply tactical icon: "+ __instance.Unit.Blueprint.PortraitSafe.name);





                                // __instance.Unit.UISettings.SetPortrait(pdata);

                                
                                if (!SmallPortraitInjector.Replacements.TryGetValue(__instance.Unit.Portrait, out __result))
                                    Main.DebugLog("turn end tactical it for nothing");
                                else
                                    Main.DebugLog("turn end found tactical sprite");
                                

                                // LoadingProcess.Instance.StartCoroutine(ExportTexture(__result.texture, Path.Combine(Main.GetTacticalPortraitsDirectory(), "test.png")));
                                return true;
                            }
                            else
                            {
                                Main.DebugLog("turn end Default?");

                                
                                __result = Utilities.GetBlueprintByGuid<BlueprintPortrait>("222c3bcbf7e342338416c0e8bdc75109").Data.SmallPortrait;

                                    //__instance.Unit?.Portrait?.SmallPortrait;

                                if (__result != null)
                                    Main.DebugLog("turn 15 where?");
                                else
                                    Main.DebugLog("turn 15 __result is null");
                                
                                return true;

                            }
                            */
                    }
                }

            }
            catch(Exception e)
            {
                Main.DebugError(e);

            }
          //  Main.DebugLog("5");
            return true;

        }

        public static Dictionary<long, int> portraitDatas = new Dictionary<long, int>();



        static System.Collections.IEnumerator ExportTexture(Texture texture, string filePath)
        {



            var copy = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);

            Graphics.ConvertTexture(texture, copy);

            yield return null;


            var request = AsyncGPUReadback.Request(copy, 0, TextureFormat.RGBA32, r =>
            {
                var data = r.GetData<Color32>(0);

                var bytes = ImageConversion.EncodeNativeArrayToPNG(
                    data,
                    copy.graphicsFormat,
                    (uint)copy.width,
                    (uint)copy.height)
                    .ToArray();

                File.WriteAllBytes(filePath, bytes);

                UnityEngine.Object.Destroy(copy);
            });

            while (!request.done)
                yield return null;


        }


    }


}
