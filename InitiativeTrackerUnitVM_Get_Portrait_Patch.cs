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
using Kingmaker.ResourceManagement;
using Kingmaker.UI.MVVM._VM.TacticalCombat.InitiativeTracker;

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


            UnitEntityData unit = __instance.Unit;
            if (unit == null)
            {
                __result = null;
                return false;
            }

            try
            {
                if (!__instance.Unit.IsMainCharacter && !__instance.Unit.Blueprint.IsCompanion)
                {

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

                        if (portraitDirectoryPath.Length > 0)
                    {

                        if (File.Exists(Path.Combine(portraitDirectoryPath, "Small.png")))
                        { 

                            Vector2Int v2 = ImageHeader.GetDimensions(Path.Combine(portraitDirectoryPath, "Small.png"));

                            //  Main.DebugLog("huh 2 " + pdata.SmallPortrait.texture.width);

                            if (v2.x > 100)
                            {
                                CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));

                                PortraitData pdata = new PortraitData(portraitDirectoryPath);

                                pdata.m_PetEyeImage = __instance.Unit.Blueprint.PortraitSafe.Data.m_PetEyeImage;



                                //Main.DebugLog("huh 3 " + Path.Combine(portraitDirectoryPath, "Small.png"));


                                //Main.DebugLog(__instance.Unit.Blueprint.name + " - new one");
                                __instance.Unit.UISettings.SetPortrait(pdata);
                                    __result = pdata.SmallPortrait;
                                    return false;

                               // }

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
            return true;

        }





    }


}
