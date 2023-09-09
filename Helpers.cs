using Kingmaker.Blueprints;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.DialogSystem;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.ElementsSystem;
using Kingmaker.Localization;
using Kingmaker.ResourceLinks;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Mechanics;
using System.CodeDom;

namespace CustomNpcPortraits
{
    internal static class Helpers
    {
        public static T CreateElement<T>(SimpleBlueprint owner) where T : Element
        {
            var element = Element.CreateInstance(typeof(T));
            element.name = $"{element.GetType()}${System.Guid.NewGuid()}";
            return (T)element;
        }

        public static T Create<T>(string name, string guid) where T : BlueprintScriptableObject, new()
        {
            T asset = new T()
            {
                name = name,
                AssetGuid = guid.ToGUID()
            };

            return asset;
        }

        public static T CreateESO<T>(string name, string guid) where T : ElementsScriptableObject, new()
        {
            T asset = new T()
            {
                name = name,
                AssetGuid = guid.ToGUID()
            };

            return asset;
        }

        public static T CreateAndAddESO<T>(string name, string guid) where T : ElementsScriptableObject, new()
        {
            var asset = CreateESO<T>(name, guid);

            AddESO(guid, asset);

            return asset;
        }

        public static void AddESO<T>(string guid, T blueprint) where T : ElementsScriptableObject
        {
            if (ResourcesLibrary.TryGetScriptable<T>(guid) == null)
                ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(guid.ToGUID(), blueprint);
        }

        public static T CreateAndAdd<T>(string name, string guid) where T : BlueprintScriptableObject, new()
        {
            var asset = Create<T>(name, guid);

            AddBlueprint(guid, asset);

            return asset;
        }

        public static void AddBlueprint<T>(string guid, T blueprint) where T : BlueprintScriptableObject
        {
            if (ResourcesLibrary.TryGetBlueprint<T>(guid) == null)
                ResourcesLibrary.BlueprintsCache.AddCachedBlueprint(guid.ToGUID(), blueprint);
        }

        public static T CopyAndAdd<T>(T original, string name, string guid) where T : BlueprintScriptableObject
        {
            var copy = ObjectCopy<T>(original);
            copy.name = name;
            copy.AssetGuid = guid.ToGUID();

            AddBlueprint(guid, copy);

            return copy;
        }

        public static T CopyAndAddESO<T>(T original, string name, string guid) where T : ElementsScriptableObject
        {
            var copy = ObjectCopy<T>(original);
            copy.name = name;
            copy.AssetGuid = guid.ToGUID();

            AddESO(guid, copy);

            return copy;
        }

        public static T ObjectCopy<T>(T original)
        {
            var result = (T)ObjectDeepCopier.Clone(original);
            return result;
        }

        internal static class Empties
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
    }
}
