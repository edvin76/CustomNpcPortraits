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

namespace CustomNpcPortraits
{
    // Token: 0x02000015 RID: 21

    [Harmony12.HarmonyPatch(typeof(InitiativeTrackerUnitVM), "Portrait", MethodType.Getter)]
    public static class InitiativeTrackerUnitVM_Get_Portrait_Patch
    {

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
            try
            {
               // Main.DebugLog("1");
                if( __instance.Unit != null && (!__instance.Unit.IsMainCharacter && !__instance.Unit.Blueprint.IsCompanion))
                {
                 //   Main.DebugLog("2");

                    string characterName = __instance.Unit.CharacterName;

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
                    string portraitDirectoryPath = GetPortrait_Patch.GetUnitPortraitPath(__instance.Unit.Blueprint, characterName);


                  //  Main.DebugLog("3");

                    if (portraitDirectoryPath.Length == 0)
                    {

                        string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                        string portraitDirectoryName = characterName;

                        portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                        if (characterName.Equals("Enemy Leader"))
                        {

                            portraitDirectoryPath = Path.Combine(portraitDirectoryPath, __instance.Unit.Blueprint.name);


                        }

                    }

                    // Main.DebugLog(portraitDirectoryPath.Length.ToString());

                   // Main.DebugLog("4");


                    if (portraitDirectoryPath.Length > 0)
                    {
                      //  Main.DebugLog(portraitDirectoryPath);


                        if (File.Exists(Path.Combine(portraitDirectoryPath, "Small.png")))
                        {
                            // Main.DebugLog("3");

                            //Vector2Int v2 = ImageHeader.GetDimensions(Path.Combine(portraitDirectoryPath, "Small.png"));

                            //  Main.DebugLog("huh 2 " + pdata.SmallPortrait.texture.width);
                            //if (v2.x > 100)
                            //{

                            // CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));

                            long ftime = ((DateTimeOffset)File.GetLastWriteTimeUtc(Path.Combine(portraitDirectoryPath, "Small.png"))).ToUnixTimeSeconds();
                            if (!portraitDatas.ContainsKey(ftime))
                            {
                                PortraitData pdata = new PortraitData(portraitDirectoryPath);

                            

                           
                                portraitDatas.Add(ftime, 1);
                                Main.pauseGetPortraitsafe = true;

                                pdata.m_PetEyeImage = __instance.Unit.Blueprint.PortraitSafe.Data.m_PetEyeImage;

                                Main.pauseGetPortraitsafe = false;


                              //  Main.DebugLog("huh 3");


                                //Main.DebugLog(__instance.Unit.Blueprint.name + " - new one");
                                Main.pauseGetPortraitsafe = true;
                                __instance.Unit.UISettings.SetPortrait(pdata);
                                Main.pauseGetPortraitsafe = false;
                                
                                __result = pdata.SmallPortrait;
                                return false;

                            }
                            // }

                            //}
                        }
                        else
                        {
                            // Main.DebugLog("Trying to find icon: " + __instance.Unit.Blueprint.PortraitSafe.name);

                            if (!SmallPortraitInjector.Replacements.ContainsKey(__instance.Unit.Blueprint.PortraitSafe.Data))
                            if (File.Exists(Path.Combine(Main.GetTacticalPortraitsDirectory(), __instance.Unit.Blueprint.PortraitSafe.name+".png")))
                            {
                                //Main.DebugLog("Trying to apply tactical icon: "+ __instance.Unit.Blueprint.PortraitSafe.name);


                                SmallPortraitInjector.Replacements[__instance.Unit.Blueprint.PortraitSafe.Data] = Image2Sprite.Create(Path.Combine(Main.GetTacticalPortraitsDirectory(), __instance.Unit.Blueprint.PortraitSafe.name + ".png"), new UnityEngine.Vector2Int(184, 240), TextureFormat.BGRA32);


                                __result = Image2Sprite.Create(Path.Combine(Main.GetTacticalPortraitsDirectory(), __instance.Unit.Blueprint.PortraitSafe.name + ".png"), new UnityEngine.Vector2Int(184, 240), TextureFormat.BGRA32);

          
                                
                               // LoadingProcess.Instance.StartCoroutine(ExportTexture(__result.texture, Path.Combine(Main.GetTacticalPortraitsDirectory(), "test.png")));
                                return false;
                            }
                        }


                        //    Main.DebugLog("huh 6");

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
