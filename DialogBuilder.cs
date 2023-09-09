using Kingmaker.Blueprints;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CustomNpcPortraits
{

    static class DescriptionTools
    {

        private class EncyclopediaEntry
        {
            public string Entry = "";
            public List<string> Patterns = new List<string>();

            public string Tag(string keyword)
            {
                return $"{{g|Encyclopedia:{Entry}}}{keyword}{{/g}}";
            }
        }

        private static readonly EncyclopediaEntry[] EncyclopediaEntries = new EncyclopediaEntry[] {
            new EncyclopediaEntry {
                Entry = "Strength",
                Patterns = { "Strength" }
            },
            new EncyclopediaEntry {
                Entry = "Dexterity",
                Patterns = { "Dexterity" }
            },
            new EncyclopediaEntry {
                Entry = "Constitution",
                Patterns = { "Constitution" }
            },
            new EncyclopediaEntry {
                Entry = "Intelligence",
                Patterns = { "Intelligence" }
            },
            new EncyclopediaEntry {
                Entry = "Wisdom",
                Patterns = { "Wisdom" }
            },
            new EncyclopediaEntry {
                Entry = "Charisma",
                Patterns = { "Charisma" }
            },
            new EncyclopediaEntry {
                Entry = "Ability_Scores",
                Patterns = { "Ability Scores?" }
            },
            new EncyclopediaEntry {
                Entry = "Athletics",
                Patterns = { "Athletics" }
            },
            new EncyclopediaEntry {
                Entry = "Persuasion",
                Patterns = { "Persuasion" }
            },
            new EncyclopediaEntry {
                Entry = "Knowledge_World",
                Patterns = { @"Knowledge \(?World\)?" }
            },
            new EncyclopediaEntry {
                Entry = "Knowledge_Arcana",
                Patterns = { @"Knowledge \(?Arcana\)?" }
            },
            new EncyclopediaEntry {
                Entry = "Lore_Nature",
                Patterns = { @"Lore \(?Nature\)?" }
            },
            new EncyclopediaEntry {
                Entry = "Lore_Religion",
                Patterns = { @"Lore \(?Religion\)?" }
            },
            new EncyclopediaEntry {
                Entry = "Mobility",
                Patterns = { "Mobility" }
            },
            new EncyclopediaEntry {
                Entry = "Perception",
                Patterns = { "Perception" }
            },
            new EncyclopediaEntry {
                Entry = "Stealth",
                Patterns = { "Stealth" }
            },
            new EncyclopediaEntry {
                Entry = "Trickery",
                Patterns = { "Trickery" }
            },
            new EncyclopediaEntry {
                Entry = "Use_Magic_Device",
                Patterns = {
                    "Use Magic Device",
                    "UMD"
                }
            },
            new EncyclopediaEntry {
                Entry = "Race",
                Patterns = { "Race" }
            },
            new EncyclopediaEntry {
                Entry = "Alignment",
                Patterns = { "Alignment" }
            },
            new EncyclopediaEntry {
                Entry = "Caster_Level",
                Patterns = {
                    "Caster Level",
                    "CL"
                }
            },
            new EncyclopediaEntry {
                Entry = "DC",
                Patterns = { "DC" }
            },
            new EncyclopediaEntry {
                Entry = "Saving_Throw",
                Patterns = { "Saving Throw" }
            },
            new EncyclopediaEntry {
                Entry = "Spell_Resistance",
                Patterns = { "Spell Resistance" }
            },
            new EncyclopediaEntry {
                Entry = "Spell_Fail_Chance",
                Patterns = { "Arcane Spell Failure" }
            },
            new EncyclopediaEntry {
                Entry = "Concentration_Checks",
                Patterns = { "Concentration Checks?" }
            },
            new EncyclopediaEntry {
                Entry = "Concealment",
                Patterns = { "Concealment" }
            },
            new EncyclopediaEntry {
                Entry = "Bonus",
                Patterns = {"Bonus(es)?"}
            },
            new EncyclopediaEntry {
                Entry = "Speed",
                Patterns = { "Speed" }
            },
            new EncyclopediaEntry {
                Entry = "Flat_Footed_AC",
                Patterns = {
                    "Flat Footed AC",
                    "Flat Footed Armor Class"
                }
            },
            new EncyclopediaEntry {
                Entry = "Flat_Footed",
                Patterns = {
                    "Flat Footed",
                    "Flat-Footed"
                }
            },
            new EncyclopediaEntry {
                Entry = "Armor_Class",
                Patterns = {
                    "Armor Class",
                    "AC"
                }
            },
            new EncyclopediaEntry {
                Entry = "Armor_Check_Penalty",
                Patterns = { "Armor Check Penalty" }
            },
            new EncyclopediaEntry {
                Entry = "Damage_Reduction",
                Patterns = { "DR" }
            },
            new EncyclopediaEntry {
                Entry = "Free_Action",
                Patterns = { "Free Action" }
            },
            new EncyclopediaEntry {
                Entry = "Swift_Action",
                Patterns = { "Swift Action" }
            },
            new EncyclopediaEntry {
                Entry = "Standard_Actions",
                Patterns = { "Standard Action" }
            },
            new EncyclopediaEntry {
                Entry = "Full_Round_Action",
                Patterns = { "Full Round Action" }
            },
            new EncyclopediaEntry {
                Entry = "Skills",
                Patterns = { "Skills? Checks?" }
            },
            new EncyclopediaEntry {
                Entry = "Combat_Maneuvers",
                Patterns = { "Combat Maneuvers?" }
            },
            new EncyclopediaEntry {
                Entry = "CMB",
                Patterns = {
                    "Combat Maneuver Bonus",
                    "CMB"
                }
            },
            new EncyclopediaEntry {
                Entry = "CMD",
                Patterns = {
                    "Combat Maneuver Defense",
                    "CMD"
                }
            },
            new EncyclopediaEntry {
                Entry = "BAB",
                Patterns = {
                    "Base Attack Bonus",
                    "BAB"
                }
            },
            new EncyclopediaEntry {
                Entry = "Incorporeal_Touch_Attack",
                Patterns = { "Incorporeal Touch Attacks?" }
            },
            new EncyclopediaEntry {
                Entry = "TouchAttack",
                Patterns = { "Touch Attacks?" }
            },
            new EncyclopediaEntry {
                Entry = "NaturalAttack",
                Patterns = {
                    "Natural Attacks?",
                    "Natural Weapons?"
                }
            },
            new EncyclopediaEntry {
                Entry = "Attack_Of_Opportunity",
                Patterns = {
                    "Attacks? Of Opportunity",
                    "AoO"
                }
            },
            new EncyclopediaEntry {
                Entry = "Penalty",
                Patterns = { "Penalty" }
            },
            new EncyclopediaEntry {
                Entry = "Check",
                Patterns = { "Checks?" }
            },
            new EncyclopediaEntry {
                Entry = "Spells",
                Patterns = { "Spells?" }
            },
            new EncyclopediaEntry {
                Entry = "Attack",
                Patterns = { "Attacks?" }
            },
            new EncyclopediaEntry {
                Entry = "Feat",
                Patterns = { "Feats?" }
            },
            new EncyclopediaEntry {
                Entry = "Charge",
                Patterns = { "Charge" }
            },
            new EncyclopediaEntry {
                Entry = "Critical",
                Patterns = { "Critical Hit" }
            },
            new EncyclopediaEntry {
                Entry = "Fast_Healing",
                Patterns = { "Fast Healing" }
            },
            new EncyclopediaEntry {
                Entry = "Temporary_HP",
                Patterns = { "Temporary HP" }
            },
            new EncyclopediaEntry {
                Entry = "Flanking",
                Patterns = {
                    "Flanking",
                    "Flanked"
                }
            },
            new EncyclopediaEntry {
                Entry = "Magic_School",
                Patterns = { "School of Magic" }
            },
            new EncyclopediaEntry {
                Entry = "Damage_Type",
                Patterns = {
                    "Bludgeoning",
                    "Piercing",
                    "Slashing"
                }
            }
        };

        private static string ApplyTags(this string str, string from, EncyclopediaEntry entry)
        {
            var pattern = from.StripHTML();
            var matches = Regex.Matches(str, pattern, RegexOptions.IgnoreCase)
                .OfType<Match>()
                .Select(m => m.Value)
                .Distinct();
            foreach (string match in matches)
            {
                str = Regex.Replace(str, Regex.Escape(match).StripHTML(), entry.Tag(match), RegexOptions.IgnoreCase);
            }
            return str;
        }
        private static string StripHTML(this string str)
        {
            return Regex.Replace(str, "<.*?>", string.Empty);
        }
        
        public class BubbleID
        {
            private static int next = 1;
            [Obsolete]
            public static string Get => $"{Context}-{next++}";

            internal static string Context = "global";
        }


        public interface IHaveContextName
        {
            string ContextName { get; }
            string ContextPath { get; }
        }

        public interface IAnswerHolder : IMustComplete
        {
            BlueprintAnswerBaseReference AnswerList { get; set; }
        }

        public class DummyAnswerHolder : IAnswerHolder, IHaveContextName
        {
            public BlueprintAnswerBaseReference AnswerList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


            public bool Completed => true;
            public string Description => "dummy";
            public string ContextName => "Dummy";
            public string ContextPath => "Dummy";
        }

        public class AnswerListBuilder<T> : Builder<BlueprintAnswersList, BlueprintAnswerBaseReference> where T : IAnswerHolder, IHaveContextName
        {
            public List<BlueprintAnswer> answerList = new List<BlueprintAnswer>();
            T parent;
            public string ContextName { get; private set; }
            public string ContextPath => $"{parent.ContextPath}/{ContextName}";

            public AnswerListBuilder(string name, DialogBuilder root, T parent) : base(name, root)
            {
                ContextName = name;
                this.parent = parent;
                Description = $"answers_for_{parent?.Description}";
            }

            public AnswerBuilder<T> Add(string name, string answerText)
            {
                return new AnswerBuilder<T>(root, name, answerText, this);
            }

            public AnswerListBuilder<T> AddExisting(BlueprintAnswer answer)
            {
                answerList.Add(answer);
                return this;
            }

            public T Commit()
            {
                Blueprint = CreateBlueprint<BlueprintAnswersList>($"bubble-answerlist://{ContextPath}", answers =>
                {
                    answers.Conditions = new ConditionsChecker();
                    answers.Answers = answerList.Select(a => a.ToReference<BlueprintAnswerBaseReference>()).ToList();
                });
                Complete();
                parent.AnswerList = Reference;
                return parent;
            }

        }

        public class AnswerBuilder<T> where T : IAnswerHolder, IHaveContextName
        {
            List<GameAction> actions = new List<GameAction>();
            private DialogBuilder root;
            string text;
            private AnswerListBuilder<T> parent;
            private List<IReferenceBuilder<BlueprintCueBaseReference>> next = new List<IReferenceBuilder<BlueprintCueBaseReference>>();
            private Condition[] conditions;
            private Condition[] selectCondition;
            public string ContextName { get; private set; }
            public string ContextPath => $"{parent.ContextPath}/{ContextName}";

            public AnswerBuilder<T> AddAction(GameAction action)
            {
                actions.Add(action);
                return this;
            }


            public AnswerBuilder<T> ContinueWith(params IReferenceBuilder<BlueprintCueBaseReference>[] cues)
            {
                next.AddRange(cues);
                return this;
            }

            //public AnswerBuilder<T> AddSkillCheck() {
            //    return this;
            //}


            public AnswerBuilder(DialogBuilder root, string name, string text, AnswerListBuilder<T> parent)
            {
                ContextName = name;
                this.root = root;
                this.text = text;
                this.parent = parent;
            }

            private BlueprintAnswer Create()
            {
                BlueprintAnswer item = CreateBlueprint<BlueprintAnswer>($"bubble-answer://{ContextPath}", answer =>
                {
                    answer.Text = CreateString($"bubble-answer-{ContextPath}.text", text);
                    answer.NextCue = new Kingmaker.DialogSystem.CueSelection();
                    answer.NextCue.Cues = new List<BlueprintCueBaseReference>();
                    answer.OnSelect = new ActionList();
                    answer.OnSelect.Actions = actions.ToArray();
                    answer.ShowOnceCurrentDialog = once;
                    answer.ShowOnce = once;
                    answer.AlignmentShift = new Kingmaker.UnitLogic.Alignments.AlignmentShift();
                    answer.ShowConditions = new ConditionsChecker();
                    if (conditions != null)
                        answer.ShowConditions.Conditions = conditions;
                    answer.SelectConditions = new ConditionsChecker();
                    if (selectCondition != null)
                    {
                        answer.SelectConditions.Conditions = selectCondition;
                    }
                    answer.ShowCheck = new ShowCheck();
                    answer.CharacterSelection = new Kingmaker.DialogSystem.CharacterSelection();
                    if (selectCharacterOn != StatType.Unknown)
                    {
                        answer.CharacterSelection.SelectionType = Kingmaker.DialogSystem.CharacterSelection.Type.Manual;
                        answer.CharacterSelection.ComparisonStats = new StatType[] { selectCharacterOn };
                    }
                    else
                        answer.CharacterSelection.SelectionType = Kingmaker.DialogSystem.CharacterSelection.Type.Player;
                    answer.SkillChecksDC.Add(new Kingmaker.Controllers.Dialog.SkillCheckDC
                    {

                    });
                });
                if (next.Count > 0)
                {
                    root.Fixups.Add(() =>
                    {
                        item.NextCue.Cues.AddRange(next.Select(n => n.Reference));
                    });
                }
                parent.answerList.Add(item);
                return item;

            }

            public AnswerListBuilder<T> Commit(out BlueprintAnswer answer)
            {
                answer = Create();
                return parent;
            }

            public AnswerListBuilder<T> Commit()
            {
                Create();
                return parent;
            }

            public AnswerBuilder<T> When(Func<bool> predicate)
            {
                return When(new DynamicCondition(predicate));
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

            public AnswerBuilder<T> When(params Condition[] conditions)
            {
                this.conditions = conditions;
                return this;
            }

            internal AnswerBuilder<T> Enabled(Func<bool> predicate)
            {
                return Enabled(new DynamicCondition(predicate));
            }
            internal AnswerBuilder<T> Enabled(params Condition[] selectCondition)
            {
                this.selectCondition = selectCondition;
                return this;
            }
            public class FalseCondition : Condition
            {
                public override bool CheckCondition()
                {
                    return false;
                }

                public override string GetConditionCaption()
                {
                    return "<false>";
                }

                public static FalseCondition Instance = new FalseCondition();
            }

            internal AnswerBuilder<T> Disabled()
            {
                return Enabled(FalseCondition.Instance);
            }

            internal AnswerBuilder<T> CharacterSelect(StatType statType)
            {
                selectCharacterOn = statType;
                return this;
            }

            StatType selectCharacterOn = StatType.Unknown;
            private bool once;

            internal AnswerBuilder<T> Once()
            {
                once = true;
                return this;
            }

        }

        public interface IReferenceBuilder<T> where T : BlueprintReferenceBase
        {
            T Reference { get; }
        }

        public static class References
        {
            public static IReferenceBuilder<T> Static<T>(T reference) where T : BlueprintReferenceBase
            {
                return new StaticReference<T>(reference);
            }

        }

        public class StaticReference<T> : IReferenceBuilder<T> where T : BlueprintReferenceBase
        {
            public T Reference => _Reference;

            private readonly T _Reference;

            public StaticReference(T reference)
            {
                _Reference = reference;
            }
        }

        public abstract class Builder<T, TRef> : IReferenceBuilder<TRef>, IMustComplete where T : SimpleBlueprint where TRef : BlueprintReferenceBase, new()
        {
            public T Blueprint;
            protected readonly DialogBuilder root;
            public String Description { get; protected set; }

            public TRef Reference => Blueprint.ToReference<TRef>();

            private bool _Complete;
            public string debug = "?";

            public bool Completed => _Complete;
            protected void Complete()
            {

                if (_Complete)
                {
                    var ex = new Exception($"cannot complete twice ({debug})");
                    Main.DebugLog(StackTraceUtility.ExtractStackTrace());
                    Main.DebugError(ex);
                }
                else
                {
                   // Main.DebugLog($"FIRST COMPLETION: {debug}");
                   // Main.DebugLog(StackTraceUtility.ExtractStackTrace());
                }
                _Complete = true;

            }

            protected Builder(string name, DialogBuilder root)
            {
                this.root = root;
                Description = name;
            }
        }

        public interface IMustComplete
        {
            bool Completed { get; }
            string Description { get; }

        }

        public class DialogBuilder
        {
            private static HashSet<string> seenNames = new HashSet<string>();
            public readonly string ContextName;
            public List<Action> Fixups = new List<Action>();
            public List<IMustComplete> Completable = new List<IMustComplete>();
            public CueBuilder root { private set; get; }
            public PageBuilder rootPage { private set; get; }

            public DialogBuilder(string name)
            {
                if (BubbleID.Context != "global")
                {
                    Main.DebugLog($"*** ERROR: trying to open dialog builder context ({name}) while context is not global, it is ({BubbleID.Context})");
                }
                ContextName = name;
                BubbleID.Context = name;

            }


            public CheckBuilder NewCheck(string name, StatType type)
            {
                return new CheckBuilder(name, this, type);
            }

            public CueBuilder Root(string text)
            {
                root = Cue("root", text);
                return root;
            }

            public CueBuilder Cue(string name, string text)
            {
                var builder = new CueBuilder(name, this, text);
                Completable.Add(builder);
                return builder;
            }

            public BlueprintCueBaseReference Build()
            {
                if (BubbleID.Context != ContextName)
                {
                    Main.DebugLog($"*** ERROR: trying to close dialog builder context ({ContextName}) while context is not mine, it is ({BubbleID.Context})");
                }
                BubbleID.Context = "global";

                foreach (var c in Completable.Where(c => !c.Completed))
                {
                    Main.DebugLog($"*** ERROR: something is incomplete: {c}:{c.Description}");
                }

                foreach (var fixup in Fixups)
                    fixup();
                if (root != null)
                    return root.Reference;
                else
                    return rootPage.Reference;

            }

            internal PageBuilder RootPage(string v)
            {
                var page = NewPage(v);
                rootPage = page;
                return page;
            }

            internal PageBuilder NewPage(string title)
            {
                var page = new PageBuilder(title, this, title);
                Completable.Add(page);
                return page;

            }
        }

        public class CheckBuilder : Builder<BlueprintCheck, BlueprintCueBaseReference>, IHaveContextName
        {
            public string ContextName { get; private set; }
            public string ContextPath => $"{root.ContextName}/{ContextName}";

            public CheckBuilder(string name, DialogBuilder root, StatType type) : base(name, root)
            {
                this.type = type;
                Description = $"check-{ContextPath}";
            }

            private StatType type;
            private IReferenceBuilder<BlueprintCueBaseReference> fail, success;
            private Condition[] conditions;
            private List<DCModifier> modifiers = new List<DCModifier>();
            private bool once;
            private int dc = 10;

            public void Commit()
            {
                Blueprint = CreateBlueprint<BlueprintCheck>($"bubble-check://{ContextPath}", check =>
                {
                    check.Conditions = new ConditionsChecker();
                    if (conditions != null)
                        check.Conditions.Conditions = conditions;

                    check.ShowOnce = once;
                    check.ShowOnceCurrentDialog = once;
                    check.DC = dc;
                    check.DCModifiers = modifiers.ToArray();
                    check.Type = type;
                });
                root.Fixups.Add(() =>
                {
                    Blueprint.m_Fail = fail.Reference;
                    Blueprint.m_Success = success.Reference;
                });
                Complete();
            }

            public CheckBuilder OnComplete(IReferenceBuilder<BlueprintCueBaseReference> next)
            {
                OnSuccess(next);
                OnFail(next);
                return this;
            }

            public CheckBuilder DC(int dc)
            {
                this.dc = dc;
                return this;
            }

            public CheckBuilder OnSuccess(IReferenceBuilder<BlueprintCueBaseReference> success)
            {
                this.success = success;
                return this;
            }
            public CheckBuilder OnFail(IReferenceBuilder<BlueprintCueBaseReference> fail)
            {
                this.fail = fail;
                return this;
            }

            internal CheckBuilder When(params Condition[] conditions)
            {
                this.conditions = conditions;
                return this;
            }

            internal CheckBuilder Once()
            {
                this.once = true;
                return this;
            }

            internal CheckBuilder AdjustDC(int delta, params Condition[] conditions)
            {
                modifiers.Add(new DCModifier()
                {
                    Conditions = new ConditionsChecker() { Conditions = conditions },
                    Mod = delta
                });
                return this;
            }
        }

        public static T CreateBlueprint<T>(string name, Action<T> init = null, Action<T> earlyInit = null) where T : SimpleBlueprint, new()
        {
            var result = new T
            {
                name = name,
                AssetGuid = new BlueprintGuid(Guid.NewGuid())
            };
            earlyInit?.Invoke(result);
            Dialog.AddBlueprint(result);
            init?.Invoke(result);
            return result;
        }
        static readonly Dictionary<String, LocalizedString> textToLocalizedString = new Dictionary<string, LocalizedString>();

        public static LocalizedString CreateString(string key, string value)
        {
            // See if we used the text previously.
            // (It's common for many features to use the same localized text.
            // In that case, we reuse the old entry instead of making a new one.)
            if (textToLocalizedString.TryGetValue(value, out var localized))
            {
                return localized;
            }

#if DEBUG
            var current = LocalizationManager.CurrentPack.GetText(key, false);
            if (current != "" && current != value)
            {
                Main.DebugLog($"Info: duplicate localized string `{key}`, different text.");
            }
#endif
            LocalizationManager.CurrentPack.PutString(key, value);
            localized = new LocalizedString { m_ShouldProcess = false, m_Key = key };
            textToLocalizedString[value] = localized;
            return localized;
        }

        public static BlueprintDialog CreateDialog(string name, Action<BlueprintDialog> init)
        {
            return CreateBlueprint<BlueprintDialog>(name, dialog => {
                dialog.Conditions = new ConditionsChecker();
                dialog.StartActions = new ActionList();
                dialog.FinishActions = new ActionList();
                dialog.ReplaceActions = new ActionList();

                dialog.FirstCue = new Kingmaker.DialogSystem.CueSelection();
                dialog.FirstCue.Cues = new List<BlueprintCueBaseReference>();

                init(dialog);
            });
        }

        public class PageBuilder : Builder<BlueprintBookPage, BlueprintCueBaseReference>, IAnswerHolder, IHaveContextName
        {
            public BlueprintAnswerBaseReference AnswerList { get; set; }
            private string text;
            private List<BlueprintCueBaseReference> cues = new List<BlueprintCueBaseReference>();
            private Condition[] conditions;
            private ActionList OnShowActions = new ActionList();
            public string ContextName { get; private set; }
            public string ContextPath => $"{root.ContextName}/{ContextName}";

            public PageBuilder(string name, DialogBuilder root, string title) : base(name, root)
            {
                ContextName = name;
                this.text = title;
                Description = title;
            }

            public AnswerListBuilder<PageBuilder> Answers()
            {
                var builder = new AnswerListBuilder<PageBuilder>("answerlist", root, this);
                root.Completable.Add(builder);
                return builder;
            }

    


 
            public void Commit()
            {
                Blueprint = CreateBlueprint<BlueprintBookPage>($"bubble-page://{ContextPath}", page =>
                {
                    page.Answers = new List<BlueprintAnswerBaseReference>();
                    if (AnswerList != null)
                    {
                        page.Answers.Add(AnswerList);
                    }
                    page.Title = CreateString($"bubble-cue-{ContextPath}.text", text);
                    page.OnShow = OnShowActions;
                    page.Conditions = new ConditionsChecker();
                    if (this.conditions != null)
                        page.Conditions.Conditions = conditions;
                    page.Cues = cues;
                    //page.ImageLink = new();
                    //page.ImageLink.AssetId = "Resource:8bc34ca461d25bc45abe1273b4964702:ui";
                });
                Complete();
            }

            public CueBuilder Cue(string name, string text)
            {
                var cue = root.Cue(name, text);
                root.Fixups.Add(() =>
                {
                    cues.Add(cue.Reference);
                });
                return cue;
            }

            public PageBuilder BasicCue(string name, string text)
            {
                Cue(name, text).Commit();
                return this;
            }

            internal PageBuilder When(params Condition[] conditions)
            {
                this.conditions = conditions;
                return this;
            }

            internal PageBuilder OnShow(params GameAction[] actions)
            {
                this.OnShowActions.Actions = actions;
                return this;
            }

            internal PageBuilder ContinueWith(IReferenceBuilder<BlueprintCueBaseReference> next)
            {
                Answers()
                    .Add("continue", "continue")
                        .ContinueWith(next)
                        .Commit()
                    .Commit();
                return this;
            }

            internal object SimpleCheck(string v1, StatType skillPerception, int v2, string v3, string v4)
            {
                throw new NotImplementedException();
            }
        }

        public class CueBuilder : Builder<BlueprintCue, BlueprintCueBaseReference>, IAnswerHolder, IHaveContextName
        {
            public BlueprintAnswerBaseReference AnswerList { get; set; }
            private string text;
            private GameAction[] stopActions;
            private ConditionsChecker conditions = new ConditionsChecker();
            private BlueprintUnitReference speaker;
            private bool focusSpeaker;
            public string ContextName { get; private set; }
            public string ContextPath => $"{root.ContextName}/{ContextName}";

            public CueBuilder(string contextName, DialogBuilder root, string text) : base(contextName, root)
            {
                ContextName = contextName;
                this.text = text;
                Description = text;
            }

            public AnswerListBuilder<CueBuilder> Answers()
            {
                var builder = new AnswerListBuilder<CueBuilder>("answers", root, this);
                root.Completable.Add(builder);
                return builder;
            }

            public void Commit()
            {
                //Main.DebugLog("1");
                Blueprint = CreateBlueprint<BlueprintCue>($"bubble-cue://{ContextPath}", cue =>

                {
                    //Main.DebugLog("2");
                    cue.Answers = new List<BlueprintAnswerBaseReference>();
                    if (AnswerList != null)
                    {
                        cue.Answers.Add(AnswerList);
                    }
                    cue.Continue = new Kingmaker.DialogSystem.CueSelection();
                    cue.Continue.Cues = new List<BlueprintCueBaseReference>();
                    cue.Text = CreateString($"bubble-cue-{ContextPath}.text", text);
                    cue.Speaker = new Kingmaker.DialogSystem.DialogSpeaker();
                    if (speaker == null)
                        cue.Speaker.NoSpeaker = true;
                    else
                    {
                        cue.Speaker.m_Blueprint = speaker;
                        cue.Speaker.MoveCamera = this.focusSpeaker;
                    }
                    //Main.DebugLog("3");
                    cue.OnShow = new ActionList();
                    cue.OnStop = new ActionList();
                    if (stopActions != null)
                        cue.OnStop.Actions = stopActions;
                    cue.AlignmentShift = new Kingmaker.UnitLogic.Alignments.AlignmentShift();
                    cue.Conditions = conditions;
                });
                Complete();
            }

            internal CueBuilder Speaker(BlueprintUnitReference speaker, bool focus = false)
            {
                this.speaker = speaker;
                this.focusSpeaker = false;
                return this;
            }

            internal CueBuilder ContinueWith(CheckBuilder with)
            {
                root.Fixups.Add(() =>
                {
                    this.Blueprint.Continue.Cues.Add(with.Reference);
                });
                return this;
            }

            internal CueBuilder ContinueWith(IReferenceBuilder<BlueprintCueBaseReference> with)
            {
                root.Fixups.Add(() =>
                {
                    this.Blueprint.Continue.Cues.Add(with.Reference);
                });
                return this;
            }

            internal CueBuilder OnStop(params GameAction[] actions)
            {
                stopActions = actions;
                return this;
            }

            internal CueBuilder When(params Condition[] conditions)
            {
                this.conditions.Conditions = conditions;
                return this;
            }
        }

    }
}
