//
// Hambeard's SimpleDialog classes for creating dialog for NPC's that have bark
// The only thing special that is needed is a publicized version of Assembly-CSharp_public
// Follow the Wiki: https://github.com/WittleWolfie/OwlcatModdingWiki/wiki/Publicize-Assemblies on how to accomplish generating your own. 
//

//using HarmonyLib;
using Harmony12;
using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Controllers.Dialog;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.DialogSystem;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.UI.Common;
using Kingmaker.UnitLogic.Interaction;
using Kingmaker.UnitLogic.Mechanics;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static CustomNpcPortraits.SimpleDialogBuilder;
//using CustomNpcPortraits.Simpledialogs;

namespace CustomNpcPortraits
{
    public class SimpleDialog
    {
        private BarkDestoryer BarkDestoryer;
        public BlueprintDialogReference Dialog { get; private set; }
        public SimpleDialogUnit DialogUnit { get; private set; }

        public SimpleDialog(SimpleDialogUnit unit, BlueprintDialogReference dialog)
        {
            DialogUnit = unit;
            Dialog = dialog;
        }

        // Should be executed at launch to setup the dialogs. If a bark blueprint is provided it will setup the startdialog through it otherwise it will just the DialogOnClick method.
        public void Init()
        {
            if (DialogUnit.BarkGUID != "")
                BarkDestoryer = new BarkDestoryer(DialogUnit, Dialog);
            else
            {
                var comp = new DialogOnClick()
                {


                    m_Dialog = Dialog,
                    Conditions = Empties.Conditions,
                    NoDialogActions = Empties.Actions,
                };

                //DialogUnit.Unit.Components = DialogUnit.Unit.Components.AddItem(comp).ToArray();



                DialogUnit.Unit.AddComponent<DialogOnClick>(dialogOnClick =>
                {
                    dialogOnClick.m_Dialog = comp.m_Dialog;
                    dialogOnClick.name = comp.name;
                    dialogOnClick.NoDialogActions = new ActionList();
                    dialogOnClick.Conditions = new ConditionsChecker();
                });




            }
        }



    }

    public static class Extensions
    {
        public static void AddComponent<T>(this BlueprintScriptableObject obj, Action<T> init = null) where T : BlueprintComponent, new()
        {
            obj.SetComponents(obj.ComponentsArray.AppendToArray(Create(init)));
        }


        public static void SetComponents(this BlueprintScriptableObject obj, params BlueprintComponent[] components)
        {
            // Fix names of components. Generally this doesn't matter, but if they have serialization state,
            // then their name needs to be unique.
            var names = new HashSet<string>();
            foreach (var c in components)
            {
                if (string.IsNullOrEmpty(c.name))
                {
                    c.name = $"${c.GetType().Name}";
                }
                if (!names.Add(c.name))
                {
                    String name;
                    for (int i = 0; !names.Add(name = $"{c.name}${i}"); i++) ;
                    c.name = name;
                }
            }
            obj.ComponentsArray = components;
            obj.OnEnable(); // To make sure components are fully initialized
        }

        public static T[] AppendToArray<T>(this T[] array, T value)
        {
            var len = array.Length;
            var result = new T[len + 1];
            Array.Copy(array, result, len);
            result[len] = value;
            return result;
        }
        public static T[] AppendToArray<T>(this T[] array, params T[] values)
        {
            var len = array.Length;
            var valueLen = values.Length;
            var result = new T[len + valueLen];
            Array.Copy(array, result, len);
            Array.Copy(values, 0, result, len, valueLen);
            return result;
        }

        public static T Create<T>(Action<T> init = null) where T : new()
        {
            var result = new T();
            init?.Invoke(result);
            return result;
        }

        public static BlueprintGuid ToGUID(this string guid)
        {
            return new BlueprintGuid(Guid.Parse(guid));
        }
    }



    //
    // This is the BlueprintUnit that will have a dialog added to. Note: This will affect every unit that uses the Blueprint, you will notice all the succubi in the area have the new dialog. You'll need to clone
    // them so they are unique if you don't want to change all of them. The contructor takes the guid of the BlueprintUnit that is to be changed and the optional guid of the ActionsHolder you wish to overwrite. Note:
    // ActionsHolders are not SimpleBlueprint's, they are ElementsScriptableObject's
    //

