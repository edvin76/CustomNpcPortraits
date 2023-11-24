using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Utility;
using UnityEngine.SceneManagement;
using System.Reflection.Emit;
using Harmony12;
using Kingmaker;
using Kingmaker.Blueprints.Root;
using System.Diagnostics;
using Kingmaker.GameModes;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Blueprints;
using Kingmaker.PubSubSystem;
using Kingmaker.ResourceLinks;
using Kingmaker.Localization;
using System.Reflection;
using Kingmaker.Blueprints.Area;
using Kingmaker.Cheats;
using Kingmaker.EntitySystem.Persistence;
using Kingmaker.Blueprints.Items;
using Kingmaker.Items;
using UniRx;
using Kingmaker.Globalmap.State;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.BundlesLoading;
using Kingmaker.UI.UnitSettings;
using ExtensionMethods;
using Kingmaker.Controllers.Dialog;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Classes;
using Kingmaker.UI.MVVM._PCView.LoadingScreen;
using Owlcat.Runtime.Core.Utils;
using Kingmaker.UnitLogic;
using TMPro;
using Kingmaker.UI.Common;
using Kingmaker.UI.MVVM;
using Kingmaker.UI.ServiceWindow;
using Kingmaker.Visual.CharacterSystem;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Controllers;
using Kingmaker.UnitLogic.ActivatableAbilities;
using Kingmaker.UnitLogic.Buffs.Blueprints;
using Kingmaker.UnitLogic.Buffs;
using Kingmaker.Visual.Particles.FxSpawnSystem;
using Kingmaker.Visual.Particles;
using UnityEngine.Rendering;
using Kingmaker.Visual;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Designers.Mechanics.EquipmentEnchants;
using Kingmaker.Enums;
//using Kingmaker.ResourceManagement;
using Kingmaker.ResourceManagement;
using Kingmaker.AreaLogic.Etudes;
using Graphics = UnityEngine.Graphics;
using Kingmaker.View;
using Kingmaker.EntitySystem;
using Kingmaker.Designers.EventConditionActionSystem.Conditions;
using Kingmaker.ElementsSystem;

namespace ExtensionMethods
{



    public class PortraitLoader
	{

		// Loosely based on https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/
		public static class Image2Sprite
		{
			public static string icons_folder = "";
			public static Sprite Create(string filePath, Vector2Int size, TextureFormat format)
			{
				
				var bytes = File.ReadAllBytes(icons_folder + filePath);
				var texture = new Texture2D(size.x, size.y, format, false);
				texture.mipMapBias = 15.0f;
				_ = texture.LoadImage(bytes);
				return Sprite.Create(texture, new Rect(0, 0, size.x, size.y), new Vector2(0, 0));
			}
		}


		public static PortraitData LoadPortraitData(string imageFolderPath)
		{
			//var imageFolderPath = Path.Combine(ModSettings.ModEntry.Path, "Assets", "Portraits");
			var smallImagePath = Path.Combine(imageFolderPath, $"Small.png");
			var mediumImagePath = Path.Combine(imageFolderPath, $"Medium.png");
			var fullImagePath = Path.Combine(imageFolderPath, $"FullLength.png");
			var smallPortraitHandle = new CustomPortraitHandle(smallImagePath, PortraitType.SmallPortrait, CustomPortraitsManager.Instance.Storage)
			{
				Request = new SpriteLoadingRequest(smallImagePath)
				{
					Resource = Image2Sprite.Create(smallImagePath, new Vector2Int(184, 242), TextureFormat.RGBA32)
				}
			};
			var mediumPortraitHandle = new CustomPortraitHandle(mediumImagePath, PortraitType.HalfLengthPortrait, CustomPortraitsManager.Instance.Storage)
			{
				Request = new SpriteLoadingRequest(mediumImagePath)
				{
					Resource = Image2Sprite.Create(mediumImagePath, new Vector2Int(330, 432), TextureFormat.RGBA32)
				}
			};
			var fullPortraitHandle = new CustomPortraitHandle(fullImagePath, PortraitType.FullLengthPortrait, CustomPortraitsManager.Instance.Storage)
			{
				Request = new SpriteLoadingRequest(fullImagePath)
				{
					Resource = Image2Sprite.Create(fullImagePath, new Vector2Int(692, 1024), TextureFormat.RGBA32)
				}
			};
			return new PortraitData(imageFolderPath)
			{
				SmallPortraitHandle = smallPortraitHandle,
				HalfPortraitHandle = mediumPortraitHandle,
				FullPortraitHandle = fullPortraitHandle,
				PortraitCategory = PortraitCategory.None,
				IsDefault = false,
				InitiativePortrait = false
			};
		}
	}
	public static class MyExtensions
	{
		public static int WordCount(this string str)
		{
			return str.Split(new char[] { ' ', '.', '?' },
							 StringSplitOptions.RemoveEmptyEntries).Length;
		}

		public static string cleanCharname(this string filename)
		{ 
			
			
			string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

			foreach (char c in invalid)
			{
				filename = filename.Replace(c.ToString(), "-");
			}

			/*string whitelist = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_ ";
			foreach (char c in filename)
			{
				if (!whitelist.Contains(c))
				{
					filename = filename.Replace(c, '-');
				}
			}*/
			return filename;
			//return Regex.Replace(filename, "[^a-zA-Z0-9_.-]+", "_", RegexOptions.Compiled);
		}
	}
}



namespace CustomNpcPortraits
{

    [Harmony12.HarmonyPatch(typeof(Polymorph), "OnDeactivate", MethodType.Normal)]
	public static class Polymorph_OnDeactivate_Patch
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00005694 File Offset: 0x00003894
		public static void Postfix(Polymorph __instance)
		{
			//Main.DebugLog("Deativate: " + __instance.OwnerBlueprint.name);

			if (!Main.enabled || !Main.settings.ManageCompanions)
				return;


			if (Game.Instance.Player.AllCharacters.Contains(__instance.Owner))
			{

				string characterName = __instance.Owner.CharacterName;

				//!blueprintCampaign.name.Equals("MainCampaign")) || Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys.FirstOrDefault(x => x.name.Contains("Fox")
				if (characterName.Equals(Main.NenioName) )
					characterName = Main.NenioName+"Fox_Portrait";


				string prefix = Main.GetCompanionPortraitDirPrefix();
				string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
				string portraitDirectoryName = prefix + characterName;

				//Main.DebugLog("1 - " + Path.Combine(portraitsDirectoryPath, portraitDirectoryName));
				string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

				if (Directory.Exists(portraitDirectoryPath))
				{
					if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
					{
						string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

						string dir = Path.GetFileNameWithoutExtension(dirs[0]);
						//Main.DebugLog(dir);
						if (!dir.Equals("root"))
							portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName, dir);
						//Main.DebugLog(portraitDirectoryPath);
					}
					else
					{
						if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
							if (File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), "Medium.png")))
							{

								portraitDirectoryPath = Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName());


							}
					}

					if (File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
					{
						//Main.DebugLog("Poly: " + portraitDirectoryPath);
						PortraitData Data = new PortraitData(portraitDirectoryPath);
						BlueprintPortrait bp = new BlueprintPortrait();

						bp.Data = Data;

						Main.pauseGetPortraitsafe = true;
						__instance.Owner.UISettings.SetPortrait(bp);
						Main.pauseGetPortraitsafe = false;


					}
				}
				return;
			}



		}
	}


	[Harmony12.HarmonyPatch(typeof(Polymorph), "OnActivate", MethodType.Normal)]
	public static class Polymorph_OnActivate_Patch
	{
		// Token: 0x06000053 RID: 83 RVA: 0x00005694 File Offset: 0x00003894
		public static void Postfix(Polymorph __instance)
		{
			if (!Main.enabled || !Main.settings.ManageCompanions)
				return;
			//return;
			//	Main.DebugLog("OnActivate");


			//Main.DebugLog("Activate: " +__instance.OwnerBlueprint.name);


			//unitEntityData.GetActivePolymorph().Component.m_Portrait


			if (Game.Instance.Player.AllCharacters.Contains(__instance.Owner))
			{
				//GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name,"Medium.png"
				string characterName = __instance.Owner.CharacterName;


				string prefix = Main.GetCompanionPortraitDirPrefix();
				string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
				string portraitDirectoryName = prefix + characterName;

				//Main.DebugLog("1 - " + Path.Combine(portraitsDirectoryPath, portraitDirectoryName));


				string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName, __instance.OwnerBlueprint.name);

				if (characterName.Equals(Main.NenioName) && __instance.OwnerBlueprint.name.Equals("KitsunePolymorphBuff_Nenio"))
				{
					//
					//check for foxbuff if foxbuff then do this otherwse normalshapeshifting from spell
					portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);
				}
				else
					Directory.CreateDirectory(portraitDirectoryPath);
				
				if (Directory.Exists(portraitDirectoryPath))
				{
					if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
					{
						string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

						string dir = Path.GetFileNameWithoutExtension(dirs[0]);
						//Main.DebugLog(dir);
						if (!dir.Equals("root"))
							portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName, dir);
						//Main.DebugLog(portraitDirectoryPath);
					}
					else
					{
						if (!File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), "Medium.png")))
						{
							if(__instance.Owner.GetActivePolymorph().Component?.m_Portrait.GetBlueprint() != null)
							Main.SaveOriginals2((__instance.Owner.GetActivePolymorph().Component.m_Portrait.GetBlueprint() as BlueprintPortrait).Data, portraitDirectoryPath);
						}

						if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
							portraitDirectoryPath = Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName());



					}


					if (File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
					{


						//	Main.DebugLog("Poly: " + portraitDirectoryPath);
						PortraitData Data = new PortraitData(Path.Combine(portraitDirectoryPath));



						//var portrait = ResourcesLibrary.TryGetBlueprint<BlueprintPortrait>("a6e4ff25a8da46a44a24ecc5da296073");

						//var buff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("a13e2e71485901045b1722824019d6f5");
						//var poly = buff.ComponentsArray.OfType<Polymorph>().FirstOrDefault();


						var poly = __instance.OwnerBlueprint.ComponentsArray.OfType<Polymorph>().FirstOrDefault();



						BlueprintPortrait bp2 = poly.m_Portrait;

						bp2.Data = Data;
						poly.m_Portrait = bp2.ToReference<BlueprintPortraitReference>();




						BlueprintPortrait bp = new BlueprintPortrait();

						bp.Data = Data;




						//	__instance.m_Portrait = bp.ToReference<BlueprintPortraitReference>();

						//	__instance.Owner.UISettings.SetOverridePortrait(bp);

						Main.pauseGetPortraitsafe = true;
						__instance.Owner.UISettings.SetPortrait(bp);
						Main.pauseGetPortraitsafe = false;

					}
				}
				return;

			}
		}
	}



	
	


	/*
	[HarmonyPatch(typeof(PortraitData), "get_PetEyePortrait")]
	public static class EyePortraitInjecotr
	{
		public static Dictionary<PortraitData, Sprite> Replacements = new Dictionary<PortraitData, Sprite>();
		public static bool Prefix(PortraitData __instance, ref Sprite __result)
		{
			if (Replacements.TryGetValue(__instance, out __result))
				return false;
			return true;
		}
	}
	*/

	[HarmonyPatch(typeof(SelectionCharacterController), nameof(SelectionCharacterController.GetGroup), new Type[] { typeof(bool), typeof(bool) })]
	public static class GetGroup_Patch
	{
		public static bool Prefix(ref List<UnitEntityData> __result, bool withRemote, bool withoutPets)
		{
			//return true;
			if (!Main.enabled || !Main.settings.RightverseGroupPortraits)
				return true;

			List<UnitEntityData> list = Game.Instance.Player.Party.ToList<UnitEntityData>();
			if (withRemote)
			{
				List<UnitEntityData> unitEntityDatas = Game.Instance.Player.RemoteCompanions.ToList<UnitEntityData>();
				//unitEntityDatas.Reverse();
				list.AddRange(unitEntityDatas);
				if (withoutPets)
				{
					list.RemoveAll((UnitEntityData c) => c.IsPet);
				}
				else
				{
					foreach (UnitEntityData unitEntityDatum1 in Game.Instance.Player.AllCharacters.Where<UnitEntityData>((UnitEntityData c) =>
					{
						CompanionState? nullable;
						CompanionState companionState;
						CompanionState? nullable1;
						CompanionState? nullable2;
						bool valueOrDefault;
						CompanionState? nullable3;
						bool flag;
						if (c.IsPet)
						{
							UnitEntityData master = c.Master;
							if (master != null)
							{
								UnitPartCompanion unitPartCompanion = master.Get<UnitPartCompanion>();
								if (unitPartCompanion != null)
								{
									nullable2 = new CompanionState?(unitPartCompanion.State);
								}
								else
								{
									nullable1 = null;
									nullable2 = nullable1;
								}
								nullable = nullable2;
								companionState = CompanionState.Remote;
								valueOrDefault = nullable.GetValueOrDefault() == companionState & nullable.HasValue;
							}
							else
							{
								valueOrDefault = false;
							}
							if (!valueOrDefault)
							{
								UnitEntityData unitEntityDatum = c.Master;
								if (unitEntityDatum != null)
								{
									UnitPartCompanion unitPartCompanion1 = unitEntityDatum.Get<UnitPartCompanion>();
									if (unitPartCompanion1 != null)
									{
										nullable3 = new CompanionState?(unitPartCompanion1.State);
									}
									else
									{
										nullable1 = null;
										nullable3 = nullable1;
									}
									nullable = nullable3;
									companionState = CompanionState.InParty;
									flag = nullable.GetValueOrDefault() == companionState & nullable.HasValue;
								}
								else
								{
									flag = false;
								}
								if (!flag)
								{
									return false;
								}
							}
							return !list.Contains(c);
						}
						return false;
					}))
					{
						list.Add(unitEntityDatum1);
					}
				}
			}
			else if (withoutPets)
			{
				list.RemoveAll((UnitEntityData c) => c.IsPet);
			}
			else
			{
				foreach (UnitEntityData unitEntityDatum2 in
					from c in Game.Instance.Player.PartyAndPets
					where c.IsPet
					select c)
				{
					list.Add(unitEntityDatum2);
				}
			}
			//	return list;
			__result = list;
			return false;
		}

	

		/*
		public static bool Prefix(ref List<UnitEntityData> __result, bool withRemote, bool withoutPets)
		{
			if (Main.enabled && Main.settings.RightverseGroupPortraits)
			{
				List<UnitEntityData> list = Game.Instance.Player.Party.ToList<UnitEntityData>();
				if (withRemote)
				{
					List<UnitEntityData> unitEntityDatas = Game.Instance.Player.RemoteCompanions.ToList<UnitEntityData>();
					//unitEntityDatas.Reverse();
					list.AddRange(unitEntityDatas);
					if (withoutPets)
					{
						list.RemoveAll((UnitEntityData c) => c.IsPet);
					}
					else
					{
						foreach (UnitEntityData unitEntityDatum1 in Game.Instance.Player.AllCharacters.Where<UnitEntityData>((UnitEntityData c) =>
						{
							CompanionState? nullable;
							CompanionState companionState;
							CompanionState? nullable1;
							CompanionState? nullable2;
							bool valueOrDefault;
							CompanionState? nullable3;
							bool flag;
							if (c.IsPet)
							{
								UnitEntityData master = c.Master;
								if (master != null)
								{
									UnitPartCompanion unitPartCompanion = master.Get<UnitPartCompanion>();
									if (unitPartCompanion != null)
									{
										nullable2 = new CompanionState?(unitPartCompanion.State);
									}
									else
									{
										nullable1 = null;
										nullable2 = nullable1;
									}
									nullable = nullable2;
									companionState = CompanionState.Remote;
									valueOrDefault = nullable.GetValueOrDefault() == companionState & nullable.HasValue;
								}
								else
								{
									valueOrDefault = false;
								}
								if (!valueOrDefault)
								{
									UnitEntityData unitEntityDatum = c.Master;
									if (unitEntityDatum != null)
									{
										UnitPartCompanion unitPartCompanion1 = unitEntityDatum.Get<UnitPartCompanion>();
										if (unitPartCompanion1 != null)
										{
											nullable3 = new CompanionState?(unitPartCompanion1.State);
										}
										else
										{
											nullable1 = null;
											nullable3 = nullable1;
										}
										nullable = nullable3;
										companionState = CompanionState.InParty;
										flag = nullable.GetValueOrDefault() == companionState & nullable.HasValue;
									}
									else
									{
										flag = false;
									}
									if (!flag)
									{

										return false;
									}
								}
								return !list.Contains(c);
							}
							return false;
						}))
						{
							list.Add(unitEntityDatum1);
						}
					}
				}
				else if (withoutPets)
				{
					list.RemoveAll((UnitEntityData c) => c.IsPet);
				}
				else
				{
					foreach (UnitEntityData unitEntityDatum2 in
						from c in Game.Instance.Player.PartyAndPets
						where c.IsPet
						select c)
					{
						list.Add(unitEntityDatum2);
					}
				}
				__result = list;
				return false;
			}
			else
				return true;
		}
		*/
	}

	//UnitHelper.AddBuff(__instance.GetAvatar()., LightHaloBuff, __instance.Unit);

	/*
	foreach (ActivatableAbility a in __instance.Unit.ActivatableAbilities.RawFacts)
	{


		Main.DebugLog(a.Name +" - "+a.IsActive + " - ");
	}
		*/


	//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");
	//LightHaloBuff.FxOnStart
	//__instance.Unit.GetFeature(LightHaloFeature).Activate();




	[HarmonyPatch(typeof(DollRoom), "UpdateCharacter")]
	public static class DollRoomHalo
	{
		public static void Postfix(DollRoom __instance)
		{
			if (Main.enabled && Main.settings.DollroomHalo)
			{
				if (__instance.Unit != null && !__instance.Unit.CharacterName.IsNullOrEmpty() && __instance.Unit.CharacterName.Equals("Arueshalae") )
				{
					/*
					//whole body holy lights
					buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("2370ee24b03d4cd3b2eb346b7a1a18c0"));
					//triangle halo
					buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("840bbe624d7dad4439199aa2dc410b1a"));
					//lighthalo
					buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0b1c9d2964b042e4aadf1616f653eb95"));
					//devil horns
					buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("d0649010d93907745a44034ad6eeeb5e"));
					//demon eyes
					buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("5f2faadfbe412fd41b2823f70f5820df"));
					// electric halo
					buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("00ca55e6c8dc07142807e40da6e3b13f"));
					 */


					//BlueprintBuff LightHaloBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(Main.bpbuffs[Main.k].AssetGuid);
					BlueprintBuff LightHaloBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0b1c9d2964b042e4aadf1616f653eb95");

					Main.dollRoomxtraFxBrightness = false;

					//foreach (BlueprintBuff buff in Main.buffs)
					//{
						if (__instance.Unit.HasFact(LightHaloBuff))
						{
							
							//MechanicsContext mechanicsContext = new MechanicsContext(__instance.Unit, __instance.Unit, LightHaloBuff, null, null);
							//__instance.Unit.Facts.Add<Buff>(new Buff(LightHaloBuff, mechanicsContext, null));

							var fxHandle = FxHelper.SpawnFxOnGameObject(LightHaloBuff.FxOnStart.Load(), __instance.GetAvatar().gameObject, true, null, default(Vector3), 1f, FxPriority.EventuallyImportant);
							__instance.GetAvatar().m_AdditionalFXs.Add(fxHandle);

							Main.fxHandles.Add(LightHaloBuff.AssetGuid.ToString(),fxHandle);


							fxHandle.RunAfterSpawn(new Action<GameObject>(DollRoom.UnscaleFxTimes));

		
						}
						else
						{
							//UIUtility.SendWarning("no");
							IFxHandle value;
							if (Main.fxHandles.TryGetValue(LightHaloBuff.AssetGuid.ToString(), out value))
							{
								FxHelper.Destroy(value, true);
								Main.fxHandles.Remove(LightHaloBuff.AssetGuid.ToString());
							}
						}
					//}
				}
			}
			return;
		}
	}



	[HarmonyPatch(typeof(DollCamera), nameof(DollCamera.OnEnable))]
	internal static class DollCamera_OnEnable
	{
		static void Postfix(DollCamera __instance)
		{
			//if(Main.dollRoomxtraFxBrightness)
			if (!Main.enabled || !Main.settings.DollroomHalo) return;

			if(__instance.GetComponentInChildren<UnityEngine.Rendering.Volume>() != null && __instance.GetComponentInChildren<UnityEngine.Rendering.Volume>().gameObject != null)
			__instance.GetComponentInChildren<UnityEngine.Rendering.Volume>().gameObject.SetActive(false);
		}
	}





	[HarmonyPatch(typeof(Character), nameof(Character.ApplyAdditionalVisualSettings))]
	static class Character_ApplyAdditionalVisualSettings
	{
		static void Postfix(Character __instance)
		{

			
			if (!__instance.IsInDollRoom) return;
			if (!Main.enabled || !Main.settings.DollroomHalo) return;

			//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("8fb2ee1d545f46f2be5b33bdcdd5f138");

#if DEBUG
			/*
			if (Main.k > -1 && Main.k < 1070)
			{
				if (Main.bpbuffs != null && Main.bpbuffs.Count() > 0 && Main.bpbuffs[Main.k] != null)
				{
					BlueprintBuff LightHaloBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>(Main.bpbuffs[Main.k].AssetGuid);
					var fxPrefab = LightHaloBuff.FxOnStart;

					//var fxPrefab = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("840bbe624d7dad4439199aa2dc410b1a").FxOnStart;

					var fxHandle = FxHelper.SpawnFxOnGameObject(fxPrefab.Load(), __instance.gameObject, partySource: true);

					__instance.m_AdditionalFXs.Add(fxHandle);

					fxHandle.RunAfterSpawn(gameObject =>
					{
						DollRoom.UnscaleFxTimes(gameObject);

					});
				}
			}
			*/
#endif

			//Main.DebugLog(__instance.name);
			UnitEntityData result;
			//foreach (BlueprintBuff buff in Main.buffs)
			//{
				BlueprintBuff LightHaloBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0b1c9d2964b042e4aadf1616f653eb95");
				if (Game.Instance.Player.Party.TryFind(u => __instance.name.Contains(u.CharacterName), out result))
					if (result.HasFact(LightHaloBuff))
					{
						//Main.DebugLog("yee");
						//
						var fxPrefab2 = LightHaloBuff.FxOnStart;

						//var fxPrefab = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("840bbe624d7dad4439199aa2dc410b1a").FxOnStart;

						var fxHandle2 = FxHelper.SpawnFxOnGameObject(fxPrefab2.Load(), __instance.gameObject, partySource: true);
						__instance.m_AdditionalFXs.Add(fxHandle2);

						fxHandle2.RunAfterSpawn(gameObject =>
						{
							/*
							//electric halo
							if (buff.AssetGuid.ToString() == "00ca55e6c8dc07142807e40da6e3b13f")
							{
								Main.dollRoomxtraFxBrightness = true;

								gameObject.transform.Find("Root_PalmLeft").gameObject.SetActive(false);

								var haloObject = gameObject.transform.Find("Head_Back1").gameObject;

								var lockAxis = haloObject.transform.Find("LockAxis").gameObject;

								// Billboard is a component, so we deactivate it with the 'enabled' property
								lockAxis.GetComponent<Billboard>().enabled = false;
								haloObject.GetComponent<SnapToLocator>().Locator.CameraOffset = 0;

								lockAxis.transform.localRotation = Quaternion.Euler(347, 180, 0);
								lockAxis.transform.localPosition = new Vector3(0, 0.04f, -0.15f);
								lockAxis.transform.localScale = new Vector3(1, 0.8f, -0.8f);

								//lockAxis.ForAllChildren(c =>
								//	{
								//	if (c == lockAxis) return;

								//		c.transform.localRotation = Quaternion.Euler(0, 270, 0);
								//	c.transform.localPosition = new Vector3(-0.15f, 0.05f, 0);

								//	});
							}
							*/

							//light halo
							//if (buff.AssetGuid.ToString() == "0b1c9d2964b042e4aadf1616f653eb95")
							//{
								//Main.dollRoomxtraFxBrightness = true;
							
							/*
								var lightObject = gameObject.GetComponentInChildren<Light>().gameObject;
								var lightObjectCopy = UnityEngine.Object.Instantiate(lightObject);

								lightObjectCopy.transform.parent = lightObject.transform.parent.parent;

								lightObjectCopy.SetActive(true);
							
							*/
							//}


							//

							DollRoom.UnscaleFxTimes(gameObject);

						});
					}
					else
					{
						//UIUtility.SendWarning("no");
						IFxHandle value;
						if (Main.fxHandles.TryGetValue(LightHaloBuff.AssetGuid.ToString(), out value))
						{
						//	Main.DebugLog("destroyed");

							FxHelper.Destroy(value, true);
							Main.fxHandles.Remove(LightHaloBuff.AssetGuid.ToString());
						}
					}
			//}




		}
	}

	/*
	[HarmonyPatch(typeof(Character), nameof(Character.ApplyAdditionalVisualSettings))]
	static class Character_ApplyAdditionalVisualSettings
	{
		static void Postfix(Character __instance)
		{
			if (!__instance.IsInDollRoom) return;

			//var fxPrefab = Blueprints.AasimarHaloBuff.GetBlueprint().FxOnStart;
			//halo
			//var fxPrefab_ = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0b1c9d2964b042e4aadf1616f653eb95").FxOnStart;

			//horns
			//var fxPrefab = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("d0649010d93907745a44034ad6eeeb5e").FxOnStart;


			//demon eyes
			//var fxPrefab = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("5f2faadfbe412fd41b2823f70f5820df").FxOnStart;

			// electric halo
			var fxPrefab = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("00ca55e6c8dc07142807e40da6e3b13f").FxOnStart;

			//weird light face mask
			//var fxPrefab = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("db2522aaf66d67c4a93986fcb411e6c7").FxOnStart;
			

			var fxHandle = FxHelper.SpawnFxOnGameObject(fxPrefab.Load(), __instance.gameObject, partySource: true);

			__instance.m_AdditionalFXs.Add(fxHandle);

			fxHandle.RunAfterSpawn(gameObject =>
			{








				gameObject.GetComponentInChildren<Billboard>().gameObject.SetActive(false);

				DollRoom.UnscaleFxTimes(gameObject); 
				
				gameObject.GetComponent<SnapToLocator>().Locator.CameraOffset = 0;



			});

//			var inGamePCView = Game.Instance.RootUiContext.m_UIView.GetComponent<InGamePCView>();

	//		__instance.GetComponentInChildren<Kingmaker.Visual.Particles.SnapToLocator>().Locator.CameraOffset = 0;


	//		var head =
//	inGamePCView?.GetComponentsInChildren<PooledFx>()?.Select(c => c.name == "Head_1");

		}
	}
	*/
