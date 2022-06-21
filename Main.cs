using UnityEngine;
using UnityEngine.UI;
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
using TurnBased.Controllers;

namespace CustomNpcPortraits
{
#if DEBUG
	[EnableReloading]
#endif
	public static class Main
	{
		public static Settings settings;
		public static void loctel()
		{
			Vector3 worldPosition = Game.Instance.ClickEventsController.WorldPosition;
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
			}
		}
			public static bool Load(UnityModManager.ModEntry modEntry)
		{

			//isInitRunning = true;
			Main.logger = modEntry.Logger;


			//(new Harmony(modEntry.Info.Id)).PatchAll(Assembly.GetExecutingAssembly());

			Main.harmonyInstance = HarmonyInstance.Create(modEntry.Info.Id);
			
			Main.settings = UnityModManager.ModSettings.Load<Settings>(modEntry);

			modEntry.OnGUI = new Action<UnityModManager.ModEntry>(Main.OnGUI);

			modEntry.OnShowGUI = new Action<UnityModManager.ModEntry>(Main.OnShowGUI);

			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);

			modEntry.OnSaveGUI = new Action<UnityModManager.ModEntry>(Main.OnSaveGUI);

#if DEBUG
			modEntry.OnUnload = new Func<UnityModManager.ModEntry, bool>(Main.Unload);
#endif
			//modEntry.OnUpdate = new Action<UnityModManager.ModEntry, float>(Main.Init);


			if (!Main.ApplyPatch(typeof(GetPortrait_Patch), "GetPortrait_Patch"))
			{
				throw Main.Error("Failed to patch GetPortrait");
			}
			

			if (!Main.ApplyPatch(typeof(GameMode_OnActivate_Patch), "GameMode_OnActivate_Patch"))
			{
				throw Main.Error("Failed to patch GameMode_OnActivate");
			}
		
			if (!Main.ApplyPatch(typeof(Game_OnAreaLoaded_Patch), "Game_OnAreaLoaded_Patch"))
			{
				throw Main.Error("Failed to patch Game_OnAreaLoaded");
			}
			
			if (!Main.ApplyPatch(typeof(Player_AddCompanion_Patch), "Player_AddCompanion_Patch"))
			{
				throw Main.Error("Failed to patch Player_AddCompanion");
			}

			if (!Main.ApplyPatch(typeof(InitiativeTrackerUnitVM_Get_Portrait_Patch), "InitiativeTrackerUnitVM_Get_Portrait_Patch"))
			{
				throw Main.Error("Failed to patch InitiativeTrackerUnitVM_Get_Portrait");
			}
			