    public class SimpleDialogUnit
    {
        public string GUID { get; private set; }
        public string BarkGUID { get; private set; }
        public BlueprintUnit Unit { get; private set; }
        public BlueprintUnitReference UnitReference { get; private set; }

        public SimpleDialogUnit(string guid, string barkGUID = "")
        {
            GUID = guid;
            Unit = ResourcesLibrary.TryGetBlueprint<BlueprintUnit>(guid);
            UnitReference = Unit.ToReference<BlueprintUnitReference>();
            BarkGUID = barkGUID;
        }
    }

    //
    // This class controls creating the dialog tree along intializing each unit's tree. Initialize() should be called at launch after the BlueprintCache is loaded.
    // 
    public static class SimpleDialogController
    {
        public static readonly SimpleDialogUnit citizen = new SimpleDialogUnit("7001e2a58c9e86e43b679eda8a59f12f");                                     // Just a citizen npc with no bark data. You can spawn him from the UMM Menu. They will spawn on top of you so you'll have to move.
        public static readonly SimpleDialogUnit succ = new SimpleDialogUnit("0d88d5c310fac90449bfd0714bb9f810", "63a4a91f33825f34baf4a99b7989feff");    // Succubi in the pool area, guid to the bark data as well.

        //private static readonly SimpleDialogUnit succ2 = new SimpleDialogUnit("");    // Succubi in the pool area, guid to the bark data as well.




        public static Dictionary<SimpleDialogUnit, SimpleDialog> Dialogs = new Dictionary<SimpleDialogUnit, SimpleDialog>()
        {
            {citizen, new SimpleDialog(citizen, CreateDialog("simpledialog.citizen.base", "CB805DEE-7C4F-4F6A-AF6A-3CC8100CAEE2", new List<BlueprintCueBaseReference>()
                {
                    CreateCue("simpledialog.citizen.greet", "BCE7D981-4578-4AF2-A48D-E86C439B66E0", citizen.UnitReference, "I am just some bum who had no dialog but I can speak now!", new List<BlueprintAnswerBaseReference>()
                    {
                        CreateAnswerList("simpledialog.citizen.greet.answerlist", "785607FF-2B69-44AC-B959-CBAD6BED959C", new List<BlueprintAnswerBaseReference>()
                        {
                            CreateAnswer("simpledialog.citizen.greet.answer.good", "9D4E451B-F6DF-4425-8345-94324D6DA455", "You sound great!", new List<BlueprintCueBaseReference>()
                            {
                                CreateCue("simpledialog.citizen.exit.good", "E82E6E36-3EC4-4EF5-A9C5-16E2F94529E1", citizen.UnitReference, "Thanks, I am off to tell the world!", new List<BlueprintAnswerBaseReference>())
                            }),
                            CreateAnswer("simpledialog.citizen.greet.answer.bad", "C343C70D-C063-4B43-A131-F0065AC8466A", "I liked you better when you couldn't talk.", new List<BlueprintCueBaseReference>()
                            {
                                CreateCue("simpledialog.citizen.exit.bad", "253E4E65-CFCB-48B3-B7C3-44D738343DA4", citizen.UnitReference, "Maybe I should drown you in the pool of water.", new List<BlueprintAnswerBaseReference>())
                            })
                        })
                    })
                }))
            },
            {succ, new SimpleDialog(succ, CreateDialog("simpledialog.succ.base", "70CCF083-9C37-4E14-BBF2-E66FFB0C816C", new List<BlueprintCueBaseReference>()
                {
                    CreateCue("simpledialog.succ.greet", "88C55921-524A-427D-8451-39B2FC8C2E7C", succ.UnitReference, "Hello there! This is a test! How do you feel today?", new List<BlueprintAnswerBaseReference>()
                    {
                        CreateAnswerList("simpledialog.succ.greet.answerlist", "84671A44-7CBF-4680-A0A4-604EB9387FC6", new List<BlueprintAnswerBaseReference>()
                        {
                            CreateAnswer("simpledialog.succ.greet.answer.good", "2ABB5DF5-72E0-44B7-9208-6E9E5B0D9D9C", "Hi I am feeling great!", new List<BlueprintCueBaseReference>()
                            {
                                CreateCue("simpledialog.succ.exit.good", "73BDD0CD-1CC2-4E95-8E78-7C89357BB2C6", succ.UnitReference, "I am glad to hear that!", new List<BlueprintAnswerBaseReference>())
                            }),
                            CreateAnswer("simpledialog.succ.greet.answer.bad", "93D45D94-485A-402F-947D-ABC3D3DDE760", "I feel like crap!", new List<BlueprintCueBaseReference>()
                            {
                                CreateCue("simpledialog.succ.exit.bad", "1A1F3585-723E-4B10-80DF-8D58F1AAD1E8", succ.UnitReference, "You'd feel better if you were sitting in the water", new List<BlueprintAnswerBaseReference>())
                            })
                        })
                    })
                }))
            }
        };