#if DEBUG
	[EnableReloading]
#endif
	public static class Main
	{
		

		public static bool Load(UnityModManager.ModEntry modEntry)
		{



			ModEntry = modEntry;


			//Logger = modEntry.Logger;


			/*
			//whole body holy lights
			buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("2370ee24b03d4cd3b2eb346b7a1a18c0"));
			//triangle halo
			buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("840bbe624d7dad4439199aa2dc410b1a"));
			//lighthalo
			buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0b1c9d2964b042e4aadf1616f653eb95"));
			//devil horns
			buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("d0649010d93907745a44034ad6eeeb5e"));
			//demon eyes
			buffs.Add(ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("5f2faadfbe412fd41b2823f70f5820df"));
			// electric halo
			buffs.Add(ResourcesLibrary.defa<BlueprintBuff>("00ca55e6c8dc07142807e40da6e3b13f"));

			bpbuffs = new List<BlueprintBuff>();

			string str = BundlesLoadService.BundlesPath("cheatdata.json");

			if(!str.IsNullOrEmpty())
			if (File.Exists(str))
				blueprintList = JsonUtility.FromJson<BlueprintList>(File.ReadAllText(str));
			//isInitRunning = true;

			if(blueprintList !=null)
			foreach (BlueprintList.Entry be in blueprintList.Entries)
			{
				if(be != null)
					if(!be.TypeFullName.IsNullOrEmpty())
				if (be.TypeFullName.EndsWith("BlueprintBuff"))
								if(!be.Guid.IsNullOrEmpty())
				{
					BlueprintBuff bc = Utilities.GetBlueprintByGuid<BlueprintBuff>(be.Guid);

					if(bc != null)
										if(!bpbuffs.Contains(bc))
											if(bc.FxOnStart.Exists())
					bpbuffs.Add(bc);
				}
			}

			UIUtility.SendWarning(bpbuffs.Count().ToString());
			*/



			Main.logger = modEntry.Logger;


			//(new Harmony(modEntry.Info.Id)).PatchAll(Assembly.GetExecutingAssembly());

			Main.harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);

			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);

			modEntry.OnShowGUI = new Action<UnityModManager.ModEntry>(Main.OnShowGUI);

			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);

			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);

			modEntry.OnHideGUI = new Action<UnityModManager.ModEntry>(Main.OnHideGUI);



#if DEBUG

			modEntry.OnUpdate = OnUpdate;

			modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Main.Unload);


			if (!Main.ApplyPatch(typeof(BlueprintDlcReward_IsAvailable_Patch), "BlueprintDlcReward_IsAvailable_Patch"))
			{
				throw Main.Error("Failed to patch BlueprintDlcReward_IsAvailable");
			}


			
#endif
			//modEntry.OnUpdate = new Action<UnityModManager.ModEntry, float>(Main.Init);


			if (!Main.ApplyPatch(typeof(GetPortrait_Patch), "GetPortrait_Patch"))
			{
				throw Main.Error("Failed to patch GetPortrait");
			}

			if (!Main.ApplyPatch(typeof(GetPortraitSafe_Patch), "GetPortraitSafe_Patch"))
			{
				throw Main.Error("Failed to patch GetPortraitSafe");
			}
			if (!Main.ApplyPatch(typeof(GetPortraitSafeGeneral_Patch), "GetPortraitSafeGeneral_Patch"))
			{
				throw Main.Error("Failed to patch GetPortraitSafeGeneral");
			}
			if (!Main.ApplyPatch(typeof(GameMode_OnActivate_Patch), "GameMode_OnActivate_Patch"))
			{
				throw Main.Error("Failed to patch GameMode_OnActivate");
			}

			if (!Main.ApplyPatch(typeof(Player_OnAreaLoaded_Patch), "Player_OnAreaLoaded_Patch"))
			{
				throw Main.Error("Failed to patch Player_OnAreaLoaded");
			}

			if (!Main.ApplyPatch(typeof(Player_AddCompanion_Patch), "Player_AddCompanion_Patch"))
			{
				throw Main.Error("Failed to patch Player_AddCompanion");
			}

			if (!Main.ApplyPatch(typeof(InitiativeTrackerUnitVM_Get_Portrait_Patch), "InitiativeTrackerUnitVM_Get_Portrait_Patch"))
			{
				throw Main.Error("Failed to patch InitiativeTrackerUnitVM_Get_Portrait");
			}

			if (!Main.ApplyPatch(typeof(BlueprintsCache_Patch), "BlueprintsCache_Patch"))
			{
				throw Main.Error("Failed to patch BlueprintsCache");
			}

			if (!Main.ApplyPatch(typeof(LoadingScreenPCView_Patch), "LoadingScreenPCView_Patch"))
			{
				throw Main.Error("Failed to patch LoadingScreenPCView");
			}

			if (!Main.ApplyPatch(typeof(DollCamera_OnEnable), "DollCamera_OnEnable"))
			{
				throw Main.Error("Failed to patch DollCamera_OnEnable");
			}

			if (!Main.ApplyPatch(typeof(Character_ApplyAdditionalVisualSettings), "Character_ApplyAdditionalVisualSettings"))
			{
				throw Main.Error("Failed to patch Character_ApplyAdditionalVisualSettings");
			}


	

			if (!Main.ApplyPatch(typeof(GetGroup_Patch), "GetGroup_Patch"))
			{
				throw Main.Error("Failed to patch GetGroup");
			}
			if (!Main.ApplyPatch(typeof(Polymorph_OnActivate_Patch), "Polymorph_OnActivate_Patch"))
			{
				throw Main.Error("Failed to patch Polymorph_OnActivate_Patch");
			}
			if (!Main.ApplyPatch(typeof(Polymorph_OnDeactivate_Patch), "Polymorph_OnDeactivate_Patch"))
			{
				throw Main.Error("Failed to patch Polymorph_OnDeactivate_Patch");
			}

			if (!Main.ApplyPatch(typeof(GameMode_OnDeactivate_Patch), "GameMode_OnDeActivate_Patch"))
			{
				throw Main.Error("Failed to patch GameMode_OnDeActivate");
			}
			

			BlueprintUnit dummyUnit = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("1b893f7cf2b150e4f8bc2b3c389ba71d");

			if (dummyUnit != null)
			{


				NenioName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("1b893f7cf2b150e4f8bc2b3c389ba71d").LocalizedName.String;

				ArueName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("a352873d37ec6c54c9fa8f6da3a6b3e1").LocalizedName.String;
				FinneanName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("ea8034769ab7d584e97b5227cbc03296").LocalizedName.String;
				CiarName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("7ece3afabe2b6f343b17d1eaa409d273").LocalizedName.String;
				WoljifName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("766435873b1361c4287c351de194e5f9").LocalizedName.String;
				GalfreyName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("e46927657a79db64ea30758db3f42bb9").LocalizedName.String;
				StauntonName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("0bcf3c125a28d164191e874e3c0c52de").LocalizedName.String;


				IrabethName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("d1e567736abf23943b9f041ba7a0bc23").LocalizedName.String;


				AstyName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("d8e2475977bd87b439c4bba8f5f55949").LocalizedName.String;
				TranName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("c35607fd0f8064f42b4d71f7eb50e96c").LocalizedName.String;
				VelhmName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("f9c01a9515cd1f347800685ddbfbcc41").LocalizedName.String;
				LannName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("cb29621d99b902e4da6f5d232352fbda").LocalizedName.String;
				EmberName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("2779754eecffd044fbd4842dba55312c").LocalizedName.String;
				PentaName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("942862a48f1644ae85ac5e3c9deb720c").LocalizedName.String;
				SeelahName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("54be53f0b35bf3c4592a97ae335fe765").LocalizedName.String;
				DaeranName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("096fc4a96d675bb45a0396bcaa7aa993").LocalizedName.String;
				RegillName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("0d37024170b172346b3769df92a971f5").LocalizedName.String;
				SendriName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("561036c882a640089b1d42f03ebe3a6c").LocalizedName.String;
				UlbrigName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("42f0d5ec3dc844feb44b04507a7c1bfc").LocalizedName.String;
				TreverName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("0bb1c03b9f7bbcf42bb74478af2c6258").LocalizedName.String;
				WenduagName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("ae766624c03058440a036de90a7f2009").LocalizedName.String;
				RekarthName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("3e0014e4be454482a2797fd81123d7b4").LocalizedName.String;
				GreyborName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("f72bb7c48bb3e45458f866045448fb58").LocalizedName.String;
				CamelliaName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("397b090721c41044ea3220445300e1b8").LocalizedName.String;
				DelamereName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("6b1f599497f5cfa42853d095bda6dafd").LocalizedName.String;
				KestoglyrName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("e551850403d61eb48bb2de010d12c894").LocalizedName.String;
				SosielName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("1cbbbb892f93c3d439f8417ad7cbb6aa").LocalizedName.String;

				companions = new List<string>{
ArueName,
CiarName,
NenioName,
GalfreyName,
StauntonName,
WoljifName,
				LannName,
				EmberName,
				PentaName,
				SeelahName,
				DaeranName,
				RegillName,
				SendriName,
				UlbrigName,
				TreverName,
				WenduagName,
				RekarthName,
				GreyborName,
				CamelliaName,
				DelamereName,
				KestoglyrName,
				SosielName          };
				BlueprintsCache_Patch.popUnitNames();

			}

			return true;
		}


		/// <summary>
		/// We cannot modify blueprints until after the game has loaded them. We patch BlueprintsCache.Init
		/// to initialize our modifications as soon as the game blueprints have loaded.
		/// </summary>
		[HarmonyPriority(Priority.First)]
		[HarmonyPatch(typeof(BlueprintsCache), "Init")]
		public static class BlueprintsCache_Patch
		{
			//[HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
			private static void Postfix()
			{
				if (loaded) return;
				loaded = true;

				Directory.CreateDirectory(Main.GetCompanionPortraitsDirectory());

				Main.GetNpcPortraitsDirectory();
				Main.GetArmyPortraitsDirectory();

				Main.DebugLog("Trying to run popUnitNames");








				//arue a352873d37ec6c54c9fa8f6da3a6b3e1
				//ciar 7ece3afabe2b6f343b17d1eaa409d273
				//wolj 766435873b1361c4287c351de194e5f9
				//galfrey e46927657a79db64ea30758db3f42bb9
				//stauntn 0bcf3c125a28d164191e874e3c0c52de


				//finnean ea8034769ab7d584e97b5227cbc03296







			NenioName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("1b893f7cf2b150e4f8bc2b3c389ba71d").LocalizedName.String;

				ArueName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("a352873d37ec6c54c9fa8f6da3a6b3e1").LocalizedName.String;
				FinneanName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("ea8034769ab7d584e97b5227cbc03296").LocalizedName.String;
				CiarName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("7ece3afabe2b6f343b17d1eaa409d273").LocalizedName.String;
				WoljifName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("766435873b1361c4287c351de194e5f9").LocalizedName.String;
				GalfreyName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("e46927657a79db64ea30758db3f42bb9").LocalizedName.String;
				StauntonName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("0bcf3c125a28d164191e874e3c0c52de").LocalizedName.String;

				
				IrabethName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("d1e567736abf23943b9f041ba7a0bc23").LocalizedName.String;


				AstyName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("d8e2475977bd87b439c4bba8f5f55949").LocalizedName.String;
				TranName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("c35607fd0f8064f42b4d71f7eb50e96c").LocalizedName.String;
				VelhmName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("f9c01a9515cd1f347800685ddbfbcc41").LocalizedName.String;

				//lann cb29621d99b902e4da6f5d232352fbda
				//ember 2779754eecffd044fbd4842dba55312c
				//penta 942862a48f1644ae85ac5e3c9deb720c
				//seelah 54be53f0b35bf3c4592a97ae335fe765
				//daeran 096fc4a96d675bb45a0396bcaa7aa993
				//regill 0d37024170b172346b3769df92a971f5
				//sendri 561036c882a640089b1d42f03ebe3a6c
				//ulbrig 42f0d5ec3dc844feb44b04507a7c1bfc
				//trever 0bb1c03b9f7bbcf42bb74478af2c6258
				//wendu  ae766624c03058440a036de90a7f2009
				//rekarth 3e0014e4be454482a2797fd81123d7b4
				//greybor f72bb7c48bb3e45458f866045448fb58
				//camelia 397b090721c41044ea3220445300e1b8
				//delamere 6b1f599497f5cfa42853d095bda6dafd
				//kestoglyr e551850403d61eb48bb2de010d12c894
				//sosiel 1cbbbb892f93c3d439f8417ad7cbb6aa
				LannName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("cb29621d99b902e4da6f5d232352fbda").LocalizedName.String;
				EmberName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("2779754eecffd044fbd4842dba55312c").LocalizedName.String;
				PentaName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("942862a48f1644ae85ac5e3c9deb720c").LocalizedName.String;
				SeelahName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("54be53f0b35bf3c4592a97ae335fe765").LocalizedName.String;
				DaeranName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("096fc4a96d675bb45a0396bcaa7aa993").LocalizedName.String;
				RegillName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("0d37024170b172346b3769df92a971f5").LocalizedName.String;
				SendriName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("561036c882a640089b1d42f03ebe3a6c").LocalizedName.String;
				UlbrigName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("42f0d5ec3dc844feb44b04507a7c1bfc").LocalizedName.String;
				TreverName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("0bb1c03b9f7bbcf42bb74478af2c6258").LocalizedName.String;
				WenduagName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("ae766624c03058440a036de90a7f2009").LocalizedName.String;
				RekarthName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("3e0014e4be454482a2797fd81123d7b4").LocalizedName.String;
				GreyborName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("f72bb7c48bb3e45458f866045448fb58").LocalizedName.String;
				CamelliaName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("397b090721c41044ea3220445300e1b8").LocalizedName.String;
				DelamereName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("6b1f599497f5cfa42853d095bda6dafd").LocalizedName.String;
				KestoglyrName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("e551850403d61eb48bb2de010d12c894").LocalizedName.String;
				SosielName = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("1cbbbb892f93c3d439f8417ad7cbb6aa").LocalizedName.String;

				companions = new List<string>{
ArueName,
CiarName,
NenioName,
GalfreyName,
StauntonName,
WoljifName,
				LannName,
				EmberName,
				PentaName,
				SeelahName,
				DaeranName,
				RegillName,
				SendriName,
				UlbrigName,
				TreverName,
				WenduagName,
				RekarthName,
				GreyborName,
				CamelliaName,
				DelamereName,
				KestoglyrName,
				SosielName			};



				Main.SafeLoad(new Action(popUnitNames), "popUnitNames");

				// asty d8e2475977bd87b439c4bba8f5f55949
				// tran c35607fd0f8064f42b4d71f7eb50e96c
				// velhm f9c01a9515cd1f347800685ddbfbcc41




				//Dialog.BubbleMasterBlueprint = Dialog.Get<BlueprintUnit>("2d6fe3abc0ed7364ba48e604ae2471bd");

				//Dialog.CreateBubbleDialog();
				//Dialog.CreateBubbleMasterBlueprint();

				Main.settings = UnityModManager.ModSettings.Load<Settings>(ModEntry);

				if (!Main.settings.isCleaned)
				{
					Directory.CreateDirectory(Path.GetFullPath(CustomPortraitsManager.PortraitsRootFolderPath));
					foreach (string sFile in System.IO.Directory.GetFiles(Main.GetCompanionPortraitsDirectory(), GetDefaultPortraitsDirName() + ".current", SearchOption.AllDirectories))
					{
						System.IO.File.Delete(sFile);
					}
					foreach (string sFile in System.IO.Directory.GetFiles(Main.GetNpcPortraitsDirectory(), GetDefaultPortraitsDirName() + ".current", SearchOption.AllDirectories))
					{
						System.IO.File.Delete(sFile);
					}
					foreach (string sFile in System.IO.Directory.GetFiles(Main.GetArmyPortraitsDirectory(), GetDefaultPortraitsDirName() + ".current", SearchOption.AllDirectories))
					{
						System.IO.File.Delete(sFile);
					}
					Main.settings.isCleaned = true;
					Main.settings.Save(ModEntry);

				}


				if (!Main.settings.isCleaned2)
				{
					Directory.CreateDirectory(Path.GetFullPath(CustomPortraitsManager.PortraitsRootFolderPath));
					foreach (string sDir in System.IO.Directory.GetDirectories(Main.GetCompanionPortraitsDirectory(), "Game Default Portraits", SearchOption.AllDirectories))
					{
						System.IO.Directory.Move(sDir, Path.Combine(Directory.GetParent(sDir).FullName, Main.GetDefaultPortraitsDirName()));
					}

					foreach (string sDir in System.IO.Directory.GetDirectories(Main.GetNpcPortraitsDirectory(), "Game Default Portraits", SearchOption.AllDirectories))
					{
						System.IO.Directory.Move(sDir, Path.Combine(Directory.GetParent(sDir).FullName, Main.GetDefaultPortraitsDirName()));
					}

					foreach (string sDir in System.IO.Directory.GetDirectories(Main.GetArmyPortraitsDirectory(), "Game Default Portraits", SearchOption.AllDirectories))
					{
						System.IO.Directory.Move(sDir, Path.Combine(Directory.GetParent(sDir).FullName, Main.GetDefaultPortraitsDirName()));
					}

					Main.settings.isCleaned2 = true;
					Main.settings.Save(ModEntry);

				}



				/*
				var groetusFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("c3e4d5681906d5246ab8b0637b98cbfe");
				groetusFeature.ComponentsArray = groetusFeature.ComponentsArray
					.Where(c => !(c is PrerequisiteFeature))
					.ToArray();*/

				//var portrait = ResourcesLibrary.TryGetBlueprint<BlueprintPortrait>("a6e4ff25a8da46a44a24ecc5da296073");
				var buff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("a13e2e71485901045b1722824019d6f5");

				var poly = buff.ComponentsArray.OfType<Polymorph>().FirstOrDefault();

				//var portrait = buff.ComponentsArray.OfType<Polymorph>().FirstOrDefault()?.Portrait.Data;


				//		foreach(var component in buff.ComponentsArray)
				//      {
				//			Main.DebugLog(component.name);
				//			Main.DebugLog(component.GetType().ToString());
				//		}

				if (poly is null) return;


				//var bp = buff.ComponentsArray.OfType<Polymorph>().FirstOrDefault()?.Portrait;

				//Main.DebugLog(bp.AssetGuid.ToString());


				string prefix = Main.GetCompanionPortraitDirPrefix();
				string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
				string portraitDirectoryName = prefix + Main.NenioName;

				//Main.DebugLog("1 - " + Path.Combine(portraitsDirectoryPath, portraitDirectoryName));
				string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

				if (Directory.Exists(portraitDirectoryPath))
				{
					if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
					{
						string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

						string dir = Path.GetFileNameWithoutExtension(dirs[0]);
						//Main.DebugLog(dir);
						if (!dir.Equals("root"))
							portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName, dir);
						//Main.DebugLog(portraitDirectoryPath);
					}
					else
					{
						if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
							if (File.Exists(Path.Combine(portraitDirectoryPath, GetDefaultPortraitsDirName(), "Medium.png")))
							{

								portraitDirectoryPath = Path.Combine(portraitDirectoryPath, GetDefaultPortraitsDirName());


							}
					}

					if (File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
					{
						//Main.DebugLog("Poly: " + portraitDirectoryPath);
						PortraitData Data = new PortraitData(Path.Combine(portraitDirectoryPath));
						BlueprintPortrait bp2 = poly.m_Portrait;

						bp2.Data = Data;
						poly.m_Portrait = bp2.ToReference<BlueprintPortraitReference>();

						//buff.ComponentsArray.Remove

						//poly.m_Portrait = bp2.ToReference<BlueprintPortraitReference>();

					}
				}


				if (Main.settings.SwarmRiddance == true)
				{

					GetRidOfSwarm();
					CountSwarm();
				}
			}
			public static bool loaded = false;

			public static void popUnitNames()
			{
				/*
				string str = BundlesLoadService.BundlesPath("cheatdata.json");
				BlueprintList blueprintList = new BlueprintList()
				{
					Entries = new List<BlueprintList.Entry>()
				};

				if (File.Exists(str))
					blueprintList = JsonUtility.FromJson<BlueprintList>(File.ReadAllText(str));

				*/
				var list = Utilities.GetAllBlueprints();

				//Main.DebugLog("popUnitNames()");
				int i = 0;
				int j = 0;
				//foreach (BlueprintList.Entry be in blueprintList.Entries)
				foreach (BlueprintList.Entry be in list.Entries)
				{
					try
					{

						//if (be.TypeFullName.Equals("Kingmaker.DialogSystem.Blueprints.BlueprintCue"))
						if (be.Type.Name.Equals("BlueprintCue"))
						{
							
							if (be != null && be.Guid != null)
							{
						//		Main.DebugLog(be.Guid);

								BlueprintCue bc = Utilities.GetBlueprintByGuid<BlueprintCue>(be.Guid);

								if (bc != null && bc.Speaker != null && bc.Speaker.Blueprint != null && !bc.Speaker.Blueprint.name.IsNullOrEmpty() && !bc.Speaker.Blueprint.CharacterName.IsNullOrEmpty() && !unitNames.Contains(bc.Speaker.Blueprint.name) && !companions.Contains(bc.Speaker.Blueprint.CharacterName))
								{
							//		Main.DebugLog("1");

									if (!bc.Speaker.Blueprint.IsCompanion /* && !fyou.Contains(bc.Speaker.Blueprint.name)*/)
									{
							//			Main.DebugLog("2");
										if (bc.Speaker.Blueprint.name != bc.Speaker.Blueprint?.CharacterName)
										{
							
											//Main.DebugLog("3");
											unitNames.Add(bc.Speaker.Blueprint.name);
											i++;
										}
									}
								}
						//		Main.DebugLog("4");
								if (bc != null && bc.Listener != null && !bc.Listener.name.IsNullOrEmpty() && !bc.Listener.CharacterName.IsNullOrEmpty() && !unitNames.Contains(bc.Listener.name) && !companions.Contains(bc.Listener.CharacterName))
								{
							//		Main.DebugLog("5");
									if (!bc.Listener.IsCompanion)
									{
							//			Main.DebugLog("6");
										if (bc.Listener.name != bc.Listener?.CharacterName)
										{
							//				Main.DebugLog("7");
											unitNames.Add(bc.Listener.name);
											i++;

										}
									}
								}
							}
						}
					}
					catch (Exception e)
					{
						//Main.DebugError(e);
						j++;
					}
				}
				Main.DebugLog("Found " + i.ToString() + " units partaking in dialogs. (" + j.ToString() + " are null?)");

			}

		}



		private static void OnShowGUI(UnityModManager.ModEntry modEntry)
		{
			//Main.DebugLog("-2");

			CountSwarm();

			if (Game.Instance.CurrentMode == GameModeType.Dialog)
			{
				//Main.DebugLog("-1");

				string charcterNameDirectoryName = Game.Instance.DialogController.CurrentSpeakerName;
				try
				{
					//Main.DebugLog("0");

					//	if (!companions.Contains(charcterNameDirectoryName))
					if (RealCurrentSpeakerEntity(charcterNameDirectoryName) == null && !Main.companions.Contains(RealCurrentSpeakerEntity(charcterNameDirectoryName)?.CharacterName))
					{
						//Main.DebugLog("1");

						if (charcterNameDirectoryName.Equals(Main.AstyName) ||
							charcterNameDirectoryName.Equals(Main.VelhmName) || 
							charcterNameDirectoryName.Equals(Main.TranName) 
							)
						{
							if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
								charcterNameDirectoryName = charcterNameDirectoryName + " - Drow";

						}

						//Irabeth scar
						if (charcterNameDirectoryName.Equals(Main.IrabethName))
							if (Game.Instance.Player.EtudesSystem.EtudeIsStarted(ResourcesLibrary.TryGetBlueprint<BlueprintEtude>("b4f08736cf124ae4996fcef7c0a33bf1")))
							{

								Directory.CreateDirectory(Path.Combine(Main.GetCompanionPortraitsDirectory(), charcterNameDirectoryName + " - Scar"));
								charcterNameDirectoryName = charcterNameDirectoryName + " - Scar";
							}

						string NpcPortraitPath = Path.Combine(GetNpcPortraitsDirectory(), charcterNameDirectoryName);


						//Main.DebugLog("2");

						if (Directory.Exists(NpcPortraitPath))
						{
							if (Directory.GetFiles(Path.Combine(NpcPortraitPath), "*.current").Length != 0)
							{
								string[] dirs = Directory.GetFiles(Path.Combine(NpcPortraitPath), "*.current");

								if (!Path.GetFileNameWithoutExtension(dirs[0]).Equals("root"))
									npcDirlabel = Path.GetFileNameWithoutExtension(dirs[0]);
								else
									npcDirlabel = " - (Npc root folder)";
								//npcDirlabel = Path.GetFileNameWithoutExtension(dirs[0]);
							}
							else
								npcDirlabel = "";
						}

						//Main.DebugLog("3");
						if (Directory.Exists(Path.Combine(NpcPortraitPath, Game.Instance.DialogController.CurrentSpeakerBlueprint.name)))
						{
							if (Directory.GetFiles(Path.Combine(NpcPortraitPath, Game.Instance.DialogController.CurrentSpeakerBlueprint.name), "*.current").Length != 0)
							{
								//	Main.DebugLog("2");
								string[] dirs = Directory.GetFiles(Path.Combine(NpcPortraitPath, Game.Instance.DialogController.CurrentSpeakerBlueprint.name), "*.current");

								if (!Path.GetFileNameWithoutExtension(dirs[0]).Equals("root"))
									npcSubDirlabel = Path.GetFileNameWithoutExtension(dirs[0]);
								else
									npcSubDirlabel = " - (Npc sub root folder)";
								//npcSubDirlabel = Path.GetFileNameWithoutExtension(dirs[0]);
								//Main.DebugLog(npcSubDirlabel);
							}
							else
							{
								npcSubDirlabel = "";
								//Main.DebugLog("2");
							}
						}



					}

				}
				catch (Exception e)
				{

					Main.DebugError(e);
				}
			}


			if (RootUIContext.Instance.CurrentServiceWindow.ToString().Equals("CharacterInfo") ||
				RootUIContext.Instance.CurrentServiceWindow.ToString().Equals("Inventory"))
			{
				try
				{
					UnitEntityData unitEntityData = Game.Instance.SelectionCharacter.CurrentSelectedCharacter;

					string charcterNameDirectoryName = Main.GetCompanionDirName(unitEntityData.CharacterName);

					//Game.Instance.SelectionCharacter.CurrentSelectedCharacter.CharacterName.cleanCharname();



					string compPortraitPath = Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + charcterNameDirectoryName);

					//	Main.DebugLog(compPortraitPath); 

					if (Directory.Exists(compPortraitPath))
					{
						if (Directory.GetFiles(Path.Combine(compPortraitPath), "*.current").Length != 0)
						{
							string[] dirs = Directory.GetFiles(Path.Combine(compPortraitPath), "*.current");

							compDirlabel = Path.GetFileNameWithoutExtension(dirs[0]);
						}
						else
							compDirlabel = "";
					}

				}
				catch (Exception e)
				{

					Main.DebugError(e);
				}

			}
			i = -1;
			//state = State.Start;
			savedNpc = false;
			savedComp = false;
			isDialog = true;
			isLoadedGame = true;
			showExperimental = false;
			//npcDirlabel = "";
			//npcSubDirlabel = "";
			//compDirlabel = "";

			npcCycle = "";
			npcSubCycle = "";
			compCycle = "";

