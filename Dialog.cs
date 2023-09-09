using JetBrains.Annotations;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Interaction;
using System;
using System.Collections.Generic;
using static CustomNpcPortraits.DescriptionTools;

namespace CustomNpcPortraits
{

    public static class Dialog
    {
        /*
        public static T[] RemoveFromArray<T>(this T[] array, T value)
        {
            var list = array.ToList();
            return list.Remove(value) ? list.ToArray() : array;
        }
        */

        /*public static T[] RemoveFromArrayByType<T, V>(this T[] array)
        {
            List<T> list = new List<T>();

            foreach (var c in array)
            {
                if (!(c is V))
                {
                    list.Add(c);
                }
            }

            return list.ToArray();
        }*/
        /*
        public static void SetLocalisedName(this BlueprintUnit unit, string key, string name)
        {
            if (unit.LocalizedName.String.Key != key)
            {
                unit.LocalizedName = ScriptableObject.CreateInstance<SharedStringAsset>();
                unit.LocalizedName.String = Helpers.CreateString(key, name);
            }
        }
        */
        /*
        public static void RemoveComponents<T>(this BlueprintScriptableObject obj) where T : BlueprintComponent
        {
            var compnents_to_remove = obj.GetComponents<T>().ToArray();
            foreach (var c in compnents_to_remove)
            {
                obj.SetComponents(obj.ComponentsArray.RemoveFromArray(c));
            }
        }
        */
        /*
        public static void RemoveComponents<T>(this BlueprintScriptableObject obj, Predicate<T> predicate) where T : BlueprintComponent
        {
            var compnents_to_remove = obj.GetComponents<T>().ToArray();
            foreach (var c in compnents_to_remove)
            {
                if (predicate(c))
                {
                    obj.SetComponents(obj.ComponentsArray.RemoveFromArray(c));
                }
            }
        }
        */
        public static T Get<T>(string id) where T : SimpleBlueprint
        {
            var assetId = new BlueprintGuid(System.Guid.Parse(id));
            return Get<T>(assetId);
        }

        public static T Get<T>(BlueprintGuid id) where T : SimpleBlueprint
        {
            SimpleBlueprint asset = ResourcesLibrary.TryGetBlueprint(id);
            T value = asset as T;
            if (value == null) { Main.DebugLog($"COULD NOT LOAD: {id} - {typeof(T)}"); }
            return value;
        }

        public static void AddBlueprint([NotNull] SimpleBlueprint blueprint)
        {
            AddBlueprint(blueprint, blueprint.AssetGuid);
        }
        public static void AddBlueprint([NotNull] SimpleBlueprint blueprint, string assetId)
        {
            var Id = BlueprintGuid.Parse(assetId);
            AddBlueprint(blueprint, Id);
        }

        public static List<Guid> AddedBlueprints = new List<Guid>();

        public static readonly Dictionary<BlueprintGuid, SimpleBlueprint> ModBlueprints = new Dictionary<BlueprintGuid, SimpleBlueprint>();
        public static void AddBlueprint([NotNull] SimpleBlueprint blueprint, BlueprintGuid assetId)
        {
            var loadedBlueprint = ResourcesLibrary.TryGetBlueprint(assetId);
            if (loadedBlueprint == null)
            {
                ModBlueprints[assetId] = blueprint;
                ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(assetId, blueprint);
                blueprint.OnEnable();
                Main.DebugLog("Added"+ assetId.ToString());
            }
            else
            {
                Main.DebugLog($"Failed to Add: {blueprint.name}");
                Main.DebugLog($"Asset ID: {assetId} already in use by: {loadedBlueprint.name}");
            }
        }
        /*public static LocalizedString CreateString(string key, string value)
        {
            // See if we used the text previously.
            // (It's common for many features to use the same localized text.
            // In that case, we reuse the old entry instead of making a new one.)
            LocalizedString localized;
 
            //var strings = LocalizationManager.CurrentPack.Strings;
            //String oldValue;
            //if (strings.TryGetValue(key, out oldValue) && value != oldValue)
            {
#if DEBUG
                //     Main.LogDebug($"Info: duplicate localized string `{key}`, different text.");
#endif
                //}
                // strings[key] = value;
                localized = new LocalizedString
                {
                    m_Key = key
                };
                LocalizationManager.CurrentPack.PutString(key, value);
                // textToLocalizedString[value] = localized;
                return localized;
            }
        }*/

