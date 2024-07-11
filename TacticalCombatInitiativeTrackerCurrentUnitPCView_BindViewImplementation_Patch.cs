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
using Kingmaker.UI.MVVM._PCView.TacticalCombat.InitiativeTracker;
using UnityEngine.UI;
using Kingmaker.View;
using Owlcat.Runtime.UI.Controls.Button;
using Owlcat.Runtime.UI.Controls.Other;
using Owlcat.Runtime.UI.MVVM;
using Owlcat.Runtime.UI.Tooltips;
using Kingmaker.UI.MVVM._ConsoleView.TacticalCombat.InitiativeTracker;
using TMPro;
using System.Reflection;

namespace CustomNpcPortraits
{
    // Token: 0x02000015 RID: 21
    /*
    [Harmony12.HarmonyPatch(typeof(TacticalCombatInitiativeTrackerCurrentUnitPCView), "BindViewImplementation")]
    public static class TacticalCombatInitiativeTrackerCurrentUnitPCView_BindViewImplementation_Patch
    {

        // Token: 0x06000038 RID: 56 RVA: 0x0000227B File Offset: 0x0000047B
        public static void Postfix(
            TacticalCombatInitiativeTrackerCurrentUnitPCView __instance
            )
        {

    /*
            /*

            // try
            //  {
            
            Type baseType = typeof(TacticalCombatInitiativeTrackerCurrentUnitPCView).BaseType;



           // Type viewBase = baseType.DeclaringType;


            //UnitEntityData unitData = (UnitEntityData)viewBase.GetField("UnitData", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);



            // var viewModelInfo = viewBase.GetField("ViewModel", BindingFlags.Instance | BindingFlags.NonPublic);


            //Type vmtype = viewModelInfo.FieldType;

            // dynamic viewModel = viewModelInfo.GetValue(baseType);

            // dynamic viewModelType = viewModel.GetType();


            // UnitEntityData unitData = viewModel.UnitData;

            //viewModelType.



            //(BlueprintPortrait)AccessTools.Field(bup.GetType(), "m_Portrait").GetValue(bup);

            //baseType.GetField("m_PortraitImage", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

            //TextMeshProUGUI tmg = (TextMeshProUGUI)baseType.GetField("m_UnitNameText", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

            // 

            string characterName = unitData.Blueprint.CharacterName.cleanCharName();


                string unitName = unitData.Blueprint.CharacterName.cleanCharName();

            Main.DebugLog(unitName);
            */

            /*
            string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
            string portraitDirectoryName = characterName;
            string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);



            Directory.CreateDirectory(portraitDirectoryPath);

            if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
            {
                //  Main.SaveOriginals2(__result, portraitDirectoryPath);

                return;
            }


            PortraitData Data = new PortraitData(portraitDirectoryPath);
            BlueprintPortrait bp = new BlueprintPortrait();

            bp.Data = Data;

            Image portraitImage = (Image)baseType.GetField("m_PortraitImage", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance);

            portraitImage.sprite = bp.Data.SmallPortrait;



            baseType.GetField("m_PortraitImage", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(__instance, portraitImage);

            return;

        }
        catch (Exception e)
        {
            Main.DebugError(e);

        }

            */



            /*
            if (!Main.enabled)
            {
                return true;
            }


            UnitEntityData unit = __instance.UnitData;
            if (unit == null)
            {
                __result = null;
                return false;
            }

            try
            {
                if (!__instance.UnitData.IsMainCharacter && !__instance.UnitData.Blueprint.IsCompanion)
                {

                    string characterName = __instance.UnitData.CharacterName.cleanCharName();

                    string portraitDirectoryPath = GetPortrait_Patch.GetUnitPortraitPath(__instance.UnitData.Blueprint, characterName);


                    if (portraitDirectoryPath.Length == 0)
                    {

                        string portraitsDirectoryPath = Main.GetArmyPortraitsDirectory();
                        string portraitDirectoryName = characterName;

                        portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

                        if (characterName.Equals("Enemy Leader"))
                        {

                            portraitDirectoryPath = Path.Combine(portraitDirectoryPath, __instance.UnitData.Blueprint.name);


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

                                pdata.m_PetEyeImage = __instance.UnitData.Blueprint.PortraitSafe.Data.m_PetEyeImage;



                                //Main.DebugLog("huh 3 " + Path.Combine(portraitDirectoryPath, "Small.png"));


                                //Main.DebugLog(__instance.Unit.Blueprint.name + " - new one");
                                __instance.UnitData.UISettings.SetPortrait(pdata);
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
            */

     //   }

    /*


        private static int[] DefaultPortraitsHashes;
        public static bool TextureIsDefaultPortrait(Texture2D texture, PortraitType portraitType)
        {
            if (DefaultPortraitsHashes == null || DefaultPortraitsHashes.Length == 0)
            {
                DefaultPortraitsHashes = new int[] { GetPseudoHash(BlueprintRoot.Instance.CharGen.BasePortraitSmall.texture), GetPseudoHash(BlueprintRoot.Instance.CharGen.BasePortraitMedium.texture), GetPseudoHash(BlueprintRoot.Instance.CharGen.BasePortraitBig.texture) };
            }
            return GetPseudoHash(texture) == DefaultPortraitsHashes[(int)portraitType];
        }



        private static int GetPseudoHash(Texture2D texture)
        {
            int num = 100;
            int num1 = texture.width * texture.height;
            int num2 = num1 / num;
            Color32[] pixels32 = texture.GetPixels32();
            int num3 = -2128831035;
            for (int i = 0; i < num1 - 1; i += num2)
            {
                Color32 color32 = pixels32[i];
                num3 = (num3 ^ color32.r) * 16777619;
                num3 = (num3 ^ color32.g) * 16777619;
                num3 = (num3 ^ color32.b) * 16777619;
            }
            num3 = num3 + (num3 << 13);
            num3 = num3 ^ num3 >> 7;
            num3 = num3 + (num3 << 3);
            num3 = num3 ^ num3 >> 17;
            num3 = num3 + (num3 << 5);
            return num3;
        }


    }
    */

}