#if DEBUG
			showPatched = false;
#endif


			string name = SceneManager.GetActiveScene().name;
			//Main.DebugLog("SceneManager.GetActiveScene().name: " + name);
			//Main.DebugLog("Game.Instance.State: " + Game.Instance.State);
			Game instance = Game.Instance;
			/*
			Main.DebugLog("Game.Instance.CurrentMode: " + instance.CurrentMode);

			Main.DebugLog("Game.Instance.State: " + Game.Instance.State);*/






			if ((instance?.Player) == null || name == "UI_MainMenu_Scene" || name == "Start")
			{
				isLoadedGame = false;
				isDialog = false;
			}
			else if (instance.CurrentMode != GameModeType.Dialog)
			{
				isLoadedGame = true;

				isDialog = false;
			}

			boldStyle.fontStyle = UnityEngine.FontStyle.Bold;
			//Color c = new Color((float)131.0, (float)192.0, (float)239.0);
			boldStyle.normal.textColor = UnityEngine.Color.cyan;

			boldStyle2.fontStyle = UnityEngine.FontStyle.Bold;
			//Color c = new Color((float)131.0, (float)192.0, (float)239.0);
			boldStyle2.normal.textColor = UnityEngine.Color.red;

		}



		private static GUIStyle boldStyle = new GUIStyle();
		private static GUIStyle boldStyle2 = new GUIStyle();

		private static GUILayoutOption[] defSize = new GUILayoutOption[]
{
				GUILayout.Width(200f),
				GUILayout.Height(20f)
};
        static List<string> allCompNames = new List<string>();

        private static void OnGUI(UnityModManager.ModEntry modEntry)
	{
		//bool globalMap = false;
		//	if (Game.Instance.CurrentMode == GameModeType.GlobalMap)
		//&& (SceneManager.GetActiveScene().name.Equals("WorldWoundGM_Light")))
		//	globalMap = true;
		
			/*
		switch (state)
		{
			case State.Start:
				//DebugLog("run once");


				state = State.Update;
				break;

			case State.Update:
				//                    DebugLog("run every frame");


				break;
		}

			*/




#if DEBUG
			//		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			//		GUILayout.Label("index2: ");
			//	index2 = int.Parse(GUILayout.TextField(Main.index2.ToString(), 25));
			//		GUILayout.EndHorizontal();
#endif

			if (Main.settings.ManageCompanions)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Open Party Portraits Directory", GUILayout.Width(200f), GUILayout.Height(20f)))
				{



					/*
					ItemsCollection inventory = Game.Instance.Player.Inventory;

					foreach (BlueprintItemEquipmentShirt scriptableObject in Utilities.GetScriptableObjects<BlueprintItemEquipmentShirt>())
					{
						//string blueprintPath = Utilities.GetBlueprintPath(scriptableObject);
						//"HosillaKey"
						if (scriptableObject != null && scriptableObject.Name != null && scriptableObject.Name.Length > 0 )//&& scriptableObject.Name.ToLower().Contains("angelbloom"))
						//if (blueprintPath.ToLower().Contains("angelbloom"))
						{

							//ItemEntitySimple key = new ItemEntitySimple(scriptableObject);

							inventory.Add(scriptableObject);
						}
						//Main.DebugLog("náme:" + scriptableObject.Name);
						//Main.DebugLog("path:"+ Utilities.GetBlueprintPath(scriptableObject));
					}
					*/
					//CheatsTransfer
					/*
					BlueprintItem scriptableObject = Utilities.GetBlueprintByName<BlueprintItem>("AngelFlowerCrownItem");


					*/

#if DEBUG


					//	Game.Instance.ReloadUI();

					//refreshPortraits();
					//object campaign = Game.Instance.Player.Campaign;

					//BlueprintCampaign blueprintCampaign = (BlueprintCampaign)Game.Instance.Player.Campaign;

					//string str = blueprintCampaign.name;


					//UIUtility.SendWarning(Game.Instance.Player.Campaign.name);


					//					UIUtility.SendWarning($"{string.Join("", BlueprintRoot.Instance.DlcSettings.Dlcs.Select(c => c?.name=="Inevitable Excess" + ": " + c?.Description + Environment.NewLine))}");

					//Game.Instance.Player.PartyCharacters

					//MoveUp<UnitEntityData>(Game.Instance.Player.RemoteCompanions, 5);


					//typeof(Player).GetField("RemoteCompanions", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Game.Instance.Player, Game.Instance.Player.RemoteCompanions.OrderBy(e => e.CharacterName));
					/*
					var parties = new Dictionary<string, IEnumerable<UnitEntityData>>
				{
					{ "group capital", UIUtility.GetGroup(Game.Instance.LoadedAreaState.Settings.CapitalPartyMode.Value) },
					{ "group capital and pets", UIUtility.GetGroup(Game.Instance.LoadedAreaState.Settings.CapitalPartyMode.Value, true) },
					{ "party", Game.Instance.Player.Party },
					{ "party and pets", Game.Instance.Player.PartyAndPets },
					{ "party and pets detached", Game.Instance.Player.PartyAndPetsDetached },
					{ "party characters", Game.Instance.Player.PartyCharacters.Select(u => u.Value).ToList() },
					{ "remote companions", Game.Instance.Player.RemoteCompanions },
					{ "active companions", Game.Instance.Player.ActiveCompanions },
					{ "all characters", Game.Instance.Player.AllCharacters },
					{ "CrossSceneState.AllEntityData", Game.Instance.Player.CrossSceneState.AllEntityData as IEnumerable<UnitEntityData>}

				};

					foreach (var p in parties)
						UIUtility.SendWarning($"{p.Key}: {string.Join(",", p.Value?.Select(c => c?.CharacterName))}");
					*/
					//UIUtility.SendWarning(Game.Instance.Player.MainCharacter.Value.Progression.GetCurrentMythicClass().CharacterClass.Name);

					/*
					if (k > 1122)
						k = 0;
					else
						k++;

					UIUtility.SendWarning(k.ToString());



					foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
					{

						if (unitEntityDatum.CharacterName == "Arueshalae")
						{

							//var LightHaloBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0b1c9d2964b042e4aadf1616f653eb95");
							//MechanicsContext mechanicsContext = new MechanicsContext(unitEntityDatum, unitEntityDatum, LightHaloBuff, null, null);
							//unitEntityDatum.Facts.Add<Buff>(new Buff(LightHaloBuff, mechanicsContext, null));

							FxHelper.Destroy(ifxh, true);


							bool? nullable = new bool?(true);
							Vector3 vector3 = new Vector3();
							ifxh = FxHelper.SpawnFxOnUnit(bpbuffs[k].FxOnStart.Load(false, false), unitEntityDatum.View, nullable, null, vector3, FxPriority.EventuallyImportant);




							UIUtility.SendWarning(bpbuffs[k].Name +" - "+  bpbuffs[k].AssetGuid);

							Main.DebugLog(bpbuffs[k].Name + " - " + bpbuffs[k].AssetGuid);

							//var NulkiVisBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("fc246eb29c8f95c449fd156759f421e9");
						//		if (bpbuffs[k] != null)
						//		{

						//			lastbb = bpbuffs[k];
						//			MechanicsContext mechanicsContext = new MechanicsContext(unitEntityDatum, unitEntityDatum, bpbuffs[k], null, null);
						//			unitEntityDatum.Facts.Add<Buff>(new Buff(bpbuffs[k], mechanicsContext, null));
						//		}
						//		


							//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");

							//give
							//unitEntityDatum.Progression.Features.AddFeature(LightHaloFeature, null);

							//take
							//


							//UnitHelper.AddBuff(unitEntityDatum, LightHaloBuff, unitEntityDatum);
							//unitEntityDatum.GetFeature(LightHaloFeature).Activate();
							//UIUtility.SendWarning(LightHaloBuff.Name);


							break;
						}

					}
						*/

					/*
					foreach (EntityDataBase allEntityDatum in Game.Instance.Player.CrossSceneState.AllEntityData)
					{
						UnitEntityData unitEntityDatum = allEntityDatum as UnitEntityData;
						if (unitEntityDatum == null)
						{
							continue;
						}

						if (unitEntityDatum.CharacterName == "Arueshalae")
						{

							var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");

							UIUtility.SendWarning(LightHaloFeature.Name);


							unitEntityDatum.GetFeature(LightHaloFeature).Activate();


							break;
						}

						//UIUtility.SendWarning($"CrossSceneState.AllEntityData: {string.Join(", ", unitEntityDatum.CharacterName)}");

					}*/

					//Main.DebugLog(Game.Instance.CurrentMode.Name);

					//Main.DebugLog(RootUIContext.Instance.CurrentServiceWindow.ToString());


					//areaLoaded = true;
					//	loctel();
					//BlueprintsCache_Patch.popUnitNames();
					//CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
					//EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);

					/*
					foreach (PartyCharacterVM pcvm in RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.FullCharactersVM)
					{
						if (pcvm.CharacterName.Equals("Arenor"))
						{

							BlueprintPortrait bp = Utilities.GetBlueprintByGuid<BlueprintPortrait>("1f5bfefa49a8aa2449ad94b1f4f61788");


							bp.Data.m_PetEyeImage = Utilities.GetBlueprintByGuid<BlueprintPortrait>("1f5bfefa49a8aa2449ad94b1f4f61788").Data.m_PetEyeImage;

							typeof(PartyCharacterPetVM).GetField("PetEyeImage", BindingFlags.Instance | BindingFlags.Public).SetValue(pcvm.FirstPinPetVm.Value, bp.Data.PetEyePortrait);

							typeof(PartyCharacterPetVM).GetField("PetEyeImage", BindingFlags.Instance | BindingFlags.Public).SetValue(pcvm.SecondPinPetVm.Value, bp.Data.PetEyePortrait);


							EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(pcvm.UnitEntityData), true);
							EventBus.RaiseEvent<IUnitActionBarUpdateHandler>((IUnitActionBarUpdateHandler h) => h.HandleActionBarUpdated(), true);


							eye = bp.Data.SmallPortrait;
							*/
					//UnitEntityData unitEntityData = null;
					/*foreach (UnitEntityData ud in Game.Instance.State.PlayerState.AllCharacters)
					{


						if (ud.CharacterName.Equals("Nenio"))
						{

							foreach (UnitEntityData ud2 in Game.Instance.State.PlayerState.AllCharacters)
							{

								if (ud.CharacterName.Equals("Ember"))
								{
									Game.Instance.SelectionCharacter.SwitchCharacter(ud, ud2);
									break;
								}
							}

							break;
						}
					}
					*/
					//Main.DebugLog(ud.GetActivePolymorph().Runtime.SourceBlueprintComponentName);

					/*	foreach (EntityFact list in ud.Facts.List)
						{
							UnitFact unitFact = list as UnitFact;
							if (unitFact == null)
							{
								continue;
							}
							Main.DebugLog(list.Name);
							//Main.DebugLog(list.IsEnabled.ToString());

							foreach (EntityFactComponent component in unitFact.Components)
							{
								Main.DebugLog("   " + component.SourceBlueprintComponentName);

							}
						}*/
					//PortraitLoader.LoadInternal("Portraits", "BlackPetEye.png", new Vector2Int(176, 24), TextureFormat.RGBA32);

					//pcvm.HandleAddPet(pcvm.UnitEntityData, unitEntityData);
					//EyePortraitInjecotr.Replacements[ud.UISettings.Portrait] = Utilities.GetBlueprintByGuid<BlueprintPortrait>("1f5bfefa49a8aa2449ad94b1f4f61788").Data.PetEyePortrait;



					//bp.Data.m_PetEyeImage = Utilities.GetBlueprintByGuid<BlueprintPortrait>("1f5bfefa49a8aa2449ad94b1f4f61788").Data.m_PetEyeImage;


					//EyePortraitInjecotr.Replacements[ud.UISettings.Portrait] = Utilities.GetBlueprintByGuid<BlueprintPortrait>("1f5bfefa49a8aa2449ad94b1f4f61788").Data.PetEyePortrait;

					//pcvm.UpdateStates();
					//break;



					//	}
					//	}

					//	RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.UpDateStates();

					//BlueprintCue b = Game.Instance.DialogController.CurrentCue;
					//Main.DebugLog(Game.Instance.CurrentMode.Name);

					//Main.DebugLog(Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait.ToReference<BlueprintUnitReference>().ToString());

					//Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait.ToReference<BlueprintUnitReference>()


					//Main.DebugLog(Game.Instance.DialogController.CurrentSpeakerBlueprint.ToReference<BlueprintUnitReference>().ToString());

					//Main.DebugLog(typeof(BlueprintCue).GetField("m_SpeakerPortrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Game.Instance.DialogController.CurrentCue.Speaker).ToString());


					//Main.DebugLog(Game.Instance.DialogController.CurrentCue.AssetGuid.ToString());

					//Main.DebugLog("ongui " + Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait.ToReference<BlueprintUnitReference>().ToString());

					//45450b2f327797e41bce701b91118cb4
#else
				Process.Start(GetCompanionPortraitsDirectory());
#endif
				}
				GUILayout.Label(GetCompanionPortraitsDirectory());
				GUILayout.EndHorizontal();
			}

		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Open NPC portraits dir", GUILayout.Width(200f), GUILayout.Height(20f)))
		{
#if DEBUG
				/*
			if (k < 0)
				k = 1122;
			else
				k--;




			UIUtility.SendWarning(k.ToString());
				*/

				/*
				foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
				{

					if (unitEntityDatum.CharacterName == "Arueshalae")
					{

						//var LightHaloBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("0b1c9d2964b042e4aadf1616f653eb95");
						//MechanicsContext mechanicsContext = new MechanicsContext(unitEntityDatum, unitEntityDatum, LightHaloBuff, null, null);
						//unitEntityDatum.Facts.Add<Buff>(new Buff(LightHaloBuff, mechanicsContext, null));

						FxHelper.Destroy(ifxh, true);


						bool? nullable = new bool?(true);
						Vector3 vector3 = new Vector3();
						ifxh = FxHelper.SpawnFxOnUnit(bpbuffs[k].FxOnStart.Load(false, false), unitEntityDatum.View, nullable, null, vector3, FxPriority.EventuallyImportant);




						UIUtility.SendWarning(bpbuffs[k].Name);
				*/
				//var NulkiVisBuff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("fc246eb29c8f95c449fd156759f421e9");
				/*	if (bpbuffs[k] != null)
					{

						lastbb = bpbuffs[k];
						MechanicsContext mechanicsContext = new MechanicsContext(unitEntityDatum, unitEntityDatum, bpbuffs[k], null, null);
						unitEntityDatum.Facts.Add<Buff>(new Buff(bpbuffs[k], mechanicsContext, null));
					}
					*/
				/*

						//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");

						//give
						//unitEntityDatum.Progression.Features.AddFeature(LightHaloFeature, null);

						//take
						//


						//UnitHelper.AddBuff(unitEntityDatum, LightHaloBuff, unitEntityDatum);
						//unitEntityDatum.GetFeature(LightHaloFeature).Activate();
						//UIUtility.SendWarning(LightHaloBuff.Name);


						break;
					}

				}
		*/


				
	

#else
				Process.Start(GetNpcPortraitsDirectory());
#endif
			}
			GUILayout.Label(GetNpcPortraitsDirectory());
		GUILayout.EndHorizontal();



		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("Open Army portraits dir", GUILayout.Width(200f), GUILayout.Height(20f)))
		{


#if DEBUG
			if (k > 1122)
				k = 0;
			else
				k = k + 50;

			UIUtility.SendWarning(k.ToString());
#else
				Process.Start(GetArmyPortraitsDirectory());
#endif
		}
		GUILayout.Label(GetArmyPortraitsDirectory());
		GUILayout.EndHorizontal();

#if DEBUG


		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("dialog", GUILayout.Width(200f), GUILayout.Height(20f)))
		{
			try
			{
				//Game.Instance.EntityCreator.SpawnUnit(ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("7001e2a58c9e86e43b679eda8a59f12f"), Game.Instance.Player.MainCharacter.Value.Position, Quaternion.identity, Game.Instance.CurrentScene.MainState);

				//	Dialog.BubbleMasterBlueprint = Dialog.Get<BlueprintUnit>("f47c2253fd7645d45a28cdee6288dc5d");

				//	Dialog.CreateBubbleDialog();
				//	Dialog.CreateBubbleMasterBlueprint();


				//Main.DebugLog(Guid.NewGuid().ToString());

				//Main.DebugLog(new BlueprintGuid(Guid.NewGuid()).ToString());




				/*
				for(int i=0; i < 100; i++)
				{
					Main.DebugLog("// "+ Guid.NewGuid().ToString().ToUpper());
				}
				*/


				//foreach (var unit in Game.Instance.State.Units)
				//{
				//if (unit.Blueprint.name.ToLower().Contains("talks"))
				//{







				//unit.View.AnimationManager.Execute(unit.View.AnimationManager.CreateHandle(UnitAnimationType.Prone, true));
				//unit.View.AnimationManager.IsSleepingInCamp = true;

				//BlueprintBuff sleepingBuff = BlueprintRoot.Instance.SystemMechanics.;

				//unit.AddFact(sleepingBuff);

				//	unit.View.AnimationManager.IsSleeping = true;
				//							partyAndPet.View.AnimationManager.IsSleepingInCamp = true;

				//	unit.View.AnimationManager.FastForwardProneAnimation(false);

				//	unit.View.AnimationManager.AnimationSet.Actions.ForEach(x => x. );

				//	unit.View.AnimationManager.AnimationSet.)));

				/*var unitRef = unit.Blueprint.ToReference<BlueprintUnitReference>();
				var w = (Wrapper)unit.Parts.Get<UnitPartInteractions>().m_Interactions[0];

				var d = (SpawnerInteractionDialog)w.Source;


				d.m_Dialog = SimpleDialogBuilder.CreateDialog("simpledialog.succ.base2", "70CCF083-9C37-4E14-BBF2-E66FFB0C816D", new List<BlueprintCueBaseReference>()
					   {
		   SimpleDialogBuilder.CreateCue("simpledialog.succ.greet2", "88C55921-524A-427D-8451-39B2FC8C2E7D", unitRef, "2 Hello there! This is a test! How do you feel today?", new List<BlueprintAnswerBaseReference>()
		   {
			   SimpleDialogBuilder.CreateAnswerList("simpledialog.succ.greet.answerlist2", "84671A44-7CBF-4680-A0A4-604EB9387FC7", new List<BlueprintAnswerBaseReference>()
			   {
				   SimpleDialogBuilder.CreateAnswer("simpledialog.succ.greet.answer.good2", "2ABB5DF5-72E0-44B7-9208-6E9E5B0D9D9D", "Hi I am feeling great!", new List<BlueprintCueBaseReference>()
				   {
					   SimpleDialogBuilder.CreateCue("simpledialog.succ.exit.good2", "73BDD0CD-1CC2-4E95-8E78-7C89357BB2C7", unitRef, "I am glad to hear that!", new List<BlueprintAnswerBaseReference>())
				   }),
				   SimpleDialogBuilder.CreateAnswer("simpledialog.succ.greet.answer.bad2", "93D45D94-485A-402F-947D-ABC3D3DDE761", "I feel like crap!", new List<BlueprintCueBaseReference>()
				   {
					   SimpleDialogBuilder.CreateCue("simpledialog.succ.exit.bad2", "1A1F3585-723E-4B10-80DF-8D58F1AAD1E9", unitRef, "You'd feel better if you were sitting in the water", new List<BlueprintAnswerBaseReference>())
				   })
			   })
		   })
	   });*/


				//	}

				//	}


			}
			catch (Exception e)
			{

				Main.DebugError(e);
				Main.DebugLog(StackTraceUtility.ExtractStackTrace());
			}
		}
		GUILayout.EndHorizontal();



		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("add custom item", GUILayout.Width(200f), GUILayout.Height(20f)))
		{

			//string absoluteFolder = Path.Combine(UnityModManager.modsPath, modEntry.Info.Id, "Items");
			//Sprite sprite = AssetLoader.LoadInternal(absoluteFolder, "HaloItem.png");

			//GUILayout.Label(sprite.texture);

			BlueprintItemEquipmentHead sourceBp = Utilities.GetBlueprintByGuid<BlueprintItemEquipmentHead>("51500154900d46ac9bd4d7ef758b3808");

			BlueprintItemEquipmentHead bp = new BlueprintItemEquipmentHead()
			{
				AssetGuid = new BlueprintGuid(new Guid("23d1cef87afd45b795b1a86398eeb10a"))
			};
			//	bp.m_DisplayNameText = DescriptionTools.CreateString("ITEMNAME", "Angel's Love");
			//bp.m_DescriptionText = Helpers.CreateString("ITEM_DESC", "Grants its wielder a +2 enhancement {g|Encyclopedia:Bonus}bonus{/g} to {g|Encyclopedia:Dexterity}Dexterity{/g}. Bonuses of the same type usually don't stack.");
			//	bp.m_DescriptionText = DescriptionTools.CreateString("ITEM_DESC", "If an angel falls in love, they can bestow the essence of their halo to someone they are in love with. ");
			bp.m_Icon = sourceBp.m_Icon;
			bp.m_EquipmentEntity = sourceBp.m_EquipmentEntity;
			bp.m_IsNotable = true;
			bp.m_Cost = 100000;
			bp.m_Weight = 1.0f;
			bp.m_InventoryPutSound = "CommonPut";
			bp.m_InventoryTakeSound = "CommonTake";
			bp.m_InventoryEquipSound = "CommonPut";
			bp.CR = 13;
			bp.m_ForcedRampColorPresetIndex = 9;

			//If DescriptionTools.
			//bp.m_Destructible = true;
			//bp.m_ShardItem = "!bp_e6820e62423d4c81a2ba20d236251b67";

			//non enchanted ones:
			//bp.m_Enchantments = new BlueprintEquipmentEnchantmentReference[0];


			BlueprintFeature blueprintFeature = new BlueprintFeature()
			{
				AssetGuid = new BlueprintGuid(new Guid("8fb2ee1d545f46f2be5b33bdcdd5f138"))
				//AssetGuid = new BlueprintGuid(Guid.NewGuid())
			};
			//				ExtentionMethods.AddComponent<AddFacts>(blueprintFeature, (AddFacts c) => c.m_Facts = 
			//					new BlueprintUnitFactReference[] 
			//					// 0b1c9d2964b042e4aadf1616f653eb95 
			//						{ ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("00ca55e6c8dc07142807e40da6e3b13f").ToReference<BlueprintUnitFactReference>() });
			ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(blueprintFeature.AssetGuid, blueprintFeature as SimpleBlueprint);


			BlueprintEquipmentEnchantment blueprintEquipmentEnchantment = new BlueprintEquipmentEnchantment()
			{
				AssetGuid = new BlueprintGuid(new Guid("9ea1373741f04a72a96fc1603d364195"))
			};
			Action<AddUnitFeatureEquipment> mFeature =
				(AddUnitFeatureEquipment c) => c.m_Feature = blueprintFeature.ToReference<BlueprintFeatureReference>();
			//ExtentionMethods.AddComponent<AddUnitFeatureEquipment>(blueprintEquipmentEnchantment, mFeature);
			ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(blueprintEquipmentEnchantment.AssetGuid, blueprintEquipmentEnchantment as SimpleBlueprint);


			bp.m_Enchantments = new BlueprintEquipmentEnchantmentReference[]
				{blueprintEquipmentEnchantment.ToReference<BlueprintEquipmentEnchantmentReference>() };

			ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(bp.AssetGuid, bp as SimpleBlueprint);




			ItemsCollection inventory = Game.Instance.Player.Inventory;
			inventory.Add(bp);

		}
		GUILayout.Label("test button");
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("test obj/comp", GUILayout.Width(200f), GUILayout.Height(20f)))
		{
			//Game.Instance.RootUiContext.m_UIView.GetComponent<InGamePCView>();

			var fxRoot = BlueprintRoot.Instance.FxRoot;

			var nrgHalo = FxHelper.FxRoot.transform.Find("Precast_Divine00(Clone)").gameObject;

			nrgHalo.transform.Find("Root_PalmLeft").gameObject.SetActive(false);




			var haloObject = nrgHalo.transform.Find("Head_Back1").gameObject;
			haloObject.GetComponent<SnapToLocator>().Locator.CameraOffset = 0;

			var lockAxis = haloObject.transform.Find("LockAxis").gameObject;
			// Billboard is a component, so we deactivate it with the 'enabled' property
			lockAxis.GetComponent<Billboard>().enabled = false;
			lockAxis.transform.localRotation = Quaternion.Euler(347, 180, 0);
			lockAxis.transform.localPosition = new Vector3(0, 0.04f, -0.15f);
			lockAxis.transform.localScale = new Vector3(1, 0.8f, -0.8f);

			var radiance = nrgHalo.transform.Find("Head_Front").gameObject;

			var raysLoop = radiance.transform.Find("Rays_Loop").gameObject;

			raysLoop.transform.localPosition = new Vector3(0, 0.08f, -0.55f);


			if (nrgHalo.transform.Find("Ground") != null)
				if (nrgHalo.transform.Find("Ground").gameObject != null)
					nrgHalo.transform.Find("Ground").gameObject.SetActive(false);

			//UIUtility.SendWarning(nrgHalo.name);

		}
		GUILayout.Label("test button 2");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("nenio shapetoggle", GUILayout.Width(200f), GUILayout.Height(20f)))
		{





			foreach (UnitEntityData unitEntityData in Game.Instance.Player.AllCharacters)
			{
				//if (unitEntityData.CharacterName == "Nenio")
				//{
				Main.DebugLog(unitEntityData.CharacterName + ": " + unitEntityData.Alignment.ValueRaw.ToString());
				Main.DebugLog(unitEntityData.CharacterName + ": " + unitEntityData.Alignment.ValueVisible.ToString());



				/*
				foreach (var abi in unitEntityDatum.Facts.m_Facts)
				{
				Main.DebugLog(abi.Name +" - "+abi.GetType() +" - "+abi.Blueprint.name +" - "+abi.Blueprint.AssetGuid);
				}*/
				//Change Shape - Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbility - ChangeShapeKitsuneToggleAbility_Nenio - 52bed4c5617625e4faf029b5c750667f

				//Main.DebugLog("add halo 2");
				/*var shapeToggle = ResourcesLibrary.TryGetBlueprint<BlueprintActivatableAbility>("52bed4c5617625e4faf029b5c750667f");

				//give

				if (unitEntityDatum.HasFact(shapeToggle))
				{
					//UIUtility.SendWarning(unitEntityDatum.GetFact(LightHaloToggle).Name);
					if (unitEntityDatum.GetFact<ActivatableAbility>(shapeToggle).IsOn)
					{
						unitEntityDatum.GetFact<ActivatableAbility>(shapeToggle).IsOn = false;
						unitEntityDatum.GetFact<ActivatableAbility>(shapeToggle).IsOn = true;
					}
					else
					{
						unitEntityDatum.GetFact<ActivatableAbility>(shapeToggle).IsOn = true;
						unitEntityDatum.GetFact<ActivatableAbility>(shapeToggle).IsOn = false;

					}
				}
				break;
				*/
				//}

			}
		}
		GUILayout.EndHorizontal();