			return true;
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
		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{

			switch (state)
			{
				case State.Start:
					//DebugLog("run once");

					string name = SceneManager.GetActiveScene().name;
					//Main.DebugLog("SceneManager.GetActiveScene().name: " + name);

					Game instance = Game.Instance;
					if (((instance != null) ? instance.Player : null) == null || name == "UI_MainMenu_Scene" || name == "Start")
					{
						isLoadedGame = false;
						isDialog = false;
					}
					else if (instance.CurrentMode != GameModeType.Dialog)
					{
						isLoadedGame = true;

						isDialog = false;
					}
					state = State.Update;
					break;

				case State.Update:
					//                    DebugLog("run every frame");


					break;
			}

			GUIStyle boldStyle = new GUIStyle();
			boldStyle.fontStyle = FontStyle.Bold;
			//Color c = new Color((float)131.0, (float)192.0, (float)239.0);
			boldStyle.normal.textColor = Color.cyan;


			

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Open NPC portraits dir", GUILayout.Width(200f), GUILayout.Height(20f)))
			{
				//Process.Start(GetNpcPortraitsDirectory());
				/*
				BlueprintAreaEnterPoint bap = Utilities.GetBlueprint<BlueprintAreaEnterPoint>("Prologue_Labyrinth_Ending");

				Game.Instance.LoadArea(bap, AutoSaveMode.None);
				*/
				Process.Start(GetNpcPortraitsDirectory());

			}
			GUILayout.Label(GetNpcPortraitsDirectory());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Open Party Portraits Directory", GUILayout.Width(200f), GUILayout.Height(20f)))
			{

				/*	foreach (BlueprintItem scriptableObject in Utilities.GetScriptableObjects<BlueprintItem>())
					{
						string blueprintPath = Utilities.GetBlueprintPath(scriptableObject);
						"HosillaKey"
						//if(scriptableObject != null && scriptableObject.Name != null && scriptableObject.Name.Length > 0 && scriptableObject.Name.ToLower().Contains("maze"))
						if (blueprintPath.ToLower().Contains("key"))
						Main.DebugLog(blueprintPath);
					}*/
				/*
				CheatsTransfer

				BlueprintItem scriptableObject = Utilities.GetBlueprintByName<BlueprintItem>("Labyrinth_Key_2");

				ItemsCollection inventory = Game.Instance.Player.Inventory;

				ItemEntitySimple key = new ItemEntitySimple(scriptableObject);

				inventory.Add(key);
				
				*/
				//loctel();
				Process.Start(GetCompanionPortraitsDirectory());
			}
			GUILayout.Label(GetCompanionPortraitsDirectory());
			GUILayout.EndHorizontal();

			//GUILayout.Label(" ");

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
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				if (GUILayout.Button("Refresh Party Portraits", GUILayout.Width(200f), GUILayout.Height(20f)))
				{
					SetPortraits();
				}
				GUILayout.Label("Use this after replacing the portrait of a party companion.");
				GUILayout.EndHorizontal();


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
					bool companion = Game.Instance.DialogController.CurrentSpeaker.Blueprint.IsCompanion;

					string CurrentSpeakerName = Game.Instance.DialogController.CurrentSpeakerName;

					string CurrentSpeakerBlueprintName = Game.Instance.DialogController.CurrentSpeakerBlueprint.name;

					string NpcPortraitPath = Path.Combine(GetNpcPortraitsDirectory(), CurrentSpeakerName);

					if (!companion)
					{
						if (Directory.Exists(NpcPortraitPath))
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
							GUILayout.Label(NpcPortraitPath);
							GUILayout.EndHorizontal();
						}
						else
						{
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							if (GUILayout.Button("Make speaker dir", GUILayout.Width(200f), GUILayout.Height(20f)))
							{
								Directory.CreateDirectory(NpcPortraitPath);


								Main.DebugLog("Create dir at: " + NpcPortraitPath);
							}
							GUILayout.Label(NpcPortraitPath);
							GUILayout.EndHorizontal();
						}
					}
					else
					{
						string CompanionPortraitPath = Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName);

						if (Directory.Exists(CompanionPortraitPath))
						{
							if (Main.settings.AutoBackup && !savedComp)
							{
								SaveOriginals(Game.Instance.DialogController.CurrentSpeakerBlueprint, CompanionPortraitPath);
								savedComp = true;
							}

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

								Main.DebugLog("Create dir at: " + CompanionPortraitPath);
							}
							GUILayout.Label(CompanionPortraitPath);
							GUILayout.EndHorizontal();
						}
					}


					if (!companion/* && (label != "CustomizationPreset")*/)
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
							GUILayout.Label("Sub dir already exists: " + portraitDirectorySubPath);
							GUILayout.EndHorizontal();
						}
						else
						{
							GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
							if (GUILayout.Button("Make speaker subcat dir", GUILayout.Width(200f), GUILayout.Height(20f)))
							{

								Directory.CreateDirectory(portraitDirectorySubPath);

							
								Main.DebugLog("Create dir at: " + portraitDirectorySubPath);
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
	

				//GUILayout.Label("Dialog options! YAAAYYY!!!");

				}


			}

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Advanced", boldStyle);
			GUILayout.EndHorizontal();

			Main.settings.AutoBackup = GUILayout.Toggle(Main.settings.AutoBackup, "Keep game defaults auto backed up in 'Game Default Portraits' of each subdir.", new GUILayoutOption[0]);
			Main.settings.AutoSecret = GUILayout.Toggle(Main.settings.AutoSecret, "Extract and use the turn based portraits of NPC-s in dialogs where available.", new GUILayoutOption[0]);

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
		private enum State { Start, Update };

		private static State state;

		private static bool isDialog = false;

		private static bool isLoadedGame = false;
