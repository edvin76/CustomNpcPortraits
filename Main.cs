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
using UniRx;
using Owlcat.Runtime.UI.MVVM;
using Kingmaker.UI.MVVM._VM.Crusade.ArmyInfo;
using Kingmaker.UI.MVVM._VM.TacticalCombat;
using Kingmaker.EntitySystem;
using Kingmaker.Armies;
using static Kingmaker.Inspect.InspectUnitsManager;
using Kingmaker.UnitLogic.Groups;
using Kingmaker.Globalmap.State;
using Kingmaker.Globalmap;
using Kingmaker.Armies.State;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.BundlesLoading;
using Kingmaker.UI.UnitSettings;
using System.Text.RegularExpressions;
using ExtensionMethods;
using Kingmaker.UI.Dialog;
using Kingmaker.Controllers.Dialog;
using Kingmaker.UnitLogic.Alignments;
using Kingmaker.DialogSystem;

namespace ExtensionMethods
{
	public static class MyExtensions
	{
		public static int WordCount(this string str)
		{
			return str.Split(new char[] { ' ', '.', '?' },
							 StringSplitOptions.RemoveEmptyEntries).Length;
		}

		public static string cleanCharname(this string filename)
		{
			string whitelist = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.-_ ";
			foreach (char c in filename)
			{
				if (!whitelist.Contains(c))
				{
					filename = filename.Replace(c, '-');
				}
			}
			return filename;
			//return Regex.Replace(filename, "[^a-zA-Z0-9_.-]+", "_", RegexOptions.Compiled);
		}
	}
}


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
			/*
			if (!Main.ApplyPatch(typeof(TacticalCombatInitiativeTrackerCurrentUnitPCView_BindViewImplementation_Patch), "TacticalCombatInitiativeTrackerCurrentUnitPCView_BindViewImplementation_Patch"))
			{
				throw Main.Error("Failed to patch TacticalCombatInitiativeTrackerCurrentUnitPCView_BindViewImplementation");
			}

			*/
			
			return true;
		}

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

			int i = 0;

			foreach (BlueprintList.Entry be in blueprintList.Entries)
			{




				if (be.TypeFullName.Equals("Kingmaker.DialogSystem.Blueprints.BlueprintCue"))
				{

					BlueprintCue bc = Utilities.GetBlueprintByGuid<BlueprintCue>(be.Guid);

					string characterName = "";


					if (bc.Speaker != null && bc.Speaker.Blueprint != null && !bc.Speaker.Blueprint.name.IsNullOrEmpty())
					{
						characterName = bc.Speaker.Blueprint.CharacterName;
						if (bc.Speaker.Blueprint.CharacterName.ToLower().Equals("asty"))
						{
							Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, "Asty"));
							characterName = "Asty - Drow";
						}
						if (bc.Speaker.Blueprint.CharacterName.ToLower().Equals("tran"))
						{
							Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, "Tran"));
							characterName = "Tran - Drow";
						}
						if (bc.Speaker.Blueprint.CharacterName.ToLower().Equals("velhm"))
						{
							Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, "Velhm"));
							characterName = "Velhm - Drow";
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

								//	Main.DebugError(e);


								}
							}
						}
					}


					if (bc.Listener != null && !bc.Listener.name.IsNullOrEmpty() )
					{
						try
						{

							characterName = bc.Listener.CharacterName;
							if (bc.Listener.CharacterName.ToLower().Equals("asty"))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, "Asty"));
								characterName = "Asty - Drow";
							}
							if (bc.Listener.CharacterName.ToLower().Equals("tran"))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, "Tran"));
								characterName = "Tran - Drow";
							}
							if (bc.Listener.CharacterName.ToLower().Equals("velhm"))
							{
								Directory.CreateDirectory(Path.Combine(portraitsDirectoryPath, "Velhm"));
								characterName = "Velhm - Drow";
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
				/*
				if (be.TypeFullName.EndsWith("BlueprintUnit"))
				{
					BlueprintUnit bu = Utilities.GetBlueprintByGuid<BlueprintUnit>(be.Guid);
					if (bu.PortraitSafe.name.ToLower().Contains("ciar"))
					{
						Main.DebugLog(bu.CharacterName +" - "+ bu.name + " - Companion: " + bu.IsCompanion.ToString() +" - "+ bu.Alignment +" - "+ bu.PortraitSafe.name);

						//string prefix = Main.GetCompanionPortraitDirPrefix();
						//string portraitsDirectoryPath2 = Main.GetCompanionPortraitsDirectory();
						//string portraitDirectoryName = bu.name;
						//string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath2, "--", portraitDirectoryName);

						//	SaveOriginals2(bc.Data, portraitDirectoryPath);
						//SaveOriginals(bu, CompanionPortraitPath);
						//savedComp = true;
						//break;
					}
				}
				*/
				/*
				if (be.TypeFullName.Contains("BlueprintPortrait"))
				{
					BlueprintPortrait bc = Utilities.GetBlueprintByGuid<BlueprintPortrait>(be.Guid);
					if (bc.name.ToLower().Contains("ciar"))
					{
						Main.DebugLog(bc.name);
						Main.DebugLog(bc.AssetGuid.ToString());
						//string prefix = Main.GetCompanionPortraitDirPrefix();
						string portraitsDirectoryPath2 = Main.GetCompanionPortraitsDirectory();
						string portraitDirectoryName = bc.name;
						string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath2, "--", portraitDirectoryName);

					//	SaveOriginals2(bc.Data, portraitDirectoryPath);
					}
				}
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
					if (bc.Speaker != null && bc.Speaker.Blueprint != null && !bc.Speaker.Blueprint.name.IsNullOrEmpty() && !speakers.ContainsKey(bc.Speaker.Blueprint.name) && !companions.Contains(bc.Speaker.Blueprint.CharacterName) )
					{
						if (!bc.Speaker.Blueprint.IsCompanion /* && !fyou.Contains(bc.Speaker.Blueprint.name)*/
				)
						{
					


								speakers.Add(bc.Speaker.Blueprint.name, bc.Speaker.Blueprint.CharacterName);

							//Main.DebugLog(speakers[bc.Speaker.Blueprint.name]);
							string portraitDirectoryName = bc.Speaker.Blueprint.CharacterName;
							string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

							if (bc.Speaker.Blueprint.name != bc.Speaker.Blueprint.CharacterName)

								Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, bc.Speaker.Blueprint.name));