#endif



		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("In game", boldStyle);
		GUILayout.EndHorizontal();

		if (!isLoadedGame)
		{
			//GUILayout.Label(" ");

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());

			GUILayout.Label("Load a save or start a new game for more options");
			GUILayout.EndHorizontal();
		}
		else
		{

				if (Main.settings.ManageCompanions)
				{
					GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
					if (GUILayout.Button("Refresh Party Portraits", GUILayout.Width(200f), GUILayout.Height(20f)))
					{
						if (Main.settings.ManageCompanions)
							SetPortraits();
					}
					GUILayout.EndHorizontal();
				}

				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());

				if (Main.settings.ManageCompanions)
			{
				if (RootUIContext.Instance.CurrentServiceWindow.ToString().Equals("CharacterInfo") ||
					RootUIContext.Instance.CurrentServiceWindow.ToString().Equals("Inventory"))
				{


					//Game.Instance.SelectionCharacter.CurrentSelectedCharacter.CharacterName;
					UnitEntityData unitEntityData = Game.Instance.SelectionCharacter.CurrentSelectedCharacter;

					string charcterNameDirectoryName = Main.GetCompanionDirName(unitEntityData.CharacterName);



					string compdirpath = Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + charcterNameDirectoryName);

					if (GUILayout.Button("Cycle portraits for " + Game.Instance.SelectionCharacter.CurrentSelectedCharacter.CharacterName/*"Cycle through dirs " */, GUILayout.Width(250f), GUILayout.Height(20f)))
					{
						List<string> strings = Directory.GetDirectories(compdirpath, "*", SearchOption.TopDirectoryOnly).ToList();



						//unitNames.ForEach(item => strings.Remove(x => x.Contains(item)));

						if (File.Exists(Path.Combine(compdirpath, "Medium.png")))
						{
							if (!strings.Exists(x => x.Equals("companion name Medium png placeholder"))) strings.Add("companion name Medium png placeholder");

						}
						if (strings.Count() == 1)
							i = -1;
						else
						if (i == strings.Count() - 1) i = -1;

						i++;

						/*foreach (string s in strings)
						{
							Main.DebugLog(s);
						}*/

						foreach (string sFile in System.IO.Directory.GetFiles(compdirpath, "*.current"))
						{
							System.IO.File.Delete(sFile);
						}

						if (strings[i].Equals("companion name Medium png placeholder"))
						{
							compCycle = "";
							compDirlabel = " - (Character root folder)";
							File.WriteAllText(Path.Combine(compdirpath, "root" + ".current"), "current");

						}
						else
						{
							compCycle = Path.GetFileName(strings[i]);
							compDirlabel = compCycle;
							File.WriteAllText(Path.Combine(compdirpath, compCycle + ".current"), "current");
						}

						SetPortraits();
						//EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityData.Descriptor), true);



						//EventBus.RaiseEvent(delegate (IUnitPortraitChangedHandler h) {
						//	h.HandlePortraitChanged(unitEntityData);
						//});


						/*
						ReactiveProperty<CharInfoComponentVM> ccc;
						Game.Instance.RootUiContext.InGameVM.StaticPartVM.ServiceWindowsVM.CharacterInfoVM.Value.ComponentVMs.TryGetValue(CharInfoComponentType.NameAndPortrait, out ccc);
						//

						ccc.Value.RefreshData();
						*/

						//Game.Instance.RootUiContext.InGameVM.StaticPartVM.PartyVM.UpdateStartValue(0);


						//Game.Instance.SelectionCharacter.d
						if (!unitEntityData.IsMainCharacter)
						{
							Game.Instance.SelectionCharacter.SetSelected(Game.Instance.Player.GetMainPartyUnit());
							Game.Instance.SelectionCharacter.SetSelected(unitEntityData);
						}
						else
						{
							//int k = 0;
							foreach (UnitEntityData ud in Game.Instance.Player.PartyCharacters)
							{
								//k++;
								//Main.DebugLog("foreach: " +ud.CharacterName);
								if (!ud.IsMainCharacter && !ud.IsPet)
								{
									Game.Instance.SelectionCharacter.SetSelected(ud);
									break;
								}
							}
							Game.Instance.SelectionCharacter.SetSelected(unitEntityData);
						}
						//
						//EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityData.Descriptor), true);

						//Game.Instance.UI.Canvas.
						compCycle = "";


					}

					if (GUILayout.Button("Next", GUILayout.Width(100f), GUILayout.Height(20f)))
					{
						j++;

						if (RootUIContext.Instance.CurrentServiceWindow.ToString().Equals("CharacterInfo") ||
							RootUIContext.Instance.CurrentServiceWindow.ToString().Equals("Inventory"))
						{


							Game.Instance.SelectionCharacter.SetSelected(Game.Instance.SelectionCharacter.ActualGroup.ToArray()[j]);

							if (j == Game.Instance.SelectionCharacter.ActualGroup.ToArray().Count() - 1) j = -1;
							try
							{

								//Game.Instance.SelectionCharacter.CurrentSelectedCharacter.CharacterName.cleanCharname();
								unitEntityData = Game.Instance.SelectionCharacter.CurrentSelectedCharacter;
								//			Main.DebugLog("4");
								charcterNameDirectoryName = Main.GetCompanionDirName(unitEntityData.CharacterName);

								string compPortraitPath = Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + charcterNameDirectoryName);

								//Main.DebugLog(compPortraitPath);

								if (Directory.Exists(compPortraitPath))
								{
									if (Directory.GetFiles(Path.Combine(compPortraitPath), "*.current").Length != 0)
									{
										string[] dirs = Directory.GetFiles(Path.Combine(compPortraitPath), "*.current");

										if (!Path.GetFileNameWithoutExtension(dirs[0]).Equals("root"))
											compDirlabel = Path.GetFileNameWithoutExtension(dirs[0]);
										else
											compDirlabel = " - (Character root folder)";
									}
									else
										compDirlabel = "";
								}

							}
							catch (Exception e)
							{

								Main.DebugError(e);
							}
						}
						/*

						//Main.DebugLog("j: "+j);
						if (
							RootUIContext.Instance != null &&
							RootUIContext.Instance.InGameVM != null &&
							RootUIContext.Instance.InGameVM.StaticPartVM != null &&
							RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM != null &&
							RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.CharactersVM != null &&
							RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.CharactersVM.Count() > 0)
						{
						//	Main.DebugLog("1");

							Game.Instance.SelectionCharacter.SetSelected(RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.CharactersVM[j].UnitEntityData);

							//	Main.DebugLog("2");

							//	Main.DebugLog("2.5");

							if (j == RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.CharactersVM.Count() - 1) j = -1;
						//	Main.DebugLog("3");

							if (RootUIContext.Instance.CurrentServiceWindow.ToString().Equals("CharacterInfo"))
						//		Main.DebugLog("4");
							{


							}

						}
						else
						{
							Main.DebugLog("No partyVM?");
							GUILayout.Label("No partyVM?");
						}
						*/
					}
					GUILayout.Label(Path.Combine(compdirpath, compDirlabel));
				}
				else
					GUILayout.Label("Open Character Info sheet first for cycle through companion portraits.");
			}
			else
				GUILayout.Label("Advanced settings: mod is not managing companion portraits.");

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Create Dirs", GUILayout.Width(200f), GUILayout.Height(20f)))
			{
				makeNpcDirs();
			}
			GUILayout.Label("Create directories for all NPC-s taking part in dialogs.");
			GUILayout.EndHorizontal();

			if (!showExperimental)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Show experimental feature", GUILayout.Width(200f), GUILayout.Height(20f)))
				{
					showExperimental = true;
				}
				GUILayout.EndHorizontal();
			}
			else
			{

				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Close", GUILayout.Width(200f), GUILayout.Height(20f)))
				{
					showExperimental = false;
				}
				GUILayout.Label("WARNING! This might hang your game for 1-2 minutes or crash.", boldStyle);

				GUILayout.EndHorizontal();




				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Save Npc Portraits", GUILayout.Width(200f), GUILayout.Height(20f)))
				{
					saveNpcPortraits();
				}
				GUILayout.Label("Save all NPC-s' turn based portraits in their respective dirs: " + portraitCounter + " have been saved OK! ( " + failCounter + " invalid )");
				GUILayout.EndHorizontal();
			}
			//GUILayout.Label(" ");

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("In dialog", boldStyle);
			GUILayout.EndHorizontal();

			if (!isDialog)
			{
				GUILayout.Label("Start a dialog for more options!");
			}
			else
			{






					if (Game.Instance.DialogController.CurrentSpeakerBlueprint != null)
					{

						string CurrentSpeakerName = Game.Instance.DialogController.CurrentSpeakerName;






						//bool companion = Main.companions.Contains(CurrentSpeakerName);

						bool companion = Game.Instance.Player.AllCharacters.Contains(RealCurrentSpeakerEntity(CurrentSpeakerName)) ||
										 Main.companions.Contains(CurrentSpeakerName);





						string CurrentSpeakerBlueprintName = Game.Instance.DialogController.CurrentSpeakerBlueprint.name;

						string NpcPortraitPath = Path.Combine(GetNpcPortraitsDirectory(), CurrentSpeakerName);

						if (CurrentSpeakerName.Equals(AstyName))
						{

							if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
								CurrentSpeakerName = AstyName + " - Drow";
							else
								savedNpc = true;
						}
						if (CurrentSpeakerName.Equals(VelhmName))
						{
							if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
								CurrentSpeakerName = VelhmName + " - Drow";
							else
								savedNpc = true;
						}
						if (CurrentSpeakerName.Equals(TranName))
						{
							if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
								CurrentSpeakerName = TranName + " - Drow";
							else
								savedNpc = true;
						}

						//Irabeth scar
						if (CurrentSpeakerName.Equals(Main.IrabethName))
						{
							if (Game.Instance.Player.EtudesSystem.EtudeIsStarted(ResourcesLibrary.TryGetBlueprint<BlueprintEtude>("b4f08736cf124ae4996fcef7c0a33bf1")))
							{

								CurrentSpeakerName = CurrentSpeakerName + " - Scar";
							}
							else
								savedNpc = true;
						}

						if (!companion)
						{


							if (Directory.Exists(NpcPortraitPath) && /*!Directory.Exists(Path.Combine(NpcPortraitPath, CurrentSpeakerBlueprintName)) && */!CurrentSpeakerName.Contains(" - Drow"))
							{

								if (Main.settings.AutoBackup && !savedNpc)
								{
									SaveOriginals(Game.Instance.DialogController.CurrentSpeakerBlueprint, NpcPortraitPath);
									savedNpc = true;
								}
								GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
								if (GUILayout.Button("Open current speaker dir", GUILayout.Width(200f), GUILayout.Height(20f)))
								{
									Process.Start(NpcPortraitPath);
								}
								if (GUILayout.Button("Cycle through dir", GUILayout.Width(200f), GUILayout.Height(20f)))
								{

									List<string> strings = Directory.GetDirectories(NpcPortraitPath, "*", SearchOption.TopDirectoryOnly).ToList();
									unitNames.ForEach(item => strings.Remove(x => x.Contains(item)));

									//Main.DebugLog("i: " + i.ToString() + " - strings.count: " + strings.Count().ToString());
									if (File.Exists(Path.Combine(NpcPortraitPath, "Medium.png")))
									{
										if (!strings.Exists(x => x.Equals("npc name root Medium png placeholder"))) strings.Add("npc name root Medium png placeholder");

									}

									if (strings.Count() == 1)
										i = -1;
									else
									if (i == strings.Count() - 1)
									{
										i = -1;
										//	Main.DebugLog("Reset! i: " + i.ToString() + " - strings.count: " + strings.Count().ToString());
									}
									i++;


									foreach (string s in strings)
									{
										//Main.DebugLog(s);
									}
									foreach (string sFile in System.IO.Directory.GetFiles(NpcPortraitPath, "*.current"))
									{
										System.IO.File.Delete(sFile);
									}
									//	Main.DebugLog("1");

									if (strings[i].Equals("npc name root Medium png placeholder"))
									{
										//	Main.DebugLog("2");

										npcCycle = "";
										npcDirlabel = " - (Npc root folder)";
										File.WriteAllText(Path.Combine(NpcPortraitPath, "root" + ".current"), "current");

									}
									else
									{
										//	Main.DebugLog(Path.GetFileName(strings[i]));

										npcCycle = Path.GetFileName(strings[i]);
										npcDirlabel = npcCycle;
										//	Main.DebugLog("3");

										File.WriteAllText(Path.Combine(NpcPortraitPath, npcCycle + ".current"), "current");
										//	Main.DebugLog(Path.Combine(NpcPortraitPath, npcCycle + ".current"));

									}
									//	Main.DebugLog("5");

									CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
									EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);
									npcCycle = "";


								}

								GUILayout.Label(Path.Combine(NpcPortraitPath, npcDirlabel));
								GUILayout.EndHorizontal();
							}
							else
							{
								GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
								if (GUILayout.Button("Make speaker dir", GUILayout.Width(200f), GUILayout.Height(20f)))
								{
									Directory.CreateDirectory(NpcPortraitPath);


									//	Main.DebugLog("Create dir at: " + NpcPortraitPath);
								}
								GUILayout.Label(NpcPortraitPath);
								GUILayout.EndHorizontal();
							}

						}
						else
						{
							if (Main.settings.ManageCompanions)
							{
								//BlueprintPortrait blueprintPortrait = null;

								CurrentSpeakerName = Main.GetCompanionDirName(Game.Instance.DialogController.CurrentSpeakerName);
								/*
								if (CurrentSpeakerName.ToLower().Contains("aruesh"))
									if (Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.Race.name.ToLower().Contains("succubusrace"))
										 = "Arueshalae - Evil";
								if (CurrentSpeakerName.Equals("Nenio"))
									if (Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime == null)
										CurrentSpeakerName = "NenioFox_Portrait";
								if (CurrentSpeakerName.Equals("Ciar"))
									if (Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.Alignment == Kingmaker.Enums.Alignment.LawfulEvil)
										CurrentSpeakerName = "Ciar - Undead";
								if (CurrentSpeakerName.Equals("Queen Galfrey"))
									if (Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.Alignment == Kingmaker.Enums.Alignment.LawfulEvil)
										CurrentSpeakerName = "Queen Galfrey - Undead";
								if (CurrentSpeakerName.Equals("Staunton Vane"))
									if (Game.Instance.DialogController.CurrentSpeaker.Descriptor.IsUndead)
										CurrentSpeakerName = "Staunton Vane - Undead";
								*/


								if (CurrentSpeakerName.Equals(Main.ArueName + " - Evil") && Main.settings.AutoBackup && !savedComp)
								{
									savedComp = true;
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("484588d56f2c2894ab6d48b91509f5e3");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}

								if (CurrentSpeakerName.Equals(Main.NenioName + "Fox_Portrait") && Main.settings.AutoBackup && !savedComp)
								{
									savedComp = true;
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}

								if (CurrentSpeakerName.Equals(Main.CiarName + " - Undead") && Main.settings.AutoBackup && !savedComp)
								{
									savedComp = true;
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("dc2f02dd42cfe2b40923eb014591a009");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}


								/*                          [CustomNpcPortraits] GalfreyUndead_Portrait
														[CustomNpcPortraits] 767456b1656ca064dadac544d39d7e40
														[CustomNpcPortraits] GalfreyOld_Portrait
														[CustomNpcPortraits] 4e8dfb75015d356469b976145c851087
														[CustomNpcPortraits] GalfreyYoung_Portrait
														[CustomNpcPortraits] a3ba06b4723c7a74fb5054ccb2289efb
							  */

								if (CurrentSpeakerName.Equals(Main.GalfreyName + " - Undead") && Main.settings.AutoBackup && !savedComp)
								{
									savedComp = true;
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("767456b1656ca064dadac544d39d7e40");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}

								if (CurrentSpeakerName.Equals(Main.GalfreyName + " - Old") && Main.settings.AutoBackup && !savedComp)
								{
									savedComp = true;
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("4e8dfb75015d356469b976145c851087");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}
								if (CurrentSpeakerName.Equals(Main.WoljifName + " - Demon") && Main.settings.AutoBackup && !savedComp)
								{
									savedComp = true;
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("f2bd5a94ade889444921b4a74b9e34a9");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}


								if (CurrentSpeakerName.Equals(Main.StauntonName + " - Undead") && Main.settings.AutoBackup && !savedComp)
								{
									savedComp = true;
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("f4bbe08217bcaa54c91fe73bcea70ede");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}





								string CompanionPortraitPath = Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName);

								if (Directory.Exists(CompanionPortraitPath))
								{
									if (Main.settings.AutoBackup && !savedComp)
									{
										//Main.DebugLog("3");

										SaveOriginals(Game.Instance.DialogController.CurrentSpeakerBlueprint, CompanionPortraitPath);
										savedComp = true;
									}





									/*	string str = BundlesLoadService.BundlesPath("cheatdata.json");
										BlueprintList blueprintList = new BlueprintList()
										{
											Entries = new List<BlueprintList.Entry>()
										};

										if (File.Exists(str))
											blueprintList = JsonUtility.FromJson<BlueprintList>(File.ReadAllText(str));


										Dictionary<string, string> speakers = new Dictionary<string, string>();


										string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();

										foreach (BlueprintList.Entry be in blueprintList.Entries)
										{

											if (be.TypeFullName.EndsWith("BlueprintUnit"))
											{
												BlueprintUnit bu = Utilities.GetBlueprintByGuid<BlueprintUnit>(be.Guid);
												if (bu.IsCompanion && bu.CharacterName.Equals("Ciar"))
												{
													Main.DebugLog(bu.CharacterName);
													Main.DebugLog(bu.name);
													//string prefix = Main.GetCompanionPortraitDirPrefix();
													//string portraitsDirectoryPath2 = Main.GetCompanionPortraitsDirectory();
													//string portraitDirectoryName = bu.name;
													//string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath2, "--", portraitDirectoryName);

													//	SaveOriginals2(bc.Data, portraitDirectoryPath);
													SaveOriginals(bu, CompanionPortraitPath);
													savedComp = true;
													break;
												}
											}
										}

									}*/

									GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());

									if (GUILayout.Button("Open current speaker dir", GUILayout.Width(200f), GUILayout.Height(20f)))
									{
										Process.Start(CompanionPortraitPath);
									}

									GUILayout.Label(CompanionPortraitPath);
									GUILayout.EndHorizontal();
								}
								else
								{
									GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
									if (GUILayout.Button("Make speaker dir", GUILayout.Width(200f), GUILayout.Height(20f)))
									{
										Directory.CreateDirectory(CompanionPortraitPath);

										//Main.DebugLog("Create dir at: " + CompanionPortraitPath);
									}
									GUILayout.Label(CompanionPortraitPath);
									GUILayout.EndHorizontal();
								}
							}
							else
								GUILayout.Label("Advanced settings: mod is not managing companion portraits.");
							
							

							
						}



						if (!companion)
						{
							string portraitDirectorySubPath = Path.Combine(NpcPortraitPath, CurrentSpeakerBlueprintName);

							if (Directory.Exists(portraitDirectorySubPath))
							{
								if (Main.settings.AutoBackup && !savedNpc)
								{
									SaveOriginals(Game.Instance.DialogController.CurrentSpeakerBlueprint, portraitDirectorySubPath);
									savedNpc = true;
								}
								GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
								if (GUILayout.Button("Open current speaker subdir", GUILayout.Width(200f), GUILayout.Height(20f)))
								{
									Process.Start(portraitDirectorySubPath);
								}
								if (GUILayout.Button("Cycle through subdirs", GUILayout.Width(200f), GUILayout.Height(20f)))
								{

									List<string> strings = Directory.GetDirectories(portraitDirectorySubPath, "*", SearchOption.TopDirectoryOnly).ToList();

									if (File.Exists(Path.Combine(portraitDirectorySubPath, "Medium.png")))
									{
										if (!strings.Exists(x => x.Equals("npc name subroot Medium png placeholder"))) strings.Add("npc name subroot Medium png placeholder");

									}
									if (strings.Count() == 1)
										i = -1;
									else
									if (i == strings.Count() - 1) i = -1;

									i++;

									//unitNames.ForEach(item => strings.Remove(x => x.Contains(item)));


									foreach (string s in strings)
									{
										//Main.DebugLog(s);
									}

									foreach (string sFile in System.IO.Directory.GetFiles(portraitDirectorySubPath, "*.current"))
									{
										System.IO.File.Delete(sFile);
									}

									if (strings[i].Equals("npc name subroot Medium png placeholder"))
									{
										npcSubCycle = "";
										npcSubDirlabel = " - (Npc sub root folder)";
										File.WriteAllText(Path.Combine(portraitDirectorySubPath, "root" + ".current"), "current");

									}
									else
									{
										npcSubCycle = Path.GetFileName(strings[i]);
										npcSubDirlabel = npcSubCycle;
										File.WriteAllText(Path.Combine(portraitDirectorySubPath, npcSubCycle + ".current"), "current");
									}

									CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
									EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);
									npcSubCycle = "";


								}
								GUILayout.Label(Path.Combine(portraitDirectorySubPath, npcSubDirlabel));
								GUILayout.EndHorizontal();

							}
							else if (!CurrentSpeakerName.Equals(CurrentSpeakerBlueprintName) && !CurrentSpeakerName.Contains(" - Drow"))
							{
								GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
								if (GUILayout.Button("Make speaker subcat dir", GUILayout.Width(200f), GUILayout.Height(20f)))
								{

									Directory.CreateDirectory(portraitDirectorySubPath);


									//Main.DebugLog("Create dir at: " + portraitDirectorySubPath);
								}
								string CNameBpName = Path.Combine(CurrentSpeakerName, CurrentSpeakerBlueprintName);
								GUILayout.Label("..\\" + Path.Combine(NpcPortraitsDirName(), CNameBpName));
								GUILayout.EndHorizontal();
							}
						}
						else
						{
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							GUILayout.Label("Speak with npc-s for more option (not companions)");
							GUILayout.EndHorizontal();
						}
					}
				/*
				if (Game.Instance.DialogController.CurrentSpeaker.CharacterName != null)
					GUILayout.Label(Game.Instance.DialogController.CurrentSpeakerBlueprint.IsCompanion.ToString());

				if (Game.Instance.DialogController.CurrentSpeakerBlueprint != null)
				{
					GUILayout.Label(Game.Instance.DialogController.CurrentSpeakerBlueprint.CharacterName);
				}
				if (Game.Instance.DialogController.CurrentSpeakerName != null)
					GUILayout.Label(Game.Instance.DialogController.CurrentSpeakerName);

				if (Game.Instance.DialogController.CurrentCue.Speaker != null)
					if (Game.Instance.DialogController.CurrentCue.Speaker.Blueprint != null)
						GUILayout.Label(Game.Instance.DialogController.CurrentCue.Speaker.Blueprint.CharacterName);


				if (Game.Instance.DialogController.CurrentSpeaker != null)
						GUILayout.Label("CurrentSpeaker != null");



					if (Game.Instance.DialogController.CurrentCue.Speaker.GetHashCode() > 0)
					{

							GUILayout.Label(Game.Instance.DialogController.CurrentCue.Speaker.GetHashCode().ToString());

						if (Game.Instance.DialogController.CurrentCue.Speaker.Blueprint != null)
							GUILayout.Label(Game.Instance.DialogController.CurrentCue.Speaker.Blueprint.CharacterName);
					}
					*/

				//GUILayout.Label("Dialog options! YAAAYYY!!!");

				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (!Directory.Exists(Path.Combine(GetNpcPortraitsDirectory(), Game.Instance.DialogController.CurrentSpeakerName, Game.Instance.DialogController.Dialog.name)))
				{
					if (GUILayout.Button("Create unique dialog folder", GUILayout.Width(200f), GUILayout.Height(20f)))
					{
						string NpcPortraitPath = Path.Combine(GetNpcPortraitsDirectory(), Game.Instance.DialogController.CurrentSpeakerName);

						Directory.CreateDirectory(Path.Combine(NpcPortraitPath, Game.Instance.DialogController.Dialog.name));
					}
					GUILayout.Label("Create sub folder only for this dialog named: " + Game.Instance.DialogController.Dialog.name);
				}
				else
				{
					if (GUILayout.Button("Open unique dialog folder", GUILayout.Width(200f), GUILayout.Height(20f)))
					{
						Process.Start(Path.Combine(GetNpcPortraitsDirectory(), Game.Instance.DialogController.CurrentSpeakerName, Game.Instance.DialogController.Dialog.name));
					}
					GUILayout.Label("Open sub folder: " + Path.Combine(GetNpcPortraitsDirectory(), Game.Instance.DialogController.CurrentSpeakerName, Game.Instance.DialogController.Dialog.name));



				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Log dialog portrait details", GUILayout.Width(200f), GUILayout.Height(20f)))
				{

						/*
						foreach (BlueprintPortrait scriptableObject in Utilities.GetScriptableObjects<BlueprintPortrait>())
						{
							Main.DebugLog(scriptableObject.name);
							//string blueprintPath = Utilities.GetBlueprintPath(scriptableObject);
							//"HosillaKey"
							if (scriptableObject != null && scriptableObject.name != null && scriptableObject.name.Length > 0 && scriptableObject.name.ToLower().Contains("balor"))																											  //if (blueprintPath.ToLower().Contains("angelbloom"))
							{
								SaveOriginals2(scriptableObject.Data, GetNpcPortraitsDirectory());
								//ItemEntitySimple key = new ItemEntitySimple(scriptableObject);


							}
							//Main.DebugLog("náme:" + scriptableObject.Name);
							//Main.DebugLog("path:"+ Utilities.GetBlueprintPath(scriptableObject));
						}
						*/


					CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
					EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);

					Main.Log("--------------------------------------------------------------------------------------");

					Main.Log("CurrentCue.Text: " + Game.Instance.DialogController.CurrentCue.Text);
					Main.Log("CurrentCue.name: " + Game.Instance.DialogController.CurrentCue.name);
					Main.Log("CurrentCue.Comment: " + Game.Instance.DialogController.CurrentCue.Comment);
					Main.Log("CurrentCue.DisplayText: " + Game.Instance.DialogController.CurrentCue.DisplayText);
					Main.Log("CurrentCue.AssetGuid: " + Game.Instance.DialogController.CurrentCue.AssetGuid);
						
					Main.Log("Dialog.AssetGuid: " + Game.Instance.DialogController.Dialog.AssetGuid);
					Main.Log("Dialog.Comment: " + Game.Instance.DialogController.Dialog.Comment);
					Main.Log("Dialog.name: " + Game.Instance.DialogController.Dialog.name);
					Main.Log("Dialog.Type: " + Game.Instance.DialogController.Dialog.Type);
					Main.Log("CurrentSpeakerName: " + Game.Instance.DialogController.CurrentSpeakerName);
					Main.Log("CurrentSpeaker.CharacterName: " + Game.Instance.DialogController.CurrentSpeaker?.CharacterName);
					Main.Log("CurrentSpeaker.Blueprint.AssetGuid: " + Game.Instance.DialogController.CurrentSpeaker?.Blueprint.AssetGuid);
					Main.Log("CurrentSpeaker.Portrait.CustomId: " + Game.Instance.DialogController.CurrentSpeaker?.Portrait?.CustomId);

						Main.Log("Answers: ");
						foreach (var item in Game.Instance.DialogController.Answers)
						{
							Main.Log("Answers.item.AssetGuid: " + item.AssetGuid);
							Main.Log("Answers.item.AssetGuid: " + item.DisplayText);
							Main.Log("Answers.item.AssetGuid: " + item.name);
							Main.Log("Conditions: ");
							if (item.ShowConditions.Conditions.Length > 0)
								foreach (var item2 in item.ShowConditions.Conditions)
								{

									Main.Log("ShowConditions.Conditions.item.name: " + item2.name);
									Main.Log("ShowConditions.Conditions.item.GetDescription: " + item2.GetDescription());
									Main.Log("ShowConditions.Conditions.item.ToString: " + item2.ToString());
									Main.Log("ShowConditions.Conditions.item.GetType: " + item2.GetType());

								}
						}

						Main.Log("Inventory: ");
					if (Game.Instance.DialogController.CurrentSpeaker?.Inventory != null)
						foreach (var item in Game.Instance.DialogController.CurrentSpeaker.Inventory)
						{
							Main.Log("   item: " + item.Name + " - " + item.Blueprint.ItemType + " - " + item.Blueprint.SubtypeName);

						}

					Main.Log("Body: ");
					Main.Log("CurrentSpeaker.Blueprint.Body.Armor: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint.Body?.Armor?.NameForAcronym);
					Main.Log("CurrentSpeaker.Blueprint.Body.Head: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint.Body?.Head?.NameForAcronym);
					Main.Log("CurrentSpeaker.Blueprint.Body.PrimaryHand: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint?.Body?.PrimaryHand?.NameForAcronym);
					Main.Log("CurrentSpeaker.Blueprint.Body.SecondaryHand: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint.Body?.SecondaryHand?.NameForAcronym);

					Main.Log("CurrentSpeaker.Blueprint.name: " + Game.Instance.DialogController.CurrentSpeaker?.Blueprint.name);
					Main.Log("CurrentSpeaker.Blueprint.Race: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint?.Race);
					Main.Log("CurrentSpeaker.Blueprint.Type: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint?.Type?.name);

					Main.Log("CurrentSpeaker.Blueprint.Gender: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint?.Gender);

					Game.Instance.DialogController.CurrentSpeaker.Progression.Classes.ForEach(n => Main.Log("CurrentSpeaker.Blueprint.Class: " + n.CharacterClass.name));

					Main.Log("CurrentSpeaker.Blueprint.Alignment: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint.Alignment);
					Main.Log("CurrentSpeaker.Blueprint.Faction: " + Game.Instance.DialogController.CurrentSpeaker.Blueprint.Faction);
					Main.Log("CurrentSpeaker.Blueprint.PortraitSafe.name: " + Game.Instance.DialogController.CurrentSpeaker?.Blueprint.PortraitSafe.name);


					Main.Log("CurrentSpeaker->isPolymorphed: " + Game.Instance.DialogController.CurrentSpeaker?.Body.IsPolymorphed);


					if ((bool)(Game.Instance.DialogController.CurrentSpeaker?.Body.IsPolymorphed))
					{
						Main.Log("---------------------Poly start");
						Main.Log("CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent.OwnerBlueprint.name: " + Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name);
						Main.Log("CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent.OwnerBlueprint.GetComponent<Polymorph>().Portrait.name: " + Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.GetComponent<Polymorph>()?.Portrait?.name);

						foreach (var comp in Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.ComponentsArray)
						{
							Main.Log("   " + comp.name + " - " + comp.GetType());
						};
						
							if(Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component?.m_Portrait.GetBlueprint() != null)
						Main.Log("Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.m_Portrait.name: " + (Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.m_Portrait.GetBlueprint() as BlueprintPortrait).name);
	

						Main.Log("Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime.Owner.View.name: " + Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime.Owner.View.name);

						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Charactername: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.CharacterName);
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.name: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.name);
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Race: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Race);
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Type: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Type?.name);

						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Alignment: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Alignment);
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Faction: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Faction);

						Main.Log("Inventory: ");
						if (Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection != null)
							foreach (var item in Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.m_StartingInventory)
							{
								Main.Log("   item: " + item.NameSafe() + " - " + ((BlueprintItem)item.GetBlueprint()).name + " - " + ((BlueprintItem)item.GetBlueprint()).ItemType + " - " + ((BlueprintItem)item.GetBlueprint()).SubtypeName);

							}

						Main.Log("Body: ");
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Body.Armor: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Body?.Armor?.NameForAcronym);
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Body.Head: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Body?.Head?.NameForAcronym);
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Body.PrimaryHand: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Body?.PrimaryHand?.NameForAcronym);
						Main.Log("CurrentSpeaker.ReplaceBlueprintForInspection.Body.SecondaryHand: " + Game.Instance.DialogController.CurrentSpeaker.ReplaceBlueprintForInspection?.Body?.SecondaryHand?.NameForAcronym);

						//Main.Log(Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.Owner?.Blueprint?.name);
						//Main.Log(Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.Owner?.Blueprint?.PortraitSafe?.name);
						//Main.Log(Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.Buff?.Owner?.Blueprint?.CharacterName);

						//Main.Log(Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.Buff?.Owner?.Blueprint?.name);
						//Main.Log(Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.Buff?.Owner?.Blueprint?.PortraitSafe?.name);


						Main.Log("GetSubscribingUnit().Blueprint.CharacterName: "+Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime.GetSubscribingUnit().Blueprint.CharacterName);
						Main.Log("GetSubscribingUnit().Blueprint.name: " + Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime.GetSubscribingUnit().Blueprint.name);
						Main.Log("GetSubscribingUnit().Blueprint.PortraitSafe.name: " + Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime.GetSubscribingUnit().Blueprint.PortraitSafe.name);


						Main.Log("---------------------Polymorph end");

					}
					Main.Log("CurrentSpeakerBlueprint.CharacterName: " + Game.Instance.DialogController.CurrentSpeakerBlueprint?.CharacterName);
					Main.Log("CurrentSpeakerBlueprint.name: " + Game.Instance.DialogController.CurrentSpeakerBlueprint?.name);
					Main.Log("CurrentSpeakerBlueprint.Name: " + Game.Instance.DialogController.CurrentSpeakerBlueprint?.Name);

					Main.Log("CurrentSpeakerBlueprint.Race: " + Game.Instance.DialogController.CurrentSpeakerBlueprint?.Race);
					Main.Log("CurrentSpeakerBlueprint.Type: " + Game.Instance.DialogController.CurrentSpeakerBlueprint?.Type?.name);

					Main.Log("CurrentSpeakerBlueprint.Gender: " + Game.Instance.DialogController.CurrentSpeakerBlueprint?.Gender);

					//Game.Instance.DialogController.CurrentSpeaker.Progression.Classes.ForEach(n => Main.Log("CurrentSpeaker.Blueprint.Class: " + n.CharacterClass.name));

					Main.Log("CurrentSpeakerBlueprint.Alignment: " + Game.Instance.DialogController.CurrentSpeakerBlueprint.Alignment);
					Main.Log("CurrentSpeakerBlueprint.Faction: " + Game.Instance.DialogController.CurrentSpeakerBlueprint.Faction);
					Main.Log("CurrentSpeakerBlueprint.PortraitSafe.name: " + Game.Instance.DialogController.CurrentSpeakerBlueprint?.PortraitSafe.name);

					Main.Log("FirstSpeaker.CharacterName: " + Game.Instance.DialogController.FirstSpeaker?.CharacterName);
					Main.Log("m_CustomSpeakerName: " + Game.Instance.DialogController.m_CustomSpeakerName);
					Main.Log("ActingUnit.CharacterName: " + Game.Instance.DialogController.ActingUnit?.CharacterName);
				}
				GUILayout.EndHorizontal();

			}


		}

		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Advanced", boldStyle);
		GUILayout.EndHorizontal();


		Main.settings.ManageCompanions = GUILayout.Toggle(Main.settings.ManageCompanions, "Let the mod manage the portraits of main char, companions, and mercenaries.", new GUILayoutOption[0]);

		Main.settings.AutoBackup = GUILayout.Toggle(Main.settings.AutoBackup, "Keep game defaults auto backed up in 'Backup of Game Default Portraits' of each subdir.", new GUILayoutOption[0]);
		Main.settings.AutoSecret = GUILayout.Toggle(Main.settings.AutoSecret, "Extract and use the turn based portraits of NPC-s in dialogs where available.", new GUILayoutOption[0]);

		Main.settings.RightverseGroupPortraits = GUILayout.Toggle(Main.settings.RightverseGroupPortraits, "Show current party members' portraits first in places like cities on group portrait bar", new GUILayoutOption[0]);






		if (!showExtra)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Show extra options", GUILayout.Width(200f), GUILayout.Height(20f)))
			{
				showExtra = true;
			}
			GUILayout.EndHorizontal();
		}
		else
		{

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Close", GUILayout.Width(200f), GUILayout.Height(20f)))
			{
				showExtra = false;
			}
			GUILayout.EndHorizontal();

			Main.settings.DollroomHalo = GUILayout.Toggle(Main.settings.DollroomHalo, "Show aasimar halo in Inventory window", new GUILayoutOption[0]);

			if (isLoadedGame)
			{
				Main.settings.ArueHalo = GUILayout.Toggle(Main.settings.ArueHalo, "Arueshalae needs a halo!", new GUILayoutOption[0]);
				if (Main.settings.ArueHalo && !Main.settings.ArueHaloAdded)
				{
					ArueAddHalo();
					Main.settings.ArueHaloAdded = true;

					foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
					{
						if (unitEntityDatum.CharacterName == "Arueshalae")
						{
							var LightHaloToggle = ResourcesLibrary.TryGetBlueprint<BlueprintActivatableAbility>("248bbb747c273684d9fdf2ed38935def");

							//give

							if (unitEntityDatum.HasFact(LightHaloToggle))
							{
								//UIUtility.SendWarning(unitEntityDatum.GetFact(LightHaloToggle).Name);

								unitEntityDatum.GetFact<ActivatableAbility>(LightHaloToggle).IsOn = true;
							}
							break;
						}

					}



				}

				if (!Main.settings.ArueHalo && Main.settings.ArueHaloAdded)
				{
					ArueAddHalo();
					Main.settings.ArueHaloAdded = false;

				}
			}
			else
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label("Load a game to see Arueshalae halo option here");
				GUILayout.EndHorizontal();
			}

			if (isLoadedGame)
			{
				Main.settings.EmberHalo = GUILayout.Toggle(Main.settings.EmberHalo, "Ember is a living saint, she should have a halo too", new GUILayoutOption[0]);
				if (Main.settings.EmberHalo && !Main.settings.EmberHaloAdded)
				{
					//Main.DebugLog("add halo");
					EmberAddHalo();
					Main.settings.EmberHaloAdded = true;

					foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
					{
						if (unitEntityDatum.CharacterName == "Ember")
						{

							//Main.DebugLog("add halo 2");
							var LightHaloToggle = ResourcesLibrary.TryGetBlueprint<BlueprintActivatableAbility>("248bbb747c273684d9fdf2ed38935def");

							//give

							if (unitEntityDatum.HasFact(LightHaloToggle))
							{
								//UIUtility.SendWarning(unitEntityDatum.GetFact(LightHaloToggle).Name);

								unitEntityDatum.GetFact<ActivatableAbility>(LightHaloToggle).IsOn = true;
							}
							break;
						}

					}



				}

				if (!Main.settings.EmberHalo && Main.settings.EmberHaloAdded)
				{
					EmberAddHalo();
					Main.settings.EmberHaloAdded = false;

				}
			}
			else
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				GUILayout.Label("Load a game to see Ember halo option here");
				GUILayout.EndHorizontal();
			}



				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				Main.settings.SwarmRiddance = GUILayout.Toggle(Main.settings.SwarmRiddance, " Remove retarded swarm dialog options", new GUILayoutOption[0]);
				
				GUILayout.Label("Answers with swarm option: " + swarmAnswers);
				if (swarmAnswers > 0 && swarmCleanRunning == false)
					if (Main.settings.SwarmRiddance == true)
                    {
						swarmCleanRunning = true;
						GetRidOfSwarm();
						CountSwarm();
						swarmCleanRunning = false;

					}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());

				GUILayout.Label("     (Good if you have ToyBox show all dialogs ON, but you don't want to always see \"Eat her?\")");
				GUILayout.EndHorizontal();


			}