        public static void Initialize()
        {
            foreach (var kvp in Dialogs)
                kvp.Value.Init();
        }
    }

    //
    // The class that replaces all the actions in the ActionsHolder bark data with a simple StartDialog() action. This class could be expanded to provide more advanced features like random dialogs
    // each time you start talking to the NPC.
    //

    public class BarkDestoryer
    {
        public BarkDestoryer(SimpleDialogUnit unit, BlueprintDialogReference dialog)
        {
            var ah = ResourcesLibrary.TryGetScriptable<ActionsHolder>(unit.BarkGUID).Actions = 
            new ActionList()
            {
                Actions = new GameAction[]
                {
                    new StartDialog()
                    {
                        m_Dialogue = dialog
                    }
                }
            };
        }
    }

    //
    // Calls the controller to initialize everthing after BlueprintsCache has been initialized.
    //


    [HarmonyPatch(typeof(StartDialog), "Dialogue", MethodType.Getter)]
    public static class DialogOnClick_Constructor_Patch
    {

        //   [HarmonyPriority(Priority.First)]
        //   [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
        public static bool Prefix(StartDialog __instance, ref BlueprintDialog __result, UnitEvaluator ___DialogueOwner, BlueprintEvaluator ___DialogEvaluator)
        {
            /*Main.DebugLog("startd pre");

            if (___DialogueOwner != null)
                Main.DebugLog("(getter) "+___DialogueOwner.GetValue().Proxy.Id);
            else
                Main.DebugLog("(getter) " + __instance.Owner.name);


          */

            /*

            var unitRef = ___DialogueOwner.GetValue().Blueprint.ToReference<BlueprintUnitReference>();

            if (___DialogueOwner.GetValue().Proxy.Id.Equals("19DB1E"))
            {
                __result = SimpleDialogBuilder.CreateDialog("simpledialog.succ.base2", "70CCF083-9C37-4E14-BBF2-E66FFB0C816D", new List<BlueprintCueBaseReference>()
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
                }).Get();
            }
            else if (___DialogueOwner.GetValue().Proxy.Id.Equals("19D923"))
            {
                __result = SimpleDialogBuilder.CreateDialog("simpledialog.succ.base3", "70CCF083-9C37-4E14-BBF2-E66FFB0C816D", new List<BlueprintCueBaseReference>()
                {
                    SimpleDialogBuilder.CreateCue("simpledialog.succ.greet2", "88C55921-524A-427D-8451-39B2FC8C2E7D", unitRef, "3 Hello there! This is a test! How do you feel today?", new List<BlueprintAnswerBaseReference>()
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
                }).Get();
                //__instance.StartDialog(dialog, initiator, unit, null, null);
            }
            else
                return false;
            //Main.DebugLog("const pre");
            */
            return true;
        }

        public static void Postfix(StartDialog __instance)
        {
            /*
            if (__instance.DialogueOwner != null)
                Main.DebugLog("3 const post: " + __instance.DialogueOwner.GetValue().Proxy.Id);
            else
                Main.DebugLog("3 const post no");


            if (__instance.DialogEvaluator != null)
                Main.DebugLog("3 const post: " + __instance.DialogEvaluator.Owner.name);
            else
                Main.DebugLog("3 const post no 2");
            */

            
        }


    }