else
															Directory.CreateDirectory(portraitDirectoryPath);



						}
					}
					if (bc.Listener != null && !bc.Listener.name.IsNullOrEmpty() && !speakers.ContainsKey(bc.Listener.name) && !companions.Contains(bc.Listener.CharacterName))
					{
						if (!bc.Listener.IsCompanion)
						{

							speakers.Add(bc.Listener.name, bc.Listener.CharacterName);
							//Main.DebugLog(bc.Listener.CharacterName);
							string portraitDirectoryName = bc.Listener.CharacterName;
							string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

							if (bc.Listener.name != bc.Listener.CharacterName)

								Directory.CreateDirectory(Path.Combine(portraitDirectoryPath, bc.Listener.name));
							else
							Directory.CreateDirectory(portraitDirectoryPath);


						}
					}

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
		private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			//bool globalMap = false;
		//	if (Game.Instance.CurrentMode == GameModeType.GlobalMap)
				//&& (SceneManager.GetActiveScene().name.Equals("WorldWoundGM_Light")))
			//	globalMap = true;

			switch (state)
			{
				case State.Start:
					//DebugLog("run once");

					string name = SceneManager.GetActiveScene().name;
					//Main.DebugLog("SceneManager.GetActiveScene().name: " + name);
					//Main.DebugLog("Game.Instance.State: " + Game.Instance.State);
					Game instance = Game.Instance;
					/*
					Main.DebugLog("Game.Instance.CurrentMode: " + instance.CurrentMode);

					Main.DebugLog("Game.Instance.State: " + Game.Instance.State);*/
				





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

			GUIStyle boldStyle2 = new GUIStyle();
			boldStyle2.fontStyle = FontStyle.Bold;
			//Color c = new Color((float)131.0, (float)192.0, (float)239.0);
			boldStyle2.normal.textColor = Color.red;

			GUILayoutOption[] defSize = new GUILayoutOption[]
			{
				GUILayout.Width(200f),
				GUILayout.Height(20f)
			};

			//GrayGarrisonBasement1From1stFloor
			string target = "DefendersHeart_DefendersHeart_Enter";

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
#if DEBUG
				//	loctel();


				//BlueprintCue b = Game.Instance.DialogController.CurrentCue;


				//Main.DebugLog(Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait.ToReference<BlueprintUnitReference>().ToString());

				//Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait.ToReference<BlueprintUnitReference>()


				//Main.DebugLog(Game.Instance.DialogController.CurrentSpeakerBlueprint.ToReference<BlueprintUnitReference>().ToString());

				//Main.DebugLog(typeof(BlueprintCue).GetField("m_SpeakerPortrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Game.Instance.DialogController.CurrentCue.Speaker).ToString());
				
				//typeof(DialogSpeaker).GetField("m_SpeakerPortrait", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Game.Instance.DialogController.CurrentCue.Speaker, null);

				CueShowData cueShowDatum = new CueShowData(Game.Instance.DialogController.CurrentCue, new List<SkillCheckResult>(), new List<AlignmentShift>());
				EventBus.RaiseEvent<IDialogCueHandler>((IDialogCueHandler h) => h.HandleOnCueShow(cueShowDatum), true);
				
				//Main.DebugLog(Game.Instance.DialogController.CurrentCue.AssetGuid.ToString());

				//Main.DebugLog("ongui " + Game.Instance.DialogController.CurrentCue.Speaker.SpeakerPortrait.ToReference<BlueprintUnitReference>().ToString());

				//45450b2f327797e41bce701b91118cb4
#else
				Process.Start(GetCompanionPortraitsDirectory());
#endif
			}
			GUILayout.Label(GetCompanionPortraitsDirectory());
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Open NPC portraits dir", GUILayout.Width(200f), GUILayout.Height(20f)))
			{
				#if DEBUG
				BlueprintAreaEnterPoint bap = Utilities.GetBlueprint<BlueprintAreaEnterPoint>(target);

				Game.Instance.LoadArea(bap, AutoSaveMode.None);
#else
				Process.Start(GetNpcPortraitsDirectory());
#endif
			}
			GUILayout.Label(GetNpcPortraitsDirectory());
			GUILayout.EndHorizontal();



			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			if (GUILayout.Button("Open Army portraits dir", GUILayout.Width(200f), GUILayout.Height(20f)))
			{

				Process.Start(GetArmyPortraitsDirectory());

			}
			GUILayout.Label(GetArmyPortraitsDirectory());
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
					if (Main.settings.ManageCompanions)
						SetPortraits();
				}
				if (Main.settings.ManageCompanions)

					GUILayout.Label("Use this after replacing the portrait of a party companion.");
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
					GUILayout.Label("Save all NPC-s' turn based portraits in their respective dirs: " + portraitCounter + " have been saved OK! ( "+ failCounter + " invalid )");
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
						bool companion = Main.companions.Contains(CurrentSpeakerName);

						if (CurrentSpeakerName.Equals("Asty"))
						{
							if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
								CurrentSpeakerName = "Asty - Drow";
						}
						if (CurrentSpeakerName.Equals("Velhm"))
						{
							if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
								CurrentSpeakerName = "Velhm - Drow";
						}
						if (CurrentSpeakerName.Equals("Tran"))
						{
							if (Game.Instance.DialogController.CurrentSpeaker.View.name.ToLower().Contains("drow"))
								CurrentSpeakerName = "Tran - Drow";
						}

						string CurrentSpeakerBlueprintName = Game.Instance.DialogController.CurrentSpeakerBlueprint.name;

						string NpcPortraitPath = Path.Combine(GetNpcPortraitsDirectory(), CurrentSpeakerName);

						

						if (!companion)
						{
							if (Directory.Exists(NpcPortraitPath) || Directory.Exists(NpcPortraitPath))
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
							//BlueprintPortrait blueprintPortrait = null;

							string CompanionPortraitPath = Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName);


							if (CurrentSpeakerName.ToLower().Contains("aruesh")&& Main.settings.AutoBackup && !savedComp)
							{
								if (Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.Race.name.ToLower().Contains("succubusrace"))
								{
									savedComp = true;

									CurrentSpeakerName = CurrentSpeakerName + " - Evil";
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("484588d56f2c2894ab6d48b91509f5e3");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}
							}

							if (CurrentSpeakerName.Equals("Nenio")&&Main.settings.AutoBackup && !savedComp)
							{
								if (Game.Instance.DialogController.CurrentSpeaker.GetActivePolymorph().Component == null)
								{
									savedComp = true;

									CurrentSpeakerName = "NenioFox_Portrait";
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}
							}

							if (CurrentSpeakerName.Equals("Ciar")&& Main.settings.AutoBackup && !savedComp)
							{

								if (Game.Instance.DialogController.CurrentSpeaker.View.Blueprint.Alignment == Kingmaker.Enums.Alignment.LawfulEvil)
								{
									Main.DebugLog("2");
									savedComp = true;

									CurrentSpeakerName = "Ciar - Undead";
									BlueprintPortrait blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("dc2f02dd42cfe2b40923eb014591a009");
									Main.SaveOriginals2(blueprintPortrait.Data, Path.Combine(GetCompanionPortraitsDirectory(), GetCompanionPortraitDirPrefix() + CurrentSpeakerName));
								}
     
							}
							if (Directory.Exists(CompanionPortraitPath))
							{
								if (Main.settings.AutoBackup && !savedComp)
								{
									Main.DebugLog("3");

									SaveOriginals(Game.Instance.DialogController.CurrentSpeaker.View.Blueprint, CompanionPortraitPath);
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

				}


			}

			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Advanced", boldStyle);
			GUILayout.EndHorizontal();
			Main.settings.ManageCompanions = GUILayout.Toggle(Main.settings.ManageCompanions, "Let the mod manage the portraits of companions and mercenaries.", new GUILayoutOption[0]);

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
						GUILayout.Label("  � <b>" + str + "</b>", noExpandwith);
					}
					GUILayout.EndVertical();
				}
				if (Main.failedPatches.Count > 0)
				{
					GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
					GUILayout.Label("<b>Error: Some patches failed to apply. These features may not work:</b>", noExpandwith);
					foreach (string str2 in Main.failedPatches)
					{
						GUILayout.Label("  � <b>" + str2 + "</b>", noExpandwith);
					}
					GUILayout.EndVertical();
				}
				if (Main.okLoading.Count > 0)
				{
					GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
					GUILayout.Label("<b>OK: Some assets loaded:</b>", noExpandwith);
					foreach (string str3 in Main.okLoading)
					{
						GUILayout.Label("  � <b>" + str3 + "</b>", noExpandwith);
					}
					GUILayout.EndVertical();
				}
				if (Main.failedLoading.Count > 0)
				{
					GUILayout.BeginVertical(Array.Empty<GUILayoutOption>());
					GUILayout.Label("<b>Error: Some assets failed to load. Saves using these features won't work:</b>", noExpandwith);
					foreach (string str4 in Main.failedLoading)
					{
						GUILayout.Label("  � <b>" + str4 + "</b>", noExpandwith);
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

		private static bool showExperimental = false;

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
			showExperimental = false;
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
		public static List<string> companions = new List<string>{"Arueshalae",
"Camellia",
"Ciar",
"Daeran",
"Delamere",
"Ember",
"Finnean",
"Greybor",
"Kestoglyr",
"Lann",
"Nenio",
"Queen Galfrey",
"Regill",
"Seelah",
"Sosiel",
"Staunton",
"Wenduag",
"Woljif" };

		public static bool isSetPortrait = false;
		public static PortraitData SetPortrait(UnitEntityData unitEntityData)
		{
			//Main.DebugLog("SetPortrait()");

			string prefix = Main.GetCompanionPortraitDirPrefix();
			string portraitsDirectoryPath = Main.GetCompanionPortraitsDirectory();
			string characterName = unitEntityData.CharacterName.cleanCharname();


			if (characterName.ToLower().Contains("aruesh"))
			{
				if (unitEntityData.View.Blueprint.Race.name.ToLower().Contains("succubusrace"))
				{
					characterName = characterName + " - Evil";
				}
			}


			if (characterName.Equals("Nenio"))
			{
				if (unitEntityData.GetActivePolymorph().Component == null)
				{

				//	Main.DebugLog("fox");
					characterName = "NenioFox_Portrait";

				}
				else
				{
					//Main.DebugLog("human"); 
				}
			}
			
			//Main.DebugLog("SetPortrait() 1");

			string portraitDirectoryName = prefix + characterName;

			string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

			Directory.CreateDirectory(portraitDirectoryPath);
			BlueprintPortrait blueprintPortrait = (BlueprintPortrait)typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Descriptor.UISettings);

		//	Main.DebugLog("SetPortrait() 2");

			if(blueprintPortrait != null)
				if (Main.settings.AutoBackup) SaveOriginals2(blueprintPortrait.Data, portraitDirectoryPath);
			if (blueprintPortrait != null)
				if (Main.settings.AutoBackup) SaveOriginals(unitEntityData.Blueprint, portraitDirectoryPath);
			bool missing = false;

			//Main.DebugLog("SetPortrait() 2b");

			foreach (string fileName in Main.PortraitFileNames)
			{
				if (!File.Exists(Path.Combine(portraitDirectoryPath, fileName)))
				{
					missing = true;
				}
			}

			//Main.DebugLog("SetPortrait() 3");

			if (!missing)
			{
				//Main.DebugLog("SetPortrait() 4");

				if (blueprintPortrait != null)
				{
					//Main.DebugLog("SetPortrait() 5");

					BlueprintPortrait oldP = blueprintPortrait;
					Main.pauseGetPortraitsafe = true;
					blueprintPortrait.Data.m_PetEyeImage = oldP.Data.m_PetEyeImage;
					Main.pauseGetPortraitsafe = false;
				}

				blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;
			//	Main.DebugLog("SetPortrait() 6");


				CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
				CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
				CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));

				//Main.DebugLog("SetPortrait() 7 " + portraitDirectoryPath);

				blueprintPortrait.Data = new PortraitData(portraitDirectoryPath);

			//	Main.DebugLog("SetPortrait() 8");

				return blueprintPortrait.Data;
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
						UIRoot uIRoot = BlueprintRoot.Instance.UIRoot;

						blueprintPortrait = uIRoot.LeaderPlaceholderPortrait;


					}
				}

				if (blueprintPortrait.BackupPortrait != null)
					blueprintPortrait = blueprintPortrait.BackupPortrait;

				if (characterName.Equals("Nenio"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("a6e4ff25a8da46a44a24ecc5da296073");
				if (characterName.Equals("NenioFox_Portrait"))
					blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");

				

				Main.pauseGetPortraitsafe = true;
				blueprintPortrait.Data.m_PetEyeImage = unitEntityData.Blueprint.PortraitSafe.Data.m_PetEyeImage;
				Main.pauseGetPortraitsafe = false;
			//	Main.DebugLog("SetPortrait() no custom portrait");

				return blueprintPortrait.Data;
				//isSetPortrait = true;
				//unitEntityData.Descriptor.UISettings.SetPortrait(blueprintPortrait);
				//isSetPortrait = false;
				//return;
			}
		}


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
		//	Main.DebugLog("SetPortraits() 1");

			foreach (UnitEntityData unitEntityData in allAvailableCommpanions)
			{
				string characterName = unitEntityData.CharacterName.cleanCharname();

//				Main.DebugLog("SetPortraits()"+ characterName);
				if (characterName.ToLower().Contains("aruesh"))
				{
					if (unitEntityData.View.Blueprint.Race.name.ToLower().Contains("succubusrace"))
                    {

						characterName = characterName + " - Evil";


					}
							//Main.DebugLog(unitEntityData.View.Blueprint.Race.name);
				}

				if (characterName.Equals("Ciar"))
				{
					if (unitEntityData.View.Blueprint.Alignment.ToString().ToLower().Contains("evil"))
						characterName = "Ciar - Undead";

				}
				//AscendingSuccubus
				/*}
				bool b = false;
				if(b)
				{*/
				//UnitEntityData unitEntityData = null;

				//string characterName = unitEntityData.CharacterName.cleanCharname();
				if (characterName.Equals("Nenio"))
				{
					if (unitEntityData.GetActivePolymorph().Component == null)
					{

				//	Main.DebugLog("setp fox");
						characterName = "NenioFox_Portrait";

					}
					else
					{
					//	Main.DebugLog("setp human"); 
					}
				}

				string portraitDirectoryName = prefix + characterName;

				//Main.DebugLog(portraitsDirectoryPath + " - " + portraitDirectoryName);
				string portraitDirectoryPath = Path.Combine(portraitsDirectoryPath, portraitDirectoryName);

				Directory.CreateDirectory(portraitDirectoryPath);


				/*			

							foreach (BlueprintUnlockableFlag bf in Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys)
							{


								//	if (Game.Instance.Player.UnlockableFlags.UnlockedFlags.Keys.FirstOrDefault(x => x.name.Contains("Fox")))
								//{
								if (bf.name.Contains("Fox"))
									Main.DebugLog(bf.name);

								//}
							}
			*/

				BlueprintPortrait blueprintPortrait = (BlueprintPortrait)typeof(UnitUISettings).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Descriptor.UISettings);



				//if(unitEntityData.Portrait.HasPortrait)
				if (blueprintPortrait != null)
					if (Main.settings.AutoBackup) SaveOriginals2(blueprintPortrait.Data, portraitDirectoryPath);
				if (blueprintPortrait != null)
					if (Main.settings.AutoBackup) SaveOriginals(unitEntityData.Blueprint, portraitDirectoryPath);
				
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
					blueprintPortrait = BlueprintRoot.Instance.CharGen.CustomPortrait;

					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Small.png"));
					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Medium.png"));
					CustomPortraitsManager.Instance.Storage.Unload(Path.Combine(portraitDirectoryPath, "Fulllength.png"));

					blueprintPortrait.Data = new PortraitData(portraitDirectoryPath);
					Main.pauseGetPortraitsafe = true;
						blueprintPortrait.Data.m_PetEyeImage = unitEntityData.Blueprint.PortraitSafe.Data.m_PetEyeImage;
					Main.pauseGetPortraitsafe = false;


					isSetPortrait = true;
					unitEntityData.Descriptor.UISettings.SetPortrait(blueprintPortrait);
					//Game.Instance.DialogController.CurrentSpeaker.UISettings.SetPortrait(blueprintPortrait);

					isSetPortrait = false;
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

						//Main.DebugLog("b");
						blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Blueprint);
				
					//Main.pauseGetPortraitsafe = true;
					//blueprintPortrait = unitEntityData.Blueprint.PortraitSafe;
					//Main.pauseGetPortraitsafe = false;
					if (blueprintPortrait == null)
					{
						//Main.DebugLog("c");

						//Main.DebugLog("unitBlueprintPortrait: " + blueprintPortrait.name);
						//		Main.DebugLog("BlueprintUnit: " + unitEntityData.Blueprint.name);
						//UIRoot uIRoot = BlueprintRoot.Instance.UIRoot;

						//blueprintPortrait = uIRoot.LeaderPlaceholderPortrait;




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
					

				//	if(blueprintPortrait.BackupPortrait != null)
				//	blueprintPortrait = blueprintPortrait.BackupPortrait;
					//	if (blueprintPortrait.name.Equals("Empty_Portrait"))

					//{

					//blueprintPortrait = uIRoot.LeaderPlaceholderPortrait;
					/*

					*/
					//}
			//		Main.DebugLog("setp no custom portrait" + characterName);


					//blueprintPortrait.Data = new PortraitData(Path.Combine(portraitDirectoryPath, Main.GetDefaultPortraitsDirName()));
					/*
					 [CustomNpcPortraits] NenioFox_Portrait
					[CustomNpcPortraits] 2b4b8a23024093e42a5db714c2f52dbc
					[CustomNpcPortraits] NenioHuman_Portrait
					[CustomNpcPortraits] a6e4ff25a8da46a44a24ecc5da296073
					 */

					if (characterName.Equals("Nenio"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("a6e4ff25a8da46a44a24ecc5da296073");
					if (characterName.Equals("NenioFox_Portrait"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("2b4b8a23024093e42a5db714c2f52dbc");

					if (characterName.Equals("Arueshalae"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("db413e67276547b40b1a6bb8178c6951"); 
					if (characterName.Equals("Arueshalae - Evil"))
						blueprintPortrait = Utilities.GetBlueprintByGuid<BlueprintPortrait>("484588d56f2c2894ab6d48b91509f5e3");
					/*
[CustomNpcPortraits] ArueshalaeEvil_Portrait
[CustomNpcPortraits] 484588d56f2c2894ab6d48b91509f5e3
[CustomNpcPortraits] Arueshalae_Portrait
[CustomNpcPortraits] db413e67276547b40b1a6bb8178c6951
						blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(unitEntityData.Blueprint);

*/



					Main.pauseGetPortraitsafe = true;
					blueprintPortrait.Data.m_PetEyeImage = unitEntityData.Blueprint.PortraitSafe.Data.m_PetEyeImage;
					Main.pauseGetPortraitsafe = false;

					isSetPortrait = true;
					unitEntityData.Descriptor.UISettings.SetPortrait(blueprintPortrait);
				//	Game.Instance.DialogController.CurrentSpeaker.UISettings.SetPortrait(blueprintPortrait);

					isSetPortrait = false;


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
			//Game.Instance.UI.Common.Initialize();

		}

		public static bool pauseGetPortraitsafe = false;

		public static bool SaveOriginals(BlueprintUnit bup, string path)
		{

			pauseGetPortraitsafe = true;

			bool result = false;





			//	Main.DebugLog("SaveOriginals() no backups yet");
			BlueprintPortrait blueprintPortrait = (BlueprintPortraitReference)typeof(BlueprintUnit).GetField("m_Portrait", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(bup);

			//BlueprintPortrait blueprintPortrait = bup.PortraitSafe;

			pauseGetPortraitsafe = false;




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
						if (isPortraitMissing(defdirpath, bup))
						{
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


				if (Main.settings.AutoBackup)
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
							{
				//				Main.DebugLog("medium");
								CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), mHalfLengthImage.Load(true, false), true, true);
					//			Main.DebugLog("small");
								CreateBaseImages(Path.Combine(defdirpath, Main.smallName), small, isNpc, false);
							}
						}
						else

						{
				//			Main.DebugLog("NPC with full portraits");

							secret = false;
							if (Main.settings.AutoBackup)
							{
								CreateBaseImages(Path.Combine(defdirpath, Main.mediumName), medium, isNpc, secret);
								CreateBaseImages(Path.Combine(defdirpath, Main.smallName), small, isNpc, secret);
								CreateBaseImages(Path.Combine(defdirpath, Main.fullName), mFullLengthImage.Load(true, false), false, secret);
							}

						}


					}

			




					result = true;
				}
				catch (Exception ex)
				{
					failCounter++;


					//Main.DebugLog("Disk, The process failed: " + ex.ToString());
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

				

				File.WriteAllBytes(path, bytes);
				//Main.DebugLog("Disk, The file was created successfully at " + path);
			}
			else
			{
			//	Main.DebugLog("Disk, not created: " + path);
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
				try
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
				catch (Exception e)
				{
					failCounter++;

					Main.DebugLog("textureFromSprite error: " + e);
					return null;

				}
			}
			else {
				failCounter++;

				return null; }

		}


		private static Texture2D duplicateTexture(Texture2D source, bool med, bool npc, bool secret, string path)
		{
			RenderTexture renderTex;
			Texture2D readableText;
			try
			{
				if (med && npc && secret/*source.width == (int)BlueprintRoot.Instance.CharGen.BasePortraitSmall.texture.width*/)
				{
				//	Main.DebugLog("Enlarging omly? " + path);
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