#if DEBUG
			if (!showPatched)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Show patched methods", GUILayout.Width(200f), GUILayout.Height(20f)))
			{
				showPatched = true;
			}
			GUILayout.EndHorizontal();
		}
		else
		{

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Close", GUILayout.Width(200f), GUILayout.Height(20f)))
			{
				showPatched = false;
			}
			GUILayout.EndHorizontal();


			GUILayoutOption[] noExpandwith = new GUILayoutOption[]
			 {
				GUILayout.ExpandWidth(false)
			 };




			if (Main.okPatches.Count > 0)
			{
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());

				GUILayout.Label("<b>OK: Some patches apply:</b>", noExpandwith);

				foreach (string str in Main.okPatches)
				{
					GUILayout.Label("  • <b>" + str + "</b>", noExpandwith);
				}
				GUILayout.EndVertical();
			}
			if (Main.failedPatches.Count > 0)
			{
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.Label("<b>Error: Some patches failed to apply. These features may not work:</b>", noExpandwith);
				foreach (string str2 in Main.failedPatches)
				{
					GUILayout.Label("  • <b>" + str2 + "</b>", noExpandwith);
				}
				GUILayout.EndVertical();
			}
			if (Main.okLoading.Count > 0)
			{
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.Label("<b>OK: Some assets loaded:</b>", noExpandwith);
				foreach (string str3 in Main.okLoading)
				{
					GUILayout.Label("  • <b>" + str3 + "</b>", noExpandwith);
				}
				GUILayout.EndVertical();
			}
			if (Main.failedLoading.Count > 0)
			{
				GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
				GUILayout.Label("<b>Error: Some assets failed to load. Saves using these features won't work:</b>", noExpandwith);
				foreach (string str4 in Main.failedLoading)
				{
					GUILayout.Label("  • <b>" + str4 + "</b>", noExpandwith);
				}
				GUILayout.EndVertical();
			}
		}
#endif
		/*
		GUILayout.Label(GetNpcPortraitsDirectory(), new GUILayoutOption[0]);
		Main.settings.AvoidRandom = GUILayout.Toggle(Main.settings.AvoidRandom, "Always allow evasion of Random Encounters", new GUILayoutOption[0]);
		GUILayout.Label("Crusader Battles", new GUILayoutOption[0]);
		Main.settings.NoCrusaderCombat = GUILayout.Toggle(Main.settings.NoCrusaderCombat, "Crusaders always win autoresolve", new GUILayoutOption[0]);
		GUILayout.Label("Corruption", new GUILayoutOption[0]);
		Main.settings.PreventCorruption = GUILayout.Toggle(Main.settings.PreventCorruption, "Prevent increasing Corruption.", new GUILayoutOption[0]);
	*/
	}

		
		public static int swarmAnswers = 0;
		public static bool swarmCleanRunning = false;


		public static void CountSwarm()
		{
			swarmAnswers = 0;

			var list = Utilities.GetAllBlueprints();

			foreach (var item in list.Entries)
			{
				if (item != null && item.Type != null && !item.Type.Name.IsNullOrEmpty() && item.Guid != null)
					if (item.Type.Name.Equals("BlueprintAnswersList"))
				{
							var bp = ResourcesLibrary.TryGetBlueprint<BlueprintAnswersList>(item.Guid);

					if(bp !=null)
					foreach (var answerRef in bp.Answers)
					{
						var bpAnswer = answerRef.GetBlueprint() as BlueprintAnswerBase;


							if(bpAnswer != null)
						if (bpAnswer.MythicRequirement == Mythic.PlayerIsLocust || bpAnswer.MythicRequirement == Mythic.LocustUnlocked)
						{
							
							swarmAnswers++;
						}
					}


				}

			}
		}

		public static void GetRidOfSwarm()
        {
			var list = Utilities.GetAllBlueprints();

			//Log("all: " + list.Entries.Count());
			foreach (var item in list.Entries)
			{

				if(item != null && item.Type != null && !item.Type.Name.IsNullOrEmpty())
				if (item.Type.Name.Equals("BlueprintAnswersList"))
				{
					if (item.Guid != null)
					{
						var bp = ResourcesLibrary.TryGetBlueprint<BlueprintAnswersList>(item.Guid);

						if (bp != null)
						{
							List<BlueprintAnswerBaseReference> bprList = new List<BlueprintAnswerBaseReference>();

							foreach (var answerRef in bp.Answers)
							{
								var bpAnswer = answerRef.GetBlueprint() as BlueprintAnswerBase;

								if (bpAnswer != null)
								{
									if (bpAnswer.MythicRequirement == Mythic.PlayerIsLocust || bpAnswer.MythicRequirement == Mythic.LocustUnlocked)
									{
											if (!bprList.Contains(answerRef))
											{
												bprList.Add(answerRef);
												swarmAnswers++;

											}

										}
								}
							}


							if (bprList.Count() > 0 && bp.Answers.Count() > 0)
							{

								foreach (var aRef in bprList)
								{
									bp.Answers.Remove(aRef);
								};
							}
						}
					}

				}

			}

			Log("Removed " + swarmAnswers + " swarn dialog options");

		}

#if DEBUG
		static void OnUpdate(UnityModManager.ModEntry modEntry, float value)
		{
			/*
			if (Input.GetKeyDown(KeyCode.Comma))
			{
				var bp = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>("2db556136eac2544fa9744314c2a5713");
				var vars = bp.CustomizationPreset.UnitVariations.FirstOrDefault().Variations;

				if (index < vars.Count)
				{
					var pc = Game.Instance.Player.MainCharacter.Value;
					var prefab = BundledResourceHandle<UnitEntityView>.Request(vars[index].Prefab.AssetId); // e6ec3993da548c24ea9fa1448c2f68f7 UnitEntityView link is what you would put in the request.

					Logger.Log($"Spawning Variation {index}: {vars[index].Prefab.AssetId}");

					var unit = Game.Instance.EntityCreator.SpawnUnit(bp, prefab.Object, pc.Position, Quaternion.LookRotation(pc.OrientationDirection), Game.Instance.CurrentScene.MainState);
					unit.SwitchFactions(ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("d64258e86eeb1d8479f35a9b16f6590a"), true);
					index++;
				}
			}
			*/

			/*
			if (Input.GetKeyDown(KeyCode.Period))
			{

				//GrayGarrisonBasement1From1stFloor
				//string target = "DefendersHeart_DefendersHeart_Enter";
				//BlueprintAreaEnterPoint bap = Utilities.GetBlueprint<BlueprintAreaEnterPoint>(target);
				//Game.Instance.LoadArea(bap, AutoSaveMode.None);


				
				List<BlueprintAreaEnterPoint> baps = Utilities.GetScriptableObjects<BlueprintAreaEnterPoint>().ToList();

				UIUtility.SendWarning("index2: "+index2.ToString()  +" - total baps: "+baps.Count());

				if (index2 < baps.Count() )
				{
					index2++;

					UIUtility.SendWarning("area ("+ index2.ToString() + "): " + baps[index2].name);


					if (baps[index2].m_Area != null && baps[index2].m_AreaPart != null)
					{
                        var area = ResourcesLibrary.TryGetBlueprint<BlueprintArea>(baps[index2].Area.AssetGuid);

                        //BlueprintArea area = baps[index2].m_Area.GetBlueprint() as BlueprintArea;



						//BlueprintArea area = baps[index2].m_Area;

						if (area != null && !area.AreaName.IsEmpty() && area.m_SoundBankNames.Length > 0 && area.GraphCache != null && !string.IsNullOrEmpty(area.AreaName) )
						{
							Log("LOAD area (" + index2.ToString() + "): " + baps[index2].name);

							Game.Instance.LoadArea(baps[index2], AutoSaveMode.None);
						}
						else
							UIUtility.SendWarning("couldnt get area bp: " + baps[index2].Area.AreaName);

					}
					else
						UIUtility.SendWarning("area or areapart is null");

				}

			}
			
			
			if (Input.GetKeyDown(KeyCode.Comma))
			{

				foreach (EntityDataBase entityDatum in Game.Instance.Player.CrossSceneState.AllEntityData)
				{
					UnitEntityData unitEntityDatum = entityDatum as UnitEntityData;
					if (unitEntityDatum == null)
					{
						continue;
					}

					if (units.Contains(unitEntityDatum.Blueprint.name))
					{
						Main.Log("FOUND: " + unitEntityDatum.CharacterName + " - " + unitEntityDatum.Blueprint.name + " - " + unitEntityDatum.Descriptor.CustomPrefabGuid );
						UIUtility.SendWarning("FOUND: " + unitEntityDatum.CharacterName + " - " + unitEntityDatum.Blueprint.name + " - " + unitEntityDatum.Descriptor.CustomPrefabGuid);
					}
				}
				UIUtility.SendWarning("Done searching.");
			}
			*/
		}