       // public static BlueprintUnit BubbleMasterBlueprint = null;

        public static BlueprintDialog BubbleDialog;

        public static BlueprintUnit BubbleMasterBlueprint;

        public static void CreateBubbleMasterBlueprint()
        {
           // BubbleMasterBlueprint = Get<BlueprintUnit>("0d88d5c310fac90449bfd0714bb9f810");

            BlueprintUnit crink = BubbleMasterBlueprint;

            //Main.DebugLog("NAME BEFORE: "+crink.CharacterName);
            
            crink.SetLocalisedName("bubble-dm", "Clitinae");
            
            //Main.DebugLog("NAME AFTER: " + crink.CharacterName);


            crink.RemoveComponents<DialogOnClick>();

            crink.RemoveComponents<BarkOnClick>();

            crink.Visual.m_Barks = null;


            crink.RemoveComponents<ActionsOnClick>();


            


            crink.AddComponent<DialogOnClick>(dialogOnClick =>
            {
                dialogOnClick.m_Dialog = Get<BlueprintDialog>("6a649fa02676f41498e0843e95ba15a6").ToReference<BlueprintDialogReference>();  //BubbleDialog.ToReference<BlueprintDialogReference>();
                dialogOnClick.name = "start-bubble-dialog";
                dialogOnClick.NoDialogActions = new ActionList();
                dialogOnClick.Conditions = new ConditionsChecker();
            });
            
        }



        public static void CreateBubbleDialog()
        {

            BlueprintCueBaseReference mainBubbleCue;
            {
                //BubbleMasterBlueprint = Get<BlueprintUnit>("0d88d5c310fac90449bfd0714bb9f810");

                //BubbleMasterBlueprint

               // Game.Instance.State.Units.All.ForEach(n => n.Descriptor.Asks.m_AllElements.ForEach(x => Main.DebugLog(x.name)));

                var dialogBuilder = new DialogBuilder("bubble-main");
                var mainRoot = dialogBuilder.Root("{bubble_gauntlet_welcome}\n{bubble_encounters_remaining}.");
                mainRoot.Speaker(BubbleMasterBlueprint.ToReference<BlueprintUnitReference>());
                var backstory = dialogBuilder.Cue("backstory", "I am the bubbliest bubble that ever bubbled");
          

                backstory.ContinueWith(mainRoot).Commit();

                var rootAnswers = mainRoot.Answers();


                rootAnswers
                    .Add("start-backstory", "Tell me more about yourself.")
                        .ContinueWith(backstory)
                        .Commit()

                    .Add("leave", "I must still wait before bubbling.")
                        .Commit();


                rootAnswers.Commit();
                mainRoot.Commit();
                mainBubbleCue = dialogBuilder.Build();
            }

            /*
            BlueprintCueBaseReference vendorActiveCue;
            {
                var vendorIsActive = new DialogBuilder("bubble-vendor-active");
                var root = vendorIsActive.Root("Would you like me to dismiss the vendor so you can continue?");
                root.Speaker(BubbleMasterBlueprint.ToReference<BlueprintUnitReference>());
                root.When(new DynamicCondition(() => false));
                root.Answers()
                        .Add("no", "No, I still have shopping to do.").Commit()
                        .Add("yes", "Yes, I am ready to continue")
                            //.AddAction(dismissVendor)
                            .ContinueWith(References.Static(mainBubbleCue))
                            .Commit()
                        .Commit();

                root.Commit();
                vendorActiveCue = vendorIsActive.Build();
            }
            */
            BubbleDialog = CreateDialog("bubble-dialog-bubblemaster", dialog => {
                dialog.FirstCue.Cues.Add(mainBubbleCue);
                //dialog.FirstCue.Cues.Add(vendorActiveCue);

                dialog.TurnFirstSpeaker = true;
            });





        }

        public class DynamicCondition : Condition
        {
            private Func<bool> predicate;

            public DynamicCondition(Func<bool> predicate)
            {
                this.predicate = predicate;
            }
            public override bool CheckCondition()
            {
                return predicate();
            }

            public override string GetConditionCaption()
            {
                return "predicated condition";
            }
        }
    }

}
