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

                    string portraitDirectoryPath = GetPortrait_Patch.GetUnitPortraitPath(__instance.Unit.Blueprint, characterName);


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


}