#endif

		public static List<string> units = new List<string>{
		"CitizenFemale1",
"CitizenFemale6",
"CitizenMale1",
"CitizenMale2",
"CommonerFemale01",
"CR0_GundrunPeasantMale",
"CR4_Bandit_Human_NecromancerSosielq0",
"Elf_Citizen_Female",
"Vilenia",
"CR3_Crusader_Human_PaladinMelee_Male",
"CR7_Crusader_Human_PaladinMelee_cap",
"CrusadeSoldier",
"CrusadeSoldier2",
"CrusadeSoldier3F",
"CrusadeSoldierNearIrabeth",
"Megidiah_Retinue_commoner1",
"AeonQ3_CapitalGuard_01_Viram",
"AeonQ3_CapitalGuard_02_Erwart",
"CR0_Sarcorian_Guard_Male",
"CR0_Sarcorian_Guard_Male2",
"Gorvo",
"Kermel",
"Lellan",
"Nexus_PleasureSlaveMale",
"SlaveHorgus",
"CR1_Crusader_Human_Recruit_Melee_Male",
"Recruit_Speaker_1",
"Recruit_Speaker_2",
"StitchHalflingVIsionByAreelu",
"ColyphyrSlave_FemaleAzata",
"MysticSlave" };

		public static int index = 0;

		public static int index2 = 80;

		public static string NenioName = "Nenio";
		public static string ArueName = "Arueshalae";
		public static string FinneanName = "Finnean the Talking Weapon";
		public static string CiarName = "Ciar";
		public static string WoljifName = "Woljif";
		public static string GalfreyName = "Queen Galfrey";
		public static string StauntonName = "Staunton Vhane";

		public static string IrabethName = "Irabeth";

		public static string AstyName = "Asty";
		public static string TranName = "Tran";
		public static string VelhmName = "Velhm";

		public static string LannName = "Lann";
		public static string EmberName = "Ember";
		public static string PentaName = "Penta";
		public static string SeelahName = "Seelah";
		public static string DaeranName = "Daeran";
		public static string RegillName = "Regill";
		public static string SendriName = "Sendri";
		public static string UlbrigName = "Ulbrig";
		public static string TreverName = "Trever";
		public static string WenduagName = "Wenduag";
		public static string RekarthName = "Rekarth";
		public static string GreyborName = "Greybor";
		public static string CamelliaName = "Camellia";
		public static string DelamereName = "Delamere";
		public static string KestoglyrName = "Kestoglyr";
		public static string SosielName = "Sosiel";

		//lann cb29621d99b902e4da6f5d232352fbda
		//ember 2779754eecffd044fbd4842dba55312c
		//penta 942862a48f1644ae85ac5e3c9deb720c
		//seelah 54be53f0b35bf3c4592a97ae335fe765
		//daeran 096fc4a96d675bb45a0396bcaa7aa993
		//regill 0d37024170b172346b3769df92a971f5
		//sendri 561036c882a640089b1d42f03ebe3a6c
		//ulbrig 42f0d5ec3dc844feb44b04507a7c1bfc
		//trever 0bb1c03b9f7bbcf42bb74478af2c6258
		//wendu  ae766624c03058440a036de90a7f2009
		//rekarth 3e0014e4be454482a2797fd81123d7b4
		//greybor f72bb7c48bb3e45458f866045448fb58
		//camelia 397b090721c41044ea3220445300e1b8
		//delamere 6b1f599497f5cfa42853d095bda6dafd
		//kestoglyr e551850403d61eb48bb2de010d12c894
		//sosiel 1cbbbb892f93c3d439f8417ad7cbb6aa
		public static UnityModManager.ModEntry.ModLogger Logger;

		public static List<BlueprintBuff> buffs = new List<BlueprintBuff>();

		//public static IFxHandle fxHandle;
		public static bool dollRoomxtraFxBrightness = false;
		public static Dictionary<string, IFxHandle> fxHandles = new Dictionary<string, IFxHandle>();

		[HarmonyPatch(typeof(LoadingScreenPCView), "SetupLoadingArea")]
		public static class LoadingScreenPCView_Patch
		{
			private static bool Prefix(
				LoadingScreenPCView __instance, 
				BlueprintArea area, 
				GameObject ___m_MapContainer, 
				UnityEngine.UI.Image ___m_Picture, 
				TextMeshProUGUI ___m_CharacterNameText, 
				TextMeshProUGUI ___m_CharacterDescriptionText,
                UnityEngine.UI.Image ___m_CharacterPortrait,
				Sprite ___m_PrologueCaves1Sprite)
			{
				LocalizedString description;
				___m_MapContainer.SetActive((area != null ? area.IsGlobalMap : false));
				___m_Picture.gameObject.SetActive((area != null ? !area.IsGlobalMap : true));
				if (area == null)
				{
					___m_Picture.sprite = UIRoot.Instance.BlueprintLoadingScreenSpriteList.GetLoadingScreen(null);
					return false;
				}
				if (!area.IsGlobalMap)
				{
					___m_Picture.sprite = UIRoot.Instance.BlueprintLoadingScreenSpriteList.GetLoadingScreen(area);
				}
				else
				{
					/*
					List<UnitEntityData> tempList = 
						(from rc in Game.Instance.Player.AllCharacters
						where !rc.IsPet && !rc.IsCustomCompanion() 
						select rc).ToTempList<UnitEntityData>();

					tempList.AddRange(Game.Instance.Player.Party.Where<UnitEntityData>((UnitEntityData pc) => {
						if (pc.IsMainCharacter || pc.IsPet)
						{
							return true;
						}

						return !pc.IsCustomCompanion();
					}));*/

					List<UnitEntityData> tempList =
						(from rc in Game.Instance.Player.Party
						 where !rc.IsPet && !rc.IsCustomCompanion() && !rc.IsMainCharacter
						 select rc).ToTempList<UnitEntityData>();

					if (!tempList.Empty<UnitEntityData>())
					{
						/*UnitEntityData unitEntityData2 = null;
						foreach (UnitEntityData unitEntityData in Game.Instance.Player.AllCharacters)
						{
							if (unitEntityData.CharacterName.Equals("Nenio"))
							{
								unitEntityData2 = unitEntityData;
							}
						}*/
						UnitReference unitReference = tempList.Random<UnitEntityData>(); 
						BlueprintCompanionStory blueprintCompanionStory = Game.Instance.Player.CompanionStories.Get(unitReference.Value).LastOrDefault<BlueprintCompanionStory>();

						TextMeshProUGUI mCharacterNameText = ___m_CharacterNameText;
						string characterName = unitReference.Value.CharacterName;
						UnityEngine.Color color = new UnityEngine.Color();
						mCharacterNameText.text = UIUtility.GetSaberBookFormat(characterName, color, 140, null, 0f);

						TextMeshProUGUI mCharacterDescriptionText = ___m_CharacterDescriptionText;
						if (blueprintCompanionStory != null)
						{
							description = blueprintCompanionStory.Description;
						}
						else
						{
							description = null;
						}
						LocalizedString localizedString = description;
						mCharacterDescriptionText.text = (localizedString != null ? localizedString : string.Empty);

						/*BlueprintPortrait bp = Utilities.GetBlueprintByGuid<BlueprintPortrait>("a6e4ff25a8da46a44a24ecc5da296073");
						if (!unitReference.Value.Body.IsPolymorphed)
							//GetActivePolymorph().Component == null)
						{

							Main.DebugLog("fox");
							bp = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");
						}
						else
						{ Main.DebugLog("human"); }*/

	

						___m_CharacterPortrait.sprite = unitReference.Value.Portrait.FullLengthPortrait;
					}
					else
					{
						___m_MapContainer.SetActive(false);
						___m_Picture.gameObject.SetActive(true);
						___m_Picture.sprite = UIRoot.Instance.BlueprintLoadingScreenSpriteList.GetLoadingScreen(null);
					}
				}
				if (___m_Picture.sprite == null && area.name == "Prologue_Caves_1")
				{
					___m_Picture.sprite = ___m_PrologueCaves1Sprite;
				}
				if (___m_Picture.gameObject.activeInHierarchy && ___m_Picture.sprite == null)
				{
					___m_Picture.gameObject.SetActive(false);
				}


				return false;
			}
		}

		
		public static Settings settings;

		public static List<string> unitNames = new List<string>();

		public static void loctel()
		{
		/*	Vector3 worldPosition = Game.Instance.ClickEventsController.WorldPosition;
			TurnController currentTurn = Game.Instance.TurnBasedCombatController.CurrentTurn;
			List<UnitEntityData> selectedUnits = Game.Instance.UI.SelectionManager.SelectedUnits;
			if (currentTurn != null)
			{
				selectedUnits = new List<UnitEntityData>()
				{
					currentTurn.Rider
				};
				if (currentTurn.Mount != null)
				{
					selectedUnits.Add(currentTurn.Mount);
				}
			}
			foreach (UnitEntityData selectedUnit in selectedUnits)
			{
				selectedUnit.Commands.InterruptAll(true);
				selectedUnit.Position = worldPosition;
			}*/
		}



		public static BlueprintList blueprintList = new BlueprintList()
		{
			Entries = new List<BlueprintList.Entry>()
		};

		//public static List<BlueprintBuff> bpbuffs = new List<BlueprintBuff>();

		public static ModEntry ModEntry;

		public static bool areaLoaded = false;
	

		public static int portraitCounter = 0;
		public static int failCounter = 0;

		public static void saveNpcPortraits()
		{


			string str = BundlesLoadService.BundlesPath("cheatdata.json");
			BlueprintList blueprintList = new BlueprintList()
			{
				Entries = new List<BlueprintList.Entry>()
			};

			if (File.Exists(str))
				 blueprintList = JsonUtility.FromJson<BlueprintList>(File.ReadAllText(str));


			Dictionary<string, string> speakers = new Dictionary<string, string>();


			string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();

			//List<string> fyou = new List<string>();

			//fyou.Add("Darrazand");
			//fyou.Add("DLC3_ShadowBalorNahindry_CUTSCENE");

			

			foreach (BlueprintList.Entry be in blueprintList.Entries)
			{




				if (be.TypeFullName.Equals("Kingmaker.DialogSystem.Blueprints.BlueprintCue"))
				{

					BlueprintCue bc = Utilities.GetBlueprintByGuid<BlueprintCue>(be.Guid);

					string characterName = "";


					if (bc.Speaker != null && bc.Speaker.Blueprint != null && !bc.Speaker.Blueprint.name.IsNullOrEmpty())
					{
						characterName = bc.Speaker.Blueprint.CharacterName;
						if (bc.Speaker.Blueprint.CharacterName.Equals(AstyName))
						{
							Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, AstyName));
							characterName = AstyName+" - Drow";
						}
						if (bc.Speaker.Blueprint.CharacterName.Equals(TranName))
						{
							Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, TranName));
							characterName = TranName+" - Drow";
						}
						if (bc.Speaker.Blueprint.CharacterName.Equals(VelhmName))
						{
							Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, VelhmName));
							characterName = VelhmName+" - Drow";
						}
						if (bc.Speaker.Blueprint.CharacterName.Equals(IrabethName))
						{
							Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, IrabethName));
							characterName = IrabethName + " - Scar";
						}




						if (!speakers.ContainsKey(bc.Speaker.Blueprint.name) && !companions.Contains(bc.Speaker.Blueprint.CharacterName))
						{
							if (!bc.Speaker.Blueprint.IsCompanion /* && !fyou.Contains(bc.Speaker.Blueprint.name)*/)
							{
								try
								{


									speakers.Add(bc.Speaker.Blueprint.name, characterName);



									string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, characterName);


									if (bc.Speaker.Blueprint.name != bc.Speaker.Blueprint.CharacterName)
										SaveOriginals(bc.Speaker.Blueprint, Path.Combine(portraitDirectoryPath, bc.Speaker.Blueprint.name));
									else
										SaveOriginals(bc.Speaker.Blueprint, portraitDirectoryPath);


									portraitCounter++;
								}
								catch (Exception e)
								{
									failCounter++;
									//Main.DebugLog("speaker fail: " + bc.Speaker.Blueprint.CharacterName + " - " + bc.Speaker.Blueprint.name);

									Main.DebugError(e);


								}
							}
						}
					}


					if (bc.Listener != null && !bc.Listener.name.IsNullOrEmpty() )
					{
						try
						{

							characterName = bc.Listener.CharacterName;
							if (bc.Listener.CharacterName.Equals(AstyName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, AstyName));
								characterName = AstyName + " - Drow";
							}
							if (bc.Listener.CharacterName.Equals(TranName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, TranName));
								characterName = TranName + " - Drow";
							}
							if (bc.Listener.CharacterName.Equals(VelhmName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, VelhmName));
								characterName = VelhmName + " - Drow";
							}
							if (bc.Listener.CharacterName.Equals(IrabethName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, IrabethName));
								characterName = IrabethName + " - Scar";
							}

							if (!speakers.ContainsKey(bc.Listener.name) && !companions.Contains(bc.Listener.CharacterName) && !bc.Listener.IsCompanion /*&& !fyou.Contains(bc.Listener.name)*/)
							{

								speakers.Add(bc.Listener.name, characterName);
								string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, characterName);



								if (bc.Listener.name != bc.Listener.CharacterName)
								SaveOriginals(bc.Listener, Path.Combine(portraitDirectoryPath, bc.Listener.name));
								else
									SaveOriginals(bc.Listener, portraitDirectoryPath);

								portraitCounter++;
							}
						}
						catch(Exception e)
						{
							failCounter++;
							//Main.DebugLog("listener fail: " + bc.Listener.CharacterName + " - " + bc.Listener.name);

							Main.DebugError(e);


						}
					}

				}
			}
			Main.DebugLog(speakers.Count().ToString());

		}


		public static void makeNpcDirs()
		{


			string str = BundlesLoadService.BundlesPath("cheatdata.json");
			BlueprintList blueprintList = new BlueprintList()
			{
				Entries = new List<BlueprintList.Entry>()
			};

			if (File.Exists(str))
				blueprintList = JsonUtility.FromJson<BlueprintList>(File.ReadAllText(str));


			Dictionary<string, string> speakers = new Dictionary<string, string>();


			string portraitsDirectoryPath = Main.GetNpcPortraitsDirectory();

			foreach (BlueprintList.Entry be in blueprintList.Entries)
			{
				try
				{
#if DEBUG
					/*
					if (be.TypeFullName.EndsWith("BlueprintFeature"))
					{
						BlueprintFeature bc = Utilities.GetBlueprintByGuid<BlueprintFeature>(be.Guid);
						if (bc.name.ToLower().Contains("devil"))
						{
							Main.DebugLog(bc.name);
							Main.DebugLog(bc.AssetGuid.ToString());

						}
					}
					*/
					/*if (be.TypeFullName.EndsWith("BlueprintUnit"))
					{
						BlueprintUnit bu = Utilities.GetBlueprintByGuid<BlueprintUnit>(be.Guid);
						if (bu.PortraitSafe.name.ToLower().Contains("staunton"))
						{
							Main.DebugLog(bu.CharacterName + " - " + bu.name + " - Companion: " + bu.IsCompanion.ToString() + " - " + bu.Alignment + " - " + bu.PortraitSafe.name);

							//string prefix = Main.GetCompanionPortraitDirPrefix();
							string portraitsDirectoryPath2 = Main.GetCompanionPortraitsDirectory();
							string portraitDirectoryName = bu.name;
							string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath2, "--", portraitDirectoryName);

							//	SaveOriginals2(bc.Data, portraitDirectoryPath);
							//SaveOriginals(bu, portraitDirectoryPath);
							//savedComp = true;
							//break;
						}
					}*/
/*
if (be.TypeFullName.Contains("BlueprintPortrait"))
{
	BlueprintPortrait bc = Utilities.GetBlueprintByGuid<BlueprintPortrait>(be.Guid);
	if (bc.name.ToLower().Contains("kestoglyr"))
	{
		Main.DebugLog(bc.name);
		Main.DebugLog(bc.AssetGuid.ToString());

		string portraitsDirectoryPath2 = Main.GetCompanionPortraitsDirectory();
		string portraitDirectoryName = bc.name;
		string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath2, "--", portraitDirectoryName +" - "+ be.Guid);
SaveOriginals2(bc.Data, portraitDirectoryPath);
	}
}
*/

/*

*/
/*
[CustomNpcPortraits] ArueshalaeEvil_Portrait
[CustomNpcPortraits] 484588d56f2c2894ab6d48b91509f5e3
[CustomNpcPortraits] Arueshalae_Portrait
[CustomNpcPortraits] db413e67276547b40b1a6bb8178c6951
						blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Blueprint);

*/

/*
if(be.TypeFullName.Contains("BlueprintUnlockableFlag"))


{
	BlueprintUnlockableFlag bc = Utilities.GetBlueprintByGuid<BlueprintUnlockableFlag>(be.Guid);
	if (bc.name.ToLower().Contains("bald"))
	{
		Main.DebugLog(bc.name);
		Main.DebugLog(bc.AssetGuid.ToString());



	}
}
*/

					if (be.TypeFullName.Equals("Kingmaker.DialogSystem.Blueprints.BlueprintCue"))
										{

											BlueprintCue bc = Utilities.GetBlueprintByGuid<BlueprintCue>(be.Guid);
											if (bc.Speaker != null && bc.Speaker.Blueprint != null && !bc.Speaker.Blueprint.name.IsNullOrEmpty() && !speakers.ContainsKey(bc.Speaker.Blueprint.name) && !companions.Contains(bc.Speaker.Blueprint.CharacterName))
											{
												//characterName = bc.Listener.CharacterName;
												string portraitDirectoryName = bc.Speaker.Blueprint.CharacterName;


							if (bc.Speaker.Blueprint.CharacterName.Equals(AstyName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, AstyName));
								portraitDirectoryName = AstyName + " - Drow";
							}
							if (bc.Speaker.Blueprint.CharacterName.Equals(TranName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, TranName));
								portraitDirectoryName = TranName + " - Drow";
							}
							if (bc.Speaker.Blueprint.CharacterName.Equals(VelhmName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, VelhmName));
								portraitDirectoryName = VelhmName + " - Drow";
							}
							if (bc.Speaker.Blueprint.CharacterName.Equals(IrabethName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, IrabethName));
								portraitDirectoryName = IrabethName + " - Scar";
							}

							if (!bc.Speaker.Blueprint.IsCompanion 
										)
												{



													speakers.Add(bc.Speaker.Blueprint.name, bc.Speaker.Blueprint.CharacterName);

													//Main.DebugLog(speakers[bc.Speaker.Blueprint.name]);
													string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

													if (bc.Speaker.Blueprint.name != bc.Speaker.Blueprint.CharacterName)

														Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, bc.Speaker.Blueprint.name+" - "+bc.Speaker.Blueprint.Prefab.AssetId));
													else
														Directory.CreateDirectory(portraitDirectoryPath);



												}
											}
											if (bc.Listener != null && !bc.Listener.name.IsNullOrEmpty() && !speakers.ContainsKey(bc.Listener.name) && !companions.Contains(bc.Listener.CharacterName))
											{
												string portraitDirectoryName = bc.Listener.CharacterName;

							if (bc.Listener.CharacterName.Equals(AstyName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, AstyName));
								portraitDirectoryName = AstyName + " - Drow";
							}
							if (bc.Listener.CharacterName.Equals(TranName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, TranName));
								portraitDirectoryName = TranName + " - Drow";
							}
							if (bc.Listener.CharacterName.Equals(VelhmName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, VelhmName));
								portraitDirectoryName = VelhmName + " - Drow";
							}
							if (bc.Listener.CharacterName.Equals(IrabethName))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, IrabethName));
								portraitDirectoryName = IrabethName + " - Scar";
							}

							if (!bc.Listener.IsCompanion)
												{

													speakers.Add(bc.Listener.name, bc.Listener.CharacterName);
													//Main.DebugLog(bc.Listener.CharacterName);
													//string portraitDirectoryName = bc.Listener.CharacterName;
													string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

													if (bc.Listener.name != bc.Listener.CharacterName)

														Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, bc.Listener.name + " - " + bc.Listener.Prefab.AssetId));
													else
														Directory.CreateDirectory(portraitDirectoryPath);


												}
											}

										}

#endif
				}
				catch (Exception e)
				{
					Main.DebugError(e);
				}
			}

		}
		/*
		//private static bool onUpdateCalled = false;
		public static void Init(UnityModManager.ModEntry modEntry, float dt)
		{
			//modEntry.OnUpdate = null;

			//|| (ResourcesLibrary.LibraryObject.Root == null) ||(ResourcesLibrary.LibraryObject.BlueprintsByAssetId == null) || (ResourcesLibrary.LibraryObject.ResourceNamesByAssetId == null)
			//if (!Application.isPlaying || (LocalizationManager.CurrentPack == null) || (ResourcesLibrary.LoadedResources == null))
			//if(SceneManager.GetActive!Application.isPlayingScene().name != "UI_MainMenu_Scene")
			if (!Application.isPlaying || (LocalizationManager.CurrentPack == null) || (SceneManager.GetActiveScene().name != "UI_MainMenu_Scene") || (ResourcesLibrary.LoadedResources == null))
			{
				modEntry.OnUpdate = null;

				modEntry.OnUpdate = new Action<UnityModManager.ModEntry, float>(Main.Init);

				return;
			}


		//	smallName = BlueprintRoot.Instance.CharGen.PortraitSmallName + BlueprintRoot.Instance.CharGen.PortraitsFormat;
		//	mediumName = BlueprintRoot.Instance.CharGen.PortraitMediumName + BlueprintRoot.Instance.CharGen.PortraitsFormat;
		//	fullName = BlueprintRoot.Instance.CharGen.PortraitBigName + BlueprintRoot.Instance.CharGen.PortraitsFormat;


		}
		*/
		public static GameModeType prevMode = GameModeType.None;
		public static bool savedNpc = false;
		public static bool savedComp = false;
		public static int i = -1;
		public static int j = -1;
		public static string npcDirlabel = "";
		public static string npcSubDirlabel = "";
		public static string compDirlabel = "";
		public static IEnumerable<T> MoveUp<T>(this IEnumerable<T> enumerable, int itemIndex)
		{
			int i = 0;

			IEnumerator<T> enumerator = enumerable.GetEnumerator();
			while (enumerator.MoveNext())
			{
				i++;

				if (itemIndex.Equals(i))
				{
					T previous = enumerator.Current;

					if (enumerator.MoveNext())
					{
						yield return enumerator.Current;
					}

					yield return previous;

					break;
				}

				yield return enumerator.Current;
			}

			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}

		public static int k = 400;
		public static BlueprintBuff lastbb;

		public static IFxHandle ifxh;


		private static void refreshPortraits()
		{

			foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.AllCharacters)
			{
				//EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityDatum.Descriptor), true);

				//unitEntityDatum.Descriptor.UISettings.SetPortrait(SetPortrait(unitEntityDatum));
				Kingmaker.Game.Instance.SelectionCharacter.SetSelected(unitEntityDatum);
					Kingmaker.Game.Instance.SelectionCharacter.SelectedUnit.Value.Value.UISettings.SetPortrait(SetPortrait(unitEntityDatum));
			}
		}


		public static string GetCompanionDirName(string characterName)
        {



			UnitEntityData unitEntityData = Game.Instance.Player.AllCharacters.FirstOrDefault(c => c.CharacterName.Equals(characterName));

			if (unitEntityData != null)
			{
				string charcterNameDirectoryName = unitEntityData.CharacterName.cleanCharname();
				//	Main.DebugLog("1 " + charcterNameDirectoryName);
				if (charcterNameDirectoryName.Equals(ArueName))
				{
					//if (unitEntityData.Blueprint.Race.name.ToLower().Contains("succubusrace"))
					if (unitEntityData.Descriptor.Progression.Race.name.ToLower().Contains("succubusrace"))
					{

						charcterNameDirectoryName = charcterNameDirectoryName+" - Evil";


					}
					//Main.DebugLog(unitEntityData.View.Blueprint.Race.name);
				}
				//Main.DebugLog("2");
				if (charcterNameDirectoryName.Equals(CiarName))
				{
					if (unitEntityData.Alignment.ValueRaw.ToString().ToLower().Contains("evil") || unitEntityData.Descriptor.IsUndead)

						//if (unitEntityData.Blueprint.Alignment.ToString().ToLower().Contains("evil") || unitEntityData.Descriptor.IsUndead)
						charcterNameDirectoryName = charcterNameDirectoryName+" - Undead";

				}
				//Main.DebugLog("3");
				if (charcterNameDirectoryName.Equals(GalfreyName))
				{
					if (unitEntityData.Blueprint.Alignment.ToString().ToLower().Contains("evil") || unitEntityData.Descriptor.IsUndead)
						charcterNameDirectoryName = charcterNameDirectoryName + " - Undead";

					//Galfrey old
					if (Game.Instance.Player.EtudesSystem.EtudeIsStarted(ResourcesLibrary.TryGetBlueprint<BlueprintEtude>("0ab81986d8f7c484a8fede0d2188125c")))
					{
						charcterNameDirectoryName = charcterNameDirectoryName + " - Old";
					}


				}
				//Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.Alignment == Kingmaker.Enums.Alignment.LawfulEvil
				//Main.DebugLog("4");
				if (charcterNameDirectoryName.Equals(StauntonName))
				{
					if (unitEntityData.Blueprint.Alignment.ToString().ToLower().Contains("evil") || unitEntityData.Descriptor.IsUndead)
						charcterNameDirectoryName = charcterNameDirectoryName + " - Undead";

				}



				//Woljif demon
				if (charcterNameDirectoryName.Equals(WoljifName))
				if (Game.Instance.Player.EtudesSystem.EtudeIsStarted(ResourcesLibrary.TryGetBlueprint<BlueprintEtude>("709161cd9da156146ac6e3c394caa854")))
				{
						charcterNameDirectoryName = charcterNameDirectoryName + " - Demon";
				}

				//AscendingSuccubus
				//Main.DebugLog("5");

				//string characterName = unitEntityData.CharacterName.cleanCharname();
				if (charcterNameDirectoryName.Equals(Main.NenioName))
				{


					//UIUtility.SendWarning("huh");
					object campaign;
					SaveInfo loadingSave = Game.Instance.LoadingSave;
					if (loadingSave != null)
					{
						campaign = loadingSave.Campaign;
					}
					else
					{
						campaign = null;
					}
					if (campaign == null)
					{
						campaign = Game.Instance.Player.Campaign;
					}
					BlueprintCampaign blueprintCampaign = (BlueprintCampaign)campaign;
					if ((blueprintCampaign != null && !blueprintCampaign.name.IsNullOrEmpty() && !blueprintCampaign.name.Equals("MainCampaign")) || Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys.FirstOrDefault(x => x.name.Contains("Fox")))
					{

						//foreach (UnitEntityData ud in Game.Instance.State.PlayerState.AllCharacters)
						//{


						//if (ud.CharacterName.Equals("Nenio"))
						//{
						if (!unitEntityData.Body.IsPolymorphed)
						{

							// if (Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component == null)



							//Main.DebugLog("fox");
							charcterNameDirectoryName = charcterNameDirectoryName + "Fox_Portrait";

						}

						//}
						//}
					}
					/*
						if (Game.Instance.DialogController != null &&
		Game.Instance.DialogController.CurrentCue != null &&
		Game.Instance.DialogController.CurrentCue.AssetGuid != null &&
		Game.Instance.DialogController.CurrentCue.AssetGuid.ToString().Equals("45450b2f327797e41bce701b91118cb4"))
						characterName = "NenioFox_Portrait";
					}
					*/

				}

				return charcterNameDirectoryName;
			}
			else
				return characterName;

		}

		/*
		public static void NenioToggle()
        {

			foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
			{
				if (unitEntityDatum.CharacterName == "Nenio")
				{
					
					//foreach (var abi in unitEntityDatum.Facts.m_Facts)
					//{
					//Main.DebugLog(abi.Name +" - "+abi.GetType() +" - "+abi.Blueprint.name +" - "+abi.Blueprint.AssetGuid);
					//}
					//Change Shape - Kingmaker.UnitLogic.ActivatableAbilities.ActivatableAbility - ChangeShapeKitsuneToggleAbility_Nenio - 52bed4c5617625e4faf029b5c750667f

					Main.DebugLog("toggle 1");
					var shapeToggle = ResourcesLibrary.TryGetBlueprint<BlueprintActivatableAbility>("52bed4c5617625e4faf029b5c750667f");

					Main.DebugLog("toggle "+ shapeToggle.name);
					//give

					if (unitEntityDatum.HasFact(shapeToggle))
					{
						Main.DebugLog("toggle 3");
						//UIUtility.SendWarning(unitEntityDatum.GetFact(LightHaloToggle).Name);
						if (unitEntityDatum.Body.IsPolymorphed)
						{
							Main.DebugLog("toggle 4");
						//	unitEntityDatum.GetFact<ActivatableAbility>(shapeToggle).IsOn = false;
							unitEntityDatum.GetFact<ActivatableAbility>(shapeToggle).IsOn = true;
						}
						
					}
					break;
				}

			}
		}*/



		public static UnitEntityData RealCurrentSpeakerEntity(string characterName)
        {

			foreach(UnitEntityData unit in Game.Instance.DialogController.InvolvedUnits)
            {
				if(unit.CharacterName.Equals(characterName))
                {
					return unit;
                }

            }



			return null;
        }

		public static string npcCycle = "";
		public static string npcSubCycle = "";
		public static string compCycle = "";
		
		//private enum State { Start, Update };

		//private static State state;

		private static bool isDialog = false;

		private static bool isLoadedGame = false;

		private static bool showExperimental = false;