    [HarmonyPatch(typeof(StartDialog), "RunAction", MethodType.Normal)]
    public static class DialogOnClick_Patch
    {

        //   [HarmonyPriority(Priority.First)]
        //   [HarmonyPatch(nameof(BlueprintsCache.Init)), HarmonyPostfix]
        public static void Postfix(StartDialog __instance)
        {
            /*
            UIUtility.SendWarning(Game.Instance.CursorController.m_HoveredUnitData.CharacterName);


            if (__instance.DialogueOwner != null)
                Main.DebugLog("2 const post: " + __instance.DialogueOwner.GetValue().Proxy.Id);
            else
                Main.DebugLog("2 const post no");


            if (__instance.DialogEvaluator != null)
                Main.DebugLog("2 const post: " + __instance.DialogEvaluator.Owner.name);
            else
                Main.DebugLog("2 const post no 2");
         */   
        }

        public static bool Prefix(StartDialog __instance, UnitEvaluator ___DialogueOwner, BlueprintEvaluator ___DialogEvaluator)
        {
            /*
            if (__instance.DialogueOwner != null)
                Main.DebugLog("const post: " + __instance.DialogueOwner.GetValue().Proxy.Id);
            else
                Main.DebugLog("const post no");


            if (__instance.DialogEvaluator != null)
                Main.DebugLog("const post: " + __instance.DialogEvaluator.Owner.name);
            else
                Main.DebugLog("const post no 2");
            */

            /* Main.DebugLog("1");

             Main.DebugLog(__instance.OwnerBlueprint.name);

             foreach (var unit in Game.Instance.State.Units)
             {
                 if (unit.Proxy.Id.Equals("19D923"))
                 {
                     var unitRef = unit.Blueprint.ToReference<BlueprintUnitReference>();
                     __result = SimpleDialogBuilder.CreateDialog("simpledialog.succ.base2", "70CCF083-9C37-4E14-BBF2-E66FFB0C816D", new List<BlueprintCueBaseReference>()
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
                    }).Get();

                     return false;
                 }

             }*/

            return true;

        }
    }

    

    //
    // Just some helpers to ensure there are no null objects in the dialog trees
    //
    public static class Empties
    {
        public static readonly ActionList Actions = new ActionList() { Actions = new GameAction[0] };
        public static readonly ConditionsChecker Conditions = new ConditionsChecker() { Conditions = new Condition[0] };
        public static readonly ContextDiceValue DiceValue = new ContextDiceValue()
        {
            DiceType = DiceType.Zero,
            DiceCountValue = 0,
            BonusValue = 0
        };
        public static readonly LocalizedString String = new LocalizedString();
        public static readonly PrefabLink PrefabLink = new PrefabLink();
        public static readonly ShowCheck ShowCheck = new ShowCheck();
        public static readonly CueSelection CueSelection = new CueSelection();
        public static readonly CharacterSelection CharacterSelection = new CharacterSelection();
        public static readonly DialogSpeaker DialogSpeaker = new DialogSpeaker() { NoSpeaker = true };
    }

    //
    // This is main class that will assit in building a very simple dialog tree. Expand as you see fit.
    //
    public class SimpleDialogBuilder
    {
        // This method must be called first to start building the dialog tree. The other methods will create the other parts of the tree where you need them. 
        public static BlueprintCue baseCue;


        public static BlueprintDialogReference CreateDialog(string name, string guid, List<BlueprintCueBaseReference> firstCue)
        {
            var dialog = Helpers.CreateAndAdd<BlueprintDialog>(name, guid);
            dialog.FirstCue = new CueSelection()
            {
                Cues = firstCue,
                Strategy = Strategy.First
            };

            dialog.Conditions = Empties.Conditions;
            dialog.StartActions = Empties.Actions;
            dialog.FinishActions = Empties.Actions;
            dialog.ReplaceActions = Empties.Actions;

            return dialog.ToReference<BlueprintDialogReference>();
        }