#if DEBUG
		private static bool showPatched = false;
#endif


		private static void OnShowGUI(UnityModManager.ModEntry modEntry)
		{
			state = State.Start;
			savedNpc = false;
			savedComp = false;
			isDialog = true;
			isLoadedGame = true;
#if DEBUG
			showPatched = false;
#endif
		}



		private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
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




		public static string GetNpcPortraitsDirectory()
		{
			string dir = Path.GetFullPath(Path.Combine(CustomPortraitsManager.PortraitsRootFolderPath, @"..\"));

			dir = Path.Combine(dir, NpcPortraitsDirName());

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
		public static string GetCompanionPortraitDirPrefix()
		{
	
			return "CustomNpcPortraits - ";

		}
		public static string GetDefaultPortraitsDirName()
		{

			return "Game Default Portraits";

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
			if (Main.okPatches.Count > 0)
			{
				settings.Save(modEntry);

				harmonyInstance.UnpatchAll(modEntry.Info.Id);

				UnityModManager.Logger.Clear();


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


		public static void SetPortraits()
		{
			//Main.DebugLog("SetPortraits()");

			string prefix = Main.GetCompanionPortraitDirPrefix();
			string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();


			List<UnitEntityData> allAvailableCommpanions = new List<UnitEntityData>();

			foreach (UnitEntityData unitEntityData in Game.Instance.State.PlayerState.AllCharacters)
			{
				if (!allAvailableCommpanions.Contains(unitEntityData))
				{
					allAvailableCommpanions.Add(unitEntityData);
				}
			}
			foreach (UnitEntityData unitEntityData in Game.Instance.Player.Party)
			{
				if (!allAvailableCommpanions.Contains(unitEntityData))
				{
					allAvailableCommpanions.Add(unitEntityData);
				}
			}
			foreach (UnitEntityData unitEntityData in Game.Instance.Player.ActiveCompanions)
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
			}


				foreach (UnitEntityData unitEntityData in allAvailableCommpanions)
			{

				string characterName = unitEntityData.CharacterName;
				string portraitDirectoryName = prefix + characterName;
				string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

				Directory.CreateDirectory(portraitDirectoryPath);
				
				if(Main.settings.AutoBackup) SaveOriginals(unitEntityData.Blueprint, portraitDirectoryPath);
				bool missing = false;
				foreach (string fileName in Main.PortraitFileNames)
				{
					if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
					{
						missing = true;
						break;
					}
				}
				if (!missing)
				{
					BlueprintPortrait blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;

					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));

					blueprintPortrait.Data = new PortraitData(portraitDirectoryPath);


					unitEntityData.UISettings.SetPortrait(blueprintPortrait);

					EventBus.RaiseEvent<IUnitPortraitChangedHandler>(delegate (IUnitPortraitChangedHandler h)
					{
						h.HandlePortraitChanged(unitEntityData);
					});
				}
				else
				{

					BlueprintPortrait blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;
					blueprintPortrait.Data = new PortraitData(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName()));


					unitEntityData.UISettings.SetPortrait(blueprintPortrait);

					EventBus.RaiseEvent<IUnitPortraitChangedHandler>(delegate (IUnitPortraitChangedHandler h)
					{
						h.HandlePortraitChanged(unitEntityData);
					});

					//Main.DebugLog("Trying to restore default portraits for " + characterName);
				}

			}


		}


		public static bool SaveOriginals(BlueprintUnit bup, string path)
		{


			bool result = false;



			
			
			//	Main.DebugLog("SaveOriginals() no backups yet");

				BlueprintPortrait blueprintPortrait = bup.PortraitSafe;





			//BlueprintPortrait blueprintPortrait = (BlueprintPortrait)AccessTools.Field(bup.GetType(), "m_Portrait").GetValue(bup);

			// (BlueprintPortrait)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(bup);

			if (blueprintPortrait != null)
			{

				SpriteLink mFullLengthImage = blueprintPortrait.Data.m_FullLengthImage;
				SpriteLink mHalfLengthImage = blueprintPortrait.Data.m_HalfLengthImage;
				SpriteLink mPortraitImage = blueprintPortrait.Data.m_PortraitImage;

				//string defdirpath = Path.Combine(path, GetDefaultPortraitsDirName());

				//Directory.CreateDirectory(defdirpath);

				//Main.DebugLog("SaveOriginals: " + defdirpath);


				//Main.DebugLog(mFullLengthImage.AssetId);
				//Main.DebugLog("SaveOriginals: Trying to write files for " + bup.CharacterName + " - " + mPortraitImage.Load(true, false));
				string defdirpath = Path.Combine(path, GetDefaultPortraitsDirName());


				if (Main.settings.AutoBackup)			
					Directory.CreateDirectory(defdirpath);


		


				bool isNpc = false;

				try
				{



					if (bup.IsCompanion || bup.CharacterName == Game.Instance.Player.MainCharacter.Value.CharacterName)
					{
						if (isPortraitMissing(defdirpath, bup))
						{
							CreateBaseImages(Path.Combine(defdirpath, Main.smallName), mPortraitImage.Load(true, false), isNpc, false);
							CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), mHalfLengthImage.Load(true, false), isNpc, false);
							CreateBaseImages(Path.Combine(defdirpath, Main.fullName), mFullLengthImage.Load(true, false), isNpc, false);

							//Main.DebugLog("SaveOriginals: all Originals saved OK for " + bup.CharacterName + " in " + defdirpath);

						}
						else
						{
							//Main.DebugLog("SaveOriginals: all Originals already saved for " + bup.CharacterName + " in " + defdirpath);
						}
					}
					else
					{
						isNpc = true;
						bool secret = false;
						if (isPortraitMissing(defdirpath, bup))
						{
							Sprite medium = mHalfLengthImage.Load(true, false);

							Sprite small = mPortraitImage.Load(true, false);





							if (small.texture.width == medium.texture.width)
							{
								//Main.DebugLog("SECRET!!!");
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
								//Main.DebugLog("NPC with full portraits");

							}
							

							//Main.DebugLog("SaveOriginals: all Originals saved OK for " + bup.CharacterName + " in " + defdirpath);
						}
						else
						{
							//Main.DebugLog("SaveOriginals: all Originals already saved for " + bup.CharacterName + " in " + defdirpath);
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



								//	}
								//Main.DebugLog("SaveOriginals: Original saved as Custom portrait OK as " + Path.Combine(path, Main.mediumName));
							}
							else
							{
								//Main.DebugLog("SaveOriginals: Original already saved as Custom portrait as " + Path.Combine(path, Main.mediumName));
							}
						}
					}




					result = true;
				}
				catch (Exception ex)
				{

					Main.DebugLog("Disk, The process failed: " + ex.ToString());
					result = false;
				}

						

			}

			return result;
		}

		public static void CreateBaseImages(string path, Sprite baseSprite, bool npc, bool secret)
		{
			//baseSprite.texture.graphicsFormat
			//baseSprite.texture.format

			Texture2D texture = textureFromSprite(baseSprite, path.Contains("Medium"), npc, secret, path);

			//Texture2D texture = baseSprite.texture;
			/*if (path.Contains("Fulllength"))
			{

				if (texture.width < 692)
				{
					texture = null;
				}

			}*/

			if (texture != null)
			{

				byte[] bytes = texture.EncodeToPNG();

				//   ((line = reader.ReadLine()) != null)

				//Main.DebugLog("SaveOriginals88 "+path);


				File.WriteAllBytes(path, bytes);
				//Main.DebugLog("Disk, The file was created successfully at " + path);
			}
			else
			{
				//Main.DebugLog("Disk, not created: " + path);
			}
		}

		public static Texture2D textureFromSprite(Sprite sprite, bool med, bool npc, bool secret, string path)
		{
			
			if (sprite == null || sprite.texture == null || sprite.rect == null)
			{ return null; }
			

			if ((sprite.texture.width == sprite.rect.width)&&secret)
			{
				return duplicateTexture(sprite.texture, med, npc, secret, path);
			}
			else if (sprite.textureRect != null && sprite.textureRectOffset != null)
			{

				Texture2D readabelAtlas = duplicateTexture(sprite.texture, med, npc, secret, path);

				int texHei = (int)sprite.rect.height;

				if (texHei > (int)BlueprintRoot.Instance.CharGen.BasePortraitSmall.rect.height && texHei < (int)BlueprintRoot.Instance.CharGen.BasePortraitBig.rect.height)
				{
					texHei = (int)BlueprintRoot.Instance.CharGen.BasePortraitMedium.rect.height;
				}

				int texWid = (int)sprite.rect.width;

				if (texWid > (int)BlueprintRoot.Instance.CharGen.BasePortraitMedium.rect.width)
				{
					texWid = (int)BlueprintRoot.Instance.CharGen.BasePortraitBig.rect.width;
				}

	
				




				Texture2D newTex = new Texture2D(texWid, texHei);

				Color[] defaultPixels = Enumerable.Repeat<Color>(new Color(0, 0, 0, 0), texWid * texHei).ToArray();
				Color[] pixels = readabelAtlas.GetPixels(
				  (int)sprite.textureRect.x
				, (int)sprite.textureRect.y
				, (int)sprite.textureRect.width
				, (int)sprite.textureRect.height
				//, sprite.texture.calculatedMipmapLevel
				);
				newTex.SetPixels(defaultPixels);
				newTex.SetPixels((int)sprite.textureRectOffset.x, (int)sprite.textureRectOffset.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height, pixels);
				newTex.Apply();
				return newTex;
			}
			else { return null; }

		}


		private static Texture2D duplicateTexture(Texture2D source, bool med, bool npc, bool secret, string path)
		{
			RenderTexture renderTex;
			Texture2D readableText;
			try
			{
				if (med && npc && secret/*source.width == (int)BlueprintRoot.Instance.CharGen.BasePortraitSmall.texture.width*/)
				{
					//Main.DebugLog("Enlarging omly? " + path);
					renderTex = RenderTexture.GetTemporary(
								 330,
								 432,
								 0,
								 RenderTextureFormat.ARGB32,
								 RenderTextureReadWrite.sRGB
								 );
					readableText = new Texture2D(330, 432);

					/*if (!path.Contains("Game Default Portraits"))
						File.Delete(Path.Combine(Path.GetDirectoryName(path), "Small.png"));*/

				}
				else
				{
					//Main.DebugLog("No enlarging! " + path);

					renderTex = RenderTexture.GetTemporary(
								source.width,
								source.height,
								0,
								RenderTextureFormat.ARGB32,
								RenderTextureReadWrite.sRGB
								);


					readableText = new Texture2D(source.width, source.height);


				}

				Graphics.Blit(source, renderTex);
				RenderTexture previous = RenderTexture.active;
				RenderTexture.active = renderTex;

				readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
				readableText.Apply();
				RenderTexture.active = previous;
				RenderTexture.ReleaseTemporary(renderTex);
				return readableText;

			}
			catch (Exception e)
			{

				Main.DebugLog("dupliactor error: " + e);
				return null;

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