#if DEBUG
		private static bool showPatched = false;
#endif
		private static bool showExtra = false;



		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

		private static void OnHideGUI(UnityModManager.ModEntry modEntry)
		{
			Main.settings.Save(modEntry);
		}

		public static bool enabled;
		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			enabled = value;
			if (enabled)
			{
				settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
				Main.DebugLog("Mod is enabled");
			}
			else
			{
				Main.DebugLog("Mod is disabled");
				settings.Save(modEntry);
			}
			return true;

			/*
            if (!Main.enabled)
                return;
			 */
		}



		public static void SetArmyPortraits()
        {


			GlobalMapState state2 = Game.Instance.Player.GlobalMap.LastActivated;

			var dirs = Directory.GetDirectories(Main.GetArmyPortraitsDirectory());

			// if (dirs.Contains(characterName))
			//  {}


			/*

			for (int i = 0; i < state2.Armies.Count; i++)
			{
				GlobalMapArmyState item = state2.Armies[i];
				if (!item.Data.Squads.Empty<SquadState>() && !item.IsGarrison)
				{
					Main.DebugLog("----------------------NAME: " + item.Data.ArmyName);
					foreach (SquadState s in item.Data.Squads)
					{
						string characterName = s.Unit.CharacterName;
						string portraitDirectoryName = characterName;
						string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);
						Main.DebugLog("squad: " + characterName + " - "+ s.Unit.name);

						if (dirs.Contains(characterName))
			            {

							PortraitData Data = new PortraitData(portraitDirectoryPath);
							BlueprintPortrait bp = new BlueprintPortrait();

							bp.Data = Data;

							s.Unit.PortraitSafe = bp;

							//Type baseType = typeof(BlueprintUnit).BaseType;
							//baseType.GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(s.Unit, bp);
							//this.m_Portrait = value.ToReference<BlueprintPortraitReference>();
						}



					}
				}
			}

		*/
			/*
			foreach (ArmyLeader ud in Game.Instance.Player.ArmyLeadersManager.Leaders.ToList<ArmyLeader>())
			{
				string characterName = ud.LocalizedName;
				string portraitDirectoryName = characterName;
				string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);
				PortraitData Data = new PortraitData(portraitDirectoryPath);
				BlueprintPortrait bp = new BlueprintPortrait();

				bp.Data = Data;

				ud.Blueprint.Unit.PortraitSafe = bp;

				Main.DebugLog("ud: " + ud.Blueprint.Unit.CharacterName + " - " + ud.Blueprint.LeaderName + " - " + ud.Blueprint.name);

			}
			
			*/

		}


		public static string GetNpcPortraitsDirectory()
		{
			string dir = Path.GetFullPath(Path.Combine(CustomPortraitsManager.PortraitsRootFolderPath, @"..\"));

			dir = Path.Combine(dir, NpcPortraitsDirName());

			Directory.CreateDirectory(dir);
			return dir;

		}

		public static string GetArmyPortraitsDirectory()
		{
			string dir = Path.GetFullPath(Path.Combine(CustomPortraitsManager.PortraitsRootFolderPath, @"..\"));

			dir = Path.Combine(dir, ArmyPortraitsDirName());

			Directory.CreateDirectory(dir);
			return dir;

		}


		public static string GetCompanionPortraitsDirectory()
		{
			
			return Path.GetFullPath(CustomPortraitsManager.PortraitsRootFolderPath);

		}

		public static string NpcPortraitsDirName()
		{

			return "Portraits - Npc";

		}

		public static string ArmyPortraitsDirName()
		{

			return "Portraits - Army";

		}
		public static string GetCompanionPortraitDirPrefix()
		{
	
			return "CustomNpcPortraits - ";

		}
		public static string GetDefaultPortraitsDirName()
		{

			return "Backup of Game Default Portraits";

		}
		



		public static string smallName = "Small.png";
		public static string mediumName = "Medium.png";
		public static string fullName = "Fulllength.png";

		public static string[] PortraitFileNames = new string[]
			{
				Main.smallName,
				Main.mediumName,
				Main.fullName
	};

		public static void DebugLog(string msg)
		{
#if DEBUG
		if (logger != null) logger.Log(msg);
#endif
		}
		public static void Log(string msg)
		{
			if (logger != null) logger.Log(msg);
		}

		public static void DebugError(Exception ex)
		{
			if (logger != null) logger.Log(ex.ToString() + "\n" + ex.StackTrace);
		}

		internal static Exception Error(string message)
		{
			// modEntry?.Logger?.Log(message);
			return new InvalidOperationException(message);
		}

		public static UnityModManagerNet.UnityModManager.ModEntry.ModLogger logger;

		

		private static readonly List<string> okLoading = new List<string>();
		private static readonly List<string> failedLoading = new List<string>();
		internal static void SafeLoad(Action load, string name)
		{
			try
			{
				load();
				Main.okLoading.Add(name);
			}
			catch (Exception e)
			{
				Main.okLoading.Remove(name);
				Main.failedLoading.Add(name);
				Main.DebugLog(e.ToString());
			}
		}


		private static HarmonyInstance harmonyInstance;

		private static readonly Dictionary<Type, bool> typesPatched = new Dictionary<Type, bool>();
		private static readonly List<string> failedPatches = new List<string>();
		private static readonly List<string> okPatches = new List<string>();
		internal static bool ApplyPatch(Type type, string featureName)
		{
			bool result;
			try
			{
				if (Main.typesPatched.ContainsKey(type))
				{
					result = Main.typesPatched[type];
				}
				else
				{
					List<HarmonyMethod> harmonyMethods = type.GetHarmonyMethods();
					if (harmonyMethods == null || harmonyMethods.Count<HarmonyMethod>() == 0)
					{

						Main.DebugLog("Failed to apply patch " + featureName + ": could not find Harmony attributes.");
						Main.failedPatches.Add(featureName);
						Main.typesPatched.Add(type, false);
						result = false;
					}
					else if (new PatchProcessor(Main.harmonyInstance, type, HarmonyMethod.Merge(harmonyMethods)).Patch().FirstOrDefault<DynamicMethod>() == null)
					{
						Main.DebugLog("Failed to apply patch " + featureName + ": no dynamic method generated");

						Main.failedPatches.Add(featureName);
						Main.typesPatched.Add(type, false);
						result = false;
					}
					else
					{
						Main.okPatches.Add(featureName);
						Main.typesPatched.Add(type, true);
						result = true;
					}
				}
			}
			catch (Exception arg)
			{
				Main.DebugLog("Failed to apply patch " + featureName + ": " + arg + ", type: " + type);
				Main.failedPatches.Add(featureName);
				Main.typesPatched.Add(type, false);
				result = false;
			}
			return result;
		}



#if DEBUG
		// Token: 0x06000026 RID: 38 RVA: 0x000021B3 File Offset: 0x000003B3
		private static bool Unload(UnityModManager.ModEntry modEntry)
		{


			harmonyInstance.UnpatchAll(modEntry.Info.Id);

			UnityModManager.Logger.Clear();

			if (Main.okPatches.Count > 0)
			{
				settings.Save(modEntry);




				foreach (string str3 in Main.okPatches)
					{
						Main.DebugLog( str3 + " (patch unloaded)");
					}

				//HarmonyInstance.Create(modEntry.Info.Id).UnpatchAll(null);
				return true;
			}
			else { Main.DebugLog("couldn't find patches to unload!"); return true; }
		}
#endif
		public static List<string> companions;


		public static bool isSetPortrait = false;
		public static PortraitData SetPortrait(UnitEntityData unitEntityData)
		{
			//Main.DebugLog("SetPortrait(!!!!!!!) "+ unitEntityData.CharacterName);

			try
			{
				string prefix = Main.GetCompanionPortraitDirPrefix();
				string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
				
				
				
				
				string characterName = Main.GetCompanionDirName(unitEntityData.CharacterName);





			//	Main.DebugLog("SetPortrait() 5");
				string portraitDirectoryName = prefix + characterName;

				string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

				Directory.CreateDirectory(portraitDirectoryPath);



		//		Main.DebugLog("SetPortrait() 6");


				//Main.DebugLog("SetPortrait() 1");



				BlueprintPortrait blueprintPortrait  = (BlueprintPortrait)typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Descriptor.UISettings);

				BlueprintPortrait oldBp = blueprintPortrait;
	//				Main.DebugLog("SetPortrait() 7");

				if (blueprintPortrait != null)
					if (Main.settings.AutoBackup)
						if (!File.Exists(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName(), "Medium.png")))
							SaveOriginals2(blueprintPortrait.Data, portraitDirectoryPath);
				//if (blueprintPortrait != null)
				//if (Main.settings.AutoBackup) SaveOriginals(unitEntityData.Blueprint, portraitDirectoryPath);
				//Main.DebugLog(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name, "Medium.png"));
				//bool useSetPortrait = true;

			if(Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name != null && Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name.Length > 2 )
				 if( !Directory.Exists(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name)) &&
					(bool)Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.CharacterName.Equals(unitEntityData.CharacterName)
					
					)
					Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name));

				if (unitEntityData.Body.IsPolymorphed && !unitEntityData.CharacterName.Equals(Main.NenioName))
				{
					if (Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name != null && Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name.Length > 2)
					{
					   if((bool)Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.CharacterName.Equals(unitEntityData.CharacterName))

					   
						{
							if (!Directory.Exists(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController?.CurrentSpeaker?.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name)))
								Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name));
							if(!File.Exists(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name, GetDefaultPortraitsDirName(), "Medium.png")))
								if(Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component?.m_Portrait!=null)
							SaveOriginals2((Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component.m_Portrait.GetBlueprint() as BlueprintPortrait).Data, Path.Combine(portraitDirectoryPath, Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name));
						}
					}
				}

				if (unitEntityData.Body.IsPolymorphed && !unitEntityData.CharacterName.Equals(Main.NenioName) &&


					Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name != null && 
					Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name.Length > 2 &&
					File.Exists(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController?.CurrentSpeaker?.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name,"Medium.png")) &&

					(bool)Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.CharacterName.Equals(unitEntityData.CharacterName)
					)
				{
					


							portraitDirectoryPath = Path.Combine(portraitDirectoryPath, Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Runtime?.SourceBlueprintComponent?.OwnerBlueprint?.name);
					


				}
				else if (Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name != null && Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name.Length > 2 &&
						File.Exists(Path.Combine(portraitDirectoryPath, Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name, "Medium.png")))
				{
					portraitDirectoryPath = Path.Combine(portraitDirectoryPath, Game.Instance.DialogController?.CurrentSpeaker?.Blueprint?.name);
					//useSetPortrait = false;
				}
				else if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
				{
					string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

					string dir = Path.GetFileNameWithoutExtension(dirs[0]);
					//Main.DebugLog(dir);
					if (!dir.Equals("root"))
						portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName, dir);
					//Main.DebugLog(portraitDirectoryPath);
				}
				else
				{
					if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
						if (File.Exists(Path.Combine(portraitDirectoryPath, GetDefaultPortraitsDirName(), "Medium.png")))
						{

							portraitDirectoryPath = Path.Combine(portraitDirectoryPath, GetDefaultPortraitsDirName());


						}
				}

				bool missing = false;

				//Main.DebugLog("SetPortrait() 2b");

				foreach (string fileName in Main.PortraitFileNames)
				{
					if (characterName != "Ciar" && !fileName.Contains("Fullength"))

						if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
					{
						missing = true;
					}
				}

				//Main.DebugLog("SetPortrait() 3");

				if (!missing)
				{
					//Main.DebugLog("SetPortrait() 4");
					/*
					if (blueprintPortrait != null)
					{
						//Main.DebugLog("SetPortrait() 5");

						BlueprintPortrait oldP = blueprintPortrait;
						Main.pauseGetPortraitsafe = true;
						blueprintPortrait.Data.m_PetEyeImage = oldP.Data.m_PetEyeImage;
						Main.pauseGetPortraitsafe = false;
					}*/
					
					//blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;
					//	Main.DebugLog("SetPortrait() 6");

					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));

				//	Main.DebugLog("SetPortrait() 7 " + portraitDirectoryPath);

					PortraitData data = /*PortraitLoader.LoadPortraitData(portraitDirectoryPath);*/ new PortraitData(portraitDirectoryPath);


					if (blueprintPortrait == null)
					{
						Main.pauseGetPortraitsafe = true;

						data.m_PetEyeImage = unitEntityData.Descriptor.Blueprint.PortraitSafe.Data.m_PetEyeImage;
						Main.pauseGetPortraitsafe = false;

					}
					else
					data.m_PetEyeImage = blueprintPortrait.Data.m_PetEyeImage;

					//		Main.DebugLog("SetPortrait() 8");

					typeof(UnitUISettings).GetField("m_CustomPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, data);

					//typeof(string).GetField("m_CustomPortraitId", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, portraitDirectoryPath);

					typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, null);

					return data;

					//isSetPortrait = true;
					//unitEntityData.Descriptor.UISettings.SetPortrait(blueprintPortrait);
					//isSetPortrait = false;
					//return;

				}
				else
				{

					if (blueprintPortrait != null)
					{
					}
					else
					{
						blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Blueprint);
						//Main.pauseGetPortraitsafe = true;
						//blueprintPortrait = unitEntityData.Blueprint.PortraitSafe;
						//Main.pauseGetPortraitsafe = false;
						if (blueprintPortrait != null)
						{

						}
						else
						{
							if (unitEntityData.Gender == Gender.Male)
							{
								//	Main.DebugLog("male");

								blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("621ada02d0b4bf64387babad3a53067b");

							}
							else
							{
								//Main.DebugLog("female");
								//blueprintPortrait = uIRoot.LeaderPlaceholderPortrait;
								blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("9fe4f89ecf15b874db9d1d2bf3ef33d2");

							}


						}
					}

					if (blueprintPortrait.BackupPortrait != null)
						blueprintPortrait = blueprintPortrait.BackupPortrait;


					if (characterName.Equals(Main.NenioName))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("a6e4ff25a8da46a44a24ecc5da296073");
					if (characterName.Equals(Main.NenioName+"Fox_Portrait"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");

					if (characterName.Equals(Main.ArueName))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("db413e67276547b40b1a6bb8178c6951");
					if (characterName.Equals(Main.ArueName+" - Evil"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("484588d56f2c2894ab6d48b91509f5e3");

					if (characterName.Equals(Main.GalfreyName))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("a3ba06b4723c7a74fb5054ccb2289efb");
					if (characterName.Equals(Main.GalfreyName + " - Undead"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("767456b1656ca064dadac544d39d7e40");
					if (characterName.Equals(Main.GalfreyName + " - Old"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("4e8dfb75015d356469b976145c851087");
					
					if (characterName.Equals(Main.WoljifName+" - Demon"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("f2bd5a94ade889444921b4a74b9e34a9");


					
					if (characterName.Equals(Main.CiarName+" - Undead"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("dc2f02dd42cfe2b40923eb014591a009");

					if (characterName.Equals(Main.StauntonName+" - Undead"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("f4bbe08217bcaa54c91fe73bcea70ede");

					PortraitData data = blueprintPortrait.Data;

					Main.pauseGetPortraitsafe = true;
					data.m_PetEyeImage = unitEntityData.Descriptor.Blueprint.PortraitSafe.Data.m_PetEyeImage;
					blueprintPortrait.Data.m_PetEyeImage = unitEntityData.Descriptor.Blueprint.PortraitSafe.Data.m_PetEyeImage;
					Main.pauseGetPortraitsafe = false;
					//	Main.DebugLog("SetPortrait() no custom portrait");

					unitEntityData.UISettings.m_CustomPortrait = null;
					//typeof(UnitUISettings).GetField("m_CustomPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, null);
					//typeof(string).GetField("m_CustomPortraitId", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, "");

					unitEntityData.UISettings.m_Portrait = blueprintPortrait;

					unitEntityData.UISettings.m_Portrait.Data.m_CustomPortraitId = "";


					return data;
					
					
					
					//isSetPortrait = true;
					//unitEntityData.Descriptor.UISettings.SetPortrait(blueprintPortrait);
					//isSetPortrait = false;
					//return;
				}
			}
			catch(Exception e)
            {

				Main.DebugError(e);
				return null;
            }
		}

		public static void ArueAddHalo()
		{

			if (Game.Instance.Player.MainCharacter.Value != null &&
				Game.Instance.Player.MainCharacter.Value.Progression != null &&
				Game.Instance.Player.MainCharacter.Value.Progression.GetCurrentMythicClass() != null &&
				Game.Instance.Player.MainCharacter.Value.Progression.GetCurrentMythicClass().CharacterClass != null &&
				Game.Instance.Player.MainCharacter.Value.Progression.GetCurrentMythicClass().CharacterClass.Name.Equals("Angel")




				)
			{
				if (Main.settings.ArueHalo && !Main.settings.ArueHaloAdded)
				{
					Main.settings.ArueHaloAdded = true;
					foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
					{
						if (unitEntityDatum.CharacterName == "Arueshalae")
						{
							//halo
							//0b1c9d2964b042e4aadf1616f653eb95
							var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");

							//horns
							//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("d0649010d93907745a44034ad6eeeb5e");
							//unitEntityDatum.AddFact(LightHaloFeature);

							//electric halo
							//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("00ca55e6c8dc07142807e40da6e3b13f");
							//unitEntityDatum.AddFact(LightHaloFeature);

							//give
							Feature fact = unitEntityDatum.Progression.Features.GetFact(LightHaloFeature);

								if (!unitEntityDatum.HasFact(fact))
								unitEntityDatum.Progression.Features.AddFeature(LightHaloFeature, null);

							break;
						}

					}
				}

				if (!Main.settings.ArueHalo && Main.settings.ArueHaloAdded)
				{
					Main.settings.ArueHaloAdded = false;
					foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
					{
						if (unitEntityDatum.CharacterName == "Arueshalae")
						{
							var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");

							//take
							Feature fact = unitEntityDatum.Progression.Features.GetFact(LightHaloFeature);

							if (unitEntityDatum.HasFact(fact))
								unitEntityDatum.Progression.Features.RemoveFact(fact);

							break;
						}

					}
				}
			}
		}


		public static void EmberAddHalo()
		{

				if (Main.settings.EmberHalo && !Main.settings.EmberHaloAdded)
				{
					Main.settings.EmberHaloAdded = true;
					foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
					{
						if (unitEntityDatum.CharacterName == "Ember")
						{
						//Main.DebugLog("add ability");
						//halo
						//0b1c9d2964b042e4aadf1616f653eb95
						var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");

							//horns
							//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("d0649010d93907745a44034ad6eeeb5e");
							//unitEntityDatum.AddFact(LightHaloFeature);

							//electric halo
							//var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("00ca55e6c8dc07142807e40da6e3b13f");
							//unitEntityDatum.AddFact(LightHaloFeature);

							//give
							Feature fact = unitEntityDatum.Progression.Features.GetFact(LightHaloFeature);

						if (!unitEntityDatum.HasFact(fact))
						{
							//Main.DebugLog("add ability 2");
							unitEntityDatum.Progression.Features.AddFeature(LightHaloFeature, null);
						}
							break;
						}

					}
				}

				if (!Main.settings.EmberHalo && Main.settings.EmberHaloAdded)
				{
					Main.settings.EmberHaloAdded = false;
					foreach (UnitEntityData unitEntityDatum in Game.Instance.Player.Party)
					{
						if (unitEntityDatum.CharacterName == "Arueshalae")
						{
							var LightHaloFeature = ResourcesLibrary.TryGetBlueprint<BlueprintFeature>("d3f14f00f675a6341a41d2194186835c");

							//take
							Feature fact = unitEntityDatum.Progression.Features.GetFact(LightHaloFeature);

							if (unitEntityDatum.HasFact(fact))
								unitEntityDatum.Progression.Features.RemoveFact(fact);

							break;
						}

					}
				}
			
		}
		public static void SetPortraits()
		{
			if (!Main.settings.ManageCompanions)
			{
				return;
			}
			//return;
			//Main.DebugLog("SetPortraits()");

			//List<PartyCharacterVM> cvm = RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.CharactersVM;


			/*	foreach (string tempList in CustomPortraitsManager.Instance.Storage.GetAll.ToTempList<string>())
				{
					CustomPortraitsManager.Instance.Storage.Unload(tempList);
				}*/

			string prefix = Main.GetCompanionPortraitDirPrefix();
			string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();


			List<UnitEntityData> allAvailableCommpanions = new List<UnitEntityData>();

		/*	foreach (UnitEntityData unitEntityData in Game.Instance.State.PlayerState.AllCharacters)
			{
				if (!allAvailableCommpanions.Contains(unitEntityData))
				{
					allAvailableCommpanions.Add(unitEntityData);
				}
			}*/
			foreach (UnitEntityData unitEntityData in Game.Instance.Player.Party)
			{
				if (!allAvailableCommpanions.Contains(unitEntityData))
				{
					allAvailableCommpanions.Add(unitEntityData);
				}
			}
		/*	foreach (UnitEntityData unitEntityData in Game.Instance.Player.ActiveCompanions)
			{
				if (!allAvailableCommpanions.Contains(unitEntityData))
				{
					allAvailableCommpanions.Add(unitEntityData);
				}
			}
			foreach (UnitEntityData unitEntityData in Game.Instance.Player.RemoteCompanions)
			{
				if (!allAvailableCommpanions.Contains(unitEntityData))
				{
					allAvailableCommpanions.Add(unitEntityData);
				}
			}*/
			//Main.DebugLog("SetPortraits() 1");

			foreach (UnitEntityData unitEntityData in allAvailableCommpanions)
			{

				//EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityData.Descriptor), true);
		//	continue;

				try
				{
					CustomPortraitsManager.Instance.Cleanup();


					string characterName = Main.GetCompanionDirName(unitEntityData.CharacterName);



					string portraitDirectoryName = prefix + characterName;

				//	Main.DebugLog("1 - "+Path.Combine(portraitsDirectoryPath, portraitDirectoryName));
					string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

//Main.DebugLog("SetPortrait() 2");

					Directory.CreateDirectory(portraitDirectoryPath);

					//	Main.DebugLog("SetPortrait() 3");
					Main.pauseGetPortraitsafe = true;

					BlueprintPortrait blueprintPortrait = unitEntityData.Blueprint.PortraitSafe;
					Main.pauseGetPortraitsafe = false;

					//blueprintPortrait = (BlueprintPortrait)typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Descriptor.UISettings);



					//	Main.DebugLog("SetPortrait() 4");

					//if (blueprintPortrait != null && blueprintPortrait.Data != null)
					//if (Main.settings.AutoBackup) SaveOriginals2(blueprintPortrait.Data, portraitDirectoryPath);
					//	if (blueprintPortrait != null)
					//			if (Main.settings.AutoBackup) SaveOriginals(unitEntityData.Blueprint, portraitDirectoryPath);

					//Main.DebugLog("SetPortrait() 5");


					if (blueprintPortrait != null && blueprintPortrait.Data != null && blueprintPortrait.Data.IsCustom && !blueprintPortrait.Data.CustomId.IsNullOrEmpty())
					{
						//if (!blueprintPortrait.Data.CustomId.Contains("Game Default"))
						//{
							portraitDirectoryPath = blueprintPortrait.Data.CustomId;

			//				Main.DebugLog("SetPortrait() found custom at: " + blueprintPortrait.Data.CustomId);
						//}
					}


					if (Directory.GetFiles(portraitDirectoryPath, "*.current").Length != 0)
					{
						string[] dirs = Directory.GetFiles(portraitDirectoryPath, "*.current");

						string dir = Path.GetFileNameWithoutExtension(dirs[0]);
						//Main.DebugLog(dir);
						if (!dir.Equals("root"))
							portraitDirectoryPath = Path.Combine(portraitDirectoryPath, dir);
						//Main.DebugLog(portraitDirectoryPath);
					}
					else
                    {
						if (!File.Exists(Path.Combine(portraitDirectoryPath, "Medium.png")))
							if (File.Exists(Path.Combine(portraitDirectoryPath, GetDefaultPortraitsDirName(),"Medium.png")))
                        {

								portraitDirectoryPath = Path.Combine(portraitDirectoryPath, GetDefaultPortraitsDirName());


						}
                    }

					bool missing = false;
				
			//		Main.DebugLog("SetPortrait() 6");

					foreach (string fileName in Main.PortraitFileNames)
					{
						if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
						{
							missing = true;
						}
					}

			//		Main.DebugLog("SetPortrait() 7" + portraitDirectoryPath);


					if (!missing)
					{
				//		Main.DebugLog("SetPortrait() 8");

						blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;

						CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
						CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
						CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));
			//			Main.DebugLog("SetPortrait() 9");

						blueprintPortrait.Data = new PortraitData(portraitDirectoryPath);

				//		Main.DebugLog("SetPortrait() 99");

						if (unitEntityData.IsPet)
						{
					//		Main.DebugLog("SetPortrait() 999");

							//typeof(PortraitData).GetField("m_PetEyeImage", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.Descriptor.UISettings, true);

							//BlueprintPortrait bp = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Blueprint);

					//		Main.DebugLog("getpeteye");

							Main.pauseGetPortraitsafe = true;
							blueprintPortrait.Data.m_PetEyeImage = unitEntityData.Blueprint.PortraitSafe.Data.m_PetEyeImage;
							Main.pauseGetPortraitsafe = false;


						}

				//		Main.DebugLog("SetPortrait() 10");


						//isSetPortrait = true;
						//unitEntityData.UISettings.SetPortrait(blueprintPortrait);

						//	typeof(UnitUISettings).GetField("m_CustomPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, blueprintPortrait.Data);
						//	typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, blueprintPortrait.Data);

						//Main.DebugLog("SetPortrait() 11");


						//Main.DebugLog("SetPortrait() 12");

						//	EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityData), true);
						//Game.Instance.DialogController.CurrentSpeaker.UISettings.SetPortrait(blueprintPortrait);

						//isSetPortrait = false;

						//KitsunePolymorphBuff_Nenio
						//BlueprintBuff polyBuffNenio = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("a13e2e71485901045b1722824019d6f5");



						if (characterName.Equals(Main.NenioName))
						{

							//unitEntityData.GetActivePolymorph().Component.OnDeactivate();

							if (unitEntityData.Body.IsPolymorphed)
							{
								unitEntityData.GetActivePolymorph().Runtime.OnActivate();
							}

							//NenioToggle();
							/*
							var buff = ResourcesLibrary.TryGetBlueprint<BlueprintBuff>("a13e2e71485901045b1722824019d6f5");
							var portrait = buff.ComponentsArray.OfType<Polymorph>().FirstOrDefault()?.Portrait.Data;
							if (portrait is null) return;
							portrait = blueprintPortrait.Data;
							*/

							/*
							unitEntityData.GetActivePolymorph().Component.m_Portrait = blueprintPortrait.ToReference<BlueprintPortraitReference>();

							unitEntityData.UISettings.SetOverridePortrait(blueprintPortrait);

						EventBus.RaiseEvent<IPolymorphActivatedHandler>(delegate (IPolymorphActivatedHandler h)
							{
								h.OnPolymorphActivated(unitEntityData, unitEntityData.GetActivePolymorph().Component);
							}, true);

							*/
							//	unitEntityData.GetActivePolymorph().Runtime.OnActivate();
							//	unitEntityData.GetActivePolymorph().Component.OnActivate();


						}
						else
						{
							Main.pauseGetPortraitsafe = true;
							unitEntityData.UISettings.SetPortrait(blueprintPortrait.Data);
							Main.pauseGetPortraitsafe = false;
						}
						
						//RootUIContext.Instance.InGameVM.StaticPartVM.PartyVM.CharactersVM[j].PortraitPartVM.HandlePortraitChanged(unitEntityData);

						//GroupController.Instance.m_Characters.ForEach(c => c.Portrait.Initialize(c.Unit));




						//						unitEntityData.Portrait = blueprintPortrait.Data;

						//		unitEntityData.UISettings.m_po


						//	typeof(UnitUISettings).GetField("m_CustomPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, blueprintPortrait.Data);
						//	typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, blueprintPortrait.Data);

						//		EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityData), true);
						//

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
					{

						//BlueprintPortrait blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;

						//BlueprintPortrait blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;



						//mPortrait= (BlueprintPortrait)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance.Owner.Blueprint);



						/*
						BlueprintDialog bd = Game.Instance.Player.Dialog.ShownDialogs.ToList<BlueprintDialog>().Find(x => x.AssetGuid == "3fb2516a55a21684aac00eb4f4015a77");
						if (bd != null)
						{
							characterName = "ValerieScar_Portrait";

						}
						//Scar Healed
						BlueprintCueBase bc = Game.Instance.Player.Dialog.ShownCues.ToList<BlueprintCueBase>().Find(x => x.AssetGuid == "3bc0f984c248897479bc30b16d91ffc5");
						if (bc != null)
						{

							characterName = "Valerie";

						}

					}
					if (characterName == "Tristian")
					{
						//Blinded

						if (Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys.FirstOrDefault(x => x.name.Equals("OculusShattered")))

							*/


						//					if (Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys.FirstOrDefault(x => x.name.Equals("OculusShattered")))

						//{


						//}


						//Main.DebugLog("HERE----------------"+characterName);

					//	Main.DebugLog("8b");
						blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Descriptor.Blueprint);

						//Main.pauseGetPortraitsafe = true;
						//blueprintPortrait = unitEntityData.Blueprint.PortraitSafe;
						//Main.pauseGetPortraitsafe = false;
						if (blueprintPortrait == null)
						{
						//	Main.DebugLog("9b");

							//Main.DebugLog("unitBlueprintPortrait: " + blueprintPortrait.name);
							//		Main.DebugLog("BlueprintUnit: " + unitEntityData.Blueprint.name);
							//UIRoot uIRoot = BlueprintRoot.Instance.UIRoot;

							//blueprintPortrait = uIRoot.LeaderPlaceholderPortrait;
						//	Main.DebugLog("no portrait? " + unitEntityData.Gender);

							//CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
							//CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
							//CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));


							if (unitEntityData.Gender == Gender.Male)
							{
								//	Main.DebugLog("male");

								blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("621ada02d0b4bf64387babad3a53067b");

							}
							else
							{
								//Main.DebugLog("female");
								//blueprintPortrait = uIRoot.LeaderPlaceholderPortrait;
								blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("9fe4f89ecf15b874db9d1d2bf3ef33d2");

							}
							/*
							[CustomNpcPortraits] Placeholder_Female_Portrait
							[CustomNpcPortraits] 9fe4f89ecf15b874db9d1d2bf3ef33d2
							[CustomNpcPortraits] Placeholder_Male_Portrait
							[CustomNpcPortraits] 621ada02d0b4bf64387babad3a53067b
							*/

						}
                        else
                        {

							//Main.DebugLog("default portrait? " + characterName);
							if(portraitDirectoryPath.Contains("Game Default Portraits"))
								blueprintPortrait.Data = new PortraitData(portraitDirectoryPath);
							else
							blueprintPortrait.Data = new PortraitData(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName()));

						}


						//	if(blueprintPortrait.BackupPortrait != null)
						//	blueprintPortrait = blueprintPortrait.BackupPortrait;
						//	if (blueprintPortrait.name.Equals("Empty_Portrait"))

						//{

						//blueprintPortrait = uIRoot.LeaderPlaceholderPortrait;
						/*

						*/
						//}



						/*
					 [CustomNpcPortraits] NenioFox_Portrait
					[CustomNpcPortraits] 2b4b8a23024093e42a5db714c2f52dbc
					[CustomNpcPortraits] NenioHuman_Portrait
					[CustomNpcPortraits] a6e4ff25a8da46a44a24ecc5da296073
					 */

						if (characterName.Equals(Main.NenioName))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("a6e4ff25a8da46a44a24ecc5da296073");
						if (characterName.Equals(Main.NenioName + "Fox_Portrait"))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");

						if (characterName.Equals(Main.ArueName))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("db413e67276547b40b1a6bb8178c6951");
						if (characterName.Equals(Main.ArueName + " - Evil"))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("484588d56f2c2894ab6d48b91509f5e3");

						if (characterName.Equals(Main.GalfreyName))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("a3ba06b4723c7a74fb5054ccb2289efb");
						if (characterName.Equals(Main.GalfreyName + " - Undead"))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("767456b1656ca064dadac544d39d7e40");
						if (characterName.Equals(Main.GalfreyName + " - Old"))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("4e8dfb75015d356469b976145c851087");

						if (characterName.Equals(Main.WoljifName + " - Demon"))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("f2bd5a94ade889444921b4a74b9e34a9");



						if (characterName.Equals(Main.CiarName + " - Undead"))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("dc2f02dd42cfe2b40923eb014591a009");

						if (characterName.Equals(Main.StauntonName + " - Undead"))
							blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("f4bbe08217bcaa54c91fe73bcea70ede");


						/*                          [CustomNpcPortraits] GalfreyUndead_Portrait
												[CustomNpcPortraits] 767456b1656ca064dadac544d39d7e40
												[CustomNpcPortraits] GalfreyOld_Portrait
												[CustomNpcPortraits] 4e8dfb75015d356469b976145c851087
												[CustomNpcPortraits] GalfreyYoung_Portrait
												[CustomNpcPortraits] a3ba06b4723c7a74fb5054ccb2289efb
					  */


						/*
	[CustomNpcPortraits] ArueshalaeEvil_Portrait
	[CustomNpcPortraits] 484588d56f2c2894ab6d48b91509f5e3
	[CustomNpcPortraits] Arueshalae_Portrait
	[CustomNpcPortraits] db413e67276547b40b1a6bb8178c6951
							blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Blueprint);

	*/
						//			Main.DebugLog("setportraits: 10b " + characterName);

						if (unitEntityData.IsPet)
						{
							//typeof(PortraitData).GetField("m_PetEyeImage", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.Descriptor.UISettings, true);

							//BlueprintPortrait bp = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Blueprint);
							// = BlueprintRoot.Instance.CharGen.CustomPortrait;

							//Main.DebugLog(bp.AssetGuid.ToString());
							if (characterName.Equals("Bismuth"))
							{
								//CustomPortraitsManager.Instance.Storage.Unload("1f5bfefa49a8aa2449ad94b1f4f61788");
								//BlueprintPortrait bp = //BlueprintRoot.Instance.CharGen.CustomPortrait;

								///				Main.DebugLog("getting bism portrait");


								//BlueprintPortrait bp = unitEntityData.Blueprint.PortraitSafe;// Utilities.GetBlueprintByGuid<BlueprintPortrait>("1f5bfefa49a8aa2449ad94b1f4f61788");

								//blueprintPortrait.Data.m_PetEyeImage = bp.Data.m_PetEyeImage;
								
								Main.pauseGetPortraitsafe = true;

								blueprintPortrait = unitEntityData.Blueprint.PortraitSafe;
		

								unitEntityData.UISettings.SetPortrait(blueprintPortrait.Data);

								Main.pauseGetPortraitsafe = false;
								//typeof(UnitUISettings).GetField("m_CustomPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, null);

								//typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, blueprintPortrait);

								eye = blueprintPortrait.Data.PetEyePortrait;

								//EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityData.Descriptor), true);
								return;

							}

							//Main.pauseGetPortraitsafe = true;
							//Main.pauseGetPortraitsafe = false;

							//typeof(UnitUISettings).GetField("m_PinPet", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.Descriptor.UISettings, true);
							//EventBus.RaiseEvent<IPinPetUpdateHandler>((IPinPetUpdateHandler h) => h.HandlePinPetUpdate(unitEntityData.Descriptor.UISettings.Owner.Unit, true), true);

						}

						//isSetPortrait = true;
						//unitEntityData.UISettings.SetPortrait(blueprintPortrait);
						//typeof(PortraitData).GetField("m_PetEyeImage", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.Descriptor.UISettings, true);

					//	Main.DebugLog("11b");

						typeof(UnitUISettings).GetField("m_CustomPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, blueprintPortrait.Data);

						//Main.DebugLog("12b");

						typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(unitEntityData.UISettings, null);
						//Main.DebugLog("13b");

						EventBus.RaiseEvent<IUnitPortraitChangedHandler>((IUnitPortraitChangedHandler h) => h.HandlePortraitChanged(unitEntityData.Descriptor), true);

						//	Game.Instance.DialogController.CurrentSpeaker.UISettings.SetPortrait(blueprintPortrait);

						//isSetPortrait = false;
						//unitEntityData.UISettings.TryToInitialize();
						//Game.Instance.DialogController.Tick();
						//Main.DebugLog("1");


						//					Main.DebugLog("2");

						//				Main.DebugLog("3");


						//			Main.DebugLog("4");
						//(PortraitData)typeof(UnitUISettings).GetField("m_CustomPortrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.UISettings);
						//string customId = (string)typeof(PortraitData).GetField("m_CustomPortraitId", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(portraitData);





						//Kingmaker.UI.MVVM._VM.Party.PartyCharacterVM.SetPinPet(null, unitEntityData);

						/*
						reactiveCharacter = null;
							Kingmaker.UI.MVVM._VM.Party.PartyCharacterPetVM partyCharacterPetVM = new Kingmaker.UI.MVVM._VM.Party.PartyCharacterPetVM();
							Kingmaker.UI.MVVM._VM.Party.PartyCharacterPetVM partyCharacterPetVM1 = partyCharacterPetVM;
							reactiveCharacter.Value = partyCharacterPetVM;
						//AddDisposable(partyCharacterPetVM1);

						reactiveCharacter.Value.SetUnitData(unitEntityData);*/

						//pcvm = new Kingmaker.UI.MVVM._VM.Party.PartyCharacterVM();

						//PartyCharacterVM.SetPinPet(null, unit)

						//unitEntityData.UISettings.SetPortrait(blueprintPortrait);

						/*
						EventBus.RaiseEvent<IUnitPortraitChangedHandler>(delegate (IUnitPortraitChangedHandler h)
						{
							h.HandlePortraitChanged(unitEntityData);
						});
						*/
						//Main.DebugLog("Trying to restore default portraits for " + characterName);
					}

					//Kingmaker.UI.MVVM._PCView.Party.PartyCharacterPetPCView ptv =



					//unitEntityData.UISettings.PinPet = false;
					//unitEntityData.UISettings.PinPet = true;

				}
				catch(Exception e)
                {

					Main.DebugError(e);
                }

			}
			//Game.Instance.UI.Common.Initialize();


		}

		public static bool pauseGetPortraitsafe = false;
		public static Sprite eye;

		public static bool SaveOriginals(BlueprintUnit bup, string path)
		{



			//pauseGetPortraitsafe = true;

			bool result = false;





			//	Main.DebugLog("SaveOriginals() no backups yet");
			BlueprintPortrait blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(bup);

			//BlueprintPortrait blueprintPortrait = bup.PortraitSafe;

			//pauseGetPortraitsafe = false;
			//



			//(BlueprintPortrait)AccessTools.Field(bup.GetType(), "m_Portrait").GetValue(bup);

			// 

			if (blueprintPortrait != null)
			{
			//	Main.DebugLog("SaveOriginals: " + "1");

				SpriteLink mFullLengthImage = blueprintPortrait.Data.m_FullLengthImage;
				//Main.DebugLog("SaveOriginals: " + "2");
				SpriteLink mHalfLengthImage = blueprintPortrait.Data.m_HalfLengthImage;
				//Main.DebugLog("SaveOriginals: " + "3");
				SpriteLink mPortraitImage = blueprintPortrait.Data.m_PortraitImage;

				//string defdirpath = Path.Combine(path, GetDefaultPortraitsDirName());

				//Directory.CreateDirectory(defdirpath);



				//Main.DebugLog(mFullLengthImage.AssetId);
			//	Main.DebugLog("SaveOriginals: Trying to write files for " + bup.CharacterName + " - " + mPortraitImage.Load(true, false));
				string defdirpath = Path.Combine(path, GetDefaultPortraitsDirName());
				//	Main.DebugLog("SaveOriginals: " + defdirpath);


				if (Main.settings.AutoBackup)
				{
					Directory.CreateDirectory(defdirpath);


				}




				bool isNpc = false;

				try
				{



					if (bup.IsCompanion || bup.CharacterName == Game.Instance.Player.MainCharacter.Value.CharacterName || Main.companions.Contains(bup.CharacterName) )
					{
						if (!File.Exists(Path.Combine(defdirpath, Main.mediumName)))
							if (isPortraitMissing(defdirpath, bup) )
						{
							CreateBaseImages(Path.Combine(defdirpath, Main.smallName), mPortraitImage.Load(true, false), isNpc, false);
							CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), mHalfLengthImage.Load(true, false), isNpc, false);
							CreateBaseImages(Path.Combine(defdirpath, Main.fullName), mFullLengthImage.Load(true, false), isNpc, false);

							//File.WriteAllText(Path.Combine(path, GetDefaultPortraitsDirName() + ".current"), "current");

							//		Main.DebugLog("SaveOriginals: all Originals saved OK for " + bup.CharacterName + " in " + defdirpath);

						}
						else
						{
					//		Main.DebugLog("SaveOriginals: all Originals already saved for " + bup.CharacterName + " in " + defdirpath);
						}
					}
					else
					{
						isNpc = true;
						bool secret = false;
						if (!File.Exists(Path.Combine(defdirpath, Main.mediumName)))

							if (isPortraitMissing(defdirpath, bup))
						{
						//	foreach (string sFile in System.IO.Directory.GetFiles(path, "*.current"))
						//	{
						//		System.IO.File.Delete(sFile);
						//	}
						//	File.WriteAllText(Path.Combine(path, GetDefaultPortraitsDirName() + ".current"), "current");

							Sprite medium = mHalfLengthImage.Load(true, false);

							Sprite small = mPortraitImage.Load(true, false);





							if (small.texture.width == medium.texture.width)
							{
						//		Main.DebugLog("SECRET!!!");
								secret = true;
								if (Main.settings.AutoBackup)
								{
									CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), mHalfLengthImage.Load(true, false), true, true);

									CreateBaseImages(Path.Combine(defdirpath, Main.smallName), small, isNpc, false);
								}
							}
							else
							{
								secret = false;
								if (Main.settings.AutoBackup)
								{
									CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), medium, isNpc, secret);
									CreateBaseImages(Path.Combine(defdirpath, Main.smallName), small, isNpc, secret);
									CreateBaseImages(Path.Combine(defdirpath, Main.fullName), mFullLengthImage.Load(true, false), false, secret);
								}
						//		Main.DebugLog("NPC with full portraits");

							}
							

				//			Main.DebugLog("SaveOriginals: all Originals saved OK for " + bup.CharacterName + " in " + defdirpath);
						}
						else
						{
				//			Main.DebugLog("SaveOriginals: all Originals already saved for " + bup.CharacterName + " in " + defdirpath);
						}

						if (Main.settings.AutoSecret && secret)
						{
							if (!File.Exists(Path.Combine(path, Main.mediumName)))
							{

									Directory.CreateDirectory(path);

								/*	if (!secret)
									{
										CreateBaseImages(Path.Combine(path, Main.mediumName), mHalfLengthImage.Load(true, false), false, secret);


										if (!File.Exists(Path.Combine(path, Main.smallName)))
										{
											CreateBaseImages(Path.Combine(path, Main.smallName), mPortraitImage.Load(true, false), false, secret);
										}
										if (!File.Exists(Path.Combine(path, Main.fullName)))
										{
											CreateBaseImages(Path.Combine(path, Main.fullName), mFullLengthImage.Load(true, false), false, secret);

										}
									}
									else
									{*/
								CreateBaseImages(Path.Combine(path, Main.mediumName), mHalfLengthImage.Load(true, false), true, secret);

								CreateBaseImages(Path.Combine(path, Main.smallName), mPortraitImage.Load(true, false), false, secret);

							//	foreach (string sFile in System.IO.Directory.GetFiles(path, "*.current"))
							//	{
						//			System.IO.File.Delete(sFile);
							//	}
							//	File.WriteAllText(Path.Combine(path, "root.current"), "current");


								//	}
								//			Main.DebugLog("SaveOriginals: Original saved as Custom portrait OK as " + Path.Combine(path, Main.mediumName));
							}
							else
							{
							//	Main.DebugLog("SaveOriginals: Original already saved as Custom portrait as " + Path.Combine(path, Main.mediumName));
							}
						}
					}




					result = true;
				}
				catch (Exception ex)
				{
					failCounter++;
					Main.DebugLog("Disk, The process failed: " + ex.ToString());
					result = false;
				}

						

			}

			return result;
		}


		public static bool SaveOriginals2(PortraitData pdata, string path)
		{

			if (!Main.settings.AutoBackup)
			{
				return false;
			}


			bool result = false;





			//Main.DebugLog("SaveOriginals2() no backups yet");

			//BlueprintPortrait blueprintPortrait = bup.PortraitSafe;


			//	BlueprintPortrait blueprintPortrait = (BlueprintPortrait)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(bup);

			//(BlueprintPortrait)AccessTools.Field(bup.GetType(), "m_Portrait").GetValue(bup);




			//(BlueprintPortrait)AccessTools.Field(bup.GetType(), "m_Portrait").GetValue(bup);

			// 

			if (pdata != null)
			{
				//Main.DebugLog("SaveOriginals2: " + "1");



				SpriteLink mFullLengthImage = pdata.m_FullLengthImage;
				//Main.DebugLog("SaveOriginals2: " + "2");
				SpriteLink mHalfLengthImage = pdata.m_HalfLengthImage;
				//Main.DebugLog("SaveOriginals2: " + "3");
				SpriteLink mPortraitImage = pdata.m_PortraitImage;

				//string defdirpath = Path.Combine(path, GetDefaultPortraitsDirName());

				//Directory.CreateDirectory(defdirpath);



				//Main.DebugLog("SaveOriginals: Trying to write files for " + " - " + mPortraitImage.Load(true, false));

				string defdirpath = Path.Combine(path, GetDefaultPortraitsDirName());
				//Main.DebugLog("SaveOriginals2: " + defdirpath);

				Directory.CreateDirectory(defdirpath);




				bool isNpc = false;

				try
				{


				//	Main.DebugLog("SaveOriginals2: " + "1");


					isNpc = true;
						bool secret = false;
					if (isNpc)
					{
					//	Main.DebugLog("SaveOriginals2: " + "2");

						Sprite medium = mHalfLengthImage.Load(true, false);

						//Main.DebugLog("SaveOriginals2: " + "3");


						Sprite small = mPortraitImage.Load(true, false);


				//		Main.DebugLog("SaveOriginals2: " + "4");



						if (pdata.InitiativePortrait /*small.texture.width == medium.texture.width*/)
						{
					//		Main.DebugLog("SECRET!!!");
							secret = true;
							if (Main.settings.AutoBackup)
								if (!File.Exists(Path.Combine(defdirpath, Main.mediumName)))

								{
									//				Main.DebugLog("medium");
									CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), mHalfLengthImage.Load(true, false), true, true);
					//			Main.DebugLog("small");
								CreateBaseImages(Path.Combine(defdirpath, Main.smallName), small, isNpc, false);

									//	foreach (string sFile in System.IO.Directory.GetFiles(path, "*.current"))
									//	{
									//		System.IO.File.Delete(sFile);
									//	}
									//	File.WriteAllText(Path.Combine(path, GetDefaultPortraitsDirName() + ".current"), "current");

								}
						}
						else

						{
				//			Main.DebugLog("NPC with full portraits");

							secret = false;
							if (Main.settings.AutoBackup)
								if (!File.Exists(Path.Combine(defdirpath, Main.mediumName)))

								{
									CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), medium, isNpc, secret);
								CreateBaseImages(Path.Combine(defdirpath, Main.smallName), small, isNpc, secret);
								CreateBaseImages(Path.Combine(defdirpath, Main.fullName), mFullLengthImage.Load(true, false), false, secret);
								//foreach (string sFile in System.IO.Directory.GetFiles(path, "*.current"))
								//{
								//	System.IO.File.Delete(sFile);
								//}
								//File.WriteAllText(Path.Combine(path, GetDefaultPortraitsDirName() + ".current"), "current");

							}

						}


					}

			




					result = true;
				}
				catch (Exception ex)
				{
					failCounter++;


					Main.DebugLog("Disk, The process failed: " + ex.ToString());
					result = false;
				}



			}
			else
            {

						Main.DebugLog("pdata is null");


			}
			return result;
		}

		public static void CreateBaseImages(string path, Sprite baseSprite, bool npc, bool secret)
		{

			LoadingProcess.Instance.StartCoroutine(ExportSprite(baseSprite, path));

		}



		static System.Collections.IEnumerator ExportSprite(Sprite sprite, string filePath)
		{
			/*
			if (File.Exists(filePath))
			{
				Log($"{filePath} exists. Skipping");
			}
			else 
			*/
			if (sprite.packed && sprite.packingMode == SpritePackingMode.Tight)
			{
				Log($"Skipping tightly-packed sprite {sprite.name}");
			}

			else
			{


				var copy = new Texture2D(sprite.texture.width, sprite.texture.height, TextureFormat.RGBA32, false);

                Graphics.ConvertTexture(sprite.texture, copy);

				yield return null;

				if (sprite.packed)
				{
					var spriteTexture = new Texture2D(
						(int)sprite.textureRect.width,
						(int)sprite.textureRect.height,
						TextureFormat.RGBA32,
						false);

					Graphics.CopyTexture(
						copy, 0, 0,
						(int)sprite.textureRect.x,
						(int)sprite.textureRect.y,
						(int)sprite.textureRect.width,
						(int)sprite.textureRect.height,
						spriteTexture, 0, 0, 0, 0);

					var old = copy;

					UnityEngine.Object.Destroy(old);

					copy = spriteTexture;
				}

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


public static bool isPortraitMissing(string path, BlueprintUnit bup)
		{
			DirectoryInfo DirInfo = new DirectoryInfo(path);

			if (bup.IsCompanion || (bup.CharacterName == Game.Instance.Player.MainCharacter.Value.CharacterName))
			{
				foreach (string fileName in Main.PortraitFileNames)
				{
					//Main.DebugLog("File looked for Comp: " + Path.Combine(path, fileName));

					if (!File.Exists(Path.Combine(path, fileName)))
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				//Main.DebugLog("File looked for Npc: " + Path.Combine(path, mediumName));
				if (!File.Exists(Path.Combine(path, smallName)))
				{
					return true;
				}
				if (!File.Exists(Path.Combine(path, mediumName)))
				{
					return true;
				}
				return false;
			}

		}




	}
}