        public static BlueprintAnswerBaseReference CreateAnswer(string name, string guid, string text, List<BlueprintCueBaseReference> nextCues)
        {
            var answer = Helpers.CreateAndAdd<BlueprintAnswer>(name, guid);
            answer.Text = GameStrings.CreateString(name, text);
            answer.NextCue = new CueSelection()
            {
                Cues = nextCues,
                Strategy = Strategy.First
            };

            answer.ShowCheck = Empties.ShowCheck;
            answer.ShowConditions = Empties.Conditions;
            answer.SelectConditions = Empties.Conditions;
            answer.OnSelect = Empties.Actions;
            answer.FakeChecks = new CheckData[0];
            answer.CharacterSelection = Empties.CharacterSelection;

            return answer.ToReference<BlueprintAnswerBaseReference>();
        }

        public static BlueprintCueBaseReference CreateCue(string name, string guid, BlueprintUnitReference speaker, string text, List<BlueprintAnswerBaseReference> answerList = null, CueSelection cueSelection = null)
        {
            var cue = Helpers.CreateAndAdd<BlueprintCue>(name, guid);
            cue.Text = GameStrings.CreateString(name, text);

            cue.Speaker = new DialogSpeaker()
            {
                m_Blueprint = speaker,
                MoveCamera = true
            };

            cue.Answers = answerList;
            cue.Continue = cueSelection;

            if (cue.Text is null)
                cue.Text = Empties.String;
            if (cue.Speaker is null)
                cue.Speaker = Empties.DialogSpeaker;
            if (cue.Answers is null)
                cue.Answers = new List<BlueprintAnswerBaseReference>();
            if (cue.Continue is null)
                cue.Continue = new CueSelection();

            cue.Conditions = Empties.Conditions;
            cue.m_Listener = Activator.CreateInstance<BlueprintUnitReference>();
            cue.OnShow = Empties.Actions;
            cue.OnStop = Empties.Actions;

            return cue.ToReference<BlueprintCueBaseReference>();
        }

        public static BlueprintAnswerBaseReference CreateAnswerList(string name, string guid, List<BlueprintAnswerBaseReference> answers)
        {
            var answerList = Helpers.CreateAndAdd<BlueprintAnswersList>(name, guid);
            answerList.Answers = answers;

            if (answerList.Answers is null)
                answerList.Answers = new List<BlueprintAnswerBaseReference>();

            answerList.Conditions = Empties.Conditions;

            return answerList.ToReference<BlueprintAnswerBaseReference>();
        }

        //
        // Creates a new blueprint T and adds it to the games BlueprintCache. Each blueprint needs a unique AssetGuid. I just use the Tools->Create GUID tool included in Visual Studio
        // to generate the guid
        //

        private static T Create<T>(string name, string guid) where T : SimpleBlueprint, new()
        {
            T asset = new T()
            {
                name = name,
                AssetGuid = guid.ToGUID()
            };

            ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(guid.ToGUID(), asset);

            return asset;
        }
    }

    //
    // Needed to create text in the game. Each text entry must have its own unique key. Generally it is good practice to precede your key with the name of your mod to avoid
    // key collisons. You can use the key to fetch the string. Helpful if you want to localize your mods to different languages.
    //

    public static class GameStrings
    {
        private static Dictionary<string, LocalizedString> Strings = new Dictionary<string, LocalizedString>();
        internal static LocalizedString CreateString(string key, string value)
        {
            var localizedString = new LocalizedString() { m_Key = key };
            LocalizationManager.CurrentPack.PutString(key, value);
            if(!Strings.ContainsKey(key))
            Strings.Add(key, localizedString);
            return localizedString;
        }

        internal static LocalizedString GetString(string key)
        {
            return Strings.ContainsKey(key) ? Strings[key] : default;
        }

        public static void SetLocalisedName(this BlueprintUnit unit, string key, string name)
        {
            if (unit.LocalizedName.String.Key != key)
            {
                unit.LocalizedName = ScriptableObject.CreateInstance<SharedStringAsset>();
                unit.LocalizedName.String = GameStrings.CreateString(key, name);
            }
        }

    }



}
