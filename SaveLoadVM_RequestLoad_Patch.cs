using Harmony12;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Persistence;
using Kingmaker.UI.MVVM._VM.SaveLoad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNpcPortraits
{
    [HarmonyPatch(typeof(SaveLoadVM), "RequestLoad", new Type[] { typeof(SaveInfo) })]
    public static class SaveLoadVM_RequestLoad_Patch
    {


        private static bool Prefix(SaveLoadVM __instance, SaveInfo saveInfo)
        {

            //if (!Main.enabled)
                return true;


            //var portrait = ResourcesLibrary.TryGetBlueprint<BlueprintPortrait>("9fe4f89ecf15b874db9d1d2bf3ef33d2");


            foreach (var pp in saveInfo.PartyPortraits)
            {
                if (saveInfo.PartyPortraits != null && pp != null)
                {

                    if (pp.m_Data != null)
                    {

                        Main.DebugLog("m_Data.CustomId: " + pp.m_Data.CustomId);

                        Main.DebugLog("m_Data.m_CustomPortraitId: " + pp.m_Data.m_CustomPortraitId);

                    }
                    else
                        Main.DebugLog("saveInfo.PartyPortraits[0].m_Data is NULL");



                    if (pp.m_Blueprint != null)
                    {
                        if (pp.m_Blueprint.Data != null)
                        {
                            Main.DebugLog("m_Blueprint.Data.CustomId: " + pp.m_Blueprint.Data.CustomId);


                            // var dir = Path.Combine(Main.GetCompanionPortraitsDirectory(), "9fe4f89ecf15b874db9d1d2bf3ef33d2_3");

                            // Directory.CreateDirectory(dir);
                            //  Main.SaveOriginals2(saveInfo.PartyPortraits[0].m_Blueprint.Data, dir);

                        }
                        else
                            Main.DebugLog("saveInfo.PartyPortraits[0].m_Blueprint.Data is NULL");


                        Main.DebugLog("m_Blueprint.name: " + pp.m_Blueprint.name);
                    }
                    else
                        Main.DebugLog("saveInfo.PartyPortraits[0].m_Blueprint is NULL");


                }
                else
                    Main.DebugLog("saveInfo.PartyPortraits is NULL");

            }
                
            return false;
        }
    }
}
